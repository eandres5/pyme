using Newtonsoft.Json;
using pyme.Models;
using pyme.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using ZXing.Net.Mobile.Forms;

namespace pyme.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Stock : ContentPage
    {
        public Stock()
        {
            InitializeComponent();
            BindingContext = new StockViewModel();
        }

        private async void btnScannerQR_Clicked(object sender, EventArgs e) {
            try
            {
                var scanner = new ZXing.Mobile.MobileBarcodeScanner();
                var result = await scanner.Scan();
                if (result != null) {
                    
                    string qrData = result.Text;
                    String[] id = qrData.Split('-');

                    var viewModel = BindingContext as StockViewModel;
                    viewModel?.GetProductoById(id[0]);
                }

            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message.ToString(), "Ok");
            }
        }

        public async Task GetProducto(string idProducto)
        {
            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
            };

            var client = new HttpClient(handler);

            // Cambia para produccion
            var response = await client.GetAsync("https://192.168.200.5:45455/api/Producto/getProducto/1");

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                ProductoDTO pro = JsonConvert.DeserializeObject<ProductoDTO>(json);

                //return JsonConvert.DeserializeObject<ProductoDTO>(json);
            }
            else
            {
                throw new Exception("Error al obtener datos del API");
            }

        }

    }
}