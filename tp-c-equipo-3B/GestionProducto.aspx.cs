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
    public partial class GestionProducto : System.Web.UI.Page
    {
        private ArticuloSQL articuloSQL = new ArticuloSQL();
        private MarcaSQL marcaSQL = new MarcaSQL();
        private CategoriaSQL categoriaSQL = new CategoriaSQL();
        private ImagenSQL imagenSQL = new ImagenSQL();

        // Paginado
        private const int PageSize = 10;
        private int CurrentPage
        {
            get
            {
                if (ViewState["CurrentPage"] == null) ViewState["CurrentPage"] = 1;
                return (int)ViewState["CurrentPage"];
            }
            set => ViewState["CurrentPage"] = value;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                CargarMarcas();
                CargarCategorias();
                InicializarPaginado();
                pnlFormulario.Visible = false;
            }
        }

        private void CargarMarcas()
        {
            ddlMarcaForm.DataSource = marcaSQL.Listar();
            ddlMarcaForm.DataTextField = "Descripcion";
            ddlMarcaForm.DataValueField = "Id";
            ddlMarcaForm.DataBind();
        }

        private void CargarCategorias()
        {
            ddlCategoriaForm.DataSource = categoriaSQL.Listar();
            ddlCategoriaForm.DataTextField = "Descripcion";
            ddlCategoriaForm.DataValueField = "Id";
            ddlCategoriaForm.DataBind();
        }

        // Nota: este método mantiene la lógica de StockDisplay/StockClass
        private void CargarProductos()
        {
            var lista = articuloSQL.Listar() ?? new List<Articulo>();

            foreach (var art in lista)
            {
                int stock = art.StockActual;

                if (stock == 0)
                {
                    art.StockDisplay = "Agotado";
                    art.StockClass = "table-stock-status-out-of-stock";
                }
                else if (stock < 5)
                {
                    art.StockDisplay = "Poco Stock";
                    art.StockClass = "table-stock-status-low-stock";
                }
                else
                {
                    art.StockDisplay = "En Stock";
                    art.StockClass = "table-stock-status-in-stock";
                }
            }

            gvProductos.DataSource = lista;
            gvProductos.DataBind();
        }

        // Helper para obtener URL de imagen (usado por el TemplateField)
        protected string GetImageUrl(object firstImageObj, object urlImagenObj)
        {
            try
            {
                var first = firstImageObj?.ToString();
                if (!string.IsNullOrEmpty(first)) return ResolveUrl(first);

                var u = urlImagenObj?.ToString();
                if (!string.IsNullOrEmpty(u)) return ResolveUrl(u);

                return ResolveUrl("~/Imagenes/default-product.png");
            }
            catch
            {
                return ResolveUrl("~/Imagenes/default-product.png");
            }
        }

        // Paginado: carga paginada usando LINQ sobre la lista completa
        private void CargarProductosPaginados()
        {
            var listaCompleta = articuloSQL.Listar() ?? new List<Articulo>();

            // Preparar StockDisplay/StockClass en toda la lista (opcional: hacerlo solo en pageItems)
            foreach (var art in listaCompleta)
            {
                int stock = art.StockActual;
                if (stock == 0)
                {
                    art.StockDisplay = "Agotado";
                    art.StockClass = "table-stock-status-out-of-stock";
                }
                else if (stock < 5)
                {
                    art.StockDisplay = "Poco Stock";
                    art.StockClass = "table-stock-status-low-stock";
                }
                else
                {
                    art.StockDisplay = "En Stock";
                    art.StockClass = "table-stock-status-in-stock";
                }
            }

            int totalItems = listaCompleta.Count;
            int totalPages = (int)Math.Ceiling((double)totalItems / PageSize);
            if (totalPages == 0) totalPages = 1;

            if (CurrentPage < 1) CurrentPage = 1;
            if (CurrentPage > totalPages) CurrentPage = totalPages;

            var pageItems = listaCompleta.Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToList();

            gvProductos.DataSource = pageItems;
            gvProductos.DataBind();

            lblPagina.Text = $"{CurrentPage} / {totalPages}";
            lblPaginado.Text = $"Mostrando {((CurrentPage - 1) * PageSize) + 1}-{Math.Min(CurrentPage * PageSize, totalItems)} de {totalItems} resultados";

            // Activar/desactivar botones según corresponda
            btnPrev.Enabled = CurrentPage > 1;
            btnNext.Enabled = CurrentPage < totalPages;
        }

        private void InicializarPaginado()
        {
            CurrentPage = 1;
            CargarProductosPaginados();
        }

        // Manejadores de paginado vinculados en el .aspx (btnPrev, btnNext)
        protected void btnPrev_Click(object sender, EventArgs e)
        {
            CurrentPage = Math.Max(1, CurrentPage - 1);
            CargarProductosPaginados();
        }

        protected void btnNext_Click(object sender, EventArgs e)
        {
            CurrentPage = CurrentPage + 1;
            CargarProductosPaginados();
        }

        // Si tenés referencias a nombres btnAnterior/btnSiguiente en otras páginas, dejo aliases que llaman a los handlers correctos
        protected void btnAnterior_Click(object sender, EventArgs e)
        {
            btnPrev_Click(sender, e);
        }

        protected void btnSiguiente_Click(object sender, EventArgs e)
        {
            btnNext_Click(sender, e);
        }

        protected void btnNuevo_Click(object sender, EventArgs e)
        {
            LimpiarFormulario();
            pnlFormulario.Visible = true;
            ViewState["IdEditar"] = null;
        }

        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                Articulo nuevo = new Articulo
                {
                    Codigo = txtCodigo.Text,
                    Nombre = txtNombre.Text,
                    Descripcion = txtDescripcion.Text,
                    Precio = decimal.Parse(txtPrecio.Text),
                    IdMarca = int.Parse(ddlMarcaForm.SelectedValue),
                    IdCategoria = int.Parse(ddlCategoriaForm.SelectedValue)
                };

                int idGenerado = articuloSQL.AgregarYDevolverId(nuevo);

                if (fuImagen.HasFile)
                {
                    string ruta = "~/Imagenes/" + fuImagen.FileName;
                    fuImagen.SaveAs(Server.MapPath(ruta));

                    Imagen img = new Imagen
                    {
                        UrlImagen = ruta,
                        IdArticulo = idGenerado
                    };
                    imagenSQL.Agregar(img, idGenerado);
                }

                lblMensaje.Text = "Producto agregado correctamente.";
                pnlFormulario.Visible = false;
                InicializarPaginado();
            }
            catch (Exception ex)
            {
                lblMensaje.Text = "Error al guardar: " + ex.Message;
            }
        }

        protected void btnModificar_Click(object sender, EventArgs e)
        {
            try
            {
                int id = (int)ViewState["IdEditar"];
                Articulo art = new Articulo
                {
                    Id = id,
                    Codigo = txtCodigo.Text,
                    Nombre = txtNombre.Text,
                    Descripcion = txtDescripcion.Text,
                    Precio = decimal.Parse(txtPrecio.Text),
                    IdMarca = int.Parse(ddlMarcaForm.SelectedValue),
                    IdCategoria = int.Parse(ddlCategoriaForm.SelectedValue)
                };

                articuloSQL.Modificar(art);
                lblMensaje.Text = "Producto modificado correctamente.";
                pnlFormulario.Visible = false;
                InicializarPaginado();
            }
            catch (Exception ex)
            {
                lblMensaje.Text = "Error al modificar: " + ex.Message;
            }
        }

        protected void gvProductos_RowEditing(object sender, GridViewEditEventArgs e)
        {
            int id = Convert.ToInt32(gvProductos.DataKeys[e.NewEditIndex].Value);
            Articulo art = articuloSQL.Listar().Find(x => x.Id == id);

            txtCodigo.Text = art.Codigo;
            txtNombre.Text = art.Nombre;
            txtDescripcion.Text = art.Descripcion;
            txtPrecio.Text = art.Precio.ToString();
            ddlMarcaForm.SelectedValue = art.IdMarca.ToString();
            ddlCategoriaForm.SelectedValue = art.IdCategoria.ToString();

            ViewState["IdEditar"] = id;
            pnlFormulario.Visible = true;
        }

        protected void gvProductos_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            try
            {
                int id = Convert.ToInt32(gvProductos.DataKeys[e.RowIndex].Value);
                imagenSQL.EliminarPorArticulo(id);
                articuloSQL.Eliminar(id);
                lblMensaje.Text = "Producto eliminado.";
                InicializarPaginado();
            }
            catch (Exception ex)
            {
                lblMensaje.Text = "Error al eliminar: " + ex.Message;
            }
        }

        protected void btnBuscar_Click(object sender, EventArgs e)
        {
            string filtro = txtBuscar.Text.ToLower();
            var lista = articuloSQL.Listar();
            var filtrados = lista.FindAll(x => x.Nombre.ToLower().Contains(filtro) || x.Codigo.ToLower().Contains(filtro));
            gvProductos.DataSource = filtrados;
            gvProductos.DataBind();
            // Actualizar paginado visual si querés mantener consistencia:
            lblPaginado.Text = $"Mostrando {filtrados.Count} resultados";
            lblPagina.Text = "1 / 1";
            btnPrev.Enabled = false;
            btnNext.Enabled = false;
        }

        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            pnlFormulario.Visible = false;
            lblMensaje.Text = "";
        }

        private void LimpiarFormulario()
        {
            txtCodigo.Text = "";
            txtNombre.Text = "";
            txtDescripcion.Text = "";
            txtPrecio.Text = "";
            if (ddlMarcaForm.Items.Count > 0) ddlMarcaForm.SelectedIndex = 0;
            if (ddlCategoriaForm.Items.Count > 0) ddlCategoriaForm.SelectedIndex = 0;
        }
    }
}
