using MotoMarket.Mobile.ViewModels;

namespace MotoMarket.Mobile.Views;

public partial class RegisterPage : ContentPage
{
	public RegisterPage()
	{
		InitializeComponent();
        BindingContext = new RegisterViewModel();
    }
}