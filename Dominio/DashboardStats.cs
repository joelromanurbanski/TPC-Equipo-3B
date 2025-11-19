using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio
{
    [Serializable]
    public class DashboardStats
    {
        public decimal IngresosTotales { get; set; }
        public decimal CostosTotales { get; set; }
        public int TotalTransacciones { get; set; }
    }
}