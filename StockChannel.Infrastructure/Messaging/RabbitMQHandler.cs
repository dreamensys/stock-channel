using System;
using System.Text;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using StockChannel.Infrastructure.Interfaces;

namespace StockChannel.Infrastructure.Messaging
{
    public class RabbitMQHandler : IQueueHandler
    {
        private IModel _channel;
        public string ReplyQueueName { get; private set; }
        public string QueueName { get; private set; }
        private string _exchangeName = "stockchannel";

        public RabbitMQHandler()
        {
            var factory = new ConnectionFactory();
            factory.HostName = "localhost";
            var connection = factory.CreateConnection();
            _channel = connection.CreateModel();
            _channel.ExchangeDeclare(_exchangeName, ExchangeType.Fanout);
        }

        public void Publish<TMessage>(TMessage message)
        {
            var json = JsonConvert.SerializeObject(message);
            var body = Encoding.UTF8.GetBytes(json);

            _channel.BasicPublish(_exchangeName, "", null, body);
        }

        public void Consume<TMessage>(string queueName, Action<TMessage> action)
        {
            
            queueName = _channel.QueueDeclare().QueueName;
            _channel.QueueBind(queue: queueName, exchange: _exchangeName, routingKey: "");
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (ch, ea) =>
            {
                var body = ea.Body.ToArray();
                var json = Encoding.UTF8.GetString(body);
                var message = JsonConvert.DeserializeObject<TMessage>(json);
                action(message);
            };
            _channel.BasicConsume(queueName, true, consumer);
        }
    }
}