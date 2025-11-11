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
        public int Id { get; set; }
        public string Codigo { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public decimal Precio { get; set; }
        public int IdMarca { get; set; }
        public int IdCategoria { get; set; }
        public Marca Marca { get; set; }
        public Categoria Categoria { get; set; }
        public string UrlImagen { get; set; }  // Imagen principal
        public List<Imagen> Imagenes { get; set; } = new List<Imagen>();
        public int StockActual { get; set; } = 0;
        public string StockDisplay { get; set; }
        public string StockClass { get; set; }
        public string FirstImage()
        {
            return Imagenes != null && Imagenes.Count > 0 ? Imagenes[0].UrlImagen : UrlImagen ?? "";
        }
        public string ProveedorNombre { get; set; }
    }
}
