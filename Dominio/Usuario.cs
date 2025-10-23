using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Rol
{
    public int Id { get; set; }
    public string Nombre { get; set; } // "Administrador", "Vendedor"
}

public class Usuario
{
    public int Id { get; set; }
    public string Nombre { get; set; }
    public string Apellido { get; set; }
    public string Email { get; set; }
    public string Clave { get; set; }
    public Rol Rol { get; set; }
}
