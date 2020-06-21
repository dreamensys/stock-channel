using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StockChannel.Infrastructure.Interfaces;
using StockChannel.Infrastructure.Network;

namespace StockChannel.Infrastructure.Extensions
{
    public static class NetworkExtensions
    {
        public static IServiceCollection AddNetworkConfiguration(
            this IServiceCollection services,
            IConfiguration configuration,
            string environment)
        
        {
            services.AddHttpClient();
            services.AddScoped<IAPIRequestHandler, ApiRequestHandler>();

            return services;
        }
    }
}