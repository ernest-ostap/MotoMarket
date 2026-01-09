using MotoMarket.Mobile.ViewModels;

namespace MotoMarket.Mobile.Views
{
    public partial class ListingDetailPage : ContentPage
    {
        // Konstruktor przyjmuj¹cy ID
        public ListingDetailPage(int listingId)
        {
            InitializeComponent();
            BindingContext = new ListingDetailViewModel(listingId);
        }
    }
}