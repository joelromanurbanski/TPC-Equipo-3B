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
    public partial class GestionProveedores : Page
    {
        private ProveedorSQL proveedorSQL = new ProveedorSQL();
        private ArticuloSQL articuloSQL = new ArticuloSQL();

        private List<Proveedor> ListaProveedores { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            ListaProveedores = proveedorSQL.Listar();

            if (!IsPostBack)
            {
                BindSidebar(ListaProveedores);

                string idSeleccionado = Request.QueryString["id"];
                if (!string.IsNullOrEmpty(idSeleccionado))
                {
                    ShowProveedor(idSeleccionado);
                }
                else
                {
                    // No hay ID, mostrar panel vacío
                    pnlVacio.Visible = true;
                    pnlDetalle.Visible = false;
                    pnlFormulario.Visible = false;
                }
            }
        }

        private void BindSidebar(List<Proveedor> lista)
        {
            rptProveedores.DataSource = lista;
            rptProveedores.DataBind();

            if (lista == null || lista.Count == 0)
            {
                pnlEmptyProveedores.Visible = true;
                rptProveedores.Visible = false;
            }
            else
            {
                pnlEmptyProveedores.Visible = false;
                rptProveedores.Visible = true;
            }
        }

        private void BindProductos(int idProveedor)
        {
            gvProductos.DataSource = articuloSQL.ListarPorProveedor(idProveedor);
            gvProductos.DataBind();
        }

        protected void rptProveedores_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "Select")
            {
                string id = e.CommandArgument.ToString();
                Response.Redirect("GestionProveedores.aspx?id=" + id, false);
            }
        }

        private void ShowProveedor(string id)
        {
            try
            {
                int idProveedor = int.Parse(id);
                Proveedor p = ListaProveedores.FirstOrDefault(x => x.Id == idProveedor);

                if (p != null)
                {
                    pnlVacio.Visible = false;
                    pnlFormulario.Visible = false;
                    pnlDetalle.Visible = true; // Mostrar detalles

                    ViewState["SelectedProveedorId"] = idProveedor;

                    // Rellenar Pestaña "Datos Generales"
                    lblProveedorNombre.Text = p.Nombre;
                    lblCompanyName.Text = p.Nombre;
                    lblEmail.Text = p.Email ?? "N/A";
                    lblAddress.Text = p.Direccion ?? "N/A";
                    lblPhone.Text = p.Telefono ?? "N/A";

                    BindProductos(idProveedor);
                }
            }
            catch (Exception ex)
            {
                lblProveedorNombre.Text = "Error al cargar";
            }
        }

        // --- Eventos de Botones ---

        protected void btnBuscarProveedorSidebar_Click(object sender, EventArgs e)
        {
            string q = txtBuscarProveedorSidebar.Text.Trim();
            var filtered = ListaProveedores.Where(x => x.Nombre.IndexOf(q, StringComparison.OrdinalIgnoreCase) >= 0).ToList();
            BindSidebar(filtered);
        }

        protected void btnGlobalSearch_Click(object sender, EventArgs e)
        {
            string q = txtGlobalSearch.Text.Trim();
            var filtered = ListaProveedores.Where(x => x.Nombre.IndexOf(q, StringComparison.OrdinalIgnoreCase) >= 0).ToList();
            BindSidebar(filtered);

            if (filtered.Count == 1)
            {
                Response.Redirect("GestionProveedores.aspx?id=" + filtered[0].Id, false);
            }
        }

        protected void btnRefrescar_Click(object sender, EventArgs e)
        {
            Response.Redirect("GestionProveedores.aspx", false);
        }

        protected void btnEliminarProveedor_Click(object sender, EventArgs e)
        {
            try
            {
                if (ViewState["SelectedProveedorId"] != null)
                {
                    int id = (int)ViewState["SelectedProveedorId"];
                    proveedorSQL.Eliminar(id);
                    Response.Redirect("GestionProveedores.aspx", false);
                }
            }
            catch (Exception ex)
            {
                lblProveedorNombre.Text = "Error al eliminar: " + ex.Message;
            }
        }

        // --- ¡LÓGICA DE FORMULARIO NUEVA! ---

        private void LimpiarFormulario()
        {
            txtNombreForm.Text = "";
            txtEmailForm.Text = "";
            txtTelefonoForm.Text = "";
            txtDireccionForm.Text = "";
        }

        protected void btnNuevoProveedor_Click(object sender, EventArgs e)
        {
            // Ocultar los otros paneles
            pnlVacio.Visible = false;
            pnlDetalle.Visible = false;

            // Mostrar el formulario en modo "Nuevo"
            pnlFormulario.Visible = true;
            litTitulo.Text = "Nuevo Proveedor";
            LimpiarFormulario();
            btnGuardar.Visible = true;
            btnModificar.Visible = false;
        }

        protected void btnEditarProveedor_Click(object sender, EventArgs e)
        {
            if (ViewState["SelectedProveedorId"] == null) return;

            // Ocultar los otros paneles
            pnlVacio.Visible = false;
            pnlDetalle.Visible = false;

            // Mostrar el formulario en modo "Editar"
            pnlFormulario.Visible = true;
            litTitulo.Text = "Editar Proveedor";
            btnGuardar.Visible = false;
            btnModificar.Visible = true;

            // Cargar datos
            try
            {
                int id = (int)ViewState["SelectedProveedorId"];
                Proveedor p = ListaProveedores.FirstOrDefault(x => x.Id == id);
                if (p != null)
                {
                    txtNombreForm.Text = p.Nombre;
                    txtEmailForm.Text = p.Email;
                    txtTelefonoForm.Text = p.Telefono;
                    txtDireccionForm.Text = p.Direccion;
                }
            }
            catch (Exception ex)
            {
                // (Manejar error)
            }
        }

        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                try
                {
                    Proveedor nuevo = new Proveedor
                    {
                        Nombre = txtNombreForm.Text,
                        Email = txtEmailForm.Text,
                        Telefono = txtTelefonoForm.Text,
                        Direccion = txtDireccionForm.Text
                    };

                    proveedorSQL.Agregar(nuevo);
                    Response.Redirect("GestionProveedores.aspx", false);
                }
                catch (Exception ex)
                {
                    // (Manejar error)
                }
            }
        }

        protected void btnModificar_Click(object sender, EventArgs e)
        {
            if (Page.IsValid && ViewState["SelectedProveedorId"] != null)
            {
                try
                {
                    int id = (int)ViewState["SelectedProveedorId"];
                    Proveedor modificado = new Proveedor
                    {
                        Id = id,
                        Nombre = txtNombreForm.Text,
                        Email = txtEmailForm.Text,
                        Telefono = txtTelefonoForm.Text,
                        Direccion = txtDireccionForm.Text
                    };

                    proveedorSQL.Modificar(modificado);
                    Response.Redirect("GestionProveedores.aspx?id=" + id, false); // Volver al proveedor
                }
                catch (Exception ex)
                {
                    // (Manejar error)
                }
            }
        }

        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            pnlFormulario.Visible = false;

            // Decidir qué panel mostrar
            if (ViewState["SelectedProveedorId"] != null)
            {
                pnlDetalle.Visible = true;
            }
            else
            {
                pnlVacio.Visible = true;
            }
        }
    }
}