using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Dominio
{
    public class Articulo
    {
        public Articulo()
        {
            this.Imagenes = new HashSet<Imagen>();
            this.Proveedores = new HashSet<Proveedor>();
        }
        public int Id { get; set; }
        public string Codigo { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public decimal UltimoPrecioCompra { get; set; }
        public decimal PorcentajeGanancia { get; set; }
        public int StockActual { get; set; } = 0;
        public int StockMinimo { get; set; }
        public int IdMarca { get; set; }
        public int IdCategoria { get; set; }
        public virtual Marca Marca { get; set; }
        public virtual Categoria Categoria { get; set; }
        public string UrlImagen { get; set; }  // Imagen principal
        public virtual ICollection<Imagen> Imagenes { get; set; }
        public virtual ICollection<Proveedor> Proveedores { get; set; }
        public string ProveedoresString { get; set; }
        public string FirstImage()
        {
            if (Imagenes != null && Imagenes.Count > 0)
                return Imagenes.First().UrlImagen;

            return UrlImagen ?? "";
        }
    }
}