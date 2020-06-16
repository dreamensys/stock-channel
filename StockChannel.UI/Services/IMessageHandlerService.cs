using System;
using System.Threading.Tasks;
using StockChannel.UI.Models;

namespace StockChannel.UI.Services
{
    public interface IMessageHandlerService
    {
        Task SendMessageAsync(MessageModel model);
        void Init(Action<MessageModel> callback);
    }
}