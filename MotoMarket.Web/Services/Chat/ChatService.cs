using System.Net.Http.Headers;
using System.Text.Json;
using MotoMarket.Web.Models.DTOs;

namespace MotoMarket.Web.Services.Chat
{
    public class ChatService : IChatService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiBaseUrl;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ChatService(HttpClient httpClient, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClient;
            _apiBaseUrl = configuration["ApiUrl"] ?? "https://localhost:7072";
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IEnumerable<ConversationDto>> GetMyConversations()
        {
            // 1. Pobieramy Token, żeby API wiedziało czyje rozmowy pobrać
            var token = _httpContextAccessor.HttpContext?.User.FindFirst("JWT")?.Value;
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            // 2. Strzał do API
            var response = await _httpClient.GetAsync($"{_apiBaseUrl}/api/Chat"); // Ten endpoint stworzyliśmy w API (GetMyConversations)

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                return JsonSerializer.Deserialize<IEnumerable<ConversationDto>>(json, options) ?? new List<ConversationDto>();
            }

            return new List<ConversationDto>();
        }

        public async Task<int> GetUnreadCount()
        {
            // 1. Pobieramy token (standardowa procedura)
            var token = _httpContextAccessor.HttpContext?.User.FindFirst("JWT")?.Value;
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            // 2. Strzał do API
            // Upewnij się, że w API masz endpoint: [HttpGet("unread-count")]
            var response = await _httpClient.GetAsync($"{_apiBaseUrl}/api/Chat/unread-count");

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                // API zwraca zwykłego inta (np. "5"), więc parsujemy
                if (int.TryParse(json, out int count))
                {
                    return count;
                }
            }

            return 0; // Jak błąd lub brak, zwracamy 0
        }
    }
}