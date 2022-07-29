using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Passengers.SharedKernel.Swagger.ApiGroup;
using Swashbuckle.AspNetCore.Filters;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Passengers.SharedKernel.Swagger
{
    public static class OpenApiFactory
    {
        public static IServiceCollection AddOpenAPI(this IServiceCollection services)
        {
            services.AddSwaggerGen(options =>
            {
                var openApiInfo = new OpenApiInfo
                {
                    Version = "v1",
                    Title = "WebApi",
                };

                typeof(ApiGroupNames).GetFields().Skip(1).ToList().ForEach(f =>
                {
                    //Gets the attribute on the enumeration value
                    var info = f.GetCustomAttributes(typeof(GroupInfoAttribute), false).OfType<GroupInfoAttribute>().FirstOrDefault();
                    openApiInfo.Title = info?.Title;
                    openApiInfo.Version = info?.Version;
                    openApiInfo.Description = info?.Description;
                    options.SwaggerDoc(f.Name, openApiInfo);
                });

                //Determine which group the interface belongs to
                options.DocInclusionPredicate((docName, apiDescription) =>
                {
                    if (!apiDescription.TryGetMethodInfo(out MethodInfo method)) return false;
                    //1. All interfaces
                    if (docName == "All") return true;
                    //The value of reflection under the grouping characteristic of the controller
                    var actionlist = apiDescription.ActionDescriptor.EndpointMetadata.FirstOrDefault(x => x is ApiGroupAttribute);
                    //2. Get the interface that has not been grouped***************
                    if (docName == "NoGroup") return actionlist == null ? true : false;
                    //3. Load the corresponding grouped interfaces
                    if (actionlist != null)
                    {
                        //Determine whether to include this group
                        var actionfilter = actionlist as ApiGroupAttribute;
                        return actionfilter.GroupName.Any(x => x.ToString().Trim() == docName);
                    }
                    return false;
                });

                options.CustomSchemaIds(x => x.FullName);

                // Defining the security schema
                var securitySchema = new OpenApiSecurityScheme()
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                };

                // Adding the bearer token authentaction option to the ui
                options.AddSecurityDefinition("Bearer", securitySchema);

                //  use the token provided with the endpoints call
                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    { securitySchema, new[] { "Bearer" } }
                });

                options.OperationFilter<AppendAuthorizeToSummaryOperationFilter>();
            });

            return services;
        }

        public static IApplicationBuilder ConfigureOpenAPI(this IApplicationBuilder app)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                //Skip (1) is because the first fieldinfo of enum is a built-in int value
                typeof(ApiGroupNames).GetFields().Skip(1).ToList().ForEach(f =>
                {
                    //Gets the attribute on the enumeration value
                    var info = f.GetCustomAttributes(typeof(GroupInfoAttribute), false).OfType<GroupInfoAttribute>().FirstOrDefault();
                    c.SwaggerEndpoint($"/swagger/{f.Name}/swagger.json", info != null ? info.Title : f.Name);
                });

                c.DocExpansion(DocExpansion.None);
            });
            return app;
        }
    }
}
