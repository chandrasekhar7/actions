using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Npa.Accounting.Infrastructure.Authentication;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Npa.Accounting.Presentation.Swagger;

public class ConfigureSwaggerGenOptions : IConfigureNamedOptions<SwaggerGenOptions>
{
    private readonly IApiVersionDescriptionProvider provider;

    public ConfigureSwaggerGenOptions(
        IApiVersionDescriptionProvider provider)
    {
        this.provider = provider;
    }

    public void Configure(SwaggerGenOptions options)
    {
        // add swagger document for every API version discovered
        foreach (var description in provider.ApiVersionDescriptions)
        {
            options.SwaggerDoc(
                description.GroupName,
                CreateVersionInfo(description));
        }
        options.MapType<DateOnly>(() => new OpenApiSchema() {Type = "string", Format = "date"});
    }

    public void Configure(string name, SwaggerGenOptions options)
    {
        Configure(options);
        var securityScheme = new OpenApiSecurityScheme
        {
            Name = "JWT Authentication",
            Description = "Enter JWT Bearer token **_only_**",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.Http,
            Scheme = "bearer", // must be lower case
            BearerFormat = "JWT",
            Reference = new OpenApiReference
            {
                Id = Schemes.ApiKey,
                Type = ReferenceType.SecurityScheme
            },
        };
        var cookieScheme = new OpenApiSecurityScheme
        {
            Name = "Authentication",
            Description = "Enter Cookie",
            In = ParameterLocation.Cookie,
            Type = SecuritySchemeType.ApiKey,
            Scheme = "", // must be lower case
            Reference = new OpenApiReference
            {
                Id = Schemes.CookieUser,
                Type = ReferenceType.SecurityScheme
            }
        };
        options.AddSecurityDefinition(securityScheme.Reference.Id, securityScheme);
        options.AddSecurityDefinition(cookieScheme.Reference.Id, cookieScheme);
        options.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {securityScheme, new string[] {}},
            {cookieScheme, new string[] {}}
        });
        // var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        // var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
        // options.IncludeXmlComments(xmlPath);

        var currentAssembly = Assembly.GetExecutingAssembly();
        var xmlDocs = currentAssembly.GetReferencedAssemblies()
            .Union(new AssemblyName[] {currentAssembly.GetName()})
            .Select(a => Path.Combine(Path.GetDirectoryName(currentAssembly.Location), $"{a.Name}.xml"))
            .Where(f => File.Exists(f)).ToArray();
        Array.ForEach(xmlDocs, (d) => { options.IncludeXmlComments(d); });
    }

    private OpenApiInfo CreateVersionInfo(
        ApiVersionDescription description)
    {
        var info = new OpenApiInfo()
        {
            Title = "Accounting",
            Version = description.ApiVersion.ToString()
        };

        if (description.IsDeprecated)
        {
            info.Description += " This API version has been deprecated.";
        }

        return info;
    }
}