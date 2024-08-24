using Auth.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Auth.Application.Interfaces.Persistence;
using Auth.Infrastructure.Persistence.Data;
using Auth.Infrastructure.Persistence.Interceptors;

namespace Auth.Infrastructure.Persistence.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddAppDbContext(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(
                    configuration.GetConnectionString("DefaultConnection"),
                    b => b.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName)));
        }

        public static void AddInfrastructurePersistenceServices(this IServiceCollection services)
        {
            services.AddScoped<AuditableEntitySaveChangesInterceptor>();
            services.AddScoped(typeof(IRepositoryBase<>), typeof(RepositoryBase<>));
            services.AddScoped<IUnitOfWork, UnitOfWork>();
        }
    }
}
