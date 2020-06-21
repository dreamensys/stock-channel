using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using StockChannel.Domain.Entities;
using StockChannel.Infrastructure.Interfaces;
using StockChannel.UI.Hubs;
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
        private readonly IServiceProvider _serviceProvider;
        public MessageHandlerService(IQueueHandler queueHandler, IAPIRequestHandler apiRequestHandler, IMessageRepository messageRepository, IServiceProvider serviceProvider)
        {
            _queueHandler = queueHandler ?? throw new ArgumentNullException(nameof(queueHandler));
            _apiRequestHandler = apiRequestHandler ?? throw new ArgumentNullException(nameof(apiRequestHandler));
            _messageRepository = messageRepository ?? throw new ArgumentNullException(nameof(messageRepository));
            _serviceProvider = serviceProvider;
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
                await QueryBots(messagePayload);
            }
            else
            {
                await PersistMessage(messagePayload);
            }

            var published = PublishDataOnQueue(messagePayload);
            if (!published) {
                var chatHub = (IHubContext<StockChannelHub>)_serviceProvider.GetService(typeof(IHubContext<StockChannelHub>));
                await chatHub.Clients.All.SendAsync("messageReceived", messagePayload);
            }
            
        }

        public async Task<IEnumerable<ChatMessage>> GetMessagesAsync(int top)
        {
            var results = await _messageRepository.GetAllMessagesAsync();
            var sorted = results.OrderByDescending(m => m.SentAt).Take(top);
            return sorted;
        }

        public async Task InsertMessageAsync(MessageModel model)
        {
            var message = new ChatMessage()
            {
                Sender = model.Sender,
                SentAt = model.SentAt,
                Content = model.Content
            };
            await _messageRepository.InsertMessageAsync(message);
        }


        private bool PublishDataOnQueue(ChatMessage messagePayload)
        {
            try
            {
                _queueHandler.Publish(messagePayload);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

       private async Task<bool> QueryBots(ChatMessage messagePayload)
       {
            _commandUrl = $"{_commandUrl}{_commandValue}";
            messagePayload.Sender = "StockBot";
            try
            {
                // Connecting to StockBot API
                var botResponse = await _apiRequestHandler.Get<string>(_commandUrl, "");
                messagePayload.Content = botResponse;
                return true;
            }
            catch (Exception e)
            {
                messagePayload.Content = "An error occurred unexpectedly.";
                return false;
            }
        }
        private async Task<bool> PersistMessage(ChatMessage messagePayload)
        {
            try
            {
                // Storing the message into the database
                await _messageRepository.InsertMessageAsync(messagePayload);
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
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
            {"stock", "http://localhost:17028/api/bots/stock/"}
        };

    }
}