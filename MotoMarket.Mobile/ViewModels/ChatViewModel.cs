using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MotoMarket.Mobile.Models.Chat;
using MotoMarket.Mobile.Services;
using System.Collections.ObjectModel;

namespace MotoMarket.Mobile.ViewModels
{
    public partial class ChatViewModel : ObservableObject
    {
        private readonly ChatService _chatService;
        private readonly string _recipientId;
        private readonly int? _listingId;
        private string _myUserId; // Musimy wiedzieć kim jesteśmy, żeby rozróżnić dymki

        public ChatViewModel(string recipientId, int? listingId)
        {
            _recipientId = recipientId;
            _listingId = listingId;
            _chatService = new ChatService();

            // Podpinamy się pod zdarzenie odbioru wiadomości
            _chatService.OnMessageReceived += ChatService_OnMessageReceived;

            // Inicjalizacja
            InitializeAsync();
        }

        public ObservableCollection<ChatMessageDto> Messages { get; } = new();

        [ObservableProperty] string newMessageText;

        private async void InitializeAsync()
        {
            _myUserId = await SecureStorage.GetAsync("user_id");

            // 1. Pobieramy historię z Twojego endpointu
            var historyDtos = await _chatService.GetHistoryAsync(_recipientId);

            MainThread.BeginInvokeOnMainThread(() =>
            {
                Messages.Clear();
                foreach (var dto in historyDtos)
                {
                    // Mapujemy DTO z API na nasz lokalny model widoku
                    Messages.Add(new ChatMessageDto
                    {
                        SenderId = dto.SenderId,   // Upewnij się, że DTO ma to pole
                        Content = dto.Content,     // Upewnij się, że DTO ma to pole
                        SentAt = dto.SentAt,       // Upewnij się, że DTO ma to pole

                        // Kluczowe: Sprawdzamy czy to moja wiadomość
                        IsMine = dto.SenderId == _myUserId
                    });
                }

                // Tutaj ewentualnie kod do przewijania na dół
            });

            // 2. Łączymy z SignalR
            await _chatService.ConnectAsync();
        }

        private void ChatService_OnMessageReceived(string senderId, string message, int? listingId)
        {
            // WAŻNE: SignalR działa w tle, a UI można zmieniać tylko z wątku głównego (MainThread)
            MainThread.BeginInvokeOnMainThread(() =>
            {
                // Filtrujemy, żeby pokazywać tylko wiadomości z tej konkretnej rozmowy
                // (W wersji pro powinieneś mieć RoomId, ale na inżynierkę wystarczy sprawdzenie ID)
                bool isRelevant = (senderId == _recipientId) || (senderId == _myUserId);

                if (isRelevant)
                {
                    Messages.Add(new ChatMessageDto
                    {
                        SenderId = senderId,
                        Content = message,
                        SentAt = DateTime.Now,
                        IsMine = senderId == _myUserId
                    });
                }
            });
        }

        [RelayCommand]
        async Task SendAsync()
        {
            if (string.IsNullOrWhiteSpace(NewMessageText)) return;

            // Wyślij przez SignalR
            await _chatService.SendMessageAsync(_recipientId, NewMessageText, _listingId);

            // SignalR (Clients.User) wysyła do odbiorcy.
            // Jeśli backend nie odsyła do nadawcy (Clients.Caller), musimy dodać wiadomość lokalnie:
            Messages.Add(new ChatMessageDto
            {
                SenderId = _myUserId,
                Content = NewMessageText,
                SentAt = DateTime.Now,
                IsMine = true
            });

            NewMessageText = string.Empty; // Wyczyść pole
        }

        [RelayCommand]
        async Task GoBackAsync()
        {
            await _chatService.DisconnectAsync();
            await Application.Current.MainPage.Navigation.PopAsync();
        }
    }
}