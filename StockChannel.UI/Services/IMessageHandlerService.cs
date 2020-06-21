using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using StockChannel.Domain.Entities;
using StockChannel.UI.Models;

namespace StockChannel.UI.Services
{
    public interface IMessageHandlerService
    {
        Task SendMessageAsync(MessageModel model);
        public Task<IEnumerable<ChatMessage>> GetMessagesAsync(int top);
        public Task InsertMessageAsync(MessageModel model);
    }
}