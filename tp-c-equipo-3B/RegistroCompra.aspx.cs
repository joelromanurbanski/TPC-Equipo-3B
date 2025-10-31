using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace tp_c_equipo_3B
{
    public partial class RegistroCompra : System.Web.UI.Page
    {
        
        
            // Modelo simple para las filas de productos en la compra
            [Serializable]
            public class CompraItem
            {
                public int IdProducto { get; set; }
                public string Nombre { get; set; }
                public int Cantidad { get; set; }
                public decimal PrecioUnitario { get; set; }
                public decimal Subtotal => Cantidad * PrecioUnitario;
            }

            private const string ViewStateKey_Items = "Compra_Items";
            private const decimal ImpuestoPorcentaje = 0.18m; // 18%

            protected void Page_Load(object sender, EventArgs e)
            {
                if (!IsPostBack)
                {
                    InicializarPagina();
                }
            }

            private void InicializarPagina()
            {
                // Inicializa la lista en ViewState
                if (ViewState[ViewStateKey_Items] == null)
                    ViewState[ViewStateKey_Items] = new List<CompraItem>();

                // Inicializar controles si existen (si adaptás .aspx a controles servidor)
                // Ejemplo: ddlProveedor.DataSource = proveedorSQL.Listar(); ddlProveedor.DataBind();
                ActualizarGridYTotales();
            }

            // Invocado por un botón "Agregar" que toma valores de búsqueda / selección
            protected void btnAgregar_Click(object sender, EventArgs e)
            {
                var items = (List<CompraItem>)ViewState[ViewStateKey_Items];

                // Ejemplo: obtener producto seleccionado desde un input o control;
                // aquí asumo que pasás IdProducto, Nombre, Cantidad y PrecioUnitario desde la UI.
                // Reemplazá estas lecturas por las IDs reales de tus controles.
                int idProducto = 0;
                string nombre = Request.Form["inputProductoNombre"] ?? "Producto sin nombre";
                int cantidad = int.TryParse(Request.Form["inputCantidad"], out var c) ? c : 1;
                decimal precio = decimal.TryParse(Request.Form["inputPrecio"], out var p) ? p : 0m;

                var existente = items.FirstOrDefault(x => x.IdProducto == idProducto && x.Nombre == nombre);
                if (existente != null)
                {
                    existente.Cantidad += cantidad;
                    existente.PrecioUnitario = precio;
                }
                else
                {
                    items.Add(new CompraItem
                    {
                        IdProducto = idProducto,
                        Nombre = nombre,
                        Cantidad = cantidad,
                        PrecioUnitario = precio
                    });
                }

                ViewState[ViewStateKey_Items] = items;
                ActualizarGridYTotales();
            }

            // Handler para eliminar una fila (si usás GridView delete)
            protected void gvProductos_RowCommand(object sender, GridViewCommandEventArgs e)
            {
                if (e.CommandName == "Eliminar")
                {
                    int index = Convert.ToInt32(e.CommandArgument);
                    var items = (List<CompraItem>)ViewState[ViewStateKey_Items];
                    if (index >= 0 && index < items.Count)
                    {
                        items.RemoveAt(index);
                        ViewState[ViewStateKey_Items] = items;
                        ActualizarGridYTotales();
                    }
                }
            }

            // Guarda la compra: validaciones básicas y persistencia mínima (adaptar a tu capa SQL)
            protected void btnGuardar_Click(object sender, EventArgs e)
            {
                try
                {
                    var items = (List<CompraItem>)ViewState[ViewStateKey_Items];
                    if (items == null || items.Count == 0)
                    {
                        MostrarMensaje("Agregá al menos un producto antes de guardar.");
                        return;
                    }

                    // Lectura de campos (adaptá nombres de controles)
                    string proveedor = Request.Form["ddlProveedor"] ?? "";
                    string numeroFactura = Request.Form["txtNumeroFactura"] ?? "";
                    DateTime fechaCompra = DateTime.TryParse(Request.Form["inputFecha"], out var f) ? f : DateTime.Now;
                    decimal otrosCostos = decimal.TryParse(Request.Form["inputOtrosCostos"], out var oc) ? oc : 0m;

                    decimal subtotal = items.Sum(i => i.Subtotal);
                    decimal impuestos = Math.Round(subtotal * ImpuestoPorcentaje, 2);
                    decimal total = subtotal + impuestos + otrosCostos;

                    // Aquí llamá a tu capa SQL para persistir la compra y sus líneas.
                    // Ejemplo conceptual:
                    // int idCompra = compraSQL.CrearCompra(proveedor, numeroFactura, fechaCompra, subtotal, impuestos, otrosCostos, total);
                    // foreach (var it in items) compraSQL.AgregarLinea(idCompra, it.IdProducto, it.Cantidad, it.PrecioUnitario);

                    // Simulación: limpiar y volver a inicializar
                    ViewState[ViewStateKey_Items] = new List<CompraItem>();
                    ActualizarGridYTotales();
                    MostrarMensaje("Compra guardada correctamente.");
                }
                catch (Exception ex)
                {
                    MostrarMensaje("Error al guardar la compra: " + ex.Message);
                }
            }

            protected void btnCancelar_Click(object sender, EventArgs e)
            {
                // Limpiar formulario y lista
                ViewState[ViewStateKey_Items] = new List<CompraItem>();
                ActualizarGridYTotales();
                // Opcional: limpiar inputs mediante Request/Form o controles server
            }

            // Calcula totales y refresca la grilla (si usás GridView, enlazalo aquí)
            private void ActualizarGridYTotales()
            {
                var items = (List<CompraItem>)ViewState[ViewStateKey_Items] ?? new List<CompraItem>();

                decimal subtotal = items.Sum(i => i.Subtotal);
                decimal impuestos = Math.Round(subtotal * ImpuestoPorcentaje, 2);

                // Leer otros costos desde form si existe
                decimal otrosCostos = decimal.TryParse(Request.Form["inputOtrosCostos"], out var oc) ? oc : 0m;

                decimal total = subtotal + impuestos + otrosCostos;

                // Si tenés controles server, actualizalos. Ejemplos (descomentar si existen en .aspx):
                // gvProductos.DataSource = items;
                // gvProductos.DataBind();
                // lblSubtotal.Text = subtotal.ToString("C");
                // lblImpuestos.Text = impuestos.ToString("C");
                // txtOtrosCostos.Text = otrosCostos.ToString("N2");
                // lblTotal.Text = total.ToString("C");

                // Si no tenés controles server, podés guardar valores en ViewState para que la UI los lea por script
                ViewState["Compra_Subtotal"] = subtotal;
                ViewState["Compra_Impuestos"] = impuestos;
                ViewState["Compra_Otros"] = otrosCostos;
                ViewState["Compra_Total"] = total;
            }

            // Mensajería simple: adaptar a control label en la UI
            private void MostrarMensaje(string texto)
            {
                // Si tenés un Label server llamado lblMensaje:
                // lblMensaje.Text = texto;
                // lblMensaje.Visible = true;

                // Alternativa: almacenar en ViewState para mostrar con JS en la página
                ViewState["Compra_Mensaje"] = texto;
            }
        }
    }

