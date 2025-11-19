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
    public partial class RegistroVenta : System.Web.UI.Page
    {
        private ClienteSQL clienteSQL = new ClienteSQL();
        private ArticuloSQL articuloSQL = new ArticuloSQL();
        private VentaSQL ventaSQL = new VentaSQL();

        private const string ViewStateKey_Items = "Venta_Items";
        private const decimal ImpuestoPorcentaje = 0.21m; // 21% IVA

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                InicializarPagina();
            }
        }

        private void InicializarPagina()
        {
            ViewState[ViewStateKey_Items] = new List<DetalleVenta>();

            CargarClientes(null); // Cargar TODOS los clientes al inicio
            CargarArticulos();

            ddlCliente.SelectedIndex = 0;
            ddlArticulo.SelectedIndex = 0;
            txtCantidad.Text = "1";
            txtDescuento.Text = "0";
            lblPrecioUnitario.Text = "$0.00";
            lblStockDisponible.Text = "";
            txtBuscarClienteDNI.Text = "";

            ActualizarGridYTotales();
        }

        private void CargarClientes(string filtroDNI)
        {
            var listaClientes = clienteSQL.Listar();

            if (!string.IsNullOrEmpty(filtroDNI))
            {
                listaClientes = listaClientes.Where(c => c.Documento.Contains(filtroDNI)).ToList();
            }

            ddlCliente.DataSource = listaClientes;
            ddlCliente.DataTextField = "NombreCompleto";
            ddlCliente.DataValueField = "Id";
            ddlCliente.DataBind();
            ddlCliente.Items.Insert(0, new ListItem("Seleccionar cliente", "0"));
        }

        private void CargarArticulos()
        {
            var articulosConStock = articuloSQL.Listar().Where(a => a.StockActual > 0).ToList();
            ddlArticulo.DataSource = articulosConStock;
            ddlArticulo.DataTextField = "Nombre";
            ddlArticulo.DataValueField = "Id";
            ddlArticulo.DataBind();
            ddlArticulo.Items.Insert(0, new ListItem("Seleccionar producto", "0"));
        }

        protected void btnBuscarCliente_Click(object sender, EventArgs e)
        {
            string dni = txtBuscarClienteDNI.Text.Trim();
            if (string.IsNullOrEmpty(dni))
            {
                MostrarMensaje("Ingrese un DNI para buscar.");
                return;
            }

            CargarClientes(dni); // Recargar el dropdown solo con los clientes filtrados
            lblMensaje.Visible = false;

            // Si solo se encuentra 1, seleccionarlo
            if (ddlCliente.Items.Count == 2) 
            {
                ddlCliente.SelectedIndex = 1;
            }
        }
        protected void btnLimpiarBusquedaCliente_Click(object sender, EventArgs e)
        {
            txtBuscarClienteDNI.Text = "";
            CargarClientes(null); // Recargar todos los clientes
            lblMensaje.Visible = false;
        }


        protected void ddlArticulo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlArticulo.SelectedValue == "0")
            {
                lblPrecioUnitario.Text = "$0.00";
                lblStockDisponible.Text = "";
                return;
            }

            int idArticulo = int.Parse(ddlArticulo.SelectedValue);
            Articulo art = articuloSQL.GetById(idArticulo);

            if (art != null)
            {
                decimal precioVenta = art.UltimoPrecioCompra * (1 + (art.PorcentajeGanancia / 100));

                lblPrecioUnitario.Text = precioVenta.ToString("C");
                lblStockDisponible.Text = $"Stock disponible: {art.StockActual} unidades";
            }
        }

        protected void btnAgregarProducto_Click(object sender, EventArgs e)
        {
            if (ddlArticulo.SelectedValue == "0")
            {
                MostrarMensaje("Debe seleccionar un producto.");
                return;
            }
            if (!int.TryParse(txtCantidad.Text, out int cantidad) || cantidad <= 0)
            {
                MostrarMensaje("La cantidad debe ser un número mayor a 0.");
                return;
            }

            int idArticulo = int.Parse(ddlArticulo.SelectedValue);
            Articulo art = articuloSQL.GetById(idArticulo);

            if (cantidad > art.StockActual)
            {
                MostrarMensaje($"Stock insuficiente. Solo quedan {art.StockActual} unidades de {art.Nombre}.");
                return;
            }

            var items = (List<DetalleVenta>)ViewState[ViewStateKey_Items];
            var existente = items.FirstOrDefault(x => x.Articulo.Id == idArticulo);

            if (existente != null)
            {
                existente.Cantidad += cantidad;
                if (existente.Cantidad > art.StockActual)
                {
                    existente.Cantidad = art.StockActual;
                    MostrarMensaje($"Stock máximo alcanzado para {art.Nombre}.");
                }
                existente.Subtotal = existente.Cantidad * existente.PrecioUnitario;
            }
            else
            {
                decimal precioVenta = art.UltimoPrecioCompra * (1 + (art.PorcentajeGanancia / 100));

                DetalleVenta nuevoItem = new DetalleVenta
                {
                    Articulo = art,
                    Cantidad = cantidad,
                    PrecioUnitario = precioVenta,
                    Subtotal = cantidad * precioVenta
                };
                items.Add(nuevoItem);
            }

            ViewState[ViewStateKey_Items] = items;
            ActualizarGridYTotales();

            ddlArticulo.SelectedIndex = 0;
            txtCantidad.Text = "1";
            lblPrecioUnitario.Text = "$0.00";
            lblStockDisponible.Text = "";
            lblMensaje.Visible = false;
        }

        protected void gvVentaItems_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Eliminar")
            {
                int idArticulo = Convert.ToInt32(e.CommandArgument);
                var items = (List<DetalleVenta>)ViewState[ViewStateKey_Items];
                var itemParaEliminar = items.FirstOrDefault(x => x.ArticuloId == idArticulo);

                if (itemParaEliminar != null)
                {
                    items.Remove(itemParaEliminar);
                }

                ViewState[ViewStateKey_Items] = items;
                ActualizarGridYTotales();
            }
        }

        protected void btnGenerarVenta_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid) return;

            var items = (List<DetalleVenta>)ViewState[ViewStateKey_Items];
            if (items == null || items.Count == 0)
            {
                MostrarMensaje("Debe agregar al menos un producto a la venta.");
                return;
            }

            try
            {
                Venta nuevaVenta = new Venta
                {
                    Cliente = new Cliente { Id = int.Parse(ddlCliente.SelectedValue) },
                    Fecha = DateTime.Now,
                    NumeroFactura = ventaSQL.GenerarNumeroFactura(),
                    Detalles = items,
                    TotalVenta = decimal.Parse(lblTotal.Text, System.Globalization.NumberStyles.Currency)
                };

                int idVenta = ventaSQL.RegistrarVenta(nuevaVenta);

                InicializarPagina();

                MostrarMensaje($"¡Venta registrada con éxito! Factura N°: {nuevaVenta.NumeroFactura}", true);
            }
            catch (Exception ex)
            {
                MostrarMensaje("Error al guardar la venta: " + ex.Message);
            }
        }

        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            InicializarPagina();
            lblMensaje.Visible = false;
        }

        protected void txtCalculo_TextChanged(object sender, EventArgs e)
        {
            ActualizarGridYTotales();
        }

        private void ActualizarGridYTotales()
        {
            var items = (List<DetalleVenta>)ViewState[ViewStateKey_Items];

            gvVentaItems.DataSource = items;
            gvVentaItems.DataBind();

            decimal subtotal = items.Sum(i => i.Subtotal);

            decimal descuentoPorc = 0;
            decimal.TryParse(txtDescuento.Text, out descuentoPorc);
            decimal montoDescuento = Math.Round(subtotal * (descuentoPorc / 100), 2);

            decimal subtotalConDescuento = subtotal - montoDescuento;
            decimal iva = Math.Round(subtotalConDescuento * ImpuestoPorcentaje, 2);
            decimal total = subtotalConDescuento + iva;

            lblSubtotal.Text = subtotal.ToString("C");
            lblIVA.Text = iva.ToString("C");
            lblTotal.Text = total.ToString("C");
        }

        private void MostrarMensaje(string texto, bool exito = false)
        {
            lblMensaje.Text = texto;
            lblMensaje.CssClass = exito ? "alert alert-success" : "alert alert-danger";
            lblMensaje.Visible = true;
        }
    }
}