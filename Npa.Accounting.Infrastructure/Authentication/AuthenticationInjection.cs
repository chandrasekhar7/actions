using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Npa.Accounting.Common.ErrorHandling;

namespace Npa.Accounting.Infrastructure.Authentication;

public static class AuthenticationInjection
{
    public static IServiceCollection AddCustomAuthentication(this IServiceCollection services,
        IConfiguration configuration)
    {
        var jwtSettings = new JwtOptions();
        configuration.Bind("JwtSettings", jwtSettings);

        var paydiniJwtSettings = new JwtOptions();
        configuration.Bind("PaydiniJwtSettings", paydiniJwtSettings);
        var apiKeySettings = new AuthBearerToken();
       configuration.Bind("ApiKey", apiKeySettings);
        services.AddSingleton(apiKeySettings);
        services.AddSingleton(jwtSettings);
        services.AddAuthentication(options =>
            {
                // set the custom bearer token to the main authentication type
                // options.DefaultAuthenticateScheme = "JWT";//"Identity.Application";//JwtBearerDefaults.AuthenticationScheme;
                // options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                // options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddCookieAuth(configuration.GetValue<string>("SharedCookie"))
            .AddJwtAuth(Schemes.TokenUser, jwtSettings)
            .AddJwtAuth(Schemes.Paydini, paydiniJwtSettings);
            
            services
            .AddAuthentication(Schemes.ApiKey)
            .AddScheme<AuthBearerTokenOptions, AuthBearerTokenHandler>(Schemes.ApiKey, null);


        return services;
    }

    private static AuthenticationBuilder AddCookieAuth(this AuthenticationBuilder b, string cookiePath)
    {
        return b.AddCookie(
            Schemes.CookieUser, options =>
            {
                options.Cookie.Name = "Authentication";
                options.Cookie.SameSite = SameSiteMode.Lax;
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
                options.ExpireTimeSpan = TimeSpan.FromMinutes(120);
                options.CookieManager = new ChunkingCookieManager();
                options.Events.OnRedirectToLogin = context =>
                {
                    context.Response.StatusCode = 401;
                    return Task.CompletedTask;
                };
                options.TicketDataFormat = new SecureDataFormat<AuthenticationTicket>(new TicketSerializer(),
                    DataProtectionProvider.Create(
                          new DirectoryInfo(cookiePath),
                            builder => { builder.SetApplicationName("NetPayAdvance.API"); })
                        .CreateProtector(
                            "Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationMiddleware",
                            "Identity.Application", "v2"));
            });
    }

    private static AuthenticationBuilder AddJwtAuth(this AuthenticationBuilder b, string scheme, JwtOptions jwtOptions)
    {
        return b.AddJwtBearer(scheme, options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = jwtOptions.ValidateIssuer,
                ValidateAudience = jwtOptions.ValidateAudience,
                ValidateIssuerSigningKey = jwtOptions.ValidateIssuerSigningKey,
                RequireExpirationTime = jwtOptions.RequireExpirationTime,
                ValidateLifetime = jwtOptions.ValidateLifetime,
                ValidIssuer = jwtOptions.Issuer,
                ValidAudience = jwtOptions.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Secret))
            };

            options.Events = new JwtBearerEvents
            {
                OnAuthenticationFailed = context =>
                {
                    if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                    {
                        context.Response.Headers.Add("Token-Expired", "true");
                        throw new AuthorizationException("Not a valid token");
                    }

                    var x = context.Exception;


                    return Task.CompletedTask;
                }
            };
        });
    }

    private static string GetCookiePath()
    {
        return Path.Combine(@"C:\", "SharedCookies");
        if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            return Path.Combine(Path.GetPathRoot(Environment.SystemDirectory), "SharedCookies");
        return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "SharedCookies");
    }
}