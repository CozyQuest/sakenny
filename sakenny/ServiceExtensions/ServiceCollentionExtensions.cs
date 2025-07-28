using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;

namespace sakenny.ServiceExtensions;

internal static class ServiceCollentionExtensions
{
    internal static IServiceCollection AddSwaggerAuth(this IServiceCollection services)
    {
        services.AddSwaggerGen(c =>
        {
            // Configure JWT Bearer Authentication
            var securityScheme = new OpenApiSecurityScheme
            {
                Name = "JWT Authorization",
                Description = "Enter your JWT access token in this field. You can get this token from the /api/User/login endpoint. When the token expires, use /api/User/refresh to get a new one.",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = JwtBearerDefaults.AuthenticationScheme,
                BearerFormat = "JWT"
            };

            c.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, securityScheme);

            var securityRequirement = new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = JwtBearerDefaults.AuthenticationScheme
                        }
                    },
                    new string[] {}
                }
            };

            c.AddSecurityRequirement(securityRequirement);

            // Add API documentation
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Sakenny API",
                Version = "v1",
                Description = @"
                                Authentication Flow:
                                1. Login: POST /api/User/login - Get access token and refresh token
                                2. Use Access Token: Include in Authorization header as Bearer {access_token}
                                3. Refresh: POST /api/User/refresh - Get new tokens when access token expires

                                Token Lifespans:
                                - Access Token: 15 minutes  
                                - Refresh Token: 7 days

                                Note: The refresh token should be stored securely and used only when the access token expires."
            });
        });

        return services;
    }
}