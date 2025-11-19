using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Dominio;
using SQL;
using System.Web.Script.Serialization; 

namespace tp_c_equipo_3B
{
    public partial class PaneldeControl : System.Web.UI.Page
    {
        // Instancias de las clases SQL
        private DashboardSQL dashboardSQL = new DashboardSQL();
        private ArticuloSQL articuloSQL = new ArticuloSQL();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                CargarStatsCards();
                CargarBajoStock();
                CargarActividadReciente();
                CargarGrafico();
            }
        }

        private void CargarStatsCards()
        {
            try
            {
                DashboardStats stats = dashboardSQL.GetDashboardStats();

                decimal gananciaNeta = stats.IngresosTotales - stats.CostosTotales;

                // Formatear los números como moneda (ej: $15.230,50)
                litIngresosTotales.Text = stats.IngresosTotales.ToString("C");
                litCostosTotales.Text = stats.CostosTotales.ToString("C");
                litGananciaNeta.Text = gananciaNeta.ToString("C");
                litTransacciones.Text = stats.TotalTransacciones.ToString();

                // Los % 
                litIngresosStats.Text = "+5.2%";
                litCostosStats.Text = "+3.1%";
                litGananciaStats.Text = "+8.7%";
                litTransaccionesStats.Text = "-1.2%";

    
                if (gananciaNeta < 0)
                {
                    litGananciaNeta.CssClass = "text-danger fw-bolder mb-0";
                    litGananciaStats.CssClass = "text-danger small fw-semibold mb-0";
                }
            }
            catch (Exception ex)
            {

                litIngresosTotales.Text = "Error";
                litCostosTotales.Text = "Error";
                litGananciaNeta.Text = "Error";
                litTransacciones.Text = "Error";
            }
        }

        private void CargarBajoStock()
        {
            var lista = articuloSQL.ListarBajoStock(5); 
            if (lista != null && lista.Count > 0)
            {
                rptBajoStock.DataSource = lista;
                rptBajoStock.DataBind();
                pnlNoBajoStock.Visible = false;
            }
            else
            {
                pnlNoBajoStock.Visible = true;
            }
        }

        private void CargarActividadReciente()
        {
            var lista = dashboardSQL.ListarActividadReciente(5); 
            if (lista != null && lista.Count > 0)
            {
                rptActividadReciente.DataSource = lista;
                rptActividadReciente.DataBind();
                pnlNoActividad.Visible = false;
            }
            else
            {
                pnlNoActividad.Visible = true;
            }
        }


        private void CargarGrafico()
        {
            try
            {
                var chartData = dashboardSQL.GetChartData();

                var labels = chartData.Select(d => d.Label).ToList();
                var ventasData = chartData.Select(d => d.TotalVentas).ToList();
                var comprasData = chartData.Select(d => d.TotalCompras).ToList();

                var serializer = new JavaScriptSerializer();
                string jsonLabels = serializer.Serialize(labels);
                string jsonVentas = serializer.Serialize(ventasData);
                string jsonCompras = serializer.Serialize(comprasData);

                string script = $@"
                    var chartLabels = {jsonLabels};
                    var chartVentasData = {jsonVentas};
                    var chartComprasData = {jsonCompras};
                ";

                ScriptManager.RegisterStartupScript(this, this.GetType(), "ChartDataScript", script, true);
            }
            catch (Exception ex)
            {
                string script = "var chartLabels = []; var chartVentasData = []; var chartComprasData = [];";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ChartDataScript", script, true);
            }
        }



        protected string GetIconBgClass(string tipo)
        {
            return tipo == "Venta" ? "bg-success-20" : "bg-info-20";
        }

        protected string GetIconClass(string tipo)
        {
            return tipo == "Venta" ? "text-success" : "text-info";
        }

        protected string GetIconName(string tipo)
        {
            return tipo == "Venta" ? "shopping_cart" : "local_shipping";
        }

        protected string GetActivityText(string tipo, object id, object monto, object nombre)
        {
            decimal montoDec = (decimal)monto;
            if (tipo == "Venta")
            {
                return $"Venta #{id} a <strong>{nombre}</strong> por {montoDec.ToString("C")}";
            }
            else
            {
                return $"Compra #{id} a <strong>{nombre}</strong> por {montoDec.ToString("C")}";
            }
        }
    }
}