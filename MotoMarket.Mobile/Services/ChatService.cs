using Microsoft.AspNetCore.SignalR.Client;
using MotoMarket.Mobile.Models.Chat;
using System.Net.Http.Json;

namespace MotoMarket.Mobile.Services
{
    public class ChatService
    {
        private HubConnection _hubConnection;

        // Zdarzenie, które poinformuje ViewModel, że przyszła nowa wiadomość
        public event Action<string, string, int?> OnMessageReceived;

        public async Task ConnectAsync()
        {
            var token = await SecureStorage.GetAsync("auth_token");

            // Budujemy połączenie
            _hubConnection = new HubConnectionBuilder()
                .WithUrl($"{Constants.ApiUrl}/chatHub", options =>
                {
                    // WAŻNE: Przekazujemy token, żeby [Authorize] na Hubie zadziałało
                    options.AccessTokenProvider = () => Task.FromResult(token);
                })
                .WithAutomaticReconnect() // Jak zerwie WiFi, spróbuje połączyć ponownie
                .Build();

            // Nasłuchujemy metody "ReceiveMessage" (tę nazwę masz w ChatHub.cs w backendzie!)
            _hubConnection.On<string, string, int?>("ReceiveMessage", (senderId, message, listingId) =>
            {
                // Przekazujemy info do ViewModelu
                OnMessageReceived?.Invoke(senderId, message, listingId);
            });

            try
            {
                await _hubConnection.StartAsync();
                System.Diagnostics.Debug.WriteLine("[SIGNALR] Połączono!");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[SIGNALR ERROR] {ex.Message}");
            }
        }

        public async Task<IEnumerable<ChatMessageDto>> GetHistoryAsync(string otherUserId)
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri(Constants.ApiUrl);

            var token = await SecureStorage.GetAsync("auth_token");
            if (!string.IsNullOrEmpty(token))
            {
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }

            // TWOJE API: [HttpGet("history/{otherUserId}")]
            var response = await client.GetAsync($"api/Chat/history/{otherUserId}");

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<IEnumerable<ChatMessageDto>>();
            }
            return new List<ChatMessageDto>();
        }

        public async Task<IEnumerable<ConversationDto>> GetConversationsAsync()
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri(Constants.ApiUrl);

            var token = await SecureStorage.GetAsync("auth_token");
            if (!string.IsNullOrEmpty(token))
            {
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }

            // TWOJE API: [HttpGet] w ChatController
            var response = await client.GetAsync("api/Chat");

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<IEnumerable<ConversationDto>>();
            }
            return new List<ConversationDto>();
        }

        public async Task SendMessageAsync(string recipientId, string message, int? listingId)
        {
            if (_hubConnection == null || _hubConnection.State != HubConnectionState.Connected) return;

            // Wywołujemy metodę "SendMessage" na serwerze (nazwa metody w ChatHub.cs)
            await _hubConnection.InvokeAsync("SendMessage", recipientId, message, listingId);
        }

        public async Task DisconnectAsync()
        {
            if (_hubConnection != null)
            {
                await _hubConnection.StopAsync();
                await _hubConnection.DisposeAsync();
                _hubConnection = null;
            }
        }
    }
}