using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Dominio;
using SQL;


namespace tp_c_equipo_3B
{
    public partial class GestionPedidos : System.Web.UI.Page
    {
        private VentaSQL ventaSQL = new VentaSQL();
        private ArticuloSQL articuloSQL = new ArticuloSQL();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindGrid();
            }
        }

        private void BindGrid()
        {
            // Buscamos los filtros desde la vista (si existen los controles)
            string busqueda = txtBuscar != null ? txtBuscar.Text.Trim() : "";
            string estado = ddlFiltroEstado != null ? ddlFiltroEstado.SelectedValue : "Todos";

            // Llamamos al método de SQL que ya soporta filtros
            gvVentas.DataSource = ventaSQL.ListarVentas(busqueda, estado);
            gvVentas.DataBind();
        }

        protected void gvVentas_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvVentas.PageIndex = e.NewPageIndex;
            BindGrid();
        }

        protected void btnBuscar_Click(object sender, EventArgs e)
        {
            gvVentas.PageIndex = 0;
            BindGrid();
            if (lblMensaje != null) lblMensaje.Visible = false;
        }

        protected void btnRefrescar_Click(object sender, EventArgs e)
        {
            if (txtBuscar != null) txtBuscar.Text = "";
            if (ddlFiltroEstado != null) ddlFiltroEstado.SelectedValue = "Todos";
            gvVentas.PageIndex = 0;

            BindGrid();
            if (lblMensaje != null) lblMensaje.Visible = false;
        }

        protected string GetEstadoClass(string estado)
        {
            switch (estado)
            {
                case "En Preparación": return "gv-estado-EnPreparacion";
                case "Enviado": return "gv-estado-Enviado";
                case "Listo para Despachar": return "gv-estado-Listo";
                case "Entregado": return "gv-estado-Entregado";
                case "Cancelado": return "gv-estado-Cancelado";
                default: return "bg-light text-dark";
            }
        }

        protected void gvVentas_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                DropDownList ddlEstado = (DropDownList)e.Row.FindControl("ddlEstado");
                if (ddlEstado != null)
                {
                    string estadoActual = DataBinder.Eval(e.Row.DataItem, "Estado").ToString();
                    if (ddlEstado.Items.FindByValue(estadoActual) != null)
                    {
                        ddlEstado.SelectedValue = estadoActual;
                    }

                    if (estadoActual == "Entregado" || estadoActual == "Cancelado")
                    {
                        ddlEstado.Enabled = false;
                        Button btn = (Button)e.Row.FindControl("btnActualizarEstado");
                        if (btn != null) btn.Enabled = false;
                    }
                }
            }
        }

        protected void gvVentas_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "ActualizarEstado")
            {
                try
                {
                    GridViewRow row = (GridViewRow)((Button)e.CommandSource).NamingContainer;
                    DropDownList ddlEstado = (DropDownList)row.FindControl("ddlEstado");

                    int idVenta = Convert.ToInt32(e.CommandArgument);
                    string nuevoEstado = ddlEstado.SelectedValue;

                    ventaSQL.ActualizarEstado(idVenta, nuevoEstado);

                    if (nuevoEstado == "Cancelado")
                    {
                        // Reponer stock si se cancela
                        List<DetalleVenta> detalles = ventaSQL.ListarDetallesPorVenta(idVenta);
                        articuloSQL.ReponerStock(detalles);

                        lblMensaje.Text = $"Pedido #{idVenta} cancelado. ¡Stock repuesto!";
                        lblMensaje.CssClass = "alert alert-warning";
                    }
                    else
                    {
                        lblMensaje.Text = $"Pedido #{idVenta} actualizado a '{nuevoEstado}'.";
                        lblMensaje.CssClass = "alert alert-success";
                    }

                    lblMensaje.Visible = true;
                    BindGrid();
                }
                catch (Exception ex)
                {
                    lblMensaje.Text = "Error: " + ex.Message;
                    lblMensaje.CssClass = "alert alert-danger";
                    lblMensaje.Visible = true;
                }
            }
        }
    }
}