using MotoMarket.Mobile.ViewModels;

namespace MotoMarket.Mobile.Views
{
    public partial class FavoritesPage : ContentPage
    {
        public FavoritesPage()
        {
            InitializeComponent();
            BindingContext = new FavoritesViewModel();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            if (BindingContext is FavoritesViewModel vm)
            {
                await vm.LoadFavoritesCommand.ExecuteAsync(null);
            }
        }
    }
}