<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="GestionProducto.aspx.cs" Inherits="tp_c_equipo_3B.GestionProducto" %>

<!DOCTYPE html>

<html class="light" lang="es">
<head runat="server">
    <meta charset="utf-8"/>
    <meta content="width=device-width, initial-scale=1.0" name="viewport"/>
    <title>Gestión de Productos</title>

    <script src="https://cdn.tailwindcss.com?plugins=forms,container-queries"></script>
    <link href="https://fonts.googleapis.com/css2?family=Inter:wght@400;500;700;900&amp;display=swap" rel="stylesheet"/>
    <link href="https://fonts.googleapis.com/css2?family=Material+Symbols+Outlined" rel="stylesheet"/>
    <script id="tailwind-config">
        tailwind.config = {
            darkMode: "class",
            theme: {
                extend: {
                    colors: {
                        "primary": "#1173d4",
                        "background-light": "#f6f7f8",
                        "background-dark": "#101922",
                    },
                    fontFamily: {
                        "display": ["Inter", "sans-serif"]
                    },
                    borderRadius: {
                        "DEFAULT": "0.5rem",
                        "lg": "0.75rem",
                        "xl": "1rem",
                        "full": "9999px"
                    },
                },
            },
        }
    </script>

    <style>
        .material-symbols-outlined {
            font-variation-settings: 'FILL' 0, 'wght' 400, 'GRAD' 0, 'opsz' 24;
        }
        .table-stock-status-in-stock { color: #2ECC71; }
        .table-stock-status-low-stock { color: #F39C12; }
        .table-stock-status-out-of-stock { color: #E74C3C; }
    </style>
</head>
<body class="bg-background-light dark:bg-background-dark font-display text-gray-800 dark:text-gray-200">
    <form id="form1" runat="server">
        <div class="relative flex h-auto min-h-screen w-full flex-col group/design-root overflow-x-hidden">
            <div class="layout-container flex h-full grow flex-col">
                <header class="flex items-center justify-between whitespace-nowrap border-b border-solid border-gray-200 dark:border-gray-700 px-10 py-3 bg-white dark:bg-background-dark">
                    <div class="flex items-center gap-4 text-[#111418] dark:text-white">
                        <div class="size-8 text-primary">
                            <span class="material-symbols-outlined !text-4xl">inventory_2</span>
                        </div>
                        <h2 class="text-[#111418] dark:text-white text-xl font-bold leading-tight tracking-[-0.015em]">Gestión de Negocios</h2>
                    </div>

                    <div class="flex flex-1 justify-center gap-8">
                        <a class="text-[#111418] dark:text-gray-300 text-sm font-medium leading-normal hover:text-primary dark:hover:text-primary" href="#">Compras</a>
                        <a class="text-[#111418] dark:text-gray-300 text-sm font-medium leading-normal hover:text-primary dark:hover:text-primary" href="#">Ventas</a>
                        <a class="text-primary dark:text-primary text-sm font-bold leading-normal" href="#">Productos</a>
                    </div>

                    <div class="flex items-center gap-4">
                        <button type="button" class="flex items-center justify-center rounded-full size-10 bg-gray-100 dark:bg-gray-800 text-gray-600 dark:text-gray-300">
                            <span class="material-symbols-outlined">notifications</span>
                        </button>
                        <div class="bg-center bg-no-repeat aspect-square bg-cover rounded-full size-10" data-alt="User avatar" style='background-image: url("https://lh3.googleusercontent.com/aida-public/AB6AXuCx8-v7mV3GbLsSHBKjayydc0r_tnSff-VzK0lRScOSHyWW7uM89Oc0KlUiPQxfdtc0yB1648c94fzzS7VkZctC5yadErzhphXejV86lEZ8QqyQJX0-F8c96ps1H62PthE93Z2Unx30tKHTPjlVXeR002wj2FWJk0YFWiXOHNB5QLquGmTj_95e0IXRDFoYOoG3FUmCYG3rg_FmxZQM-5bANcH6L5DsOkHllufhjnV82DXrFuJmYZGrHh6QRPHRREMrImNfViawD1U0");'></div>
                    </div>
                </header>

                <main class="flex-1 px-10 py-8">
                    <div class="max-w-7xl mx-auto">
                        <div class="flex flex-wrap justify-between items-center gap-4 mb-6">
                            <p class="text-[#111418] dark:text-white text-4xl font-black leading-tight tracking-[-0.033em] min-w-72">Gestión de Productos</p>

                            <asp:Button ID="btnNuevo" runat="server" Text="Agregar Producto" OnClick="btnNuevo_Click"
                                CssClass="flex min-w-[84px] items-center justify-center gap-2 overflow-hidden rounded-lg h-12 px-5 bg-primary text-white text-sm font-bold leading-normal tracking-[0.015em] hover:bg-primary/90 transition-colors" />
                        </div>

                        <asp:Label ID="lblMensaje" runat="server" CssClass="text-red-500 font-semibold mb-4 block" />

                        <div class="bg-white dark:bg-background-dark p-6 rounded-xl shadow-sm mb-6">
                            <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
                                <div class="col-span-1 md:col-span-2 lg:col-span-4">
                                    <label class="flex flex-col min-w-40 h-12 w-full">
                                        <div class="flex w-full flex-1 items-stretch rounded-lg h-full">
                                            <div class="text-gray-500 dark:text-gray-400 flex border-none bg-gray-100 dark:bg-gray-800 items-center justify-center pl-4 rounded-l-lg border-r-0">
                                                <span class="material-symbols-outlined">search</span>
                                            </div>

                                            <asp:TextBox ID="txtBuscar" runat="server" CssClass="form-input flex w-full min-w-0 flex-1 resize-none overflow-hidden rounded-r-lg text-[#111418] dark:text-white focus:outline-0 focus:ring-2 focus:ring-primary/50 border-none bg-gray-100 dark:bg-gray-800 h-full placeholder:text-gray-500 dark:placeholder:text-gray-400 px-4 text-base font-normal leading-normal"
                                                Placeholder="Buscar por nombre o código/SKU" />

                                            <asp:Button ID="btnBuscar" runat="server" Text="Buscar" OnClick="btnBuscar_Click"
                                                CssClass="ml-3 px-4 py-2 rounded-lg bg-primary text-white" />
                                        </div>
                                    </label>
                                </div>

                                <div class="flex flex-col gap-2">
                                    <label class="text-sm font-medium text-gray-600 dark:text-gray-300">Categoría</label>
                                    <asp:DropDownList ID="ddlCategoriaForm" runat="server" CssClass="flex h-10 w-full items-center justify-between rounded-lg bg-gray-100 dark:bg-gray-800 px-4 text-left" />
                                </div>

                                <div class="flex flex-col gap-2">
                                    <label class="text-sm font-medium text-gray-600 dark:text-gray-300">Marca</label>
                                    <asp:DropDownList ID="ddlMarcaForm" runat="server" CssClass="flex h-10 w-full items-center justify-between rounded-lg bg-gray-100 dark:bg-gray-800 px-4 text-left" />
                                </div>

                                <div class="flex flex-col gap-2">
                                    <label class="text-sm font-medium text-gray-600 dark:text-gray-300">Proveedor</label>
                                    <asp:DropDownList ID="ddlProveedorFilter" runat="server" CssClass="flex h-10 w-full items-center justify-between rounded-lg bg-gray-100 dark:bg-gray-800 px-4 text-left" />
                                </div>

                                <div class="flex flex-col gap-2">
                                    <label class="text-sm font-medium text-gray-600 dark:text-gray-300">Estado de Stock</label>
                                    <asp:DropDownList ID="ddlStockFilter" runat="server" CssClass="flex h-10 w-full items-center justify-between rounded-lg bg-gray-100 dark:bg-gray-800 px-4 text-left">
                                        <asp:ListItem Text="Todos" Value="Todos" />
                                        <asp:ListItem Text="En Stock" Value="InStock" />
                                        <asp:ListItem Text="Poco Stock" Value="LowStock" />
                                        <asp:ListItem Text="Agotado" Value="OutOfStock" />
                                    </asp:DropDownList>
                                </div>
                            </div>
                        </div>

                        <asp:Panel ID="pnlFormulario" runat="server" CssClass="bg-white dark:bg-background-dark p-6 rounded-xl shadow-sm mb-6" Visible="false">
                            <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
                                <asp:TextBox ID="txtCodigo" runat="server" CssClass="form-input p-3 rounded-lg bg-gray-100 dark:bg-gray-800" Placeholder="Código/SKU" />
                                <asp:TextBox ID="txtNombre" runat="server" CssClass="form-input p-3 rounded-lg bg-gray-100 dark:bg-gray-800" Placeholder="Nombre del Producto" />
                                <asp:TextBox ID="txtDescripcion" runat="server" TextMode="MultiLine" Rows="3" CssClass="form-input p-3 rounded-lg bg-gray-100 dark:bg-gray-800 col-span-2" Placeholder="Descripción" />
                                <asp:TextBox ID="txtPrecio" runat="server" CssClass="form-input p-3 rounded-lg bg-gray-100 dark:bg-gray-800" Placeholder="Precio de Venta" />

                                <asp:DropDownList ID="ddlMarcaForm_inner" runat="server" CssClass="form-input p-3 rounded-lg bg-gray-100 dark:bg-gray-800" Visible="false" />
                                <asp:DropDownList ID="ddlCategoriaForm_inner" runat="server" CssClass="form-input p-3 rounded-lg bg-gray-100 dark:bg-gray-800" Visible="false" />

                                <asp:DropDownList ID="ddlMarcaForm_Select" runat="server" CssClass="form-input p-3 rounded-lg bg-gray-100 dark:bg-gray-800" />
                                <asp:DropDownList ID="ddlCategoriaForm_Select" runat="server" CssClass="form-input p-3 rounded-lg bg-gray-100 dark:bg-gray-800" />

                                <div class="col-span-2">
                                    <label class="text-sm font-medium text-gray-600 dark:text-gray-300">Imagen principal</label>
                                    <asp:FileUpload ID="fuImagen" runat="server" CssClass="mt-2" />
                                </div>

                                <div class="col-span-2 flex gap-2">
                                    <asp:Button ID="btnGuardar" runat="server" Text="Guardar" OnClick="btnGuardar_Click"
                                        CssClass="px-4 py-2 rounded-lg bg-primary text-white" />
                                    <asp:Button ID="btnModificar" runat="server" Text="Modificar" OnClick="btnModificar_Click"
                                        CssClass="px-4 py-2 rounded-lg bg-yellow-500 text-white" />
                                    <asp:Button ID="btnCancelar" runat="server" Text="Cancelar" OnClick="btnCancelar_Click"
                                        CssClass="px-4 py-2 rounded-lg bg-gray-500 text-white" />
                                </div>
                            </div>
                        </asp:Panel>

                        <div class="@container">
                            <div class="flex overflow-hidden rounded-xl border border-gray-200 dark:border-gray-700 bg-white dark:bg-background-dark">
                                <asp:GridView ID="gvProductos" runat="server" AutoGenerateColumns="False" DataKeyNames="Id"
                                    CssClass="min-w-full" OnRowEditing="gvProductos_RowEditing" OnRowDeleting="gvProductos_RowDeleting" EmptyDataText="No hay productos para mostrar">
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

                        <div class="flex justify-between items-center mt-6 px-2">
                            <asp:Label ID="lblPaginado" runat="server" Text="Mostrando resultados" CssClass="text-sm text-gray-600 dark:text-gray-400"></asp:Label>
                            <div class="flex items-center gap-2">
                                <asp:Button ID="btnPrev" runat="server" Text="Anterior" CssClass="px-3 py-1 rounded-md border" OnClick="btnPrev_Click" />
                                <asp:Label ID="lblPagina" runat="server" Text="1" CssClass="px-3 py-1 rounded-md"></asp:Label>
                                <asp:Button ID="btnNext" runat="server" Text="Siguiente" CssClass="px-3 py-1 rounded-md border" OnClick="btnNext_Click" />
                            </div>
                        </div>

                    </div>
                </main>
            </div>
        </div>
    </form>
</body>
</html>
