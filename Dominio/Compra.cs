using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio
{
    [Serializable]
    public class Compra
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; }
        public Proveedor Proveedor { get; set; }
        public List<DetalleCompra> Detalles { get; set; }
        public decimal TotalCompra { get; set; }
    }

    [Serializable]
    public class DetalleCompra
    {
        public int Id { get; set; }
        public Articulo Articulo { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioCompra { get; set; }
        public int ArticuloId => Articulo.Id;
        [NonSerialized]
        private decimal _subtotal;
        public decimal Subtotal => Cantidad * PrecioCompra;
    }
}