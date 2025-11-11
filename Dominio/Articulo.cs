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
        // Constructor para inicializar colecciones
        public Articulo()
        {
            this.Imagenes = new HashSet<Imagen>();
            this.Proveedores = new HashSet<Proveedor>();
        }

        public int Id { get; set; }
        public string Codigo { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }

        // --- PRECIOS (Según Requisitos) ---
        public decimal UltimoPrecioCompra { get; set; }
        public decimal PorcentajeGanancia { get; set; }

        // --- STOCK (Según Requisitos) ---
        public int StockActual { get; set; } = 0;
        public int StockMinimo { get; set; }

        // --- RELACIONES (Foreign Keys) ---
        public int IdMarca { get; set; }
        public int IdCategoria { get; set; }

        // --- PROPIEDADES DE NAVEGACIÓN ---
        public virtual Marca Marca { get; set; }
        public virtual Categoria Categoria { get; set; }

        // --- Relación 1-a-Muchos con Imagen ---
        public string UrlImagen { get; set; }  // Imagen principal
        public virtual ICollection<Imagen> Imagenes { get; set; }

        // --- Relación M-a-M con Proveedor (Según Requisito) ---
        public virtual ICollection<Proveedor> Proveedores { get; set; }

        // --- Propiedad 'ViewModel' para la grilla ---
        // (Transporta el string de proveedores desde SQL)
        public string ProveedoresString { get; set; }


        // --- Métodos Helper ---
        public string FirstImage()
        {
            if (Imagenes != null && Imagenes.Count > 0)
                return Imagenes.First().UrlImagen;

            return UrlImagen ?? "";
        }
    }
}