using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;



namespace tp_c_equipo_3B
{
    public partial class GestionProveedores : Page
    {
        private const string SESSION_PROVEEDORES = "ProveedoresList";
        private const string SESSION_CONTACTOS = "ProveedoresContactos";
        private const string SESSION_PRODUCTOS = "ProveedoresProductos";
        private const string SESSION_RANK = "RankDemoList";

        protected void Page_Load(object sender, EventArgs e)
        {
            

            var tab = Request.QueryString["tab"];
            if (!string.IsNullOrEmpty(tab))
            {
                var script = $"document.addEventListener('DOMContentLoaded', function(){{ var b = document.querySelector('#tabsGestionProveedores #tab-{tab}'); if(b) new bootstrap.Tab(b).show(); }});";
                ScriptManager.RegisterStartupScript(this, GetType(), "activateTabProveedores", script, true);
            }
        }
        

        private void BindSidebar()
        {
            var list = (List<Proveedor>)Session[SESSION_PROVEEDORES] ?? new List<Proveedor>();
            rptProveedores.DataSource = list;
            rptProveedores.DataBind();

            var first = list.FirstOrDefault();
            if (first != null) ShowProveedor(first);
        }

        protected void rptProveedores_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "Select")
            {
                var id = e.CommandArgument.ToString();
                var list = (List<Proveedor>)Session[SESSION_PROVEEDORES];
                var p = list.FirstOrDefault(x => x.Id == id);
                if (p != null) ShowProveedor(p);
            }
        }

        private void ShowProveedor(Proveedor p)
        {
            lblProveedorNombre.Text = p.Nombre;
            lblCompanyName.Text = p.Nombre;
            lblTaxId.Text = p.TaxId;
            lblAddress.Text = p.Address;
            lblPhone.Text = p.Telefono;
            lblWebsite.Text = p.Website;
            lblNotes.Text = p.Notes;
        }

        protected void btnBuscarProveedorSidebar_Click(object sender, EventArgs e)
        {
            var q = Request.Form["txtBuscarProveedorSidebar"] ?? string.Empty;
            var list = (List<Proveedor>)Session[SESSION_PROVEEDORES] ?? new List<Proveedor>();
            var filtered = list.Where(x => x.Nombre.IndexOf(q, StringComparison.OrdinalIgnoreCase) >= 0).ToList();
            rptProveedores.DataSource = filtered;
            rptProveedores.DataBind();
        }

        protected void btnGlobalSearch_Click(object sender, EventArgs e)
        {
            var q = Request.Form["txtGlobalSearch"] ?? string.Empty;
           
            var list = (List<Proveedor>)Session[SESSION_PROVEEDORES] ?? new List<Proveedor>();
            var filtered = list.Where(x => x.Nombre.IndexOf(q, StringComparison.OrdinalIgnoreCase) >= 0).ToList();
            rptProveedores.DataSource = filtered;
            rptProveedores.DataBind();
        }

        protected void btnExportar_Click(object sender, EventArgs e)
        {
            var list = (List<Proveedor>)Session[SESSION_PROVEEDORES];
            if (list == null || !list.Any()) return;
            var csv = "Nombre,Telefono,TaxId,Address,Website,Notes\r\n" +
                      string.Join("\r\n", list.Select(c => $"{EscapeCsv(c.Nombre)},{EscapeCsv(c.Telefono)},{EscapeCsv(c.TaxId)},{EscapeCsv(c.Address)},{EscapeCsv(c.Website)},{EscapeCsv(c.Notes)}"));
            Response.Clear();
            Response.ContentType = "text/csv";
            Response.AddHeader("content-disposition", "attachment;filename=proveedores.csv");
            Response.Write(csv);
            Response.End();
        }

        protected void btnImportar_Click(object sender, EventArgs e)
        {
            
        }

        protected void btnRefrescar_Click(object sender, EventArgs e)
        {
            
        }

        protected void btnNuevoProveedor_Click(object sender, EventArgs e)
        {
            Response.Redirect(Request.Path + "?tab=datos&new=1");
        }

        protected void btnEditarProveedor_Click(object sender, EventArgs e)
        {
            
        }

        protected void btnEliminarProveedor_Click(object sender, EventArgs e)
        {
            var name = lblCompanyName.Text;
            var list = (List<Proveedor>)Session[SESSION_PROVEEDORES];
            var toRemove = list.FirstOrDefault(x => x.Nombre == name);
            if (toRemove != null)
            {
                list.Remove(toRemove);
                Session[SESSION_PROVEEDORES] = list;
                BindSidebar();
            }
        }

        #region Rank demo
        private void BindRankDemo()
        {
            var list = Session[SESSION_RANK] as List<RankItem>;
            if (list == null)
            {
                list = new List<RankItem>
                {
                    new RankItem { Name = "1000 Clash", Score = 1200 },
                    new RankItem { Name = "Basehead", Score = 1100 },
                    new RankItem { Name = "casper", Score = 1050 },
                    new RankItem { Name = "Rank demo", Score = 1000 }
                };
                Session[SESSION_RANK] = list;
            }
            rptRankDemo.DataSource = list.OrderByDescending(r => r.Score).ToList();
            rptRankDemo.DataBind();
        }

        protected void btnJoinRank_Click(object sender, EventArgs e)
        {
            var list = Session[SESSION_RANK] as List<RankItem> ?? new List<RankItem>();
            var me = User?.Identity?.Name ?? "You";
            if (!list.Any(x => x.Name.Equals(me, StringComparison.OrdinalIgnoreCase)))
            {
                list.Add(new RankItem { Name = me, Score = 900 });
                Session[SESSION_RANK] = list;
            }
            BindRankDemo();
        }

        protected void btnLeaveRank_Click(object sender, EventArgs e)
        {
            var list = Session[SESSION_RANK] as List<RankItem>;
            if (list == null) return;
            var me = User?.Identity?.Name ?? "You";
            var existing = list.FirstOrDefault(x => x.Name.Equals(me, StringComparison.OrdinalIgnoreCase));
            if (existing != null)
            {
                list.Remove(existing);
                Session[SESSION_RANK] = list;
            }
            BindRankDemo();
        }
        #endregion

        #region Helpers
        private static string EscapeCsv(string s)
        {
            if (s == null) return "";
            if (s.Contains(",") || s.Contains("\"") || s.Contains("\r") || s.Contains("\n"))
                return $"\"{s.Replace("\"", "\"\"")}\"";
            return s;
        }
        #endregion
    }
}



    public class Proveedor
    {
        public string Id { get; set; }
        public string Nombre { get; set; }
        public string Telefono { get; set; }
        public string TaxId { get; set; }
        public string Address { get; set; }
        public string Website { get; set; }
        public string Notes { get; set; }
    }

    public class RankItem
    {
        public string Name { get; set; }
        public int Score { get; set; }
    }

