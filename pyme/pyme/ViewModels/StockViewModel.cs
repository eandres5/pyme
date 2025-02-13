using pyme.Models;
using pyme.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Xamarin.Forms;
using static System.Net.Mime.MediaTypeNames;

namespace pyme.ViewModels
{
    class StockViewModel : BindableObject
    {
        private readonly StockService _stockService;
        private ProductoDTO _producto;
        private string stock;

        public StockViewModel()
        {
            _stockService = new StockService();
            SaveCommand = new Command(OnSaveAsync);
            CancelCommand = new Command(OnCancel);
            stock = "";
            Producto = new ProductoDTO
            {
                stock = 0,
                nombreProducto = string.Empty,
                descripcion = string.Empty,
                precio = 0
            };

        }

        private async void OnSaveAsync(object obj)
        {
            if (Producto.stock < 0)
            {
                await Xamarin.Forms.Application.Current.MainPage.DisplayAlert("Error", "Debe ingresar un stock válido", "Aceptar");
            }
            else if (String.IsNullOrWhiteSpace(Producto.nombreProducto))
            {
                await Xamarin.Forms.Application.Current.MainPage.DisplayAlert("Error", "Debe seleccionar un producto", "Aceptar");
            }
            else
            {
                try
                {

                    var productoAsString = new
                    {
                        idProducto = Producto.idProducto.ToString(),
                        nombreProducto = Producto.nombreProducto,
                        descripcion = Producto.descripcion,
                        stock = Producto.stock?.ToString(),
                        precio = Producto.precio?.ToString(),
                        nombreCategoria = Producto.nombreCategoria,
                        observacion = Producto.observacion,
                        idProveedor = "1"
                    };

                    // Serializar el objeto Producto a JSON
                    string productoDTO = System.Text.Json.JsonSerializer.Serialize(productoAsString);

                    // Crear cliente HTTP
                    using (var client = new HttpClient())
                    {
                        client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                        // URL del API
                        var url = "https://www.pymesecuador.org/api/Producto/updateProducto";

                        // Crear contenido de la solicitud
                        var content = new StringContent(productoDTO, Encoding.UTF8, "application/json");

                        // Realizar la solicitud PUT
                        var response = await client.PutAsync(url, content);

                        // Verificar si la solicitud fue exitosa
                        if (response.IsSuccessStatusCode)
                        {
                            OnCancel();
                            await Xamarin.Forms.Application.Current.MainPage.DisplayAlert("Éxito", "Producto actualizado correctamente", "Aceptar");
                        }
                        else
                        {
                            var errorMessage = await response.Content.ReadAsStringAsync();
                            await Xamarin.Forms.Application.Current.MainPage.DisplayAlert("Error", $"No se pudo actualizar el producto: {errorMessage}", "Aceptar");
                        }
                    }
                }
                catch (Exception ex)
                {
                    await Xamarin.Forms.Application.Current.MainPage.DisplayAlert("Error", $"Ocurrió un problema: {ex.Message}", "Aceptar");
                }
            }
        }

        private void OnCancel() {
            Producto = new ProductoDTO
            {
                stock = 0,
                nombreProducto = string.Empty,
                descripcion = string.Empty,
                precio = 0
            };
        }

        public Command SaveCommand { get; }
        public Command CancelCommand { get; }

        public ProductoDTO Producto
        {
            get => _producto;
            set
            {
                _producto = value;
                OnPropertyChanged();
            }
        }

        public async void GetProductoById(string idProducto)
        {
            try
            {
                Producto = await _stockService.GetProductoDTO(idProducto);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener el producto: {ex.Message}");
            }
        }

    }
}
