using Auth.Application.Interfaces.Identity;
using Auth.Application.Interfaces.Persistence;
using Auth.Identity.Extensions;
using Auth.Infrastructure.Identity.Data;
using Auth.Infrastructure.Identity.Mapper;
using Auth.Infrastructure.Identity.Models;
using Auth.Infrastructure.Identity.Services;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Auth.Infrastructure.Identity.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddIdentityDbContext(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<AppIdentityDbContext>(options =>
                options.UseNpgsql(
                    configuration.GetConnectionString("DefaultConnection"),
                    b => b.MigrationsAssembly(typeof(AppIdentityDbContext).Assembly.FullName)));
        }

        public static void AddIdentityAuth(this IServiceCollection services)
        {
            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<AppIdentityDbContext>()
                .AddDefaultTokenProviders();
        }

        public static void AddInfrastructureIdentityServices(this IServiceCollection services)
        {
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IRabbitMqService, RabbitMqService>();
            services.AddSingleton<RabbitMqConnectionService>();
        }

        public static void AddInfrastructureIdentityMappingProfile(this IServiceCollection services)
        {
            MapperConfiguration mappingConfig = new(mc =>
            {
                mc.AddProfile(new InfrastructureIdentityProfile());
            });

            services.AddSingleton(mappingConfig.CreateMapper());
        }
    }
}
