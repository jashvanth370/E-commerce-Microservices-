using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Microsoft.AspNetCore.Builder;
using eCommerce.SharedLibrary.Middleware;

namespace eCommerce.SharedLibrary.DependencyInjection
{
    public static class SharedServiceContainer
    {
        public static IServiceCollection AddSharedServices<TContext>
            (this IServiceCollection services, IConfiguration configuration, string fileName) where TContext : DbContext
        {
            services.AddDbContext<TContext>(option => option.UseSqlServer(
                configuration.GetConnectionString("eCommerceConnection"), sqlserverOptions =>
                sqlserverOptions.EnableRetryOnFailure()));

            //configure serilog logging
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.Debug()
                .WriteTo.Console()
                .WriteTo.File(path: $"{fileName}-.text",
                restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Information,
                outputTemplate: "[{Timestamp:yyyy:mm:dd HH:mm:ss.fff zzz} [{Level:u3}] {message:lj}{NewLine}{Exception}",
                rollingInterval: RollingInterval.Day)
                .CreateLogger();

            //Add jwt authentication scheme
            JWTAuthenticationScheme.AddJWTAuthentication(services, configuration);
            return services;
        }

        public static IApplicationBuilder UseSharedServices(this IApplicationBuilder app)
        {
            //Use global exception handling and listen to only api gateway middlewares
            app.UseMiddleware<GlobalException>();
            //listen to only api gateway
            //app.UseMiddleware<ListenToOnlyApiGateway>();
            return app;
        }
    }
}
