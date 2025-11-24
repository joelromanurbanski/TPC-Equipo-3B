using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio
{
    [Serializable]
    public class Usuario
    {
        public int Id { get; set; }
        public string Email { get; set; }      // Usuario para Login
        public string Contrasenia { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public bool EsAdmin { get; set; }      // true = Admin, false = Vendedor

        public string NombreCompleto => $"{Nombre} {Apellido}";
        public string RolTexto => EsAdmin ? "Administrador" : "Vendedor";

        public Usuario() { }
    }
}