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
            _messageHandlerService.Init(y => BroadcastMessage(y));
        }

        public async Task BroadcastMessage(MessageModel model)
        {
            await Clients.All.SendAsync("NewMessage", model);
        }
        
        public async Task GetAllMessages()
        {
            await Clients.All.SendAsync("ReceiveMessage", _messagesList);
        }

        public async Task SendMessage(MessageModel newMessage)
        {
            try
            {
                var list= await _messageHandlerService.GetMessages(50);
                _messagesList.Add(newMessage);
                // await Clients.All.SendAsync("NewMessage", newMessage);
                //await Clients.All.SendAsync("NewMessageList", list);
                await _messageHandlerService.SendMessageAsync(newMessage);
            }
            catch
            {
                newMessage.Content = "Broadcast failed!";
                _messageHandlerService.SendMessageAsync(newMessage);
            }
            
        }

        
    }
}