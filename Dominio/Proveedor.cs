using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio
{
    
    [Serializable]
    public class Proveedor
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Email { get; set; }
        public string Telefono { get; set; }
        public string Direccion { get; set; }


        [NonSerialized]
        private ICollection<Articulo> _articulos;
        public virtual ICollection<Articulo> Articulos
        {
            get { return _articulos; }
            set { _articulos = value; }
        }
    }
}