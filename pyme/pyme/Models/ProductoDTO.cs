using System;
using System.Collections.Generic;
using System.Text;

namespace pyme.Models
{
    public class ProductoDTO
    {
        public ProductoDTO productoDTO;

        public ProductoDTO(ProductoDTO productoDTO)
        {
            this.productoDTO = productoDTO;
        }

        public string nombreProducto { get; set; }
        public string descripcion { get; set; }
        public int? stock { get; set; }
        public float? precio { get; set; }
        public string nombreCategoria { get; set; }
        public string observacion { get; set; }
        public string idProveedor { get; set; }
    }
}
