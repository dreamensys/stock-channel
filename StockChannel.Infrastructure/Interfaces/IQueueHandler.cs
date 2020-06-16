using System;

namespace StockChannel.Infrastructure.Interfaces
{
    public interface IQueueHandler
    {
        void Publish<TMessage>(TMessage message);
        void Consume<TMessage>(string queueName, Action<TMessage> action);
    }
}