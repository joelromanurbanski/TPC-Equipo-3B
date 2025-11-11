<%@ Page Title="Gestión de Productos" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeBehind="GestionProducto.aspx.cs" Inherits="tp_c_equipo_3B.GestionProducto" %>

<asp:Content ID="HeadContent" ContentPlaceHolderID="head" runat="server">
    <!-- Mantengo sólo los recursos específicos necesarios; Bootstrap ya se carga en la MasterPage -->
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
    <!-- Header local simplificado: solo título (el navbar y step-bar vienen de la MasterPage) -->
    <div class="py-6">
        <div class="container">
            <h1 class="h2 mb-0">Gestión de Productos</h1>
        </div>
    </div>

    <div class="container my-4">
        <div class="mb-4 d-flex justify-content-between align-items-center">
            <asp:Button ID="btnNuevo" runat="server" Text="Agregar Producto" OnClick="btnNuevo_Click"
                CssClass="btn btn-primary" />
            <asp:Label ID="lblMensaje" runat="server" CssClass="text-danger mb-0" />
        </div>

        <!-- Filtros -->
        <div class="card mb-4">
            <div class="card-body">
                <div class="row g-3">
                    <div class="col-12 col-md-6">
                        <div class="input-group">
                            <span class="input-group-text"><span class="material-symbols-outlined">search</span></span>
                            <asp:TextBox ID="txtBuscar" runat="server" CssClass="form-control"
                                Placeholder="Buscar por nombre o código/SKU" />
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
                        <asp:DropDownList ID="ddlProveedorFilter" runat="server" CssClass="form-select" />
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

        <!-- Formulario (collapsible handled server-side) -->
        <asp:Panel ID="pnlFormulario" runat="server" CssClass="card mb-4" Visible="false">
            <div class="card-body">
                <div class="row g-3">
                    <div class="col-md-6">
                        <asp:TextBox ID="txtCodigo" runat="server" CssClass="form-control" Placeholder="Código/SKU" />
                    </div>
                    <div class="col-md-6">
                        <asp:TextBox ID="txtNombre" runat="server" CssClass="form-control" Placeholder="Nombre del Producto" />
                    </div>
                    <div class="col-12">
                        <asp:TextBox ID="txtDescripcion" runat="server" TextMode="MultiLine" Rows="3" CssClass="form-control" Placeholder="Descripción" />
                    </div>
                    <div class="col-md-4">
                        <asp:TextBox ID="txtPrecio" runat="server" CssClass="form-control" Placeholder="Precio de Venta" />
                    </div>

                    <div class="col-md-4 d-none">
                        <asp:DropDownList ID="ddlMarcaForm_inner" runat="server" CssClass="form-select" Visible="false" />
                    </div>
                    <div class="col-md-4 d-none">
                        <asp:DropDownList ID="ddlCategoriaForm_inner" runat="server" CssClass="form-select" Visible="false" />
                    </div>

                    <div class="col-md-6">
                        <asp:DropDownList ID="ddlMarcaForm_Select" runat="server" CssClass="form-select" />
                    </div>
                    <div class="col-md-6">
                        <asp:DropDownList ID="ddlCategoriaForm_Select" runat="server" CssClass="form-select" />
                    </div>

                    <div class="col-12">
                        <label class="form-label">Imagen principal</label>
                        <asp:FileUpload ID="fuImagen" runat="server" CssClass="form-control" />
                    </div>

                    <div class="col-12 d-flex gap-2">
                        <asp:Button ID="btnGuardar" runat="server" Text="Guardar" OnClick="btnGuardar_Click" CssClass="btn btn-primary" />
                        <asp:Button ID="btnModificar" runat="server" Text="Modificar" OnClick="btnModificar_Click" CssClass="btn btn-warning" />
                        <asp:Button ID="btnCancelar" runat="server" Text="Cancelar" OnClick="btnCancelar_Click" CssClass="btn btn-secondary" />
                    </div>
                </div>
            </div>
        </asp:Panel>

        <!-- Grid -->
        <div class="card">
            <div class="card-body p-0">
                <asp:GridView ID="gvProductos" runat="server" AutoGenerateColumns="False" DataKeyNames="Id"
                    CssClass="table table-striped table-hover mb-0" OnRowEditing="gvProductos_RowEditing" OnRowDeleting="gvProductos_RowDeleting" EmptyDataText="No hay productos para mostrar">
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
                        <asp:BoundField DataField="ProveedorNombre" HeaderText="Proveedor Principal" />
                        <asp:BoundField DataField="Precio" HeaderText="Precio Venta" DataFormatString="{0:C}" />
                        <asp:TemplateField HeaderText="Stock">
                            <ItemTemplate>
                                <asp:Label ID="lblStock" runat="server" Text='<%# Eval("StockDisplay") %>' CssClass='<%# Eval("StockClass") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:ImageField DataImageUrlField="UrlImagen" HeaderText="Imagen" ControlStyle-Width="60px" DataImageUrlFormatString="{0}" />
                        <asp:CommandField ShowEditButton="True" ShowDeleteButton="True" />
                    </Columns>
                </asp:GridView>
            </div>
        </div>

        <!-- Paginado -->
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
