<%@ Page Title="Gestión de Productos" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeBehind="GestionProducto.aspx.cs" Inherits="tp_c_equipo_3B.GestionProducto" %>

<asp:Content ID="HeadContent" ContentPlaceHolderID="head" runat="server">
    <link href="https://fonts.googleapis.com/css2?family=Inter:wght@400;500;700;900&amp;display=swap" rel="stylesheet"/>
    <link href="https://fonts.googleapis.com/css2?family=Material+Symbols+Outlined" rel="stylesheet"/>
    <style>
        .material-symbols-outlined { font-variation-settings: 'FILL' 0, 'wght' 400, 'GRAD' 0, 'opsz' 24; }
        .table-stock-status-in-stock { color: #2ECC71; }
        .table-stock-status-low-stock { color: #F39C12; }
        .table-stock-status-out-of-stock { color: #E74C3C; }
    </style>
</asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="py-6">
        <div class="container">
            <h1 class="h2 mb-0">Gestión de Productos</h1>
        </div>
    </div>

    <div class="container my-4">
        <div class="mb-4 d-flex justify-content-between align-items-center">
            <asp:Button ID="btnNuevo" runat="server" Text="Agregar Producto" OnClick="btnNuevo_Click" CssClass="btn btn-primary" />
            <asp:Label ID="lblMensaje" runat="server" CssClass="text-danger mb-0" />
        </div>

        <div class="card mb-4">
            <div class="card-body">
                <div class="row g-3">
                    <div class="col-12 col-md-6">
                        <div class="input-group">
                            <span class="input-group-text"><span class="material-symbols-outlined">search</span></span>
                            <asp:TextBox ID="txtBuscar" runat="server" CssClass="form-control" Placeholder="Buscar por nombre o código/SKU" />
                            <button class="btn btn-outline-primary" type="button" onclick="document.getElementById('<%= btnBuscar.ClientID %>').click();">Buscar</button>
                            <asp:Button ID="btnBuscar" runat="server" Text="Buscar" OnClick="btnBuscar_Click" CssClass="d-none" />
                        </div>
                    </div>
                    <div class="col-6 col-md-2">
                        <label class="form-label mb-1">Categoría</label>
                        <asp:DropDownList ID="ddlCategoriaForm" runat="server" CssClass="form-select" />
                    </div>
                    <div class="col-6 col-md-2">
                        <label class="form-label mb-1">Marca</label>
                        <asp:DropDownList ID="ddlMarcaForm" runat="server" CssClass="form-select" />
                    </div>
                    <div class="col-6 col-md-2">
                        <label class="form-label mb-1">Proveedor</label>
                        <asp:DropDownList ID="ddlProveedorFilter" runat="server" CssClass="form-select" Enabled="false" />
                    </div>
                    <div class="col-6 col-md-2">
                        <label class="form-label mb-1">Estado de Stock</label>
                        <asp:DropDownList ID="ddlStockFilter" runat="server" CssClass="form-select">
                            <asp:ListItem Text="Todos" Value="Todos" />
                            <asp:ListItem Text="En Stock" Value="InStock" />
                            <asp:ListItem Text="Poco Stock" Value="LowStock" />
                            <asp:ListItem Text="Agotado" Value="OutOfStock" />
                        </asp:DropDownList>
                    </div>
                </div>
            </div>
        </div>

        <asp:Panel ID="pnlFormulario" runat="server" CssClass="card mb-4" Visible="false">
            <div class="card-body">
                <div class="row g-3 p-3">
                   
                    <div class="col-md-6">
                        <label class="form-label">Código/SKU</label>
                        <asp:TextBox ID="txtCodigo" runat="server" CssClass="form-control" />
                    </div>
                    <div class="col-md-6">
                        <label class="form-label">Nombre del Producto</label>
                        <asp:TextBox ID="txtNombre" runat="server" CssClass="form-control" />
                    </div>
                    <div class="col-12">
                        <label class="form-label">Descripción</label>
                        <asp:TextBox ID="txtDescripcion" runat="server" TextMode="MultiLine" Rows="3" CssClass="form-control" />
                    </div>
     
                    <div class="col-md-6">
                        <label class="form-label">Precio de Costo (Última Compra)</label>
                        <asp:TextBox ID="txtUltimoPrecioCompra" runat="server" CssClass="form-control" 
                            Placeholder="Ej: 750000" TextMode="Number" step="0.01" />
                    </div>
                     <div class="col-md-6">
                        <label class="form-label">Porcentaje de Ganancia (%)</label>
                        <asp:TextBox ID="txtPorcentajeGanancia" runat="server" CssClass="form-control" 
                            Placeholder="Ej: 30.5" TextMode="Number" step="0.01" />
                    </div>

                    <div class="col-md-6">
                        <label class="form-label">Stock Actual</label>
                         <asp:TextBox ID="txtStockActual" runat="server" CssClass="form-control" 
                            Placeholder="Ej: 25" TextMode="Number" />
                    </div>
                    <div class="col-md-6">
                        <label class="form-label">Stock Mínimo</label>
                         <asp:TextBox ID="txtStockMinimo" runat="server" CssClass="form-control" 
                            Placeholder="Ej: 5" TextMode="Number" />
                    </div>
                    
                    <div class="col-12">
                        <label class="form-label">Proveedores</label>
                        <asp:CheckBoxList ID="cblProveedoresForm" runat="server" 
                            CssClass="form-control" RepeatColumns="3" RepeatDirection="Horizontal" 
                            RepeatLayout="Table" CellPadding="5" CellSpacing="5" />
                    </div>

                    <div class="col-12">
                        <label class="form-label">Imagen principal (y galería)</label>
                        <asp:RadioButtonList ID="rblImagenTipo" runat="server" 
                            RepeatDirection="Horizontal" AutoPostBack="true" 
                            OnSelectedIndexChanged="rblImagenTipo_SelectedIndexChanged">
                            <asp:ListItem Text="Subir una o más imágenes" Value="UPLOAD" Selected="True" />
                            <asp:ListItem Text="Pegar URL externa" Value="URL" />
                        </asp:RadioButtonList>
                    </div>

                    <asp:Panel ID="pnlUpload" runat="server" CssClass="col-12">
                        <asp:FileUpload ID="fuImagen" runat="server" CssClass="form-control" AllowMultiple="true" />
                        <small class="form-text">Si subes múltiples archivos, el primero será la imagen principal.</small>
                    </asp:Panel>

                    <asp:Panel ID="pnlUrl" runat="server" CssClass="col-12" Visible="false">
                        <asp:TextBox ID="txtUrlImagen" runat="server" CssClass="form-control" 
                            Placeholder="https://ejemplo.com/imagen.jpg" TextMode="Url" />
                    </asp:Panel>

                    <div class="col-12 d-flex gap-2">
                        <asp:Button ID="btnGuardar" runat="server" Text="Guardar" OnClick="btnGuardar_Click" CssClass="btn btn-primary" />
                        <asp:Button ID="btnModificar" runat="server" Text="Modificar" OnClick="btnModificar_Click" CssClass="btn btn-warning" />
                        <asp:Button ID="btnCancelar" runat="server" Text="Cancelar" OnClick="btnCancelar_Click" CssClass="btn btn-secondary" />
                    </div>
                </div>
            </div>
        </asp:Panel>

        <div class="card">
            <div class="card-body p-0">
                <asp:GridView ID="gvProductos" runat="server" AutoGenerateColumns="False" DataKeyNames="Id"
                    CssClass="table table-striped table-hover mb-0" 
                    OnRowEditing="gvProductos_RowEditing" 
                    OnRowDeleting="gvProductos_RowDeleting" 
                    EmptyDataText="No hay productos para mostrar">
                    <Columns>
                        <asp:TemplateField HeaderText="">
                            <ItemTemplate>
                                <asp:CheckBox ID="chkSelect" runat="server" />
                            </ItemTemplate>
                            <HeaderStyle Width="40px" />
                            <ItemStyle Width="40px" />
                        </asp:TemplateField>

                        <asp:BoundField DataField="Nombre" HeaderText="Nombre del Producto" />
                        <asp:BoundField DataField="Codigo" HeaderText="Código/SKU" />
                        <asp:BoundField DataField="Categoria.Descripcion" HeaderText="Categoría" />
                        <asp:BoundField DataField="Marca.Descripcion" HeaderText="Marca" />
                        <asp:BoundField DataField="ProveedoresString" HeaderText="Proveedores" />
                        
                        <asp:BoundField DataField="UltimoPrecioCompra" HeaderText="Precio Costo" DataFormatString="{0:C}" />
                        <asp:BoundField DataField="PorcentajeGanancia" HeaderText="Ganancia (%)" DataFormatString="{0:N2} %" />
                        
                        <asp:BoundField DataField="PrecioVenta" HeaderText="Precio Venta" DataFormatString="{0:C}" />
                
                        <asp:TemplateField HeaderText="Stock">
                            <ItemTemplate>
                                <asp:Label ID="lblStock" runat="server" 
                                    Text='<%# Eval("StockDisplay") %>' 
                                    CssClass='<%# Eval("StockClass") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="Imagen">
                            <ItemTemplate>
                                <asp:Image runat="server" 
                                    ImageUrl='<%# GetImageSource(Eval("UrlImagen")) %>' 
                                    Width="60px" 
                                    Height="60px" 
                                    CssClass="img-thumbnail" 
                                    Style="object-fit: cover;" />
                            </ItemTemplate>
                            <HeaderStyle Width="80px" />
                            <ItemStyle Width="80px" />
                        </asp:TemplateField>
                        
                        <asp:CommandField ShowEditButton="True" ShowDeleteButton="True" />
                    </Columns>
                </asp:GridView>
            </div>
        </div>

        <div class="d-flex justify-content-between align-items-center mt-3">
            <asp:Label ID="lblPaginado" runat="server" Text="Mostrando resultados" CssClass="text-muted"></asp:Label>
            <div>
                <asp:Button ID="btnPrev" runat="server" Text="Anterior" CssClass="btn btn-outline-secondary me-2" OnClick="btnPrev_Click" />
                <asp:Label ID="lblPagina" runat="server" Text="1" CssClass="mx-2" />
                <asp:Button ID="btnNext" runat="server" Text="Siguiente" CssClass="btn btn-outline-secondary" OnClick="btnNext_Click" />
            </div>
        </div>
    </div>
</asp:Content>