using pyme.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace pyme.Services
{
    public class UsuarioService
    {

        private ObservableCollection<Usuario> Usuarios { get; set; }
        private int currentPage = 0;
        private const int pageSize = 10;

        public async Task LoadUsuariosAsync()
        {
            try
            {
                // Llamar al endpoint con paginación
                var url = $"https://api.url/api/Usuario/getProveedores/{currentPage}/{pageSize}/null";
                using (var client = new HttpClient())
                {
                    var response = await client.GetStringAsync(url);
                    var result = JsonSerializer.Deserialize<UsuarioResponse>(response);
                    if (result != null && result.items.Any())
                    {
                        foreach (var usuario in result.items)
                        {
                            Usuarios.Add(usuario);
                        }
                        currentPage++; // Incrementa para la próxima página
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al cargar usuarios: {ex.Message}");
            }
        }

    }
}
