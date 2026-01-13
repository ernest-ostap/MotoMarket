using MotoMarket.Mobile.ViewModels;

namespace MotoMarket.Mobile.Views
{
    public partial class CreateListingPage : ContentPage
    {
        public CreateListingPage()
        {
            InitializeComponent();
            BindingContext = new CreateListingViewModel();
        }
    }
}