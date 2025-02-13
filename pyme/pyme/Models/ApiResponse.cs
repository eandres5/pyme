using System;
using System.Collections.Generic;
using System.Text;

namespace pyme.Models
{
    public class ApiResponse
    {
        public int TotalCount { get; set; }
        public List<ProductoDTO> Items { get; set; }
    }
}
