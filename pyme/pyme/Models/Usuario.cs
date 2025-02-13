using System;
using System.Collections.Generic;
using System.Text;

namespace pyme.Models
{
    public class Usuario
    {
        public Usuario() { }
        public Usuario(int idUsuario, string nombres, string apellidos, string identificacion) { 
            this.idUsuario = idUsuario;
            this.nombres = nombres;
            this.apellidos = apellidos; 
            this.identificacion = identificacion;
        }  
        public int idUsuario { get; set; }
        public string nombres { get; set; }
        public string apellidos { get; set; }
        public string identificacion { get; set; }
        public bool activo { get; set; }
    }

    public class UsuarioResponse
    {
        public int totalCount { get; set; }
        public List<Usuario> items { get; set; }
        public IEnumerable<Usuario> Items { get; internal set; }
    }

}
