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
        private UsuarioSQL usuarioSQL = new UsuarioSQL(); // Necesario para cargar vendedores

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                CargarVendedores();
                BindGrid();
            }
        }

        private void CargarVendedores()
        {
            // Cargar lista de usuarios para el filtro
            List<Usuario> usuarios = usuarioSQL.Listar();
            ddlFiltroVendedor.DataSource = usuarios;
            ddlFiltroVendedor.DataTextField = "NombreCompleto";
            ddlFiltroVendedor.DataValueField = "Id";
            ddlFiltroVendedor.DataBind();

            // Agregar opción por defecto
            ddlFiltroVendedor.Items.Insert(0, new ListItem("Vendedor: Todos", "0"));
        }

        private void BindGrid()
        {
            // Filtros
            string busqueda = txtBuscar.Text.Trim();
            string estado = ddlFiltroEstado.SelectedValue;
            bool ordenAsc = ddlOrden.SelectedValue == "ASC";
            int idUsuario = int.Parse(ddlFiltroVendedor.SelectedValue); // Filtro de Vendedor

            // Fechas
            DateTime? fechaInicio = null;
            DateTime? fechaFin = null;
            if (!string.IsNullOrEmpty(txtFechaDesde.Text)) fechaInicio = DateTime.Parse(txtFechaDesde.Text);
            if (!string.IsNullOrEmpty(txtFechaHasta.Text)) fechaFin = DateTime.Parse(txtFechaHasta.Text);

            try
            {
                // Pasar todos los filtros al método SQL
                gvVentas.DataSource = ventaSQL.ListarVentas(busqueda, estado, ordenAsc, fechaInicio, fechaFin, idUsuario);
                gvVentas.DataBind();
            }
            catch (Exception ex)
            {
                MostrarMensaje("Error al cargar: " + ex.Message, false);
            }
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
            ddlFiltroVendedor.SelectedValue = "0"; // Resetear vendedor
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
                case "Listo para Despachar": return "gv-estado-Listo";
                case "Enviado": return "gv-estado-Enviado";
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
                    ListItem item = ddlEstado.Items.FindByValue(estadoActual);
                    if (item != null) ddlEstado.SelectedValue = estadoActual;

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
                    GridViewRow row = (GridViewRow)((System.Web.UI.Control)e.CommandSource).NamingContainer;
                    DropDownList ddlEstado = (DropDownList)row.FindControl("ddlEstado");

                    int idVenta = Convert.ToInt32(e.CommandArgument);
                    string nuevoEstado = ddlEstado.SelectedValue;

                    ventaSQL.ActualizarEstado(idVenta, nuevoEstado);

                    if (nuevoEstado == "Cancelado")
                    {
                        var detalles = ventaSQL.ListarDetallesPorVenta(idVenta);
                        articuloSQL.ReponerStock(detalles);
                        MostrarMensaje($"Pedido #{idVenta} cancelado. Stock devuelto al inventario.", false);
                        lblMensaje.CssClass = "alert alert-warning d-block mb-3 rounded-3";
                    }
                    else
                    {
                        MostrarMensaje($"Estado del pedido #{idVenta} actualizado a '{nuevoEstado}'.", true);
                    }

                    BindGrid();
                }
                catch (Exception ex)
                {
                    MostrarMensaje("Error: " + ex.Message, false);
                }
            }
        }

        private void MostrarMensaje(string texto, bool exito)
        {
            lblMensaje.Text = texto;
            lblMensaje.CssClass = exito ? "alert alert-success d-block mb-3 rounded-3" : "alert alert-danger d-block mb-3 rounded-3";
            lblMensaje.Visible = true;
        }
    }
}