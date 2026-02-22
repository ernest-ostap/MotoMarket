using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MotoMarket.Mobile.Models.Chat;
using MotoMarket.Mobile.Services;
using System.Collections.ObjectModel;

namespace MotoMarket.Mobile.ViewModels
{
    public partial class MyChatsViewModel : ObservableObject
    {
        private readonly ChatService _chatService;

        public MyChatsViewModel()
        {
            _chatService = new ChatService();
        }

        public ObservableCollection<ConversationDto> Conversations { get; } = new();

        [RelayCommand]
        async Task LoadConversationsAsync()
        {
            var items = await _chatService.GetConversationsAsync();
            Conversations.Clear();
            foreach (var item in items) Conversations.Add(item);
        }

        [RelayCommand]
        async Task OpenChatAsync(ConversationDto conversation)
        {
            await Application.Current.MainPage.Navigation.PushAsync(
                new Views.ChatPage(conversation.OtherUserId , null)); 
        }
    }
}