using EntityFramework.Audit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AuditDbContext<T>(this IServiceCollection services, Action<AuditOptionsBuilder> options) where T : DbContext, IAuditDbContext
        {
            services.AddScoped<IAuditTracker<T>, AuditTracker<T>>();
            var optionsBuilder = new AuditOptionsBuilder();
            options.Invoke(optionsBuilder);
            var auditOptions = optionsBuilder.Build();
            if(optionsBuilder.IdentityProvider != null)
            {
                services.TryAddScoped(typeof(IAuditIdentityProvider), optionsBuilder.IdentityProvider);
            }
            else
            {
                services.AddSingleton<IAuditIdentityProvider>(new DefaultIdentityProvider(optionsBuilder.DefaultUser));
            }

            services.AddSingleton(auditOptions);

            return services;
        }
    }
}
