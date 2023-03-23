using System;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using FluentValidation.AspNetCore;
using MediatR;
using MicroElements.Swashbuckle.FluentValidation;
using MicroElements.Swashbuckle.FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Npa.Accounting.Application;
using Npa.Accounting.Infrastructure;
using Npa.Accounting.Persistence.DEPRECATED;
using Npa.Accounting.Presentation.Converters;
using Npa.Accounting.Presentation.Extensions;
using Npa.Accounting.Presentation.Swagger;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.MSSqlServer;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.

builder.Host.UseSerilog((ctx, ls) => ls.ReadFrom.Configuration(builder.Configuration)
    .Enrich.WithProperty("ServerName", Environment.MachineName)
    .Enrich.WithProperty("Environment", Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production")
    .WriteTo.MSSqlServer(builder.Configuration.GetConnectionString("Logging"), new MSSqlServerSinkOptions()
    {
        AutoCreateSqlTable = true,
        TableName = "ErrorLogs"
    }, restrictedToMinimumLevel: LogEventLevel.Error)
);

builder.Services.AddControllers(o =>
    {
        o.UseGeneralRoutePrefix("api/accounting/v{version:apiVersion}/");
        o.UseDateOnlyTimeOnlyStringConverters();
    })
    .AddFluentValidation(c =>
    {
        c.RegisterValidatorsFromAssemblyContaining<ApplicationLayerException>();
        // Optionally set validator factory if you have problems with scope resolve inside validators.
        c.ValidatorFactoryType = typeof(HttpContextServiceProviderValidatorFactory);
    })
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.Converters.Add(new DateOnlyConverter());
        options.JsonSerializerOptions.Converters.Add(new TimeOnlyConverter());
    });

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder => { builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod(); });
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddApplicationLayer(addValidation: true, addRequestLogging: true);
builder.Services.AddInfrastructureLayer(builder.Configuration, builder.Environment);
builder.Services.AddPersistenceLayer(builder.Configuration);


// builder.Services.Configure<RouteOptions>(o =>
// {
//     o.ConstraintMap.Add("reportType", typeof(ReportTypeRouteConstraint));
// });

if (builder.Environment.IsDevelopment() || builder.Environment.IsStaging() || builder.Environment.IsEnvironment("Azure-Staging"))
{
    builder.Services.ConfigureOptions<ConfigureSwaggerOptions>();
    builder.Services.ConfigureOptions<ConfigureSwaggerGenOptions>();
    builder.Services.ConfigureOptions<ConfigureSwaggerUIOptions>();
    builder.Services.AddSwaggerGen(options =>
    {
        options.CustomSchemaIds(type => type.Name.ToString().Replace("ViewModel", ""));    
    });
    builder.Services.AddFluentValidationRulesToSwagger();
}

builder.Services.AddMediatR((from t in new[] {typeof(ApplicationLayer)} select t.Assembly).ToArray());

builder.Services.AddApiVersioning(o =>
{
    o.AssumeDefaultVersionWhenUnspecified = true;
    o.DefaultApiVersion = new Microsoft.AspNetCore.Mvc.ApiVersion(1, 0);
    o.ReportApiVersions = true;
});
builder.Services.AddVersionedApiExplorer(setup =>
{
    setup.GroupNameFormat = "'v'VVV";
    setup.SubstituteApiVersionInUrl = true;
});

var app = builder.Build();



// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.IsStaging() || app.Environment.IsEnvironment("Azure-Staging"))
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCustomErrors();
app.UseRouting();

app.UseCors();

app.UseAuthorization();

app.UseEndpoints(endpoints => { endpoints.MapControllers().RequireAuthorization(); });

app.Run();