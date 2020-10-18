using Microsoft.Extensions.DependencyInjection;
using System;

namespace Yugen.DJ.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddFactory<TImplementation>(this IServiceCollection services)
            where TImplementation : class
        {
            services.AddSingleton<TImplementation>();
            services.AddSingleton<Func<TImplementation>>(x => () => x.GetService<TImplementation>());
            return services;
        }
    }

}