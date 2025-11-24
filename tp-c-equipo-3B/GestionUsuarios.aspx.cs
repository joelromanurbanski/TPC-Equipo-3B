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
    public partial class GestionUsuarios : System.Web.UI.Page
    {
        private UsuarioSQL usuarioSQL = new UsuarioSQL();

        protected void Page_Load(object sender, EventArgs e)
        {
            // Seguridad
            if (Session["usuario"] != null)
            {
                Usuario user = (Usuario)Session["usuario"];
                if (!user.EsAdmin) Response.Redirect("PaneldeControl.aspx");
            }
            else
            {
                Response.Redirect("InicioSesion.aspx");
            }

            if (!IsPostBack) CargarGrilla();
        }

        private void CargarGrilla()
        {
            gvUsuarios.DataSource = usuarioSQL.Listar();
            gvUsuarios.DataBind();
        }

        protected void btnNuevo_Click(object sender, EventArgs e)
        {
            LimpiarForm();
            pnlFormulario.Visible = true;
        }

        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                Usuario user = new Usuario();
                user.Email = txtEmail.Text;
                user.Contrasenia = txtPass.Text;
                user.Nombre = txtNombre.Text;
                user.Apellido = txtApellido.Text;
                user.EsAdmin = chkAdmin.Checked;

                if (string.IsNullOrEmpty(hfIdUsuario.Value))
                    usuarioSQL.Agregar(user);
                else
                {
                    user.Id = int.Parse(hfIdUsuario.Value);
                    usuarioSQL.Modificar(user);
                }

                pnlFormulario.Visible = false;
                CargarGrilla();
            }
            catch (Exception ex)
            {
                lblMensaje.Text = "Error: " + ex.Message;
                lblMensaje.Visible = true;
            }
        }

        protected void gvUsuarios_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int id = Convert.ToInt32(e.CommandArgument);
            if (e.CommandName == "Eliminar")
            {
                Usuario actual = (Usuario)Session["usuario"];
                if (actual.Id == id) return; // No auto-eliminar
                usuarioSQL.Eliminar(id);
                CargarGrilla();
            }
            else if (e.CommandName == "Editar")
            {
                Usuario user = usuarioSQL.GetById(id);
                if (user != null)
                {
                    hfIdUsuario.Value = user.Id.ToString();
                    txtEmail.Text = user.Email;
                    txtPass.Text = user.Contrasenia;
                    txtNombre.Text = user.Nombre;
                    txtApellido.Text = user.Apellido;
                    chkAdmin.Checked = user.EsAdmin;
                    pnlFormulario.Visible = true;
                }
            }
        }

        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            pnlFormulario.Visible = false;
            LimpiarForm();
        }

        private void LimpiarForm()
        {
            txtEmail.Text = ""; txtPass.Text = ""; txtNombre.Text = ""; txtApellido.Text = "";
            chkAdmin.Checked = false; hfIdUsuario.Value = "";
        }
    }
}