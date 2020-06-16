using System;

namespace StockChannel.UI.Models
{
    public class MessageModel
    {
        public string Content { get; set; }
        public string Sender { get; set; }
        public DateTime SentAt { get; set; }
    }
}