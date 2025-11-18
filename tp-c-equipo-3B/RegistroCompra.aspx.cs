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
    public partial class RegistroCompra : System.Web.UI.Page
    {
        private ProveedorSQL proveedorSQL = new ProveedorSQL();
        private ArticuloSQL articuloSQL = new ArticuloSQL();
        private CompraSQL compraSQL = new CompraSQL();

        // ViewState
        private const string ViewStateKey_Items = "Compra_Items";
        // Impuesto
        private const decimal ImpuestoPorcentaje = 0.21m;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                InicializarPagina();
            }
        }

        private void InicializarPagina()
        {
            
            ViewState[ViewStateKey_Items] = new List<DetalleCompra>();

           
            CargarProveedores();
            CargarArticulos();

            // Fecha de hoy
            txtFecha.Text = DateTime.Now.ToString("yyyy-MM-dd");

            
            ActualizarGridYTotales();
        }

        private void CargarProveedores()
        {
            ddlProveedor.DataSource = proveedorSQL.Listar();
            ddlProveedor.DataTextField = "Nombre";
            ddlProveedor.DataValueField = "Id";
            ddlProveedor.DataBind();
            ddlProveedor.Items.Insert(0, new ListItem("Seleccionar proveedor", "0"));
        }

        private void CargarArticulos()
        {
            ddlArticulo.DataSource = articuloSQL.Listar();
            ddlArticulo.DataTextField = "Nombre";
            ddlArticulo.DataValueField = "Id";
            ddlArticulo.DataBind();
            ddlArticulo.Items.Insert(0, new ListItem("Seleccionar producto", "0"));
        }

        protected void btnAgregarProducto_Click(object sender, EventArgs e)
        {
            // Validaciones
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
            if (!decimal.TryParse(txtPrecioCompraUnitario.Text, out decimal precio) || precio <= 0)
            {
                MostrarMensaje("El precio de costo debe ser un número mayor a 0.");
                return;
            }

            // Cargar lista ViewState
            var items = (List<DetalleCompra>)ViewState[ViewStateKey_Items];
            int idArticulo = int.Parse(ddlArticulo.SelectedValue);

            // Verificar si el producto ya está en la lista
            var existente = items.FirstOrDefault(x => x.Articulo.Id == idArticulo);

            if (existente != null)
            {
                // Si existe, actualizar cantidad y precio
                existente.Cantidad += cantidad;
                existente.PrecioCompra = precio;
            }
            else
            {
                // Si no existe, crear el nuevo DetalleCompra
                DetalleCompra nuevoItem = new DetalleCompra
                {
                    Articulo = new Articulo
                    {
                        Id = idArticulo,
                        Nombre = ddlArticulo.SelectedItem.Text
                    },
                    Cantidad = cantidad,
                    PrecioCompra = precio
                };
                items.Add(nuevoItem);
            }

            // Guardar lista en ViewState y actualizar UI
            ViewState[ViewStateKey_Items] = items;
            ActualizarGridYTotales();

            // Limpiar campos de agregado
            ddlArticulo.SelectedIndex = 0;
            txtCantidad.Text = "1";
            txtPrecioCompraUnitario.Text = "0";
            lblMensaje.Visible = false;
        }

        protected void gvProductos_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Eliminar")
            {
                int idArticulo = Convert.ToInt32(e.CommandArgument);
                var items = (List<DetalleCompra>)ViewState[ViewStateKey_Items];

                var itemParaEliminar = items.FirstOrDefault(x => x.ArticuloId == idArticulo);
                if (itemParaEliminar != null)
                {
                    items.Remove(itemParaEliminar);
                }

                ViewState[ViewStateKey_Items] = items;
                ActualizarGridYTotales();
            }
        }

        protected void btnGuardarCompra_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid) return;

            var items = (List<DetalleCompra>)ViewState[ViewStateKey_Items];
            if (items == null || items.Count == 0)
            {
                MostrarMensaje("Debe agregar al menos un producto a la compra.");
                return;
            }

            try
            {
                // Crear el objeto Compra 
                Compra nuevaCompra = new Compra
                {
                    Proveedor = new Proveedor { Id = int.Parse(ddlProveedor.SelectedValue) },
                    Fecha = DateTime.Parse(txtFecha.Text),
                    Detalles = items, 
                    TotalCompra = decimal.Parse(lblTotal.Text, System.Globalization.NumberStyles.Currency)
                };

                // Llamar  SQL
                compraSQL.RegistrarCompra(nuevaCompra);

                // Limpiar
                InicializarPagina();
                MostrarMensaje("¡Compra registrada con éxito! El stock y los precios de costo han sido actualizados.", true);
            }
            catch (Exception ex)
            {
                MostrarMensaje("Error al guardar la compra: " + ex.Message);
            }
        }

        protected void btnCancelarCompra_Click(object sender, EventArgs e)
        {
            InicializarPagina();
            lblMensaje.Visible = false;
        }

        protected void txtOtrosCostos_TextChanged(object sender, EventArgs e)
        {
            // Calcular totales si hay costos adicionales
            ActualizarGridYTotales();
        }

        private void ActualizarGridYTotales()
        {
            var items = (List<DetalleCompra>)ViewState[ViewStateKey_Items];

            
            gvProductos.DataSource = items;
            gvProductos.DataBind();

            // Calcular totales
            decimal subtotal = items.Sum(i => i.Subtotal);
            decimal impuestos = Math.Round(subtotal * ImpuestoPorcentaje, 2);
            decimal otrosCostos = 0;
            decimal.TryParse(txtOtrosCostos.Text, out otrosCostos);

            decimal total = subtotal + impuestos + otrosCostos;

            // Actualizar Labels del resumen
            lblSubtotal.Text = subtotal.ToString("C"); // "C" = Formato Moneda
            lblImpuestos.Text = impuestos.ToString("C");
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