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
    public partial class InicioSesión : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            string usuario = txtUsuario.Text;
            string password = txtPassword.Text;

            // ejemplo que puse para ingresar
            if (usuario == "Prueba" && password == "1234")
            {
                Response.Redirect("GestionProducto.aspx");
            }
            else
            {
                ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Usuario o contraseña incorrectos');", true);
            }
        }
    }
}