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
            private const decimal ImpuestoPorcentaje = 0.18m; 

            protected void Page_Load(object sender, EventArgs e)
            {
                if (!IsPostBack)
                {
                    InicializarPagina();
                }
            }

            private void InicializarPagina()
            {
                
                if (ViewState[ViewStateKey_Items] == null)
                    ViewState[ViewStateKey_Items] = new List<CompraItem>();

               
                ActualizarGridYTotales();
            }

           
            protected void btnAgregar_Click(object sender, EventArgs e)
            {
                var items = (List<CompraItem>)ViewState[ViewStateKey_Items];

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

                   
                    string proveedor = Request.Form["ddlProveedor"] ?? "";
                    string numeroFactura = Request.Form["txtNumeroFactura"] ?? "";
                    DateTime fechaCompra = DateTime.TryParse(Request.Form["inputFecha"], out var f) ? f : DateTime.Now;
                    decimal otrosCostos = decimal.TryParse(Request.Form["inputOtrosCostos"], out var oc) ? oc : 0m;

                    decimal subtotal = items.Sum(i => i.Subtotal);
                    decimal impuestos = Math.Round(subtotal * ImpuestoPorcentaje, 2);
                    decimal total = subtotal + impuestos + otrosCostos;

               
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
                
                ViewState[ViewStateKey_Items] = new List<CompraItem>();
                ActualizarGridYTotales();
                
            }

            
            private void ActualizarGridYTotales()
            {
                var items = (List<CompraItem>)ViewState[ViewStateKey_Items] ?? new List<CompraItem>();

                decimal subtotal = items.Sum(i => i.Subtotal);
                decimal impuestos = Math.Round(subtotal * ImpuestoPorcentaje, 2);

                
                decimal otrosCostos = decimal.TryParse(Request.Form["inputOtrosCostos"], out var oc) ? oc : 0m;

                decimal total = subtotal + impuestos + otrosCostos;

              
                ViewState["Compra_Subtotal"] = subtotal;
                ViewState["Compra_Impuestos"] = impuestos;
                ViewState["Compra_Otros"] = otrosCostos;
                ViewState["Compra_Total"] = total;
            }

           
            private void MostrarMensaje(string texto)
            {
               
                ViewState["Compra_Mensaje"] = texto;
            }
        }
    }

