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
using Microcharts;
using SkiaSharp;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Kernel.Colors;

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

            // Validar si hay productos en la lista
            if (productos == null || productos.Count == 0)
            {
                return;
            }

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
                // Obtener la ruta donde se guardará el PDF
                string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Reporte_Productos.pdf");

                // Crear un archivo PDF
                using (FileStream stream = new FileStream(filePath, FileMode.Create))
                {
                    PdfWriter writer = new PdfWriter(stream);
                    PdfDocument pdf = new PdfDocument(writer);
                    Document document = new Document(pdf);

                    // Agregar título
                    Paragraph title = new Paragraph("Reporte de Productos")
                        .SetFontSize(16)
                        .SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER);
                    document.Add(title);

                    // Agregar fecha actual
                    string fechaActual = DateTime.Now.ToString("dd/MM/yyyy");
                    Paragraph fecha = new Paragraph($"Fecha: {fechaActual}")
                        .SetFontSize(12)
                        .SetTextAlignment(iText.Layout.Properties.TextAlignment.RIGHT);
                    document.Add(fecha);

                    // Espacio antes de la tabla
                    document.Add(new Paragraph("\n"));

                    // Crear tabla con 4 columnas
                    Table table = new Table(4).UseAllAvailableWidth();

                    // Definir encabezados
                    string[] headers = { "Nombre del Producto", "Descripción", "Stock", "Precio" };
                    foreach (var header in headers)
                    {
                        // Referencia explícita a la clase de iTextSharp
                        iText.Layout.Element.Cell cell = new iText.Layout.Element.Cell()
                            .Add(new Paragraph(header))
                            .SetBackgroundColor(ColorConstants.LIGHT_GRAY)
                            .SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER);
                        table.AddHeaderCell(cell);
                    }

                    // Agregar los datos de la lista
                    foreach (var producto in productos)
                    {
                        table.AddCell(new iText.Layout.Element.Cell().Add(new Paragraph(producto.nombreProducto)).SetTextAlignment(iText.Layout.Properties.TextAlignment.LEFT));
                        table.AddCell(new iText.Layout.Element.Cell().Add(new Paragraph(producto.descripcion)).SetTextAlignment(iText.Layout.Properties.TextAlignment.LEFT));
                        table.AddCell(new iText.Layout.Element.Cell().Add(new Paragraph(producto.stock?.ToString() ?? "0")).SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER));
                        table.AddCell(new iText.Layout.Element.Cell().Add(new Paragraph(producto.precio?.ToString("C2") ?? "$0.00")).SetTextAlignment(iText.Layout.Properties.TextAlignment.RIGHT));
                    }

                    // Agregar tabla al documento
                    document.Add(table);

                    // Cerrar el documento
                    document.Close();
                }

                // Retornar la ruta del archivo generado
                return filePath;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al generar el PDF: {ex.Message}");
                return null;
            }
        }

        //public async Task<string> GenerarPDF(List<ProductoDTO> productos)
        //{
        //    try
        //    {
        //         Crear un nuevo documento PDF
        //        PdfDocument documento = new PdfDocument();

        //         Agregar una página al documento
        //        PdfPage pagina = documento.Pages.Add();

        //         Crear un objeto gráfico para escribir en la página
        //        PdfGraphics graphics = pagina.Graphics;

        //         Establecer fuentes para el texto
        //        PdfFont titleFont = new PdfStandardFont(PdfFontFamily.Helvetica, 16, PdfFontStyle.Bold);
        //        PdfFont font = new PdfStandardFont(PdfFontFamily.Helvetica, 12);

        //         Obtener la fecha actual del sistema en formato dd/MM/yyyy
        //        string fechaActual = DateTime.Now.ToString("dd/MM/yyyy");

        //         Escribir un encabezado
        //        graphics.DrawString("Reporte de Productos", titleFont, PdfBrushes.Black, new Syncfusion.Drawing.PointF(180, 30));

        //         Escribir la fecha actual debajo del título
        //        graphics.DrawString($"Fecha: {fechaActual}", font, PdfBrushes.Black, new Syncfusion.Drawing.PointF(400, 60));

        //         Crear un objeto PdfGrid para mostrar los datos en forma de tabla
        //        PdfGrid grid = new PdfGrid();

        //         Definir solo las columnas necesarias
        //        grid.Columns.Add(4); // 4 columnas: Nombre, Descripción, Stock, Precio

        //         Crear la fila de encabezado
        //        PdfGridRow headerRow = grid.Headers.Add(1)[0];
        //        headerRow.Cells[0].Value = "Nombre del Producto";
        //        headerRow.Cells[1].Value = "Descripción";
        //        headerRow.Cells[2].Value = "Stock";
        //        headerRow.Cells[3].Value = "Precio";

        //         Aplicar estilos al encabezado
        //        foreach (PdfGridCell cell in headerRow.Cells)
        //        {
        //            cell.StringFormat = new PdfStringFormat(PdfTextAlignment.Center, PdfVerticalAlignment.Middle);
        //            cell.Style.Font = new PdfStandardFont(PdfFontFamily.Helvetica, 12, PdfFontStyle.Bold);
        //            cell.Style.BackgroundBrush = PdfBrushes.LightGray;
        //        }

        //         Agregar filas con los datos filtrados
        //        foreach (var producto in productos)
        //        {
        //            PdfGridRow row = grid.Rows.Add();
        //            row.Cells[0].Value = producto.nombreProducto;
        //            row.Cells[1].Value = producto.descripcion;
        //            row.Cells[2].Value = producto.stock?.ToString();
        //            row.Cells[3].Value = producto.precio?.ToString("C2"); // Formato de moneda
        //        }

        //         Ajustar tamaño de las celdas y aplicar estilo a las filas
        //        grid.Style.Font = font;
        //        grid.Style.CellPadding = new PdfPaddings(5, 5, 5, 5);

        //         Ajustar la tabla para que no se corte en la parte final
        //        PdfLayoutFormat layoutFormat = new PdfLayoutFormat();
        //        layoutFormat.Layout = PdfLayoutType.Paginate;

        //         Dibujar la tabla en la página y permitir paginación automática
        //        PdfLayoutResult result = grid.Draw(pagina, new Syncfusion.Drawing.PointF(30, 90), layoutFormat);

        //         Guardar el documento en un MemoryStream en lugar de un archivo
        //        using (MemoryStream ms = new MemoryStream())
        //        {
        //            documento.Save(ms);

        //             Obtener la ruta del archivo para guardarlo físicamente
        //            string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Reporte_Productos.pdf");

        //             Guardar el contenido del MemoryStream en un archivo
        //            File.WriteAllBytes(filePath, ms.ToArray());

        //             Cerrar el documento
        //            documento.Close(true);

        //             Devolver la ruta del archivo
        //            return filePath;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"Error al generar el PDF: {ex.Message}");
        //        return null;
        //    }
        //}

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