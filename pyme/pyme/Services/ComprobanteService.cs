using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace pyme.Services
{
    public class ComprobanteService
    {
        private readonly HttpClient _httpClient;

        public ComprobanteService()
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri("https://10.0.2.2:7214/api/Comprobante/");
        }

        public async Task<List<ResumenComprobanteResponse>> GetResumenComprobantes(string tipoTransaccion)
        {
            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
            };

            var client = new HttpClient(handler);

            // Cambia para produccion
            var response = await client.GetAsync("https://www.pymesecuador.org/api/Comprobante/getResumenComprobantes/" + tipoTransaccion);

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<ResumenComprobanteResponse>>(json);
            }
            else
            {
                throw new Exception("Error al obtener datos del API");
            }
        }

        public async Task<List<ResumenComprobanteResponse>> GetResumenCompras()
        {
            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
            };

            var client = new HttpClient(handler);

            // Cambia para produccion
            var response = await client.GetAsync("https://www.pymesecuador.org/api/Compra/getResumenCompras");

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<ResumenComprobanteResponse>>(json);
            }
            else
            {
                throw new Exception("Error al obtener datos del API");
            }
        }

    }

    public class ResumenComprobanteResponse
    {
        public string TipoComprobante { get; set; }
        public string Total { get; set; }
    }
}