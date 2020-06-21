using System.Collections.Generic;
using Microsoft.AspNet.Identity.EntityFramework;

namespace StockChannel.UI.Models
{
    public class ChatUser : IdentityUser
    {
        public ChatUser()
        {
            //Chat = new HashSet<MessageModel>();
        }

        public virtual ICollection<MessageModel> Messages { get; set; }
    }
}