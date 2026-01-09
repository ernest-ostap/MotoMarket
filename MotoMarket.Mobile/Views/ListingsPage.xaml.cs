namespace MotoMarket.Mobile.Views;

public partial class ListingsPage : ContentPage
{
	public ListingsPage()
	{
		InitializeComponent();
        BindingContext = new ViewModels.ListingsViewModel();
    }
}