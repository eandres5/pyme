using pyme.Models;
using pyme.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using Microcharts;
using SkiaSharp;

namespace pyme.ViewModels
{
    public class ResumenComprobantesViewModel : BindableObject
    {
        private readonly ComprobanteService _comprobanteService;
        private readonly StockService _stockService;
        private ObservableCollection<ResumenComprobanteResponse> _compras;
        private ObservableCollection<ResumenComprobanteResponse> _ventas;
        private ObservableCollection<ResumenComprobanteResponse> _devoluciones;
        private ObservableCollection<ProductoDTO> _productosReporte;
        private ObservableCollection<ProductoDTO> _productos;
        public string descripcionProducto { get; set; }
        public Command CommandPdf { get; }

        private List<ChartEntry> _chartEntries;
        public List<ChartEntry> ChartEntries
        {
            get => _chartEntries;
            set
            {
                _chartEntries = value;
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

        public ObservableCollection<ProductoDTO> ProductosReporte
        {
            get => _productosReporte;
            set
            {
                _productosReporte = value;
                OnPropertyChanged();
            }
        }
        public ObservableCollection<ProductoDTO> Productos
        {
            get => _productos;
            set
            {
                _productos = value;
                OnPropertyChanged();
            }
        }

        public ResumenComprobantesViewModel()
        {
            _comprobanteService = new ComprobanteService();
            _stockService = new StockService();
            CommandPdf = new Command(OnDownloadPdfClicked);
            CargarDatos();
            CargarBajoStock();
        }

        public async void CargarDatos()
        {
            try
            {
                Compras = new ObservableCollection<ResumenComprobanteResponse>(await _comprobanteService.GetResumenCompras()); 
                Ventas = new ObservableCollection<ResumenComprobanteResponse>(await _comprobanteService.GetResumenComprobantes("VENTA"));
                Devoluciones = new ObservableCollection<ResumenComprobanteResponse>(await _comprobanteService.GetResumenComprobantes("DEVOLUCION"));

                CargarGrafico();

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

        private async void OnDownloadPdfClicked()
        {
            List<ProductoDTO> productos = await CargarProductosReporte();
            //await GenerarPDF(productos);

            var pdfFilePath = await GenerarPDF(productos);

            if (!string.IsNullOrEmpty(pdfFilePath) && File.Exists(pdfFilePath))
            {
                try
                {
                    await Launcher.OpenAsync(new OpenFileRequest
                    {
                        File = new ReadOnlyFile(pdfFilePath)
                    });
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al abrir el PDF: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine("El PDF no se generó correctamente o no se encuentra.");
            }
        }

        private async Task<List<ProductoDTO>> CargarProductosReporte()
        {
            try
            {
                var productosRep = await _stockService.GetProductoReporte(descripcionProducto); // Asumiendo que retorna ObservableCollection<ProductoDTO>
                return productosRep.ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return new List<ProductoDTO>(); // Evita retornar null
            }
        }

        public async Task<string> GenerarPDF(List<ProductoDTO> productos)
        {
            try
            {
                // Crear documento PDF
                PdfDocument documento = new PdfDocument();
                documento.Info.Title = "Reporte de Productos";

                // Agregar una página al documento
                PdfPage pagina = documento.AddPage();
                if (pagina == null) throw new Exception("No se pudo crear la página en el documento PDF.");

                // Definir la ruta donde se guardará el PDF
                string fileName = Path.Combine(FileSystem.AppDataDirectory, "Reporte_Productos.pdf");

                // Guardar directamente el archivo en el sistema de archivos
                documento.Save(fileName);

                if (File.Exists(fileName))
                {
                    Console.WriteLine($"PDF generado con éxito en: {fileName}");
                }
                else
                {
                    Console.WriteLine("Error: El archivo PDF no se generó.");
                }

                return fileName;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error generando PDF: {ex.Message}");
                return null;
            }
        }
        private void CargarGrafico()
        {
            float totalVentas = CalcularTotal(Ventas);
            float totalDevoluciones = CalcularTotal(Devoluciones);
            float totalCompras = CalcularTotal(Compras);

            ChartEntries = new List<ChartEntry>
    {
        new ChartEntry(totalVentas)
        {
            Label = "Ventas",
            ValueLabel = totalVentas.ToString("N2"), // Formato con 2 decimales
            Color = SKColor.Parse("#2ecc71") // Verde
        },
        new ChartEntry(totalDevoluciones)
        {
            Label = "Devoluciones",
            ValueLabel = totalDevoluciones.ToString("N2"),
            Color = SKColor.Parse("#e74c3c") // Rojo
        },
        new ChartEntry(totalCompras)
        {
            Label = "Compras",
            ValueLabel = totalCompras.ToString("N2"),
            Color = SKColor.Parse("#3498db") // Azul
        }
    };

            OnPropertyChanged(nameof(ChartEntries));
        }

        // Método modificado para aceptar IEnumerable en lugar de List
        private float CalcularTotal(IEnumerable<ResumenComprobanteResponse> lista)
        {
            if (lista == null || !lista.Any())
                return 0;

            var culture = new System.Globalization.CultureInfo("en-US"); // Usa cultura que tenga punto como separador decimal

            return lista.Sum(x => {
                // Convierte el total a float usando el punto como separador decimal
                if (float.TryParse(x.Total.ToString(), System.Globalization.NumberStyles.Any, culture, out float total))
                {
                    return (float)Math.Floor(total * 100) / 100; // Truncar a dos decimales
                }
                return 0;
            });
        }
    }
}