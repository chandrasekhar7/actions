using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentValidation;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using Npa.Accounting.Application;
using Npa.Accounting.Common.ErrorHandling;
using Npa.Accounting.Domain.DEPRECATED;
using JsonConverter = Newtonsoft.Json.JsonConverter;

namespace Npa.Accounting.Infrastructure.ErrorHandling;

internal class CustomErrorHandlingMiddleware
{
    private readonly RequestDelegate next;
    private readonly ILogger logger;
    private static readonly JsonSerializerSettings serializerSettings = new JsonSerializerSettings()
    {
        Converters = new List<JsonConverter>()
        {
            new StringEnumConverter()
        },
        ContractResolver = new CamelCasePropertyNamesContractResolver()
    };

    public CustomErrorHandlingMiddleware(RequestDelegate next, ILogger<ApplicationLayer> logger)
    {
        this.logger = logger;
        this.next = next;
    }

    public async Task Invoke(HttpContext context, IWebHostEnvironment env)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex, env);
        }
    }

    private Task HandleExceptionAsync(HttpContext context, Exception ex, IWebHostEnvironment env)
    {
        var code = HttpStatusCode.InternalServerError; // 500 if unexpected
        var level = LogLevel.None;
        
        if (ex is AccessDeniedException)
        {
            level = LogLevel.Warning;
            code = HttpStatusCode.Forbidden;
        }

        if (ex is AuthorizationException)
        {
            level = LogLevel.Warning;
            code = HttpStatusCode.Unauthorized;
        }

        if (ex is ForbiddenException)
        {
            level = LogLevel.Warning;
            code = HttpStatusCode.Forbidden;
        }
        else if (ex is NotFoundException)
        {
            level = LogLevel.Information;
            code = HttpStatusCode.NotFound;
        }
        else if (ex is ValidationException)
        {
            level = LogLevel.Information;
            code = HttpStatusCode.BadRequest;
        }
        else if (ex is ApplicationLayerException)
        {
            level = LogLevel.Warning;
            code = HttpStatusCode.BadRequest;
        }
        else if (ex is DomainLayerException)
        {
            level = LogLevel.Error;
            code = HttpStatusCode.BadRequest;
        }
        else if (ex is ConflictException)
        {
            level = LogLevel.Error;
            code = HttpStatusCode.Conflict;
        }
        else if (ex is HttpRequestException nx)
        {
            level = LogLevel.Error;
            code = nx.StatusCode ?? HttpStatusCode.InternalServerError;
        }
        else
        {
            level = LogLevel.Error;
        }

        var includeDetails = env.IsDevelopment() || env.IsStaging() || env.IsEnvironment("Azure-Staging");
        var details = includeDetails ? ex.ToString() : null;

        var problem = new ProblemDetails
        {
            Status = (int?) code,
            Title = ex.Message,
            Detail = details
        };
        logger.Log(level, ex, ex.Message);

        context.Response.ContentType = "application/problem+json";
        context.Response.StatusCode = (int) code;

        var result = JsonConvert.SerializeObject(problem, serializerSettings);
        return context.Response.WriteAsync(result);
    }
}