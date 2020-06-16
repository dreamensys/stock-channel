using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StockChannel.Infrastructure.Interfaces;
using StockChannel.Infrastructure.Messaging;
using StockChannel.Infrastructure.Network;

namespace StockChannel.Infrastructure.Extensions
{
    public static class MessagingExtensions
    {
        public static IServiceCollection AddMessagingConfiguration(
            this IServiceCollection services,
            IConfiguration configuration,
            string environment)
        
        {
            services.AddScoped<IQueueHandler, RabbitMQHandler>();

            return services;
        }
    }
}