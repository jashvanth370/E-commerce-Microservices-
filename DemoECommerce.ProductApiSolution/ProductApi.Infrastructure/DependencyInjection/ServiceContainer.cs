using eCommerce.SharedLibrary.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProductApi.Application.Interfaces;
using ProductApi.Infrastructure.Data;
using ProductApi.Infrastructure.Repositories;

namespace ProductApi.Infrastructure.DependencyInjection
{
    public static class ServiceContainer
    {
        public static IServiceCollection AddIInfrastructureService(this IServiceCollection services, IConfiguration config)
        {
            //Add database connectivity
            //Add authendication scheme
            SharedServiceContainer.AddSharedServices<ProductDbContext>(services, config, config["MySerilog:FileName"]);

            //create dependency injection for repositories
            services.AddScoped<IProduct, ProductRepository>();
            return services;
        }

        public static IApplicationBuilder UseInfrastructurePolicy(this IApplicationBuilder app)
        {
            //Register middleware for exception handling, logging, etc.
            //G;obal exception: handling external errors
            //Listen to only Api gateway: blocks all outsider calls

            SharedServiceContainer.UseSharedServices(app);
            return app;
        }
    }
}
