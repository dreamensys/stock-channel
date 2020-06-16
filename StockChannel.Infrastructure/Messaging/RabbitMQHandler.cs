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
            // _channel.QueueDeclare($"{exchangeName}-broadcast", false, false, false, null);
            // _channel.QueueBind($"{exchangeName}-broadcast", exchangeName, "");

            // if (createReplyQueue)
            // {
            //     var result = _model.QueueDeclare("reply-" + Environment.MachineName, false, false, true, null);
            //     ReplyQueueName = result.QueueName;
            // }
        }

        public void Publish<TMessage>(TMessage message)
        {
            var properties = _channel.CreateBasicProperties();
            properties.Type = typeof(TMessage).Name;
            // if (!string.IsNullOrEmpty(ReplyQueueName))
            // {
            //     properties.ReplyTo = ReplyQueueName;
            // }

            var json = JsonConvert.SerializeObject(message);
            var body = Encoding.UTF8.GetBytes(json);

            //exchange: "logs"

            // _channel.BasicPublish(exchangeName, queueName, properties, body);
            _channel.BasicPublish(_exchangeName, "", null, body);
        }

        public void Consume<TMessage>(string queueName, Action<TMessage> action)
        {
            
            queueName = _channel.QueueDeclare().QueueName;
            _channel.QueueBind(queue: queueName, exchange: _exchangeName, routingKey: "");
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (ch, ea) =>
            {
                // _lastMessageReplyQueueName = ea.BasicProperties.ReplyTo;
                // if (ea.BasicProperties.Type == typeof(TMessage).Name)
                // {
                    var body = ea.Body.ToArray();
                    var json = Encoding.UTF8.GetString(body);
                    var message = JsonConvert.DeserializeObject<TMessage>(json);
                    action(message);
                    // _channel.BasicAck(ea.DeliveryTag, false);
                // }
                // else
                // {
                //     _model.BasicNack(ea.DeliveryTag, false, true);
                // }
            };
            _channel.BasicConsume(queueName, true, consumer);
        }
    }
}