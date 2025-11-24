using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio
{
    [Serializable]
    public class Venta
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; }
        public Cliente Cliente { get; set; }
        public Usuario Usuario { get; set; }
        public string NumeroFactura { get; set; }
        public List<DetalleVenta> Detalles { get; set; }
        public decimal TotalVenta { get; set; }
        public string Estado { get; set; }
    }

    [Serializable]
    public class DetalleVenta
    {
        public int Id { get; set; }
        public Articulo Articulo { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal Subtotal { get; set; }
        public int ArticuloId => Articulo != null ? Articulo.Id : 0;
    }
}