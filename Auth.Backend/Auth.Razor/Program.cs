using Auth.Application.Extensions;
using Auth.Infrastructure.Persistence.Extensions;
using Auth.Infrastructure.Identity.Extensions;
using Web.Razor.Extensions;
using Auth.Infrastructure.Shared.Extensions;

namespace Web.Razor
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add Db contexts

            builder.Services.AddAppDbContext(builder.Configuration);

            builder.Services.AddIdentityDbContext(builder.Configuration);

            // Add identity

            builder.Services.AddIdentityAuth();

            // Add services

            builder.Services.AddInfrastructureIdentityServices();

            builder.Services.AddInfrastructurePersistenceServices();

            builder.Services.AddInfrastructureSharedServices();

            builder.Services.AddWebRazorServices();

            // Add mapping profiles

            builder.Services.AddApplicationMappingProfile();

            builder.Services.AddInfrastructureIdentityMappingProfile();

            builder.Services.AddRazorPages();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapRazorPages();

            app.Run();
        }
    }
}