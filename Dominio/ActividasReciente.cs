using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio
{
    [Serializable]
    public class ActividadReciente
    {
        public string Tipo { get; set; } // "Venta" o "Compra"
        public int Id { get; set; }
        public DateTime Fecha { get; set; }
        public decimal Monto { get; set; }
        public string NombreClienteOProveedor { get; set; }
    }
}