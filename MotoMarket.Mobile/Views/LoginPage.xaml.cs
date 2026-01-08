using MotoMarket.Mobile.ViewModels;

namespace MotoMarket.Mobile.Views
{
    public partial class LoginPage : ContentPage
    {
        public LoginPage()
        {
            InitializeComponent();
            // moment w którym mówimy widokowi, ¿e jego kontekst danych to LoginViewModel
            BindingContext = new LoginViewModel();
        }
    }
}