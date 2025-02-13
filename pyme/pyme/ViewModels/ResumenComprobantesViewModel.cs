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
            CommandPdf = new Command(OnDownloadPdfClicked);
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

        private async void OnDownloadPdfClicked()
        {
            List<ProductoDTO> productos = await CargarProductosReporte();
            await GenerarPDF(productos);
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
                if (productos == null || productos.Count == 0)
                    throw new Exception("La lista de productos está vacía o es nula.");

                PdfDocument documento = new PdfDocument();
                documento.Info.Title = "Reporte de Productos";

                PdfPage pagina = documento.AddPage();
                if (pagina == null) throw new Exception("No se pudo crear la página en el documento PDF.");

                XGraphics gfx = XGraphics.FromPdfPage(pagina);
                if (gfx == null) throw new Exception("No se pudo crear el objeto gráfico para la página.");

                // Crear fuentes usando los estilos correctos
                XFont fontRegular = new XFont("Arial", 12); // Estilo regular (ningún estilo adicional)
                XFont fontBold = new XFont("Arial", 16); // Estilo negrita

                int yPoint = 40;

                // Título (usando la fuente en negrita)
                gfx.DrawString("Reporte de Productos", fontBold, XBrushes.Black, new XPoint(40, yPoint));
                yPoint += 30;

                // Dibujar encabezados (usando la fuente regular)
                gfx.DrawString("Nombre", fontRegular, XBrushes.Black, new XPoint(40, yPoint));
                gfx.DrawString("Descripción", fontRegular, XBrushes.Black, new XPoint(200, yPoint));
                gfx.DrawString("Precio", fontRegular, XBrushes.Black, new XPoint(400, yPoint));
                gfx.DrawString("Stock", fontRegular, XBrushes.Black, new XPoint(500, yPoint));
                yPoint += 20;

                // Dibujar productos (usando la fuente regular)
                foreach (var producto in productos)
                {
                    gfx.DrawString(producto.nombreProducto ?? "N/A", fontRegular, XBrushes.Black, new XPoint(40, yPoint));
                    gfx.DrawString(producto.descripcion ?? "N/A", fontRegular, XBrushes.Black, new XPoint(200, yPoint));
                    gfx.DrawString($"${producto.precio:0.00}", fontRegular, XBrushes.Black, new XPoint(400, yPoint));
                    gfx.DrawString(producto.stock.ToString(), fontRegular, XBrushes.Black, new XPoint(500, yPoint));
                    yPoint += 20;
                }

                string fileName = Path.Combine(FileSystem.AppDataDirectory, "Reporte_Productos.pdf");
                using (MemoryStream stream = new MemoryStream())
                {
                    documento.Save(stream, false);
                    File.WriteAllBytes(fileName, stream.ToArray());
                }

                return fileName;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error generando PDF: {ex.Message}");
                return null;
            }
        }

    }
}