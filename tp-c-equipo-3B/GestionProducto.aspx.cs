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
        private ProveedorSQL proveedorSQL = new ProveedorSQL();
        private ArticuloProveedorSQL articuloProveedorSQL = new ArticuloProveedorSQL();

        #region "Propiedades y Helpers de Página"

   
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

      
        protected string GetImageSource(object url)
        {
            string imageUrl = url?.ToString();

            if (string.IsNullOrEmpty(imageUrl))
            {
                return ResolveUrl("~/Imagenes/default-product.png");
            }

            if (imageUrl.StartsWith("http", StringComparison.OrdinalIgnoreCase) || imageUrl.StartsWith("https", StringComparison.OrdinalIgnoreCase))
            {
                return imageUrl; 
            }
            else
            {
                return ResolveUrl("~/Imagenes/" + imageUrl); 
            }
        }

        #endregion

        #region "Eventos de Carga y Paginado"

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                CargarMarcas();
                CargarCategorias();
                CargarProveedores();
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

        private void CargarProveedores()
        {
            var proveedores = proveedorSQL.Listar();

            cblProveedoresForm.DataSource = proveedores;
            cblProveedoresForm.DataTextField = "Nombre";
            cblProveedoresForm.DataValueField = "Id";
            cblProveedoresForm.DataBind();

            ddlProveedorFilter.DataSource = proveedores;
            ddlProveedorFilter.DataTextField = "Nombre";
            ddlProveedorFilter.DataValueField = "Id";
            ddlProveedorFilter.DataBind();
            ddlProveedorFilter.Items.Insert(0, new ListItem("Todos", "0"));
            ddlProveedorFilter.Enabled = true;
        }

        private void CargarProductosPaginados()
        {
            var listaCompleta = articuloSQL.Listar() ?? new List<Articulo>();

           
            var listaProcesada = listaCompleta.Select(art => {
                string stockDisplay;
                string stockClass;
                int stock = art.StockActual;

                if (stock == 0)
                {
                    stockDisplay = "Agotado";
                    stockClass = "table-stock-status-out-of-stock";
                }
                else if (stock < art.StockMinimo)
                {
                    stockDisplay = "Poco Stock";
                    stockClass = "table-stock-status-low-stock";
                }
                else
                {
                    stockDisplay = "En Stock";
                    stockClass = "table-stock-status-in-stock";
                }

                decimal precioVenta = art.UltimoPrecioCompra * (1 + (art.PorcentajeGanancia / 100));

                return new
                {
                    art.Id,
                    art.Codigo,
                    art.Nombre,
                    art.Descripcion,
                    art.IdMarca,
                    art.IdCategoria,
                    art.Marca,
                    art.Categoria,
                    art.UrlImagen,
                    art.PorcentajeGanancia,
                    art.StockMinimo,
                    art.ProveedoresString,
                    art.UltimoPrecioCompra,

                    PrecioVenta = precioVenta,
                    StockDisplay = stockDisplay,
                    StockClass = stockClass
                };
            });

            int totalItems = listaCompleta.Count;
            int totalPages = (int)Math.Ceiling((double)totalItems / PageSize);
            if (totalPages == 0) totalPages = 1;

            if (CurrentPage < 1) CurrentPage = 1;
            if (CurrentPage > totalPages) CurrentPage = totalPages;

            var pageItems = listaProcesada.Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToList();

            gvProductos.DataSource = pageItems;
            gvProductos.DataBind();

            lblPagina.Text = $"{CurrentPage} / {totalPages}";
            lblPaginado.Text = $"Mostrando {((CurrentPage - 1) * PageSize) + 1}-{Math.Min(CurrentPage * PageSize, totalItems)} de {totalItems} resultados";

            btnPrev.Enabled = CurrentPage > 1;
            btnNext.Enabled = CurrentPage < totalPages;
        }

        private void InicializarPaginado()
        {
            CurrentPage = 1;
            CargarProductosPaginados();
        }

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

        protected void btnBuscar_Click(object sender, EventArgs e)
        {
            string filtro = txtBuscar.Text.ToLower();
            var lista = articuloSQL.Listar();
            var filtrados = lista.FindAll(x => x.Nombre.ToLower().Contains(filtro) || x.Codigo.ToLower().Contains(filtro));

            var listaProcesada = filtrados.Select(art => {
                string stockDisplay;
                string stockClass;
                int stock = art.StockActual;
                if (stock == 0)
                {
                    stockDisplay = "Agotado";
                    stockClass = "table-stock-status-out-of-stock";
                }
                else if (stock < art.StockMinimo)
                {
                    stockDisplay = "Poco Stock";
                    stockClass = "table-stock-status-low-stock";
                }
                else
                {
                    stockDisplay = "En Stock";
                    stockClass = "table-stock-status-in-stock";
                }
                decimal precioVenta = art.UltimoPrecioCompra * (1 + (art.PorcentajeGanancia / 100));

                return new
                {
                    art.Id,
                    art.Codigo,
                    art.Nombre,
                    art.Descripcion,
                    art.IdMarca,
                    art.IdCategoria,
                    art.Marca,
                    art.Categoria,
                    art.UrlImagen,
                    art.PorcentajeGanancia,
                    art.StockMinimo,
                    art.ProveedoresString,
                    art.UltimoPrecioCompra,

                    PrecioVenta = precioVenta,
                    StockDisplay = stockDisplay,
                    StockClass = stockClass
                };
            }).ToList();

            gvProductos.DataSource = listaProcesada;
            gvProductos.DataBind();

            lblPaginado.Text = $"Mostrando {listaProcesada.Count} resultados";
            lblPagina.Text = "1 / 1";
            btnPrev.Enabled = false;
            btnNext.Enabled = false;
        }

        #endregion

        #region "Eventos del Formulario (Guardar, Modificar, etc.)"

        protected void btnNuevo_Click(object sender, EventArgs e)
        {
            LimpiarFormulario();
            pnlFormulario.Visible = true;
            ViewState["IdEditar"] = null;

            btnGuardar.Visible = true;
            btnModificar.Visible = false;
        }

        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                string urlImagenPrincipal = ObtenerUrlImagenPrincipal(null);
                Articulo nuevo = new Articulo
                {
                    Codigo = txtCodigo.Text,
                    Nombre = txtNombre.Text,
                    Descripcion = txtDescripcion.Text,
                    PorcentajeGanancia = decimal.Parse(txtPorcentajeGanancia.Text),
                    StockMinimo = int.Parse(txtStockMinimo.Text),
                    IdMarca = int.Parse(ddlMarcaForm.SelectedValue),
                    IdCategoria = int.Parse(ddlCategoriaForm.SelectedValue),
                    UrlImagen = urlImagenPrincipal,
                    StockActual = int.Parse(txtStockActual.Text),
                    UltimoPrecioCompra = decimal.Parse(txtUltimoPrecioCompra.Text)
                };
                int idGenerado = articuloSQL.AgregarYDevolverId(nuevo);

                List<int> idsProveedores = ObtenerIdsProveedoresSeleccionados();
                articuloProveedorSQL.ActualizarProveedores(idGenerado, idsProveedores);

                ProcesarImagenesGaleria(idGenerado);

                lblMensaje.Text = "Producto agregado correctamente.";
                pnlFormulario.Visible = false;
                InicializarPaginado();

                btnGuardar.Visible = true;
                btnModificar.Visible = true;
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

                string urlImagenPrincipal = ObtenerUrlImagenPrincipal(id);
                Articulo art = new Articulo
                {
                    Id = id,
                    Codigo = txtCodigo.Text,
                    Nombre = txtNombre.Text,
                    Descripcion = txtDescripcion.Text,
                    PorcentajeGanancia = decimal.Parse(txtPorcentajeGanancia.Text),
                    StockMinimo = int.Parse(txtStockMinimo.Text),
                    IdMarca = int.Parse(ddlMarcaForm.SelectedValue),
                    IdCategoria = int.Parse(ddlCategoriaForm.SelectedValue),
                    UrlImagen = urlImagenPrincipal,
                    StockActual = int.Parse(txtStockActual.Text),
                    UltimoPrecioCompra = decimal.Parse(txtUltimoPrecioCompra.Text)
                };
                articuloSQL.Modificar(art);

                List<int> idsProveedores = ObtenerIdsProveedoresSeleccionados();
                articuloProveedorSQL.ActualizarProveedores(id, idsProveedores);

                ProcesarImagenesGaleria(id);

                lblMensaje.Text = "Producto modificado correctamente.";
                pnlFormulario.Visible = false;
                InicializarPaginado();

                btnGuardar.Visible = true;
                btnModificar.Visible = true;
            }
            catch (Exception ex)
            {
                lblMensaje.Text = "Error al modificar: " + ex.Message;
            }
        }

        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            pnlFormulario.Visible = false;
            lblMensaje.Text = "";

            btnGuardar.Visible = true;
            btnModificar.Visible = true;
        }

        protected void rblImagenTipo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (rblImagenTipo.SelectedValue == "URL")
            {
                pnlUrl.Visible = true;
                pnlUpload.Visible = false;
            }
            else
            {
                pnlUrl.Visible = false;
                pnlUpload.Visible = true;
            }
        }

        #endregion

        #region "Helpers de Lógica de Formulario"

        private string ObtenerUrlImagenPrincipal(int? idArticuloExistente)
        {
            string urlParaGuardar = "";

            if (rblImagenTipo.SelectedValue == "URL" && !string.IsNullOrEmpty(txtUrlImagen.Text))
            {
                urlParaGuardar = txtUrlImagen.Text;
            }
            else if (rblImagenTipo.SelectedValue == "UPLOAD" && fuImagen.HasFiles)
            {
                urlParaGuardar = fuImagen.PostedFiles[0].FileName;
            }
            else if (idArticuloExistente.HasValue)
            {
                Articulo artActual = articuloSQL.Listar().Find(x => x.Id == idArticuloExistente.Value);
                if (artActual != null)
                {
                    urlParaGuardar = artActual.UrlImagen;
                }
            }
            return urlParaGuardar;
        }

        private void ProcesarImagenesGaleria(int idArticulo)
        {
            if (rblImagenTipo.SelectedValue == "UPLOAD" && fuImagen.HasFiles)
            {
                if (ViewState["IdEditar"] != null)
                {
                    imagenSQL.EliminarPorArticulo(idArticulo);
                }

                foreach (HttpPostedFile postedFile in fuImagen.PostedFiles)
                {
                    string nombreArchivo = postedFile.FileName;
                    string rutaLocal = Server.MapPath("~/Imagenes/" + nombreArchivo);

                    postedFile.SaveAs(rutaLocal);

                    Imagen img = new Imagen
                    {
                        UrlImagen = nombreArchivo,
                        IdArticulo = idArticulo
                    };
                    imagenSQL.Agregar(img, idArticulo);
                }
            }
        }

        private List<int> ObtenerIdsProveedoresSeleccionados()
        {
            return cblProveedoresForm.Items.Cast<ListItem>()
                                     .Where(li => li.Selected)
                                     .Select(li => int.Parse(li.Value))
                                     .ToList();
        }

        private void LimpiarFormulario()
        {
            txtCodigo.Text = "";
            txtNombre.Text = "";
            txtDescripcion.Text = "";
            txtPorcentajeGanancia.Text = "";
            txtStockMinimo.Text = "";
            txtStockActual.Text = "0";
            txtUltimoPrecioCompra.Text = "0";

            if (ddlMarcaForm.Items.Count > 0) ddlMarcaForm.SelectedIndex = 0;
            if (ddlCategoriaForm.Items.Count > 0) ddlCategoriaForm.SelectedIndex = 0;
            cblProveedoresForm.ClearSelection();

            rblImagenTipo.SelectedValue = "UPLOAD";
            pnlUpload.Visible = true;
            pnlUrl.Visible = false;
            txtUrlImagen.Text = "";
        }

        #endregion

        #region "Eventos de la Grilla (GridView)"

        protected void gvProductos_RowEditing(object sender, GridViewEditEventArgs e)
        {
            int id = Convert.ToInt32(gvProductos.DataKeys[e.NewEditIndex].Value);
            Articulo art = articuloSQL.Listar().Find(x => x.Id == id);
            if (art == null) return;


            txtCodigo.Text = art.Codigo;
            txtNombre.Text = art.Nombre;
            txtDescripcion.Text = art.Descripcion;

            txtPorcentajeGanancia.Text = art.PorcentajeGanancia.ToString("N2");
            txtStockMinimo.Text = art.StockMinimo.ToString();
            txtStockActual.Text = art.StockActual.ToString();
            txtUltimoPrecioCompra.Text = art.UltimoPrecioCompra.ToString("N2");

            ddlMarcaForm.SelectedValue = art.IdMarca.ToString();
            ddlCategoriaForm.SelectedValue = art.IdCategoria.ToString();

            if (!string.IsNullOrEmpty(art.UrlImagen) && (art.UrlImagen.StartsWith("http") || art.UrlImagen.StartsWith("https")))
            {
                rblImagenTipo.SelectedValue = "URL";
                pnlUrl.Visible = true;
                pnlUpload.Visible = false;
                txtUrlImagen.Text = art.UrlImagen;
            }
            else
            {
                rblImagenTipo.SelectedValue = "UPLOAD";
                pnlUrl.Visible = false;
                pnlUpload.Visible = true;
                txtUrlImagen.Text = "";
            }

 
            cblProveedoresForm.ClearSelection();
            List<int> idsProveedores = articuloProveedorSQL.ListarIdsPorArticulo(id);
            foreach (ListItem li in cblProveedoresForm.Items)
            {
                li.Selected = idsProveedores.Contains(int.Parse(li.Value));
            }

            ViewState["IdEditar"] = id;
            pnlFormulario.Visible = true;

   
            btnGuardar.Visible = false;
            btnModificar.Visible = true;

            e.Cancel = true;
        }

        protected void gvProductos_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            try
            {
                int id = Convert.ToInt32(gvProductos.DataKeys[e.RowIndex].Value);

                articuloProveedorSQL.ActualizarProveedores(id, new List<int>());
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

        #endregion
    }
}