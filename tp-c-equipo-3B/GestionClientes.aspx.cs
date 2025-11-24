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
        private ClienteSQL clienteSQL = new ClienteSQL();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindGrid();
                FillEstadisticas(); // Llenar stats al cargar
            }
        }

        private void BindGrid(string filtro = null)
        {
            var lista = clienteSQL.Listar();

            if (!string.IsNullOrEmpty(filtro))
            {
                filtro = filtro.ToLowerInvariant();
                lista = lista.Where(c => c.NombreCompleto.ToLowerInvariant().Contains(filtro)
                                      || c.Documento.Contains(filtro)).ToList();
            }

            gvClientes.DataSource = lista;
            gvClientes.DataBind();
        }

        protected void btnNuevoCliente_Click(object sender, EventArgs e)
        {
            LimpiarFormulario();
            pnlClienteForm.Visible = true;
            lblMensaje.Visible = false;
        }

        protected void btnCancelarCliente_Click(object sender, EventArgs e)
        {
            pnlClienteForm.Visible = false;
            lblMensaje.Visible = false;
        }

        protected void btnGuardarCliente_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid) return;

            try
            {
                Cliente cliente = new Cliente
                {
                    Nombre = txtNombre.Text.Trim(),
                    Apellido = txtApellido.Text.Trim(),
                    Documento = txtCedula.Text.Trim(),
                    Email = txtEmail.Text.Trim(),
                    Telefono = txtTelefono.Text.Trim(),
                    Direccion = txtDireccion.Text.Trim()
                };

                if (string.IsNullOrEmpty(hfEditingId.Value))
                {
                    // Nuevo
                    clienteSQL.Agregar(cliente);
                    lblMensaje.Text = "Cliente agregado exitosamente.";
                    lblMensaje.CssClass = "alert alert-success d-block mb-3";
                }
                else
                {
                    // Modificar
                    cliente.Id = int.Parse(hfEditingId.Value);
                    clienteSQL.Modificar(cliente);
                    lblMensaje.Text = "Cliente modificado exitosamente.";
                    lblMensaje.CssClass = "alert alert-success d-block mb-3";
                }

                lblMensaje.Visible = true;
                pnlClienteForm.Visible = false;
                BindGrid();
                FillEstadisticas();
            }
            catch (Exception ex)
            {
                lblMensaje.Text = "Error: " + ex.Message;
                lblMensaje.CssClass = "alert alert-danger d-block mb-3";
                lblMensaje.Visible = true;
            }
        }

        protected void gvClientes_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Editar")
            {
                int id = int.Parse(e.CommandArgument.ToString());
                var cliente = clienteSQL.Listar().FirstOrDefault(c => c.Id == id);

                if (cliente != null)
                {
                    hfEditingId.Value = cliente.Id.ToString();
                    txtNombre.Text = cliente.Nombre;
                    txtApellido.Text = cliente.Apellido;
                    txtCedula.Text = cliente.Documento;
                    txtEmail.Text = cliente.Email;
                    txtTelefono.Text = cliente.Telefono;
                    txtDireccion.Text = cliente.Direccion;

                    pnlClienteForm.Visible = true;
                }
            }
            else if (e.CommandName == "Eliminar")
            {
                try
                {
                    int id = int.Parse(e.CommandArgument.ToString());
                    clienteSQL.Eliminar(id);
                    BindGrid();
                    FillEstadisticas();
                    lblMensaje.Text = "Cliente eliminado.";
                    lblMensaje.CssClass = "alert alert-warning d-block mb-3";
                    lblMensaje.Visible = true;
                }
                catch (Exception ex)
                {
                    lblMensaje.Text = "Error al eliminar: " + ex.Message;
                    lblMensaje.CssClass = "alert alert-danger d-block mb-3";
                    lblMensaje.Visible = true;
                }
            }
        }

        protected void btnBuscarClientes_Click(object sender, EventArgs e)
        {
            gvClientes.PageIndex = 0;
            BindGrid(txtBuscarClientes.Text.Trim());
        }

        protected void btnRefrescar_Click(object sender, EventArgs e)
        {
            txtBuscarClientes.Text = "";
            gvClientes.PageIndex = 0;
            BindGrid();
            lblMensaje.Visible = false;
            pnlClienteForm.Visible = false;
        }

        protected void gvClientes_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvClientes.PageIndex = e.NewPageIndex;
            BindGrid(txtBuscarClientes.Text.Trim());
        }

        private void LimpiarFormulario()
        {
            hfEditingId.Value = "";
            txtNombre.Text = "";
            txtApellido.Text = "";
            txtCedula.Text = "";
            txtEmail.Text = "";
            txtTelefono.Text = "";
            txtDireccion.Text = "";
        }

        private void FillEstadisticas()
        {
            // Lógica simple para las tarjetas de estadísticas
            var lista = clienteSQL.Listar();
            lblTotalClientes.Text = lista.Count.ToString();
            lblClientesActivos.Text = lista.Count.ToString(); // Por ahora igual al total

            // Simulación de "Recientes" (podrías mejorarlo con SQL si tuvieras fecha de alta)
            lblClientesRecientes.Text = "N/A";
        }
    }

    // Clase simple para el historial (placeholder visual)
    public class HistoryEntry
    {
        public DateTime Date { get; set; }
        public string Action { get; set; }
        public string Details { get; set; }
    }
}