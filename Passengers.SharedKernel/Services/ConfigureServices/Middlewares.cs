using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Passengers.SharedKernel.Services.ConfigureServices
{
    public static class Middlewares
    {
        private static IServiceScope ServiceScope;

        public static IApplicationBuilder UseSqlServerSeed<T>(this IApplicationBuilder builder, Action<T> context) where T : DbContext
        {
            ServiceScope = builder.CreateFactoryScope();
            context(ServiceScope.GetProviderService<T>());
            return builder;
        }

        public static IApplicationBuilder UseSqlServerSeed<T>(this IApplicationBuilder builder, Action<IServiceProvider> context) where T : DbContext
        {
            ServiceScope = builder.CreateFactoryScope();
            context(ServiceScope.ServiceProvider);
            return builder;
        }

        public static IApplicationBuilder UseSqlServerSeed<T>(this IApplicationBuilder builder, Action<T, IServiceProvider> context) where T : DbContext
        {
            ServiceScope = builder.CreateFactoryScope();
            context(ServiceScope.GetProviderService<T>(), ServiceScope.ServiceProvider);
            return builder;
        }

        public static void DisposeSqlServerSeed(this IApplicationBuilder builder)
        => ServiceScope.Dispose();



        public static IServiceScope CreateFactoryScope(this IApplicationBuilder app)
         => app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope();
        public static T GetProviderService<T>(this IServiceScope scop)
            => scop.ServiceProvider.GetService<T>();

        public static T GetProviderRequiredService<T>(this IServiceScope scop)
            => scop.ServiceProvider.GetRequiredService<T>();
    }
}
