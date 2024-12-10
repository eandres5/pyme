using pyme.Models;
using pyme.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Xamarin.Forms;

namespace pyme.ViewModels
{
    public class ResumenComprobantesViewModel : BindableObject
    {
        private readonly ComprobanteService _comprobanteService;
        private readonly StockService _stockService;
        private ObservableCollection<ResumenComprobanteResponse> _compras;
        private ObservableCollection<ResumenComprobanteResponse> _ventas;
        private ObservableCollection<ResumenComprobanteResponse> _devoluciones;
        private ObservableCollection<ProductoDTO> _productos;
        public ObservableCollection<ProductoDTO> Productos
        {
            get => _productos;
            set
            {
                _productos = value;
                OnPropertyChanged();
            }
        }


        public ObservableCollection<ResumenComprobanteResponse> Compras
        {
            get => _compras;
            set
            {
                _compras = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<ResumenComprobanteResponse> Ventas
        {
            get => _ventas;
            set
            {
                _ventas = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<ResumenComprobanteResponse> Devoluciones
        {
            get => _devoluciones;
            set
            {
                _devoluciones = value;
                OnPropertyChanged();
            }
        }

        public ResumenComprobantesViewModel()
        {
            _comprobanteService = new ComprobanteService();
            _stockService = new StockService();
            CargarDatos();
            CargarBajoStock();
        }

        private async void CargarDatos()
        {
            try
            {
                Compras = new ObservableCollection<ResumenComprobanteResponse>(await _comprobanteService.GetResumenCompras()); 
                Ventas = new ObservableCollection<ResumenComprobanteResponse>(await _comprobanteService.GetResumenComprobantes("VENTA"));
                Devoluciones = new ObservableCollection<ResumenComprobanteResponse>(await _comprobanteService.GetResumenComprobantes("DEVOLUCION"));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        private async void CargarBajoStock() {
            try {
                Productos = new ObservableCollection<ProductoDTO>(await _stockService.GetProductoStock());

                var mensaje = new StringBuilder();
                mensaje.AppendLine("Productos con bajo stock:");

                if (Productos.Count > 0) {

                    foreach (var producto in Productos)
                    {
                        mensaje.AppendLine($"{producto.nombreProducto}{new string(' ', 5)}{producto.stock}");
                    }

                    MessagingCenter.Send(this, "MostrarAlerta", mensaje.ToString());
                }
            }
            catch(Exception ex) {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}