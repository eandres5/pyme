using Newtonsoft.Json;
using pyme.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace pyme.Services
{
    class StockService
    {
        private readonly HttpClient _httpClient;

        public async Task<ProductoDTO> GetProductoDTO(string idProducto)
        {
            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
            };

            var client = new HttpClient(handler);

            // Cambia para produccion
            var response = await client.GetAsync("https://192.168.200.5:45455/api/Producto/getProducto/" + idProducto);

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<ProductoDTO>(json);
            }
            else
            {
                throw new Exception("Error al obtener datos del API");
            }

        }

        public async Task<List<ProductoDTO>> GetProductoStock()
        {
            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
            };

            var client = new HttpClient(handler);

            // Cambia para produccion
            var response = await client.GetAsync("https://192.168.200.5:45455/api/Producto/getBajoStock");

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<ProductoDTO>>(json);
            }
            else
            {
                throw new Exception("Error al obtener datos del API");
            }

        }

    }
}
