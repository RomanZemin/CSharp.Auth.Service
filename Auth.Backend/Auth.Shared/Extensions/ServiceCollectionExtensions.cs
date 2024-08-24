using Auth.Application.Interfaces.Application;
using Auth.Infrastructure.Shared.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Auth.Infrastructure.Shared.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddInfrastructureSharedServices(this IServiceCollection services)
        {
            services.AddScoped<IDateTimeService, DateTimeService>();
        }
    }
}
