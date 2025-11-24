using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Dominio;

namespace tp_c_equipo_3B
{
    public partial class SiteMaster : MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["usuario"] != null)
                {
                    Usuario user = (Usuario)Session["usuario"];

                    // CORRECCIÓN: Mostramos el Nombre o el Email, no el objeto
                    lblUsuario.Text = "Hola, " + (string.IsNullOrEmpty(user.Nombre) ? user.Email : user.Nombre);

                    // Si NO es admin, ocultamos el menú de usuarios
                    if (!user.EsAdmin && linkUsuarios != null)
                    {
                        linkUsuarios.Visible = false;
                    }
                }
                else if (!(Page is InicioSesion))
                {
                    Response.Redirect("InicioSesion.aspx", false);
                }
            }
        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Response.Redirect("InicioSesion.aspx");
        }
    }
}