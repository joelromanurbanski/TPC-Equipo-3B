using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

namespace tp_c_equipo_3B
{
    public partial class GestionClientes : Page
    {
        private const string SESSION_KEY = "ClientesList";
        private const string SESSION_HISTORY = "HistorialClientes";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session[SESSION_KEY] == null)
                {
                    Session[SESSION_KEY] = new List<Cliente>();
                    SeedDemoData();
                }

                BindGrid();
                BindHistorial();
                FillEstadisticas();

                // Activar pestaña desde querystring usando script cliente (Bootstrap debe estar cargado)
                var tab = Request.QueryString["tab"];
                if (!string.IsNullOrEmpty(tab))
                {
                    var script = $"document.addEventListener('DOMContentLoaded', function(){{ var b = document.querySelector('#tabsGestionClientes #tab-{tab}'); if(b) new bootstrap.Tab(b).show(); }});";
                    ScriptManager.RegisterStartupScript(this, GetType(), "activateTabFromServer", script, true);
                }
            }
        }

        protected void btnNuevoCliente_Click(object sender, EventArgs e)
        {
            ClearForm();
            pnlClienteForm.Visible = true;
            hfEditingId.Value = string.Empty;
            lblMensaje.Text = string.Empty;
        }

        protected void btnCancelarCliente_Click(object sender, EventArgs e)
        {
            pnlClienteForm.Visible = false;
            lblMensaje.Text = string.Empty;
            ClearForm();
        }

        protected void btnGuardarCliente_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNombre.Text) ||
                string.IsNullOrWhiteSpace(txtTelefono.Text) ||
                string.IsNullOrWhiteSpace(txtEmail.Text))
            {
                lblMensaje.Text = "Por favor, complete los campos Nombre, Teléfono y Email.";
                return;
            }

            var list = (List<Cliente>)Session[SESSION_KEY];

            if (string.IsNullOrEmpty(hfEditingId.Value))
            {
                var cliente = new Cliente
                {
                    Id = Guid.NewGuid().ToString(),
                    Nombre = txtNombre.Text.Trim(),
                    Apellido = txtApellido.Text.Trim(),
                    Cedula = txtCedula.Text.Trim(),
                    Telefono = txtTelefono.Text.Trim(),
                    Email = txtEmail.Text.Trim(),
                    Direccion = txtDireccion.Text.Trim()
                };
                list.Add(cliente);
                AddHistory("Alta", "system", $"Cliente {cliente.Nombre} {cliente.Apellido} creado.");
            }
            else
            {
                var id = hfEditingId.Value;
                var existing = list.FirstOrDefault(c => c.Id == id);
                if (existing != null)
                {
                    existing.Nombre = txtNombre.Text.Trim();
                    existing.Apellido = txtApellido.Text.Trim();
                    existing.Cedula = txtCedula.Text.Trim();
                    existing.Telefono = txtTelefono.Text.Trim();
                    existing.Email = txtEmail.Text.Trim();
                    existing.Direccion = txtDireccion.Text.Trim();
                    AddHistory("Edición", "system", $"Cliente {existing.Nombre} actualizado.");
                }
            }

            Session[SESSION_KEY] = list;
            BindGrid();
            FillEstadisticas();
            BindHistorial();
            pnlClienteForm.Visible = false;
            ClearForm();
            lblMensaje.Text = string.Empty;
        }

        protected void btnBuscarClientes_Click(object sender, EventArgs e)
        {
            BindGrid(txtBuscarClientes.Text.Trim());
        }

        protected void gvClientes_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            var list = (List<Cliente>)Session[SESSION_KEY];
            if (e.CommandName == "Editar")
            {
                var id = e.CommandArgument.ToString();
                var c = list.FirstOrDefault(x => x.Id == id);
                if (c != null)
                {
                    hfEditingId.Value = c.Id;
                    txtNombre.Text = c.Nombre;
                    txtApellido.Text = c.Apellido;
                    txtCedula.Text = c.Cedula;
                    txtTelefono.Text = c.Telefono;
                    txtEmail.Text = c.Email;
                    txtDireccion.Text = c.Direccion;
                    pnlClienteForm.Visible = true;
                }
            }
            else if (e.CommandName == "Eliminar")
            {
                var id = e.CommandArgument.ToString();
                var toRemove = list.FirstOrDefault(x => x.Id == id);
                if (toRemove != null)
                {
                    list.Remove(toRemove);
                    Session[SESSION_KEY] = list;
                    AddHistory("Eliminación", "system", $"Cliente {toRemove.Nombre} {toRemove.Apellido} eliminado.");
                    BindGrid();
                    FillEstadisticas();
                    BindHistorial();
                }
            }
        }

        protected void btnCareer_Click(object sender, EventArgs e)
        {
            lblMensaje.Text = "Career button clicked.";
        }

        // Toolbar handlers
        protected void btnCerrarPanel_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/PaneldeControl.aspx");
        }

        protected void btnExportar_Click(object sender, EventArgs e)
        {
            var list = (List<Cliente>)Session[SESSION_KEY];
            if (list == null || !list.Any())
            {
                lblMensaje.Text = "No hay datos para exportar.";
                return;
            }

            var csv = "Nombre,Apellido,Telefono,Email,Direccion\r\n" +
                      string.Join("\r\n", list.Select(c =>
                          $"{EscapeCsv(c.Nombre)},{EscapeCsv(c.Apellido)},{EscapeCsv(c.Telefono)},{EscapeCsv(c.Email)},{EscapeCsv(c.Direccion)}"));

            Response.Clear();
            Response.ContentType = "text/csv";
            Response.AddHeader("content-disposition", "attachment;filename=clientes.csv");
            Response.Write(csv);
            Response.End();
        }

        protected void btnImportar_Click(object sender, EventArgs e)
        {
            lblMensaje.Text = "Función Importar aún no implementada.";
        }

        protected void btnRefrescar_Click(object sender, EventArgs e)
        {
            BindGrid();
            lblMensaje.Text = string.Empty;
        }

        // Card handlers
        protected void btnExitoCard_Click(object sender, EventArgs e)
        {
            lblMensaje.Text = "Acción Éxito desde la tarjeta.";
        }

        protected void btnContinuarServer_Click(object sender, EventArgs e)
        {
            // Ejemplo: redirigir a siguiente lección
            Response.Redirect("~/Curso/GestionNegocios/Leccion2.aspx");
        }

        // Historial y estadísticas
        private void BindHistorial()
        {
            var history = Session[SESSION_HISTORY] as List<HistoryEntry>;
            if (history == null)
            {
                history = new List<HistoryEntry>();
                // ejemplo
                history.Add(new HistoryEntry { Date = DateTime.Now.AddDays(-3), Action = "Alta", User = "admin", Details = "Cliente Juan Pérez creado." });
                history.Add(new HistoryEntry { Date = DateTime.Now.AddDays(-1), Action = "Edición", User = "admin", Details = "Email de Ana actualizado." });
                Session[SESSION_HISTORY] = history;
            }
            gvHistorial.DataSource = history;
            gvHistorial.DataBind();
        }

        private void FillEstadisticas()
        {
            var list = (List<Cliente>)Session[SESSION_KEY] ?? new List<Cliente>();
            var total = list.Count;
            var activos = list.Count; // placeholder; ajustar según lógica real
            var recientes = list.Count(c => false); // placeholder para 30d

            lblTotalClientes.Text = total.ToString();
            lblClientesActivos.Text = activos.ToString();
            lblClientesRecientes.Text = recientes.ToString();
        }

        private void BindGrid(string filtro = null)
        {
            var list = (List<Cliente>)Session[SESSION_KEY] ?? new List<Cliente>();
            var data = list.Select(c => new
            {
                Id = c.Id,
                NombreCompleto = $"{c.Nombre} {c.Apellido}".Trim(),
                Telefono = c.Telefono,
                Email = c.Email
            }).ToList();

            if (!string.IsNullOrEmpty(filtro))
            {
                filtro = filtro.ToLowerInvariant();
                data = data.Where(d => d.NombreCompleto.ToLowerInvariant().Contains(filtro)
                    || d.Telefono.ToLowerInvariant().Contains(filtro)
                    || d.Email.ToLowerInvariant().Contains(filtro)).ToList();
            }

            gvClientes.DataSource = data;
            gvClientes.DataBind();
        }

        private void ClearForm()
        {
            txtNombre.Text = "";
            txtApellido.Text = "";
            txtCedula.Text = "";
            txtTelefono.Text = "";
            txtEmail.Text = "";
            txtDireccion.Text = "";
            hfEditingId.Value = "";
        }

        private void SeedDemoData()
        {
            var list = (List<Cliente>)Session[SESSION_KEY];
            list.Add(new Cliente { Id = Guid.NewGuid().ToString(), Nombre = "Juan", Apellido = "Pérez", Telefono = "555-1234", Email = "juan.perez@example.com", Direccion = "Calle Falsa 123" });
            list.Add(new Cliente { Id = Guid.NewGuid().ToString(), Nombre = "Ana", Apellido = "Gómez", Telefono = "555-5678", Email = "ana.gomez@example.com", Direccion = "Av Siempre Viva 742" });
            list.Add(new Cliente { Id = Guid.NewGuid().ToString(), Nombre = "Rae", Apellido = "María", Telefono = "555-0000", Email = "raemaria@example.com", Direccion = "C/ Demo 1" });
            Session[SESSION_KEY] = list;
        }

        private void AddHistory(string action, string user, string details)
        {
            var history = Session[SESSION_HISTORY] as List<HistoryEntry>;
            if (history == null)
            {
                history = new List<HistoryEntry>();
            }
            history.Insert(0, new HistoryEntry { Date = DateTime.Now, Action = action, User = user, Details = details });
            Session[SESSION_HISTORY] = history;
        }

        private static string EscapeCsv(string s)
        {
            if (s == null) return "";
            if (s.Contains(",") || s.Contains("\"") || s.Contains("\r") || s.Contains("\n"))
            {
                return $"\"{s.Replace("\"", "\"\"")}\"";
            }
            return s;
        }
    }

    // Aux classes
    public class Cliente
    {
        public string Id { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Cedula { get; set; }
        public string Telefono { get; set; }
        public string Email { get; set; }
        public string Direccion { get; set; }
    }

    public class HistoryEntry
    {
        public DateTime Date { get; set; }
        public string Action { get; set; }
        public string User { get; set; }
        public string Details { get; set; }
    }
}
