<%@ Page Title="Registrar Compra" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeBehind="RegistroCompra.aspx.cs" Inherits="tp_c_equipo_3B.RegistroCompra" %>

<asp:Content ID="HeadContent" ContentPlaceHolderID="head" runat="server">
    <style>
        .form-control, .form-select, .btn { border-radius: 0.5rem; }
        .form-control:not(.form-control-sm), .form-select:not(.form-select-sm), .btn:not(.btn-sm) { height: 3rem; }
        .material-symbols-outlined { font-variation-settings: 'FILL' 0, 'wght' 400, 'GRAD' 0, 'opsz' 24; }
    </style>
</asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="py-5 bg-light min-vh-100"> 
        <div class="container my-4">
            <header class="mb-5">
                <h1 class="text-dark fw-bolder mb-1" style="font-size: 2.5rem;">Registrar nueva compra</h1>
                <p class="text-muted small mt-2">Completa los siguientes campos para registrar una nueva compra de inventario.</p>
            </header>

            <asp:Label ID="lblMensaje" runat="server" CssClass="alert alert-danger" Visible="false" />

            <div class="row g-4">
                <div class="col-lg-8">
                    <div class="card shadow-sm border-0 rounded-xl"> 
                        <div class="card-body p-4 p-md-5">
                            <h2 class="card-title h5 fw-bold mb-4 text-dark">Detalles de la Compra</h2>
                            
                            <div class="row g-4">
                                <div class="col-12 col-md-6">
                                    <label class="form-label fw-semibold mb-2">Proveedor *</label>
                                    <asp:DropDownList ID="ddlProveedor" runat="server" CssClass="form-select" />
                                    <asp:RequiredFieldValidator ErrorMessage="Debe seleccionar un proveedor." ControlToValidate="ddlProveedor" InitialValue="0" CssClass="text-danger" Display="Dynamic" runat="server" />
                                </div>
                                <div class="col-12 col-md-6">
                                    <label class="form-label fw-semibold mb-2">Fecha de compra *</label>
                                    <asp:TextBox ID="txtFecha" runat="server" CssClass="form-control" TextMode="Date" />
                                </div>
                            </div>

                            <hr class="my-5" />

                            <h3 class="h6 fw-bold mb-3 text-dark">Añadir Productos</h3>
                            <div class="row g-3 align-items-end mb-4">
                                <div class="col-md-5">
                                    <label class="form-label small fw-semibold mb-2">Producto</label>
                                    <asp:DropDownList ID="ddlArticulo" runat="server" CssClass="form-select" />
                                </div>
                                <div class="col-md-2">
                                    <label class="form-label small fw-semibold mb-2">Cantidad</label>
                                    <asp:TextBox ID="txtCantidad" runat="server" CssClass="form-control" TextMode="Number" Text="1" />
                                </div>
                                <div class="col-md-3">
                                    <label class="form-label small fw-semibold mb-2">Precio Costo (Unit.)</label>
                                    <asp:TextBox ID="txtPrecioCompraUnitario" runat="server" CssClass="form-control" TextMode="Number" step="0.01" Text="0" />
                                </div>
                                <div class="col-md-2 d-grid">
                                    <asp:Button ID="btnAgregarProducto" runat="server" Text="Agregar" CssClass="btn btn-success fw-bold" OnClick="btnAgregarProducto_Click" CausesValidation="false" />
                                </div>
                            </div>
                            
                            <div class="table-responsive border rounded-lg">
                                <asp:GridView ID="gvProductos" runat="server" 
                                    CssClass="table table-striped table-hover mb-0" 
                                    AutoGenerateColumns="False" 
                                    
                                    DataKeyNames="ArticuloId" 
                                    
                                    OnRowCommand="gvProductos_RowCommand"
                                    EmptyDataText="Aún no has agregado productos a la compra.">
                                    <HeaderStyle CssClass="bg-light small text-uppercase" />
                                    <Columns>
                                        <asp:BoundField DataField="Articulo.Nombre" HeaderText="Producto" HeaderStyle-CssClass="px-4 py-3" ItemStyle-CssClass="px-4 py-3 fw-normal" />
                                        <asp:BoundField DataField="Cantidad" HeaderText="Cantidad" HeaderStyle-CssClass="px-4 py-3" ItemStyle-CssClass="px-4 py-3" />
                                        <asp:BoundField DataField="PrecioCompra" HeaderText="Precio Unit." DataFormatString="{0:C}" HeaderStyle-CssClass="px-4 py-3" ItemStyle-CssClass="px-4 py-3" />
                                        <asp:BoundField DataField="Subtotal" HeaderText="Subtotal" DataFormatString="{0:C}" HeaderStyle-CssClass="px-4 py-3" ItemStyle-CssClass="px-4 py-3" />
                                        <asp:TemplateField HeaderStyle-CssClass="px-4 py-3 text-end" ItemStyle-CssClass="px-4 py-3 text-end">
                                            <ItemTemplate>
                                                <asp:LinkButton ID="btnEliminar" runat="server" CommandName="Eliminar" 
                                                    CommandArgument='<%# Eval("ArticuloId") %>' 
                                                    CssClass="btn btn-link p-0 text-danger" 
                                                    CausesValidation="false">
                                                    <span class="material-symbols-outlined small">delete</span>
                                                </asp:LinkButton>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                    </Columns>
                                </asp:GridView>
                            </div>
                        </div>
                    </div>
                </div>

                <div class="col-lg-4">
                     <div class="card shadow-sm border-0 rounded-xl sticky-top" style="top: 2rem;">
                        <div class="card-body p-4">
                            <h2 class="card-title h5 fw-bold mb-4 text-dark">Resumen de la Compra</h2>
                            
                            <div class="d-flex flex-column gap-3">
                                <div class="d-flex justify-content-between">
                                    <p class="text-muted mb-0">Subtotal</p>
                                    <asp:Label ID="lblSubtotal" runat="server" Text="$0.00" CssClass="fw-semibold mb-0 text-dark" />
                                </div>
                                <div class="d-flex justify-content-between align-items-center">
                                    <p class="text-muted mb-0">Impuestos (21%)</p>
                                    <asp:Label ID="lblImpuestos" runat="server" Text="$0.00" CssClass="fw-semibold mb-0 text-dark" />
                                </div>
                                <div class="d-flex justify-content-between align-items-center">
                                    <p class="text-muted mb-0">Otros Costos</p>
                                    <asp:TextBox ID="txtOtrosCostos" runat="server" Text="0" TextMode="Number" step="0.01" 
                                        CssClass="form-control text-end" style="width: 100px; height: 3rem;" 
                                        AutoPostBack="true" OnTextChanged="txtOtrosCostos_TextChanged" />
                                </div>

                                <div class="border-top my-3"></div>

                                <div class="d-flex justify-content-between h5 fw-bold mb-0">
                                    <p class="text-dark mb-0">Total</p>
                                    <asp:Label ID="lblTotal" runat="server" Text="$0.00" CssClass="text-primary mb-0" />
                                </div>
                            </div>

                            <div class="mt-4 d-grid gap-3">
                                <asp:Button ID="btnGuardarCompra" runat="server" Text="Guardar Compra" OnClick="btnGuardarCompra_Click"
                                    CssClass="btn btn-primary d-flex align-items-center justify-content-center fw-bold h-12" />
                                <asp:Button ID="btnCancelarCompra" runat="server" Text="Cancelar" OnClick="btnCancelarCompra_Click"
                                    CssClass="btn btn-secondary d-flex align-items-center justify-content-center fw-bold h-12" CausesValidation="false" />
                            </div>
                        </div>
                     </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>