<%@ Page Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeBehind="GestionProveedores.aspx.cs" Inherits="tp_c_equipo_3B.GestionProveedores" %>

<asp:Content ID="HeadContent" ContentPlaceHolderID="head" runat="server">
  <meta charset="utf-8" />
  <meta name="viewport" content="width=device-width, initial-scale=1.0" />
  <title>Gestión de Proveedores</title>
  <link href="https://fonts.googleapis.com/css2?family=Inter:wght@400;500;700;900&amp;display=swap" rel="stylesheet" />
  <link href="https://fonts.googleapis.com/css2?family=Material+Symbols+Outlined" rel="stylesheet" />
  <style>
    .material-symbols-outlined { font-variation-settings: 'FILL' 0, 'wght' 400, 'GRAD' 0, 'opsz' 24; }
  </style>
</asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="MainContent" runat="server">

  <div class="d-flex justify-content-end gap-2 mb-3 align-items-center">
    <div class="input-group me-2" style="max-width:420px;">
      <asp:TextBox ID="txtGlobalSearch" runat="server" CssClass="form-control" placeholder="Buscar en proveedores..." />
      <asp:Button ID="btnGlobalSearch" runat="server" Text="Buscar" CssClass="btn btn-primary" OnClick="btnGlobalSearch_Click" />
    </div>
    
    <asp:Button ID="btnRefrescar" runat="server" Text="Refrescar" CssClass="btn btn-outline-info btn-sm" OnClick="btnRefrescar_Click" />
    <asp:Button ID="btnNuevoProveedor" runat="server" Text="Nuevo Proveedor" CssClass="btn btn-primary btn-sm" OnClick="btnNuevoProveedor_Click" CausesValidation="false" />
  </div>

  <div class="row gx-4">
    <aside class="col-md-4">
      <div class="mb-3">
        <div class="input-group">
          <asp:TextBox ID="txtBuscarProveedorSidebar" runat="server" CssClass="form-control" placeholder="Buscar proveedor" />
          <asp:Button ID="btnBuscarProveedorSidebar" runat="server" Text="Buscar" CssClass="btn btn-outline-secondary" OnClick="btnBuscarProveedorSidebar_Click" />
        </div>
      </div>

      <div class="card p-0">
        <div class="overflow-auto" style="max-height:520px;">
          <asp:Repeater ID="rptProveedores" runat="server" OnItemCommand="rptProveedores_ItemCommand">
            <HeaderTemplate>
              <table class="table mb-0 table-hover">
                <thead class="table-light">
                  <tr>
                    <th>Nombre</th>
                    <th>Tel.</th>
                  </tr>
                </thead>
                <tbody>
            </HeaderTemplate>
            <ItemTemplate>
                  <tr>
                    <td>
                      <asp:LinkButton ID="lnkProveedor" runat="server" CommandName="Select" CommandArgument='<%# Eval("Id") %>'><%# Eval("Nombre") %></asp:LinkButton>
                    </td>
                    <td class="text-muted"><%# Eval("Telefono") %></td>
                  </tr>
            </ItemTemplate>
            <FooterTemplate>
                </tbody>
              </table>
            </FooterTemplate>
          </asp:Repeater>
          <asp:Panel ID="pnlEmptyProveedores" runat="server" Visible="false" CssClass="p-4 text-center text-muted">
            No se encontraron proveedores.
          </asp:Panel>
        </div>
      </div>
    </aside>

    <main class="col-md-8">
      
      <asp:Panel ID="pnlVacio" runat="server" CssClass="card p-5 text-center text-muted" Visible="true">
          Seleccione un proveedor de la lista para ver sus detalles o cree uno nuevo.
      </asp:Panel>

      <asp:Panel ID="pnlFormulario" runat="server" Visible="false">
          <h3>
              <asp:Literal ID="litTitulo" runat="server" Text="Nuevo Proveedor"></asp:Literal>
          </h3>
          <hr />
          <div class="card">
              <div class="card-body">
                  <div class="mb-3">
                      <label for="<%= txtNombreForm.ClientID %>" class="form-label">Nombre del Proveedor *</label>
                      <asp:TextBox ID="txtNombreForm" runat="server" CssClass="form-control" />
                      <asp:RequiredFieldValidator 
                          ErrorMessage="El nombre es obligatorio." 
                          ControlToValidate="txtNombreForm" 
                          CssClass="text-danger" 
                          Display="Dynamic" 
                          runat="server" />
                  </div>
                  <div class="mb-3">
                      <label for="<%= txtEmailForm.ClientID %>" class="form-label">Email</label>
                      <asp:TextBox ID="txtEmailForm" runat="server" CssClass="form-control" TextMode="Email" />
                  </div>
                  <div class="mb-3">
                      <label for="<%= txtTelefonoForm.ClientID %>" class="form-label">Teléfono</label>
                      <asp:TextBox ID="txtTelefonoForm" runat="server" CssClass="form-control" />
                  </div>
                  <div class="mb-3">
                      <label for="<%= txtDireccionForm.ClientID %>" class="form-label">Dirección</label>
                      <asp:TextBox ID="txtDireccionForm" runat="server" CssClass="form-control" />
                  </div>
                  <hr />
                  <div class="d-flex gap-2">
                      <asp:Button ID="btnGuardar" runat="server" Text="Guardar" CssClass="btn btn-primary" OnClick="btnGuardar_Click" />
                      <asp:Button ID="btnModificar" runat="server" Text="Modificar" CssClass="btn btn-warning" OnClick="btnModificar_Click" Visible="false" />
                      <asp:Button ID="btnCancelar" runat="server" Text="Cancelar" CssClass="btn btn-secondary" OnClick="btnCancelar_Click" CausesValidation="false" />
                  </div>
              </div>
          </div>
      </asp:Panel>

      <asp:Panel ID="pnlDetalle" runat="server" Visible="false">
          <div class="d-flex justify-content-between align-items-center mb-3">
            <div>
              <asp:Label ID="lblProveedorNombre" runat="server" CssClass="h5 mb-0">Proveedor A</asp:Label>
            </div>
            <div class="d-flex gap-2">
              <asp:Button ID="btnEditarProveedor" runat="server" Text="Editar" CssClass="btn btn-secondary btn-sm" OnClick="btnEditarProveedor_Click" CausesValidation="false" />
              <asp:Button ID="btnEliminarProveedor" runat="server" Text="Eliminar" CssClass="btn btn-danger btn-sm" 
                  OnClick="btnEliminarProveedor_Click" OnClientClick="return confirm('¿Está seguro que desea eliminar este proveedor? Esta acción no se puede deshacer.');" CausesValidation="false" />
            </div>
          </div>

          <ul class="nav nav-tabs mb-3" id="tabsGestionProveedores" role="tablist">
            <li class="nav-item" role="presentation">
              <button class="nav-link active" id="tab-datos" data-bs-toggle="tab" data-bs-target="#content-datos" type="button" role="tab" aria-controls="content-datos" aria-selected="true">Datos Generales</button>
            </li>
            <li class="nav-item" role="presentation">
              <button class="nav-link" id="tab-productos" data-bs-toggle="tab" data-bs-target="#content-productos" type="button" role="tab" aria-controls="content-productos" aria-selected="false">Productos Suministrados</button>
            </li>
          </ul>

          <div class="tab-content" id="tabsContentGestionProveedores">
            <div class="tab-pane fade show active" id="content-datos" role="tabpanel" aria-labelledby="tab-datos">
              <div class="card p-3 mb-3">
                <div class="row g-3">
                  <div class="col-md-6">
                    <label class="form-label small text-muted">Nombre</label>
                    <asp:Label ID="lblCompanyName" runat="server" CssClass="d-block"></asp:Label>
                  </div>
                  <div class="col-md-6">
                    <label class="form-label small text-muted">Email</label>
                    <asp:Label ID="lblEmail" runat="server" CssClass="d-block"></asp:Label>
                  </div>
                  <div class="col-12">
                    <label class="form-label small text-muted">Dirección</label>
                    <asp:Label ID="lblAddress" runat="server" CssClass="d-block"></asp:Label>
                  </div>
                  <div class="col-md-6">
                    <label class="form-label small text-muted">Teléfono</label>
                    <asp:Label ID="lblPhone" runat="server" CssClass="d-block"></asp:Label>
                  </div>
                </div>
              </div>
            </div>

            <div class="tab-pane fade" id="content-productos" role="tabpanel" aria-labelledby="tab-productos">
              <div class="card p-3 mb-3">
                <asp:GridView ID="gvProductos" runat="server" CssClass="table table-sm table-striped" AutoGenerateColumns="false" EmptyDataText="Este proveedor no suministra ningún producto actualmente.">
                  <Columns>
                    <asp:BoundField DataField="Nombre" HeaderText="Producto" />
                    <asp:BoundField DataField="Codigo" HeaderText="Código/SKU" />
                    <asp:BoundField DataField="Categoria.Descripcion" HeaderText="Categoría" />
                    <asp:BoundField DataField="StockActual" HeaderText="Stock" />
                  </Columns>
                </asp:GridView>
              </div>
            </div>
          </div>
      </asp:Panel>
    </main>
  </div>
 
  <script type="text/javascript">
      
  </script>

</asp:Content>