using pyme.ViewModels;
using System.ComponentModel;
using Xamarin.Forms;

namespace pyme.Views
{
    public partial class ItemDetailPage : ContentPage
    {
        public ItemDetailPage()
        {
            InitializeComponent();
            BindingContext = new ItemDetailViewModel();
        }
    }
}