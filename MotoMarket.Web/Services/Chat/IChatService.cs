using MotoMarket.Web.Models.DTOs;

namespace MotoMarket.Web.Services.Chat
{
    public interface IChatService
    {
        Task<IEnumerable<ConversationDto>> GetMyConversations();
    }
}
