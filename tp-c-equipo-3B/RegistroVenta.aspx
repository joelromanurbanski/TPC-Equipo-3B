<%@ Page Title="Registrar Venta" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeBehind="RegistroVenta.aspx.cs" Inherits="tp_c_equipo_3B.RegistroVenta" %>

<asp:Content ID="HeadContent" ContentPlaceHolderID="head" runat="server">
    <style>
        .form-control, .form-select { border-radius: 0.5rem; height: 3.5rem; }
        .btn { border-radius: 0.5rem; }
        .material-symbols-outlined { font-variation-settings: 'FILL' 0, 'wght' 400, 'GRAD' 0, 'opsz' 24; }
    </style>
</asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="py-4 py-lg-5 bg-light min-vh-100">
        <div class="container-fluid max-w-7xl mx-auto">
            
            <header class="d-flex flex-wrap justify-content-between align-items-center gap-3 mb-5">
                <h1 class="text-dark fw-bolder mb-0" style="font-size: 2.5rem;">Registrar Nueva Venta</h1>
                <asp:Button ID="btnCancelar" runat="server" Text="Cancelar" 
                    CssClass="btn btn-secondary fw-bold" style="height: 2.5rem; min-width: 84px;"
                    OnClick="btnCancelar_Click" CausesValidation="false" />
            </header>

            <asp:Label ID="lblMensaje" runat="server" CssClass="alert alert-danger" Visible="false" />
            
            <div class="row g-4 g-lg-5">
                <div class="col-lg-8 space-y-5">
                    
                    <section class="card shadow-sm border-0 rounded-xl">
                        <div class="card-body p-4 p-md-5">
                            <h2 class="h4 fw-bold text-dark mb-4">Cliente</h2>
                            <div class="d-flex align-items-start gap-3">
                                <div class="flex-grow-1">
                                    <label class="form-label" for="<%= ddlCliente.ClientID %>">Seleccionar Cliente *</label>
                                    <asp:DropDownList ID="ddlCliente" runat="server" CssClass="form-select" />
                                    <asp:RequiredFieldValidator ErrorMessage="Debe seleccionar un cliente." ControlToValidate="ddlCliente" InitialValue="0" CssClass="text-danger" Display="Dynamic" runat="server" />
                                </div>
                                <a href="GestionClientes.aspx" class="flex-shrink-0 btn bg-primary-light hover-bg-primary-light text-primary d-flex align-items-center justify-content-center" 
                                    style="width: 3.5rem; height: 3.5rem; margin-top: 2rem;">
                                    <span class="material-symbols-outlined fs-4">add</span>
                                </a>
                            </div>
                        </div>
                    </section>
                     
                    <section class="card shadow-sm border-0 rounded-xl">
                        <div class="card-body p-4 p-md-5">
                            <h2 class="h4 fw-bold text-dark mb-4">Añadir Productos</h2>
                            
                            <div class="row g-3 align-items-end mb-4">
                                <div class="col-md-5">
                                    <label class="form-label small fw-semibold mb-2">Producto</label>
                                    <asp:DropDownList ID="ddlArticulo" runat="server" CssClass="form-select" 
                                        AutoPostBack="true" OnSelectedIndexChanged="ddlArticulo_SelectedIndexChanged" />
                                </div>
                                <div class="col-md-2">
                                    <label class="form-label small fw-semibold mb-2">Cantidad</label>
                                    <asp:TextBox ID="txtCantidad" runat="server" CssClass="form-control" TextMode="Number" Text="1" />
                                </div>
                                <div class="col-md-3">
                                    <label class="form-label small fw-semibold mb-2">Precio Venta (Unit.)</label>
                                    <asp:Label ID="lblPrecioUnitario" runat="server" Text="$0.00" CssClass="form-control-plaintext fs-5 fw-bold" />
                                </div>
                                <div class="col-md-2 d-grid">
                                    <asp:Button ID="btnAgregarProducto" runat="server" Text="Agregar" CssClass="btn btn-primary fw-bold" OnClick="btnAgregarProducto_Click" CausesValidation="false" />
                                </div>
                                <div class="col-12">
                                    <asp:Label ID="lblStockDisponible" runat="server" CssClass="text-muted small" />
                                </div>
                            </div>

                            <div class="table-responsive">
                                <asp:GridView ID="gvVentaItems" runat="server"
                                    CssClass="table table-borderless table-striped mb-0 text-start"
                                    AutoGenerateColumns="False"
                                    DataKeyNames="ArticuloId"
                                    OnRowCommand="gvVentaItems_RowCommand"
                                    EmptyDataText="Agrega productos para comenzar la venta.">
                                    <HeaderStyle CssClass="border-bottom border-secondary-subtle small text-uppercase" />
                                    <Columns>
                                        <asp:BoundField DataField="Articulo.Nombre" HeaderText="Producto" HeaderStyle-CssClass="p-3 fw-semibold text-muted" ItemStyle-CssClass="p-3 text-dark fw-medium" />
                                        <asp:BoundField DataField="Cantidad" HeaderText="Cantidad" HeaderStyle-CssClass="p-3 fw-semibold text-muted text-center" ItemStyle-CssClass="p-3 text-center" />
                                        <asp:BoundField DataField="PrecioUnitario" HeaderText="Precio Unit." DataFormatString="{0:C}" HeaderStyle-CssClass="p-3 fw-semibold text-muted text-end" ItemStyle-CssClass="p-3 text-dark text-end" />
                                        <asp:BoundField DataField="Subtotal" HeaderText="Precio Total" DataFormatString="{0:C}" HeaderStyle-CssClass="p-3 fw-semibold text-muted text-end" ItemStyle-CssClass="p-3 text-dark fw-bold text-end" />
                                        <asp:TemplateField HeaderStyle-CssClass="p-3 fw-semibold text-muted text-center" ItemStyle-CssClass="p-3 text-center">
                                            <ItemTemplate>
                                                <asp:LinkButton ID="btnEliminar" runat="server" CommandName="Eliminar" CommandArgument='<%# Eval("ArticuloId") %>' CssClass="btn btn-link p-0 text-danger" CausesValidation="false">
                                                    <span class="material-symbols-outlined small">delete</span>
                                                </asp:LinkButton>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                    </Columns>
                                </asp:GridView>
                            </div>
                        </div>
                    </section>
                </div>

                <div class="col-lg-4 space-y-5">
                    <div class="sticky-top" style="top: 2rem;">
                        <section class="card shadow-sm border-0 rounded-xl mb-4">
                            <div class="card-body p-4 p-md-5">
                                <h2 class="h4 fw-bold text-dark mb-4">Resumen de Venta</h2>
                                <div class="d-flex flex-column gap-3">
                                    <div class="d-flex justify-content-between text-muted">
                                        <span>Subtotal</span>
                                        <asp:Label ID="lblSubtotal" runat="server" Text="$0.00" CssClass="fw-semibold text-dark" />
                                    </div>
                                    <div class="d-flex justify-content-between align-items-center text-muted">
                                        <span>Descuento (%)</span>
                                        <asp:TextBox ID="txtDescuento" runat="server" Text="0" TextMode="Number" CssClass="form-control text-end" style="width: 80px; height: 2.5rem;" AutoPostBack="true" OnTextChanged="txtCalculo_TextChanged" />
                                    </div>
                                    <div class="d-flex justify-content-between align-items-center text-muted">
                                        <span>IVA (21%)</span>
                                        <asp:Label ID="lblIVA" runat="server" Text="$0.00" CssClass="fw-semibold text-dark" />
                                    </div>
                                    
                                    <div class="border-top border-secondary-subtle my-3"></div>
                                    
                                    <div class="d-flex justify-content-between h5 fw-bold text-dark mb-0">
                                        <span>TOTAL</span>
                                        <asp:Label ID="lblTotal" runat="server" Text="$0.00" CssClass="text-primary" style="font-size: 1.5rem;" />
                                    </div>
                                </div>
                                
                                <div class="mt-4">
                                    <label class="form-label fw-semibold text-dark" for="<%= ddlMetodoPago.ClientID %>">Método de Pago</label>
                                    <asp:DropDownList ID="ddlMetodoPago" runat="server" CssClass="form-select mt-2" style="height: 3.5rem;">
                                        <asp:ListItem Text="Efectivo" Value="Efectivo" />
                                        <asp:ListItem Text="Tarjeta de Crédito" Value="Tarjeta" />
                                        <asp:ListItem Text="Transferencia Bancaria" Value="Transferencia" />
                                    </asp:DropDownList>
                                </div>
                                
                                <div class="mt-4 d-grid gap-3">
                                    <asp:Button ID="btnGenerarVenta" runat="server" Text="Generar Venta y Factura" 
                                        CssClass="btn btn-primary fw-bold" style="height: 3rem;" 
                                        OnClick="btnGenerarVenta_Click" />
                                    <asp:Button ID="btnGuardarBorrador" runat="server" Text="Guardar Borrador" 
                                        CssClass="btn btn-secondary fw-bold" style="height: 3rem;" Visible="false" />
                                </div>
                            </div>
                        </section>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>