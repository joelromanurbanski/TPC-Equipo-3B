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
                // Cargar listas de SQL
                CargarMarcas();
                CargarCategorias();
                CargarProveedores();

                // Cargar la grilla
                gvProductos.PageIndex = 0; // Iniciar en la página 0
                BindGrid();

                pnlFormulario.Visible = false;
            }
        }

        private void CargarMarcas()
        {
            var lista = marcaSQL.Listar();

            // Cargar filtro
            ddlMarcaForm.DataSource = lista;
            ddlMarcaForm.DataTextField = "Descripcion";
            ddlMarcaForm.DataValueField = "Id";
            ddlMarcaForm.DataBind();
            ddlMarcaForm.Items.Insert(0, new ListItem("Todas", "0"));

            // Cargar formulario
            ddlMarcaForm_Form.DataSource = lista;
            ddlMarcaForm_Form.DataTextField = "Descripcion";
            ddlMarcaForm_Form.DataValueField = "Id";
            ddlMarcaForm_Form.DataBind();
            ddlMarcaForm_Form.Items.Insert(0, new ListItem("Seleccionar", "0"));
        }

        private void CargarCategorias()
        {
            var lista = categoriaSQL.Listar();

            // Cargar filtro
            ddlCategoriaForm.DataSource = lista;
            ddlCategoriaForm.DataTextField = "Descripcion";
            ddlCategoriaForm.DataValueField = "Id";
            ddlCategoriaForm.DataBind();
            ddlCategoriaForm.Items.Insert(0, new ListItem("Todas", "0"));

            // Cargar formulario
            ddlCategoriaForm_Form.DataSource = lista;
            ddlCategoriaForm_Form.DataTextField = "Descripcion";
            ddlCategoriaForm_Form.DataValueField = "Id";
            ddlCategoriaForm_Form.DataBind();
            ddlCategoriaForm_Form.Items.Insert(0, new ListItem("Seleccionar", "0"));
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

        // CARGAR Y FILTRAR
        private void BindGrid()
        {
            var listaFiltrada = articuloSQL.Listar();

            // Aplicar filtro de texto
            string filtroTexto = txtBuscar.Text.Trim().ToLower();
            if (!string.IsNullOrEmpty(filtroTexto))
            {
                listaFiltrada = listaFiltrada.Where(a => a.Nombre.ToLower().Contains(filtroTexto) || a.Codigo.ToLower().Contains(filtroTexto)).ToList();
            }

            // Aplicar filtro de Categoría
            int idCategoria = int.Parse(ddlCategoriaForm.SelectedValue);
            if (idCategoria > 0)
            {
                listaFiltrada = listaFiltrada.Where(a => a.IdCategoria == idCategoria).ToList();
            }

            // Aplicar filtro de Marca
            int idMarca = int.Parse(ddlMarcaForm.SelectedValue);
            if (idMarca > 0)
            {
                listaFiltrada = listaFiltrada.Where(a => a.IdMarca == idMarca).ToList();
            }

            // Aplicar filtro de Stock
            string stockFiltro = ddlStockFilter.SelectedValue;
            if (stockFiltro != "Todos")
            {
                listaFiltrada = listaFiltrada.Where(a => {
                    if (stockFiltro == "InStock") return a.StockActual > a.StockMinimo;
                    if (stockFiltro == "LowStock") return a.StockActual > 0 && a.StockActual <= a.StockMinimo;
                    if (stockFiltro == "OutOfStock") return a.StockActual == 0;
                    return true;
                }).ToList();
            }

            // Aplicar filtro de Proveedor
            int idProveedor = int.Parse(ddlProveedorFilter.SelectedValue);
            if (idProveedor > 0)
            {
                List<int> idsArticulosDelProveedor = articuloProveedorSQL.ListarIdsArticuloPorProveedor(idProveedor);
                listaFiltrada = listaFiltrada.Where(a => idsArticulosDelProveedor.Contains(a.Id)).ToList();
            }

            var listaProcesada = listaFiltrada.Select(art => {
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

            // Actualizar etiquetas
            int totalItems = listaProcesada.Count;
            int totalPages = gvProductos.PageCount;
            if (totalPages == 0) totalPages = 1;
            int currentPage = gvProductos.PageIndex + 1;

            lblPagina.Text = $"{currentPage} / {totalPages}";
            lblPaginado.Text = $"Mostrando {totalItems} resultados";

            btnPrev.Enabled = !gvProductos.PageIndex.Equals(0);
            btnNext.Enabled = !gvProductos.PageIndex.Equals(totalPages - 1);
        }

        // Filtros

        protected void btnPrev_Click(object sender, EventArgs e)
        {
            if (gvProductos.PageIndex > 0)
            {
                gvProductos.PageIndex--;
                BindGrid();
            }
        }

        protected void btnNext_Click(object sender, EventArgs e)
        {
            if (gvProductos.PageIndex < gvProductos.PageCount - 1)
            {
                gvProductos.PageIndex++;
                BindGrid();
            }
        }

        protected void gvProductos_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvProductos.PageIndex = e.NewPageIndex;
            BindGrid();
        }

        protected void btnBuscar_Click(object sender, EventArgs e)
        {
            gvProductos.PageIndex = 0; // Resetear a la página 1
            BindGrid();
        }

        protected void Filtro_SelectedIndexChanged(object sender, EventArgs e)
        {
            gvProductos.PageIndex = 0; // Resetear a la página 1
            BindGrid();
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
                    IdMarca = int.Parse(ddlMarcaForm_Form.SelectedValue),
                    IdCategoria = int.Parse(ddlCategoriaForm_Form.SelectedValue),
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
                BindGrid(); // Recargar grilla

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
                    IdMarca = int.Parse(ddlMarcaForm_Form.SelectedValue),
                    IdCategoria = int.Parse(ddlCategoriaForm_Form.SelectedValue),
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
                BindGrid(); // Recargar grilla

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
                // Cargar la lista fresca para buscar
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

            if (ddlMarcaForm_Form.Items.Count > 0) ddlMarcaForm_Form.SelectedIndex = 0;
            if (ddlCategoriaForm_Form.Items.Count > 0) ddlCategoriaForm_Form.SelectedIndex = 0;
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

            // Cargar la lista para buscar
            Articulo art = articuloSQL.Listar().Find(x => x.Id == id);
            if (art == null) return;

            // Rellenar campos de texto
            txtCodigo.Text = art.Codigo;
            txtNombre.Text = art.Nombre;
            txtDescripcion.Text = art.Descripcion;

            txtPorcentajeGanancia.Text = art.PorcentajeGanancia.ToString("N2");
            txtStockMinimo.Text = art.StockMinimo.ToString();
            txtStockActual.Text = art.StockActual.ToString();
            txtUltimoPrecioCompra.Text = art.UltimoPrecioCompra.ToString("N2");

            ddlMarcaForm_Form.SelectedValue = art.IdMarca.ToString();
            ddlCategoriaForm_Form.SelectedValue = art.IdCategoria.ToString();

            if (!string.IsNullOrEmpty(art.UrlImagen) && (art.UrlImagen.StartsWith("http") || art.UrlImagen.StartsWith("httpsU")))
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
                BindGrid(); // Recargar grilla
            }
            catch (Exception ex)
            {
                lblMensaje.Text = "Error al eliminar: " + ex.Message;
            }
        }

        #endregion
    }
}