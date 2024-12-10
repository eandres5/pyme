using pyme.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace pyme.Services
{
    public class AuthenticationService
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "https://localhost:7214/api/Login/login";

        public AuthenticationService()
        {
            _httpClient = new HttpClient();
        }

        public async Task<bool> LoginAsync(string identificacion, string password)
        {
            //var loginRequest = new LoginRequestDto
            //{
            //    Identificacion = identificacion,
            //    Password = password
            //};

            //var json = JsonConvert.SerializeObject(loginRequest);
            //var content = new StringContent(json, Encoding.UTF8, "application/json");

            //var response = await _httpClient.PostAsync(BaseUrl, content);

            //return response.IsSuccessStatusCode;
            return false;
        }
    }
}
