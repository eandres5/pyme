using pyme.Models;
using pyme.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace pyme.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Dashboard : ContentPage
    {
        public Dashboard()
        {
            InitializeComponent();
            BindingContext = new ResumenComprobantesViewModel();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            MessagingCenter.Subscribe<ResumenComprobantesViewModel, string>(this, "MostrarAlerta", async (sender, mensaje) =>
            {
                await DisplayAlert("Advertencia", mensaje, "Aceptar");
            });
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            MessagingCenter.Unsubscribe<ResumenComprobantesViewModel, string>(this, "MostrarAlerta");
        }

    }
}