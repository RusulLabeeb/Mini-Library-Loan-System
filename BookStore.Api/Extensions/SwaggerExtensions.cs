using BookStore.Infrastructure.Constants;
using Microsoft.OpenApi.Models;

namespace BookStore.Api.Extensions;

public static class SwaggerExtensions
{
    public static IServiceCollection AddSwaggerDocumentation(this IServiceCollection services)
    {
        services.AddSwaggerGen(c =>
        {
            // 1. Define the Security Scheme (The "Lock" definition)
            // --------------------------------------------------------
            // 1. JWT Definition (The "Smart" Way)
            // --------------------------------------------------------
            c.AddSecurityDefinition("Jwt", new OpenApiSecurityScheme
            {
                // "Http" type tells Swagger: "I know how standard HTTP auth works"
                Type = SecuritySchemeType.Http, 
                Scheme = "bearer", 
                BearerFormat = "JWT", 
                Description = "Enter your JWT token directly (no 'Bearer ' prefix needed)."
            });

            // --------------------------------------------------------
            // 2. Basic Auth Definition
            // --------------------------------------------------------
            c.AddSecurityDefinition("Basic", new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.Http,
                Scheme = "basic",
                Description = "Enter your Username and Password."
            });

            // 2. Define the Requirement (Apply the Lock to all endpoints)
            // --------------------------------------------------------
            // 3. The Requirement (Apply BOTH locks globally)
            // --------------------------------------------------------
            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                // JWT Requirement
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Jwt" // Must match the string in AddSecurityDefinition
                        }
                    },
                    new List<string>() 
                },
                // Basic Auth Requirement
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Basic" // Must match the string in AddSecurityDefinition
                        }
                    },
                    new List<string>()
                }
            });
        });

        return services;
    }
}