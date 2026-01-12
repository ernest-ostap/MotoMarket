namespace MotoMarket.Mobile.Views;

public partial class ChatPage : ContentPage
{
    public ChatPage(string recipientId, int? listingId)
    {
        InitializeComponent();
        BindingContext = new ViewModels.ChatViewModel(recipientId, listingId);
    }
}