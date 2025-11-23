<%@ Page Title="Gestión de Pedidos" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeBehind="GestionPedidos.aspx.cs" Inherits="tp_c_equipo_3B.GestionPedidos" %>

<asp:Content ID="HeadContent" ContentPlaceHolderID="head" runat="server">
    <style>
        .badge { font-weight: 600; letter-spacing: 0.3px; padding: 0.65em 0.9em; border: 1px solid rgba(0,0,0,0.15); }
        .gv-estado-EnPreparacion { background-color: #ffd700; color: #000 !important; }
        .gv-estado-Listo { background-color: #90caf9; color: #000 !important; }
        .gv-estado-Enviado { background-color: #64b5f6; color: #000 !important; }
        .gv-estado-Entregado { background-color: #66bb6a; color: #000 !important; }
        .gv-estado-Cancelado { background-color: #ef5350; color: #000 !important; }
    </style>
</asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="MainContent" runat="server">
    
    <div class="container my-4">
        <div class="d-flex justify-content-between align-items-center mb-4">
            <h1 class="h2 mb-0 fw-bold text-dark">Gestión de Pedidos</h1>
        </div>
        
        <div class="card mb-4 border-0 shadow-sm rounded-3">
            <div class="card-body">
                
                <div class="row g-2 mb-2">
                    <div class="col-md-6">
                        <div class="input-group">
                             <span class="input-group-text bg-white border-end-0"><i class="material-symbols-outlined text-muted">search</i></span>
                             <asp:TextBox ID="txtBuscar" runat="server" CssClass="form-control border-start-0" Placeholder="Buscar cliente, factura..." />
                        </div>
                    </div>
                    <div class="col-md-3">
                        <asp:DropDownList ID="ddlFiltroEstado" runat="server" CssClass="form-select">
                            <asp:ListItem Value="Todos">Todos los Estados</asp:ListItem>
                            <asp:ListItem Value="En Preparación">En Preparación</asp:ListItem>
                            <asp:ListItem Value="Listo para Despachar">Listo para Despachar</asp:ListItem>
                            <asp:ListItem Value="Enviado">Enviado</asp:ListItem>
                            <asp:ListItem Value="Entregado">Entregado</asp:ListItem>
                            <asp:ListItem Value="Cancelado">Cancelado</asp:ListItem>
                        </asp:DropDownList>
                    </div>
                    <div class="col-md-3">
                         <asp:DropDownList ID="ddlOrden" runat="server" CssClass="form-select" AutoPostBack="true" OnSelectedIndexChanged="ddlOrden_SelectedIndexChanged">
                            <asp:ListItem Value="DESC">Más Recientes</asp:ListItem>
                            <asp:ListItem Value="ASC">Más Antiguos</asp:ListItem>
                        </asp:DropDownList>
                    </div>
                </div>

                <div class="row g-2 align-items-end">
                    <div class="col-md-3">
                        <label class="form-label small text-muted mb-1">Desde</label>
                        <asp:TextBox ID="txtFechaDesde" runat="server" TextMode="Date" CssClass="form-control form-control-sm" />
                    </div>
                    <div class="col-md-3">
                        <label class="form-label small text-muted mb-1">Hasta</label>
                        <asp:TextBox ID="txtFechaHasta" runat="server" TextMode="Date" CssClass="form-control form-control-sm" />
                    </div>
                    <div class="col-md-6 d-flex gap-2">
                        <asp:Button ID="btnBuscar" runat="server" Text="Aplicar Filtros" CssClass="btn btn-primary w-100 fw-semibold btn-sm" OnClick="btnBuscar_Click" />
                        <asp:Button ID="btnRefrescar" runat="server" Text="Limpiar Todo" CssClass="btn btn-light border w-100 btn-sm" OnClick="btnRefrescar_Click" />
                    </div>
                </div>

            </div>
        </div>

        <asp:Label ID="lblMensaje" runat="server" CssClass="alert d-block mb-3 rounded-3" Visible="false" />

        <div class="card shadow-sm border-0 rounded-xl overflow-hidden">
            <div class="card-body p-0">
                <asp:GridView ID="gvVentas" runat="server" 
                    CssClass="table table-hover mb-0 align-middle"
                    AutoGenerateColumns="False" 
                    DataKeyNames="Id"
                    OnRowCommand="gvVentas_RowCommand"
                    OnRowDataBound="gvVentas_RowDataBound"
                    EmptyDataText="No se encontraron ventas con esos filtros."
                    AllowPaging="True" PageSize="10" OnPageIndexChanging="gvVentas_PageIndexChanging"
                    GridLines="None">
                    
                    <HeaderStyle CssClass="bg-light text-uppercase small fw-bold text-secondary" Height="50px" />
                    
                    <Columns>
                        <asp:BoundField DataField="NumeroFactura" HeaderText="Factura" ItemStyle-CssClass="fw-medium" />
                        <asp:BoundField DataField="Cliente.NombreCompleto" HeaderText="Cliente" />
                        <asp:BoundField DataField="Fecha" HeaderText="Fecha" DataFormatString="{0:dd/MM/yyyy}" />
                        <asp:BoundField DataField="TotalVenta" HeaderText="Total" DataFormatString="{0:C}" ItemStyle-CssClass="fw-bold text-dark" />
                        
                        <asp:TemplateField HeaderText="Estado">
                            <ItemTemplate>
                                <span class="badge rounded-pill <%# GetEstadoClass(Eval("Estado").ToString()) %>">
                                    <%# Eval("Estado") %>
                                </span>
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="Acciones" ItemStyle-Width="280px">
                            <ItemTemplate>
                                <div class="d-flex gap-2 align-items-center">
                                    <asp:LinkButton ID="btnVerDetalle" runat="server" 
                                        CssClass="btn btn-sm btn-outline-primary d-flex align-items-center"
                                        CommandName="VerDetalle" CommandArgument='<%# Eval("Id") %>' 
                                        ToolTip="Ver productos">
                                        <span class="material-symbols-outlined" style="font-size: 18px;">visibility</span>
                                    </asp:LinkButton>

                                    <asp:DropDownList ID="ddlEstado" runat="server" CssClass="form-select form-select-sm" style="width: 140px;">
                                        <asp:ListItem Value="En Preparación">En Preparación</asp:ListItem>
                                        <asp:ListItem Value="Listo para Despachar">Listo</asp:ListItem>
                                        <asp:ListItem Value="Enviado">Enviado</asp:ListItem>
                                        <asp:ListItem Value="Entregado">Entregado</asp:ListItem>
                                        <asp:ListItem Value="Cancelado">Cancelado</asp:ListItem>
                                    </asp:DropDownList>
                                    
                                    <asp:LinkButton ID="btnActualizarEstado" runat="server" 
                                        CssClass="btn btn-sm btn-light border d-flex align-items-center"
                                        CommandName="ActualizarEstado" CommandArgument='<%# Eval("Id") %>'
                                        ToolTip="Guardar estado" CausesValidation="false">
                                        <span class="material-symbols-outlined" style="font-size: 18px;">save</span>
                                    </asp:LinkButton>
                                </div>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <PagerStyle CssClass="pagination-container p-3" HorizontalAlign="Center" />
                </asp:GridView>
            </div>
        </div>
    </div>

    <div class="modal fade" id="modalDetalle" tabindex="-1" aria-hidden="true">
      <div class="modal-dialog modal-dialog-centered modal-lg">
        <div class="modal-content border-0 shadow">
          <div class="modal-header bg-light">
            <h5 class="modal-title fw-bold">Detalle del Pedido #<asp:Label ID="lblModalIdVenta" runat="server" /></h5>
            <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Cerrar"></button>
          </div>
          <div class="modal-body p-0">
              <asp:GridView ID="gvDetallePedido" runat="server" 
                  CssClass="table table-striped mb-0" 
                  AutoGenerateColumns="False" GridLines="None">
                  <Columns>
                      <asp:BoundField DataField="Articulo.Nombre" HeaderText="Producto" />
                      <asp:BoundField DataField="Cantidad" HeaderText="Cant." ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center" />
                      <asp:BoundField DataField="PrecioUnitario" HeaderText="Precio Unit." DataFormatString="{0:C}" ItemStyle-HorizontalAlign="Right" HeaderStyle-HorizontalAlign="Right" />
                      <asp:BoundField DataField="Subtotal" HeaderText="Subtotal" DataFormatString="{0:C}" ItemStyle-HorizontalAlign="Right" HeaderStyle-HorizontalAlign="Right" ItemStyle-Font-Bold="true" />
                  </Columns>
              </asp:GridView>
          </div>
          <div class="modal-footer bg-light">
            <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Cerrar</button>
          </div>
        </div>
      </div>
    </div>

    <script>
        function openModal() {
            var myModal = new bootstrap.Modal(document.getElementById('modalDetalle'));
            myModal.show();
        }
    </script>

</asp:Content>