using System;
using System.Collections.Generic;
using System.Text;

namespace pyme.Models
{
    public class ProductoDTO
    {
        public ProductoDTO productoDTO;

        public ProductoDTO()
        {
        }

        public ProductoDTO(ProductoDTO productoDTO)
        {
            this.productoDTO = productoDTO;
        }

        public ProductoDTO(int stock, string nombreProducto, string descripcion, float precio)
        {
            this.stock = stock;
            this.nombreProducto = nombreProducto;
            this.descripcion = descripcion;
            this.precio = precio;
        }

        public ProductoDTO(int stock, string nombreProducto, string descripcion, float precio, string idProveedor, string nombreCategoria)
        {
            this.stock = stock;
            this.nombreProducto = nombreProducto;
            this.descripcion = descripcion;
            this.precio = precio;
            this.idProveedor = idProveedor;
            this.nombreCategoria = nombreCategoria;
        }

        public int idProducto { get; set; }
        public string nombreProducto { get; set; }
        public string descripcion { get; set; }
        public int? stock { get; set; }
        public float? precio { get; set; }
        public string nombreCategoria { get; set; }
        public string observacion { get; set; }
        public string idProveedor { get; set; }
    }
}
