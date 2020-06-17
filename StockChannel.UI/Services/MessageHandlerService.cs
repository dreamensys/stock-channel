using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using StockChannel.Domain.Entities;
using StockChannel.Infrastructure.Interfaces;
using StockChannel.UI.DataAccess;
using StockChannel.UI.Models;
using StockChannel.UI.Repositories;

namespace StockChannel.UI.Services
{
    public class MessageHandlerService : IMessageHandlerService
    {
        private string _commandValue;
        private string _commandUrl;
        private readonly IQueueHandler _queueHandler;
        private readonly IMessageRepository _messageRepository;
        private readonly IAPIRequestHandler _apiRequestHandler;
        public MessageHandlerService(IQueueHandler queueHandler, IAPIRequestHandler apiRequestHandler, IMessageRepository messageRepository)
        {
            _queueHandler = queueHandler ?? throw new ArgumentNullException(nameof(queueHandler));
            _apiRequestHandler = apiRequestHandler ?? throw new ArgumentNullException(nameof(apiRequestHandler));
            _messageRepository = messageRepository ?? throw new ArgumentNullException(nameof(messageRepository));
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
                    var botResponse = await _apiRequestHandler.Get<string>(_commandUrl, "");
                    messagePayload.Content = botResponse;
                }
                catch (Exception e)
                {
                    messagePayload.Content = "StockBot: An error occurred unexpectedly.";
                }
            }
            else
            {
                _messageRepository.InsertMessageAsync(messagePayload);
            }
            _queueHandler.Publish(messagePayload);
        }

        public async Task<IEnumerable<ChatMessage>> GetMessages(int top)
        {
            var results = await _messageRepository.GetAllMessagesAsync();
            var sorted = results.OrderByDescending(m => m.SentAt).Take(top);
            return sorted;
        }

        public async Task InsertMessage(MessageModel model)
        {
            var message = new ChatMessage()
            {
                Sender = model.Sender,
                SentAt = model.SentAt,
                Content = model.Content
            };
            await _messageRepository.InsertMessageAsync(message);
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