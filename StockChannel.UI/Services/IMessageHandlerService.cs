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
        public Task<IEnumerable<ChatMessage>> GetMessages(int top);
        public Task InsertMessage(MessageModel model);
        void Init(Action<MessageModel> callback);
    }
}