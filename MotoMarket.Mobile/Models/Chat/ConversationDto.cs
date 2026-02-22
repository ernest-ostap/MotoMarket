using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MotoMarket.Mobile.Models.Chat
{
    public class ConversationDto
    {
        public string OtherUserId { get; set; } = string.Empty;
        public string OtherUserName { get; set; } = string.Empty; 
        public string LastMessage { get; set; } = string.Empty; 
        public DateTime LastMessageDate { get; set; }           
        public int? ListingId { get; set; }                     
    }
}
