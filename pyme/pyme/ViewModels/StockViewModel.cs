using pyme.Models;
using pyme.Services;
using System;
using System.Collections.Generic;
using System.Text;
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
            //SaveCommand = new Command(OnSave, ValidateSave);
        }

        private bool ValidateSave()
        {
            return !String.IsNullOrWhiteSpace(stock);
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


        private async void OnCancel()
        {
            // This will pop the current page off the navigation stack
            await Shell.Current.GoToAsync("..");
        }

    }
}
