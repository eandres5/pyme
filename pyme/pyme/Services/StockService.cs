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
            var response = await client.GetAsync("https://www.pymesecuador.org/api/Producto/getProducto/" + idProducto);

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
            var response = await client.GetAsync("https://www.pymesecuador.org/api/Producto/getBajoStock");

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

        public async Task<List<ProductoDTO>> GetProductoReporte(string descripcion)
        {
            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
            };

            var client = new HttpClient(handler);

            // Cambia para producción
            var response = await client.GetAsync("https://www.pymesecuador.org/api/Producto/getProductoUsuarioDescripcion/0/100/" + descripcion);

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();

                // Deserializa en un objeto que contenga la lista de productos
                var resultado = JsonConvert.DeserializeObject<ApiResponse>(json);

                return resultado?.Items ?? new List<ProductoDTO>(); // Retorna la lista de productos o una lista vacía
            }
            else
            {
                throw new Exception("Error al obtener datos del API");
            }

        }

    }
}
