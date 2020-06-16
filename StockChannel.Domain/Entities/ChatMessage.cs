using System;

namespace StockChannel.Domain.Entities
{
    public class ChatMessage
    {
        public string Content { get; set; }
        public string Sender { get; set; }
        public DateTime SentAt { get; set; }
    }
}