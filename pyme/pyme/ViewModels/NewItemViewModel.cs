using Newtonsoft.Json;
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
using System.Windows.Input;

using Xamarin.Forms;

namespace pyme.ViewModels
{
    public class NewItemViewModel : BaseViewModel
    {
        private string text;
        private string description;
        private ProductoDTO _producto;
        private Usuario _usuarioSeleccionado;
        private ObservableCollection<Usuario> _users;
        private readonly UsuarioService _usuarioService;
        public ObservableCollection<Usuario> Usuarios { get; set; }


        public ObservableCollection<Usuario> Users
        {
            get { return _users; }
            set { SetProperty(ref _users, value); }
        }
        private int currentPage = 0;
        private const int pageSize = 200;


        public NewItemViewModel()
        {
            _usuarioService = new UsuarioService();
            Usuarios = new ObservableCollection<Usuario>();
            SaveCommand = new Command(OnSave);
            CancelCommand = new Command(OnCancel);
            this.PropertyChanged +=
                (_, __) => SaveCommand.ChangeCanExecute();
            _ = LoadUsuariosAsync();

            Producto = new ProductoDTO
            {
                stock = 0,
                nombreProducto = string.Empty,
                descripcion = string.Empty,
                precio = 0,
                idProveedor = string.Empty
            };

            UsuarioSeleccionado = new Usuario
            {
                idUsuario = 0,
                nombres = string.Empty,
                apellidos = string.Empty,
                identificacion = string.Empty
            };
        }

        public Usuario UsuarioSeleccionado
        {
            get => _usuarioSeleccionado;
            set
            {
                _usuarioSeleccionado = value;
                OnPropertyChanged();
                if (_usuarioSeleccionado != null)
                {
                    // Captura el idUsuario del usuario seleccionado
                    Console.WriteLine($"Usuario seleccionado: {_usuarioSeleccionado.idUsuario}");
                }
            }
        }

        public ProductoDTO Producto
        {
            get => _producto;
            set
            {
                _producto = value;
                OnPropertyChanged();
            }
        }

        private bool ValidateSave()
        {
            return !String.IsNullOrWhiteSpace(text)
                && !String.IsNullOrWhiteSpace(description);
        }

        public string Text
        {
            get => text;
            set => SetProperty(ref text, value);
        }

        public string Description
        {
            get => description;
            set => SetProperty(ref description, value);
        }

        public Command SaveCommand { get; }
        public Command CancelCommand { get; }

        private async void OnCancel()
        {
            Producto = new ProductoDTO
            {
                stock = 0,
                nombreProducto = string.Empty,
                descripcion = string.Empty,
                precio = 0,
                idProveedor = string.Empty
            };

            UsuarioSeleccionado = new Usuario
            {
                idUsuario = 0,
                nombres = string.Empty,
                apellidos = string.Empty,
                identificacion = string.Empty
            };
        }

        private async void OnSave()
        {
            if (Producto.stock <= 0)
            {
                await Xamarin.Forms.Application.Current.MainPage.DisplayAlert("Error", "Debe ingresar una cantidad válido", "Aceptar");
            }
            else if (String.IsNullOrWhiteSpace(Producto.nombreProducto))
            {
                await Xamarin.Forms.Application.Current.MainPage.DisplayAlert("Error", "Debe ingresar el nombre del producto", "Aceptar");
            }
            else if (Producto.precio <= 0)
            {
                await Xamarin.Forms.Application.Current.MainPage.DisplayAlert("Error", "Debe ingresar un precio válido", "Aceptar");
            }
            else if (String.IsNullOrEmpty(Producto.nombreCategoria))
            {
                await Xamarin.Forms.Application.Current.MainPage.DisplayAlert("Error", "Debe ingresar la categoria", "Aceptar");
            }
            else if (String.IsNullOrEmpty(Producto.descripcion))
            {
                await Xamarin.Forms.Application.Current.MainPage.DisplayAlert("Error", "Debe ingresar la descripcion", "Aceptar");
            }
            else if (UsuarioSeleccionado == null) 
            {
                await Xamarin.Forms.Application.Current.MainPage.DisplayAlert("Error", "Debe seleccionar un proveedor", "Aceptar");
            }
            else
            {

                try
                {

                    var productoAsString = new
                    {
                        nombreProducto = Producto.nombreProducto,
                        descripcion = Producto.descripcion,
                        stock = Producto.stock.ToString(),
                        precio = Producto.precio.ToString(),
                        nombreCategoria = Producto.nombreCategoria,
                        observacion = "",
                        idProveedor = UsuarioSeleccionado.idUsuario.ToString()
                    };

                    // Serializar el objeto Producto a JSON
                    string productoDTO = System.Text.Json.JsonSerializer.Serialize(productoAsString);

                    // Crear cliente HTTP
                    using (var client = new HttpClient())
                    {
                        client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                        // URL del API
                        var url = "https://www.pymesecuador.org/api/Producto/save";

                        // Crear contenido de la solicitud
                        var content = new StringContent(productoDTO, Encoding.UTF8, "application/json");

                        // Realizar la solicitud PUT
                        var response = await client.PostAsync(url, content);

                        // Verificar si la solicitud fue exitosa
                        if (response.IsSuccessStatusCode)
                        {
                            OnCancel();
                            await Xamarin.Forms.Application.Current.MainPage.DisplayAlert("Éxito", "Producto registrado exitosamente", "Aceptar");
                        }
                        else
                        {
                            var errorMessage = await response.Content.ReadAsStringAsync();
                            await Xamarin.Forms.Application.Current.MainPage.DisplayAlert("Error", $"No se pudo registrar el producto: {errorMessage}", "Aceptar");
                        }
                    }
                }
                catch (Exception ex)
                {
                    await Xamarin.Forms.Application.Current.MainPage.DisplayAlert("Error", $"Ocurrió un problema: {ex.Message}", "Aceptar");
                }

            }
        }

        public async Task LoadUsuariosAsync()
        {
            try
            {
                // Llamar al endpoint con paginación
                var url = $"https://www.pymesecuador.org/api/Usuario/getProveedores/{currentPage}/{pageSize}/null";
                using (var client = new HttpClient())
                {
                    var response = await client.GetStringAsync(url);
                    var result = System.Text.Json.JsonSerializer.Deserialize<UsuarioResponse>(response);
                    if (result != null && result.items.Any())
                    {
                        foreach (var usuario in result.items)
                        {
                            Usuarios.Add(usuario);
                        }
                        currentPage++;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al cargar usuarios: {ex.Message}");
            }
        }

    }
}
