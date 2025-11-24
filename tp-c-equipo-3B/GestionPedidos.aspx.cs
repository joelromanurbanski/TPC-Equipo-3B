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
            string busqueda = txtBuscar.Text.Trim();
            string estado = ddlFiltroEstado.SelectedValue;
            bool ordenAsc = ddlOrden.SelectedValue == "ASC";

            // --- LOGICA DE FECHAS ---
            DateTime? fechaInicio = null;
            DateTime? fechaFin = null;

            if (!string.IsNullOrEmpty(txtFechaDesde.Text))
            {
                fechaInicio = DateTime.Parse(txtFechaDesde.Text);
            }
            if (!string.IsNullOrEmpty(txtFechaHasta.Text))
            {
                fechaFin = DateTime.Parse(txtFechaHasta.Text);
            }

            // Llamar al método con los nuevos parámetros
            gvVentas.DataSource = ventaSQL.ListarVentas(busqueda, estado, ordenAsc, fechaInicio, fechaFin);
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
            lblMensaje.Visible = false;
        }

        protected void btnRefrescar_Click(object sender, EventArgs e)
        {
            txtBuscar.Text = "";
            ddlFiltroEstado.SelectedValue = "Todos";
            ddlOrden.SelectedValue = "DESC";

            // Limpiar fechas
            txtFechaDesde.Text = "";
            txtFechaHasta.Text = "";

            gvVentas.PageIndex = 0;
            BindGrid();
            lblMensaje.Visible = false;
        }

        protected void ddlOrden_SelectedIndexChanged(object sender, EventArgs e)
        {
            gvVentas.PageIndex = 0;
            BindGrid();
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
                        ddlEstado.SelectedValue = estadoActual;

                    if (estadoActual == "Entregado" || estadoActual == "Cancelado")
                    {
                        ddlEstado.Enabled = false;
                        LinkButton btn = (LinkButton)e.Row.FindControl("btnActualizarEstado");
                        if (btn != null) btn.Visible = false;
                    }
                }
            }
        }

        protected void gvVentas_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "VerDetalle")
            {
                int idVenta = Convert.ToInt32(e.CommandArgument);
                lblModalIdVenta.Text = idVenta.ToString();
                gvDetallePedido.DataSource = ventaSQL.ListarDetallesPorVenta(idVenta);
                gvDetallePedido.DataBind();
                ScriptManager.RegisterStartupScript(this, this.GetType(), "OpenModal", "openModal();", true);
            }
            else if (e.CommandName == "ActualizarEstado")
            {
                try
                {
                    GridViewRow row = (GridViewRow)((LinkButton)e.CommandSource).NamingContainer;
                    DropDownList ddlEstado = (DropDownList)row.FindControl("ddlEstado");
                    int idVenta = Convert.ToInt32(e.CommandArgument);
                    string nuevoEstado = ddlEstado.SelectedValue;

                    ventaSQL.ActualizarEstado(idVenta, nuevoEstado);

                    if (nuevoEstado == "Cancelado")
                    {
                        List<DetalleVenta> detalles = ventaSQL.ListarDetallesPorVenta(idVenta);
                        articuloSQL.ReponerStock(detalles);

                        lblMensaje.Text = $"Pedido #{idVenta} cancelado. Stock repuesto.";
                        lblMensaje.CssClass = "alert alert-warning";
                    }
                    else
                    {
                        lblMensaje.Text = $"Estado actualizado a '{nuevoEstado}'.";
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