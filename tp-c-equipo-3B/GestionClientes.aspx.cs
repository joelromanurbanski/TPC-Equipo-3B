using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using Dominio;
using SQL;

namespace tp_c_equipo_3B
{
    public partial class GestionClientes : Page
    {
        // Instancia de la capa de datos
        private ClienteSQL clienteSQL = new ClienteSQL();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindGrid();
            }
        }

        private void BindGrid(string filtro = null)
        {
            var lista = clienteSQL.Listar();

            // Proyección para la grilla
            var data = lista.Select(c => new
            {
                c.Id,
                NombreCompleto = $"{c.Nombre} {c.Apellido}".Trim(),
                c.Documento,
                c.Telefono,
                c.Email
            }).ToList();

            if (!string.IsNullOrEmpty(filtro))
            {
                filtro = filtro.ToLowerInvariant();
                data = data.Where(d => d.NombreCompleto.ToLowerInvariant().Contains(filtro)
                    || (d.Documento != null && d.Documento.ToLowerInvariant().Contains(filtro))
                    || (d.Email != null && d.Email.ToLowerInvariant().Contains(filtro))).ToList();
            }

            gvClientes.DataSource = data;
            gvClientes.DataBind();
        }

        private void ClearForm()
        {
            txtNombre.Text = "";
            txtApellido.Text = "";
            txtDocumento.Text = "";
            txtTelefono.Text = "";
            txtEmail.Text = "";
            txtDireccion.Text = "";
            txtCiudad.Text = "";
            txtCP.Text = "";
            hfEditingId.Value = "";
        }

        protected void btnNuevoCliente_Click(object sender, EventArgs e)
        {
            ClearForm();
            pnlClienteForm.Visible = true;
            hfEditingId.Value = string.Empty;
            lblMensaje.Text = string.Empty;

            litTitulo.Text = "Nuevo Cliente";
            btnGuardar.Visible = true;
            btnModificar.Visible = false;
        }

        protected void btnCancelarCliente_Click(object sender, EventArgs e)
        {
            pnlClienteForm.Visible = false;
            lblMensaje.Text = string.Empty;
            ClearForm();
        }

        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                try
                {
                    var cliente = new Cliente
                    {
                        Nombre = txtNombre.Text.Trim(),
                        Apellido = txtApellido.Text.Trim(),
                        Documento = txtDocumento.Text.Trim(),
                        Telefono = txtTelefono.Text.Trim(),
                        Email = txtEmail.Text.Trim(),
                        Direccion = txtDireccion.Text.Trim(),
                        Ciudad = txtCiudad.Text.Trim(),
                        CP = string.IsNullOrEmpty(txtCP.Text) ? 0 : int.Parse(txtCP.Text)
                    };

                    clienteSQL.Agregar(cliente);

                    BindGrid();
                    pnlClienteForm.Visible = false;
                    ClearForm();
                }
                catch (Exception ex)
                {
                    lblMensaje.Text = "Error al guardar: " + ex.Message;
                }
            }
        }

        protected void btnModificar_Click(object sender, EventArgs e)
        {
            if (Page.IsValid && !string.IsNullOrEmpty(hfEditingId.Value))
            {
                try
                {
                    var cliente = new Cliente
                    {
                        Id = int.Parse(hfEditingId.Value),
                        Nombre = txtNombre.Text.Trim(),
                        Apellido = txtApellido.Text.Trim(),
                        Documento = txtDocumento.Text.Trim(),
                        Telefono = txtTelefono.Text.Trim(),
                        Email = txtEmail.Text.Trim(),
                        Direccion = txtDireccion.Text.Trim(),
                        Ciudad = txtCiudad.Text.Trim(),
                        CP = string.IsNullOrEmpty(txtCP.Text) ? 0 : int.Parse(txtCP.Text)
                    };

                    clienteSQL.Modificar(cliente);

                    BindGrid();
                    pnlClienteForm.Visible = false;
                    ClearForm();
                }
                catch (Exception ex)
                {
                    lblMensaje.Text = "Error al modificar: " + ex.Message;
                }
            }
        }

        protected void btnBuscarClientes_Click(object sender, EventArgs e)
        {
            BindGrid(txtBuscarClientes.Text.Trim());
        }

        protected void gvClientes_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                int id = int.Parse(e.CommandArgument.ToString());
                var lista = clienteSQL.Listar(); // Volver a cargar la lista

                if (e.CommandName == "Editar")
                {
                    var c = lista.FirstOrDefault(x => x.Id == id);
                    if (c != null)
                    {
                        hfEditingId.Value = c.Id.ToString();
                        txtNombre.Text = c.Nombre;
                        txtApellido.Text = c.Apellido;
                        txtDocumento.Text = c.Documento;
                        txtTelefono.Text = c.Telefono;
                        txtEmail.Text = c.Email;
                        txtDireccion.Text = c.Direccion;
                        txtCiudad.Text = c.Ciudad;
                        txtCP.Text = c.CP.ToString();

                        pnlClienteForm.Visible = true;
                        litTitulo.Text = "Editar Cliente";
                        btnGuardar.Visible = false;
                        btnModificar.Visible = true;
                    }
                }
                else if (e.CommandName == "Eliminar")
                {
                    clienteSQL.Eliminar(id);
                    BindGrid();
                }
            }
            catch (Exception ex)
            {
                lblMensaje.Text = "Error: " + ex.Message;
            }
        }

        protected void gvClientes_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvClientes.PageIndex = e.NewPageIndex;
            BindGrid(txtBuscarClientes.Text.Trim()); // Mantener el filtro al paginar
        }

        protected void btnRefrescar_Click(object sender, EventArgs e)
        {
            txtBuscarClientes.Text = "";
            BindGrid();
            pnlClienteForm.Visible = false;
            lblMensaje.Text = string.Empty;
        }
    }
}