using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;
using System.Linq;

namespace webstore.filters
{
    public class AuthorizationOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var hasAuthorize = context.MethodInfo.GetCustomAttributes(true)
                    .OfType<AuthorizeAttribute>().Any()
                || (context.MethodInfo.DeclaringType?.GetCustomAttributes(true)
                    .OfType<AuthorizeAttribute>()
                    .Any() ?? false);

            var hasAllowAnonymous = context.MethodInfo.GetCustomAttributes(true)
                    .OfType<AllowAnonymousAttribute>().Any()
                || (context.MethodInfo.DeclaringType?.GetCustomAttributes(true)
                    .OfType<AllowAnonymousAttribute>()
                    .Any() ?? false);

            if (hasAuthorize && !hasAllowAnonymous)
            {
                operation.Security ??= new List<OpenApiSecurityRequirement>();

                var scheme = new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                };

                operation.Security.Add(new OpenApiSecurityRequirement
                {
                    [scheme] = new List<string>()
                });
            }
        }
    }
}