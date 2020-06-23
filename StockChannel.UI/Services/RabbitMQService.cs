using System;
using System.Text;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using StockChannel.Domain.Entities;
using StockChannel.UI.Hubs;
public class RabbitMQService : IRabbitMQService
{
    private readonly ConnectionFactory _factory;
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private readonly string exchangeName = "stockchannel";
    private readonly IServiceProvider _serviceProvider;

    public RabbitMQService(IServiceProvider serviceProvider)
    {
        try
        {
            _factory = new ConnectionFactory() { HostName = "localhost" };
            _connection = _factory.CreateConnection();
            _channel = _connection.CreateModel();
            _serviceProvider = serviceProvider;
        }
        catch (Exception)
        {

            //TODO set a log here
        }

    }

    public virtual void Connect()
    {
        if (_channel == null)
            throw new ArgumentNullException("The application could not stablish a connection to the queue");

        var queueName = _channel.QueueDeclare().QueueName;
        _channel.ExchangeDeclare(exchangeName, ExchangeType.Fanout);
        _channel.QueueBind(queue: queueName, exchange: exchangeName, routingKey: "");
        var consumer = new EventingBasicConsumer(_channel);

        consumer.Received += delegate (object model, BasicDeliverEventArgs ea) {
            var body = ea.Body.ToArray();
            var json = Encoding.UTF8.GetString(body);
            var message = JsonConvert.DeserializeObject<ChatMessage>(json);
            var chatHub = (IHubContext<StockChannelHub>)_serviceProvider.GetService(typeof(IHubContext<StockChannelHub>));
            chatHub.Clients.All.SendAsync("messageReceived", message);
        };

        _channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);
    }

}
public interface IRabbitMQService
{
    void Connect();
}