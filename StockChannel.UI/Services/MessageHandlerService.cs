using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using StockChannel.Domain.Entities;
using StockChannel.Infrastructure.Interfaces;
using StockChannel.UI.Models;

namespace StockChannel.UI.Services
{
    public class MessageHandlerService : IMessageHandlerService
    {
        private string _commandValue;
        private string _commandUrl;
        private readonly IQueueHandler _queueHandler;
        private readonly IAPIRequestHandler _apiRequestHandler;
        public MessageHandlerService(IQueueHandler queueHandler, IAPIRequestHandler apiRequestHandler)
        {
            _queueHandler = queueHandler ?? throw new ArgumentNullException(nameof(queueHandler));
            _apiRequestHandler = apiRequestHandler ?? throw new ArgumentNullException(nameof(apiRequestHandler));
        }
        public async Task SendMessageAsync(MessageModel model)
        {
            var messagePayload = new ChatMessage()
            {
                Content = model.Content,
                Sender = model.Sender,
                SentAt = model.SentAt
            };

            var valid = IsValidBotCommand(messagePayload.Content);
            if (valid)
            {
                _commandUrl = $"{_commandUrl}{_commandValue}";
                try
                {
                    var botResponse = await _apiRequestHandler.Get<string>(_commandUrl);
                    messagePayload.Content = botResponse;
                }
                catch (Exception e)
                {
                    messagePayload.Content = "StockBot: An error occurred unexpectedly.";
                }
                
            }

            _queueHandler.Publish(messagePayload);
        }

        public void Init(Action<MessageModel> callback)
        {
            _queueHandler.Consume<MessageModel>("", callback);
        }
        private bool IsValidBotCommand(string messageContent)
        {
            if (!messageContent.StartsWith("/"))
                return false;

            var body = messageContent.Substring(1);
            if (body.Contains("/"))
                return false;
            
            var equalMarkIndex = body.IndexOf('=');
            var commandName = body.Substring(0,equalMarkIndex);
            var bots = GetInstalledBots();
            bots.TryGetValue(commandName, out _commandUrl);
            if (_commandUrl == string.Empty)
                return false;
            
            _commandValue = body.Substring(equalMarkIndex + 1).Trim();

            return true;
        }
    
        private Dictionary<string, string> GetInstalledBots() => new Dictionary<string, string>()
        {
            {"stock", "http://localhost:5000/api/bots/stocks/"}
        };

    }
}