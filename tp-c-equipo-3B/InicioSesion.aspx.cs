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
    public partial class InicioSesion : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["usuario"] != null)
            {
                Response.Redirect("PaneldeControl.aspx", false);
            }
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            string user = txtEmail.Text;
            string pass = txtPass.Text;

            UsuarioSQL negocio = new UsuarioSQL();
            try
            {
                Usuario usuarioLogueado = negocio.Loguear(user, pass);

                if (usuarioLogueado != null)
                {
                    Session.Add("usuario", usuarioLogueado);
                    Response.Redirect("PaneldeControl.aspx", false);
                }
                else
                {
                    pnlError.Visible = true;
                    lblError.Text = "Usuario o contraseña incorrectos.";
                }
            }
            catch (Exception ex)
            {
                pnlError.Visible = true;
                lblError.Text = "Error: " + ex.Message;
            }
        }
    }
}