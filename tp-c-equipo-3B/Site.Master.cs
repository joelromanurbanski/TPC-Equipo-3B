using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace tp_c_equipo_3B
{
    public partial class SiteMaster : MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["Usuario"] != null)
                    lblUsuario.Text = "Bienvenido, " + Session["Usuario"].ToString();

                if (Session["Rol"]?.ToString() != "Admin" && linkUsuarios != null)
                    linkUsuarios.Visible = false;
            }
        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Response.Redirect("~/InicioSesion.aspx");
        }

        protected void btnGenerar_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Generador.aspx");
        }

        protected void btnContinue_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/SiguientePaso.aspx");
        }
    }
}
