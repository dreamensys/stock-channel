using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using StockChannel.Domain.Entities;
using StockChannel.UI.DataAccess;
using StockChannel.UI.Models;

namespace StockChannel.UI.Repositories
{
    public class MessageRepository : IMessageRepository
    {
        private readonly AppDbContext _context;

        public MessageRepository(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IEnumerable<ChatMessage>> GetAllMessagesAsync()
        {
            List<ChatMessage> messages = new List<ChatMessage>();
            messages = await _context.Messages.ToListAsync();
            return messages;
        }

        public async Task InsertMessageAsync(ChatMessage model)
        {
            try
            {
                await _context.Messages.AddAsync(model);
                _context.SaveChanges();
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

    }
}