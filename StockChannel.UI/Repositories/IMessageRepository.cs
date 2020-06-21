using System.Collections.Generic;
using System.Threading.Tasks;
using StockChannel.Domain.Entities;
using StockChannel.UI.Models;

namespace StockChannel.UI.Repositories
{
    public interface IMessageRepository
    {
        Task<IEnumerable<ChatMessage>> GetAllMessagesAsync();
        Task InsertMessageAsync(ChatMessage model);
    }
}