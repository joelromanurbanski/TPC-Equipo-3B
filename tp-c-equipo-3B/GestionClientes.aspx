<%@ Page Title="Administración de Clientes" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeBehind="GestionClientes.aspx.cs" Inherits="tp_c_equipo_3B.GestionClientes" %>

<asp:Content ID="HeadContent" ContentPlaceHolderID="head" runat="server">
    <style>
        .nav-tabs .nav-link.active { font-weight: 600; border-bottom: 2px solid #0d6efd; color: #0d6efd; }
    </style>
</asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="MainContent" runat="server">

    <div class="d-flex justify-content-between align-items-center mb-4">
        <h1 class="h2 mb-0">Gestión de Clientes</h1>
        <div class="d-flex gap-2">
             <asp:Button ID="btnRefrescar" runat="server" Text="Refrescar" CssClass="btn btn-outline-secondary" OnClick="btnRefrescar_Click" CausesValidation="false" />
             <asp:Button ID="btnNuevoClienteTop" runat="server" Text="Nuevo Cliente" CssClass="btn btn-primary" OnClick="btnNuevoCliente_Click" CausesValidation="false" />
        </div>
    </div>

    <ul class="nav nav-tabs mb-3" id="tabsGestionClientes" role="tablist">
      <li class="nav-item" role="presentation">
        <button class="nav-link active" id="tab-clientes" data-bs-toggle="tab" data-bs-target="#content-clientes" type="button" role="tab" aria-controls="content-clientes" aria-selected="true">Listado de Clientes</button>
      </li>
      <li class="nav-item" role="presentation">
        <button class="nav-link" id="tab-historial" data-bs-toggle="tab" data-bs-target="#content-historial" type="button" role="tab" aria-controls="content-historial" aria-selected="false">Historial</button>
      </li>
      <li class="nav-item" role="presentation">
        <button class="nav-link" id="tab-estadisticas" data-bs-toggle="tab" data-bs-target="#content-estadisticas" type="button" role="tab" aria-controls="content-estadisticas" aria-selected="false">Estadísticas</button>
      </li>
    </ul>

    <div class="tab-content" id="tabsContentGestionClientes">
      
      <div class="tab-pane fade show active" id="content-clientes" role="tabpanel" aria-labelledby="tab-clientes">
        
        <asp:Label ID="lblMensaje" runat="server" CssClass="alert d-block mb-3" Visible="false" />

        <asp:Panel ID="pnlClienteForm" runat="server" CssClass="card p-4 mb-4 shadow-sm border-0" Visible="false">
            <h4 class="mb-3">Datos del Cliente</h4>
            <asp:HiddenField ID="hfEditingId" runat="server" />
            <div class="row g-3">
                <div class="col-md-4">
                    <label class="form-label" for="<%= txtNombre.ClientID %>">Nombre *</label>
                    <asp:TextBox ID="txtNombre" runat="server" CssClass="form-control" />
                    <asp:RequiredFieldValidator ErrorMessage="Requerido" ControlToValidate="txtNombre" CssClass="text-danger small" Display="Dynamic" runat="server" />
                </div>
                <div class="col-md-4">
                    <label class="form-label" for="<%= txtApellido.ClientID %>">Apellido *</label>
                    <asp:TextBox ID="txtApellido" runat="server" CssClass="form-control" />
                    <asp:RequiredFieldValidator ErrorMessage="Requerido" ControlToValidate="txtApellido" CssClass="text-danger small" Display="Dynamic" runat="server" />
                </div>
                <div class="col-md-4">
                    <label class="form-label" for="<%= txtCedula.ClientID %>">DNI/Documento</label>
                    <asp:TextBox ID="txtCedula" runat="server" CssClass="form-control" />
                </div>
                <div class="col-md-6">
                    <label class="form-label" for="<%= txtEmail.ClientID %>">Email</label>
                    <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control" TextMode="Email" />
                </div>
                <div class="col-md-6">
                    <label class="form-label" for="<%= txtTelefono.ClientID %>">Teléfono</label>
                    <asp:TextBox ID="txtTelefono" runat="server" CssClass="form-control" />
                </div>
                <div class="col-12">
                    <label class="form-label" for="<%= txtDireccion.ClientID %>">Dirección</label>
                    <asp:TextBox ID="txtDireccion" runat="server" CssClass="form-control" />
                </div>
            </div>
            <div class="mt-4 d-flex justify-content-end gap-2">
                <asp:Button ID="btnCancelarCliente" runat="server" Text="Cancelar" CssClass="btn btn-secondary" OnClick="btnCancelarCliente_Click" CausesValidation="false" />
                <asp:Button ID="btnGuardarCliente" runat="server" Text="Guardar" CssClass="btn btn-success" OnClick="btnGuardarCliente_Click" />
            </div>
        </asp:Panel>

        <div class="card mb-4 border-0 shadow-sm">
            <div class="card-body">
                <div class="row g-2">
                    <div class="col-md-8">
                        <div class="input-group">
                             <span class="input-group-text bg-white border-end-0"><i class="material-symbols-outlined text-muted">search</i></span>
                             <asp:TextBox ID="txtBuscarClientes" runat="server" CssClass="form-control border-start-0" Placeholder="Buscar por nombre, apellido o DNI..." />
                        </div>
                    </div>
                    <div class="col-md-4">
                        <asp:Button ID="btnBuscarClientes" runat="server" Text="Buscar" CssClass="btn btn-primary w-100" OnClick="btnBuscarClientes_Click" />
                    </div>
                </div>
            </div>
        </div>

        <div class="card shadow-sm border-0 rounded-xl overflow-hidden">
            <div class="card-body p-0">
                <asp:GridView ID="gvClientes" runat="server" CssClass="table table-hover mb-0 align-middle" 
                    AutoGenerateColumns="false" 
                    DataKeyNames="Id"
                    EmptyDataText="No se encontraron clientes."
                    OnRowCommand="gvClientes_RowCommand"
                    AllowPaging="True" PageSize="10" OnPageIndexChanging="gvClientes_PageIndexChanging"
                    GridLines="None">
                    
                    <HeaderStyle CssClass="bg-light text-uppercase small fw-bold text-secondary" Height="50px" />
                    
                    <Columns>
                        <asp:BoundField DataField="NombreCompleto" HeaderText="Nombre Completo" ItemStyle-CssClass="fw-medium" />
                        <asp:BoundField DataField="Documento" HeaderText="Documento" />
                        <asp:BoundField DataField="Email" HeaderText="Email" />
                        <asp:BoundField DataField="Telefono" HeaderText="Teléfono" />
                        
                        <asp:TemplateField HeaderText="Acciones" ItemStyle-Width="150px" ItemStyle-HorizontalAlign="Right">
                            <ItemTemplate>
                                <asp:LinkButton ID="lnkEditar" runat="server" CommandName="Editar" CommandArgument='<%# Eval("Id") %>' CssClass="btn btn-sm btn-outline-primary me-1">
                                    <span class="material-symbols-outlined" style="font-size: 18px;">edit</span>
                                </asp:LinkButton>
                                <asp:LinkButton ID="lnkEliminar" runat="server" CommandName="Eliminar" CommandArgument='<%# Eval("Id") %>' CssClass="btn btn-sm btn-outline-danger" OnClientClick="return confirm('¿Seguro que desea eliminar este cliente?');">
                                    <span class="material-symbols-outlined" style="font-size: 18px;">delete</span>
                                </asp:LinkButton>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <PagerStyle CssClass="pagination-container p-3" HorizontalAlign="Center" />
                </asp:GridView>
            </div>
        </div>
      </div>

      <div class="tab-pane fade" id="content-historial" role="tabpanel" aria-labelledby="tab-historial">
        <div class="card p-4 border-0 shadow-sm">
          <h5 class="mb-3">Historial de cambios</h5>
          <asp:GridView ID="gvHistorial" runat="server" CssClass="table table-sm table-bordered" AutoGenerateColumns="false" EmptyDataText="No hay historial disponible.">
            <Columns>
              <asp:BoundField DataField="Date" HeaderText="Fecha" DataFormatString="{0:g}" />
              <asp:BoundField DataField="Action" HeaderText="Acción" />
              <asp:BoundField DataField="Details" HeaderText="Detalles" />
            </Columns>
          </asp:GridView>
        </div>
      </div>

      <div class="tab-pane fade" id="content-estadisticas" role="tabpanel" aria-labelledby="tab-estadisticas">
        <div class="card p-4 border-0 shadow-sm">
          <h5 class="mb-4">Estadísticas</h5>
          <div class="row g-4">
            <div class="col-md-4">
               <div class="p-4 border rounded-3 text-center bg-light">
                <asp:Label ID="lblTotalClientes" runat="server" CssClass="display-6 fw-bold text-primary">0</asp:Label>
                <div class="text-muted mt-2">Total Clientes</div>
              </div>
            </div>
            <div class="col-md-4">
              <div class="p-4 border rounded-3 text-center bg-light">
                <asp:Label ID="lblClientesActivos" runat="server" CssClass="display-6 fw-bold text-success">0</asp:Label>
                <div class="text-muted mt-2">Activos</div>
              </div>
            </div>
            <div class="col-md-4">
              <div class="p-4 border rounded-3 text-center bg-light">
                 <asp:Label ID="lblClientesRecientes" runat="server" CssClass="display-6 fw-bold text-info">0</asp:Label>
                <div class="text-muted mt-2">Nuevos (30 días)</div>
              </div>
            </div>
          </div>
        </div>
      </div>
    
    </div>

    <script type="text/javascript">
        document.addEventListener('DOMContentLoaded', function () {
            var triggerTabList = [].slice.call(document.querySelectorAll('#tabsGestionClientes button'))
            triggerTabList.forEach(function (triggerEl) {
                var tabTrigger = new bootstrap.Tab(triggerEl)
                triggerEl.addEventListener('click', function (event) {
                    event.preventDefault()
                    tabTrigger.show()
                })
            })
        });
    </script>

</asp:Content>