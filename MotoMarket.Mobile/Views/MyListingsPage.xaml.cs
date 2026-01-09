namespace MotoMarket.Mobile.Views;

public partial class MyListingsPage : ContentPage
{
	public MyListingsPage()
	{
		InitializeComponent();
        BindingContext = new ViewModels.MyListingsViewModel();
    }
}