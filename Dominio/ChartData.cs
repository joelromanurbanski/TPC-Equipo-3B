using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio
{
    [Serializable]
    public class ChartData
    {
        public string Label { get; set; } // Ej: "Semana 1"
        public decimal TotalVentas { get; set; }
        public decimal TotalCompras { get; set; }
    }
}