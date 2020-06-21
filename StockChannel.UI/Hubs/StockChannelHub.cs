using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR.Hubs;
using Microsoft.AspNetCore.SignalR;
using StockChannel.UI.Models;
using StockChannel.UI.Services;

namespace StockChannel.UI.Hubs
{
    public class StockChannelHub : Hub
    {
        private readonly IMessageHandlerService _messageHandlerService;
        public static string HubName { get; set; } = "StockChannelHub";
        private static List<MessageModel> _messagesList = new List<MessageModel>();
        
        public StockChannelHub(IMessageHandlerService messageHandlerService)
        {
            _messageHandlerService = messageHandlerService ?? throw new ArgumentNullException(nameof(messageHandlerService));
        }

        public async Task SendMessage(MessageModel newMessage)
        {
            try
            {
                await _messageHandlerService.SendMessageAsync(newMessage);
            }
            catch
            {
                // log message
            }
            
        }
    }
}