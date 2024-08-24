using Auth.Application.Interfaces.Application;
using Auth.Application.Mapper;
using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace Auth.Application.Extensions
{
    public static class ServiceCollectionExtension
    {
        public static void AddApplicationMappingProfile(this IServiceCollection services)
        {
            MapperConfiguration mappingConfig = new(mc =>
            {
                mc.AddProfile(new ApplicationProfile());
            });

            services.AddSingleton(mappingConfig.CreateMapper());
        }
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            //Регистрация других сервисов приложения
            services.AddScoped<ICurrentUserService, CurrentUserService>();
            services.AddScoped<IDateTimeService, DateTimeService>();

            return services;
        }
        public class CurrentUserService : ICurrentUserService
        {
            private readonly IHttpContextAccessor httpContextAccessor;
            public CurrentUserService(IHttpContextAccessor httpContextAccessor)
            {
                this.httpContextAccessor = httpContextAccessor;
            }
            public string? UserId => httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }
        public class DateTimeService : IDateTimeService
        {
            public DateTime Now => DateTime.UtcNow;
        }
    }
}
