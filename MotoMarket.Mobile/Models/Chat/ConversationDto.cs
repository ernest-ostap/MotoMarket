using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MotoMarket.Mobile.Models.Chat
{
    public class ConversationDto
    {
        public string OtherUserId { get; set; } = string.Empty; // ID rozmówcy
        public string OtherUserName { get; set; } = string.Empty; // Nazwa rozmówcy
        public string LastMessage { get; set; } = string.Empty; // Podgląd ostatniej wiadomości
        public DateTime LastMessageDate { get; set; }           // Kiedy
        public int? ListingId { get; set; }                     // Opcjonalnie: o jakie auto chodzi
    }
}
