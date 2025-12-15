using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MotoMarket.Application.Chat.Queries
{
    public class ChatMessageDto
    {
        public int Id { get; set; }
        public string SenderId { get; set; }
        public string Content { get; set; }
        public DateTime SentAt { get; set; }
        public bool IsMine { get; set; } // Czy to ja wysłałem? (Przydatne dla CSS: dymek z lewej czy z prawej)
    }
}
