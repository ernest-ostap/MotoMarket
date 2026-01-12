using MotoMarket.Mobile.ViewModels;

namespace MotoMarket.Mobile.Views
{
    public partial class MyChatsPage : ContentPage
    {
        public MyChatsPage()
        {
            InitializeComponent();
            BindingContext = new MyChatsViewModel();
        }

        // To zastêpuje EventToCommandBehavior
        protected override async void OnAppearing()
        {
            base.OnAppearing();

            // Rzutujemy BindingContext na nasz ViewModel
            if (BindingContext is MyChatsViewModel vm)
            {
                // Uruchamiamy komendê ³adowania
                await vm.LoadConversationsCommand.ExecuteAsync(null);
            }
        }
    }
}