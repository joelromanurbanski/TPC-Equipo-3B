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


  <asp:ScriptManagerProxy ID="ScriptManagerProxyPage" runat="server" />


  <div class="d-flex justify-content-end gap-2 mb-3 align-items-center">
    <div class="input-group me-2" style="max-width:420px;">
      <input id="txtGlobalSearch" name="txtGlobalSearch" class="form-control" placeholder="Buscar en proveedores..." />
      <button class="btn btn-primary" type="button" onclick="document.getElementById('<%= btnGlobalSearch.ClientID %>').click()">Buscar</button>
      <asp:Button ID="btnGlobalSearch" runat="server" CssClass="d-none" OnClick="btnGlobalSearch_Click" />
    </div>

    <asp:Button ID="btnExportar" runat="server" Text="Exportar" CssClass="btn btn-outline-primary btn-sm" OnClick="btnExportar_Click" />
    <asp:Button ID="btnImportar" runat="server" Text="Importar" CssClass="btn btn-outline-success btn-sm" OnClick="btnImportar_Click" />
    <asp:Button ID="btnRefrescar" runat="server" Text="Refrescar" CssClass="btn btn-outline-info btn-sm" OnClick="btnRefrescar_Click" />
    <asp:Button ID="btnNuevoProveedor" runat="server" Text="Nuevo Proveedor" CssClass="btn btn-primary btn-sm" OnClick="btnNuevoProveedor_Click" />
  </div>

  <div class="row gx-4">
 
    <aside class="col-md-4">
      <div class="mb-3">
        <div class="input-group">
          <input id="txtBuscarProveedorSidebar" name="txtBuscarProveedorSidebar" class="form-control" placeholder="Buscar proveedor" />
          <button class="btn btn-outline-secondary" type="button" onclick="document.getElementById('<%= btnBuscarProveedorSidebar.ClientID %>').click()">Buscar</button>
          <asp:Button ID="btnBuscarProveedorSidebar" runat="server" CssClass="d-none" OnClick="btnBuscarProveedorSidebar_Click" />
        </div>
      </div>

      <div class="card p-0">
        <div class="overflow-auto" style="max-height:520px;">
          <asp:Repeater ID="rptProveedores" runat="server" OnItemCommand="rptProveedores_ItemCommand">
            <HeaderTemplate>
              <table class="table mb-0">
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
        </div>
      </div>
    </aside>

   
    <main class="col-md-8">
      <div class="d-flex justify-content-between align-items-center mb-3">
        <div>
          <asp:Label ID="lblProveedorNombre" runat="server" CssClass="h5 mb-0">Proveedor A</asp:Label>
          <div class="text-muted" style="font-size:.9rem" id="lblProveedorSub">Información detallada</div>
        </div>
        <div class="d-flex gap-2">
          <asp:Button ID="btnEditarProveedor" runat="server" Text="Editar" CssClass="btn btn-secondary btn-sm" OnClick="btnEditarProveedor_Click" />
          <asp:Button ID="btnEliminarProveedor" runat="server" Text="Eliminar" CssClass="btn btn-danger btn-sm" OnClick="btnEliminarProveedor_Click" />
        </div>
      </div>


      <ul class="nav nav-tabs mb-3" id="tabsGestionProveedores" role="tablist">
        <li class="nav-item" role="presentation">
          <button class="nav-link active" id="tab-datos" data-bs-toggle="tab" data-bs-target="#content-datos" type="button" role="tab" aria-controls="content-datos" aria-selected="true">Datos Generales</button>
        </li>
        <li class="nav-item" role="presentation">
          <button class="nav-link" id="tab-contactos" data-bs-toggle="tab" data-bs-target="#content-contactos" type="button" role="tab" aria-controls="content-contactos" aria-selected="false">Contactos</button>
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
                <asp:Label ID="lblCompanyName" runat="server" CssClass="d-block">Proveedor A</asp:Label>
              </div>
              <div class="col-md-6">
                <label class="form-label small text-muted">Tax ID</label>
                <asp:Label ID="lblTaxId" runat="server" CssClass="d-block">RFC-12345678</asp:Label>
              </div>
              <div class="col-12">
                <label class="form-label small text-muted">Dirección</label>
                <asp:Label ID="lblAddress" runat="server" CssClass="d-block">Av. Principal #123</asp:Label>
              </div>
              <div class="col-md-6">
                <label class="form-label small text-muted">Teléfono</label>
                <asp:Label ID="lblPhone" runat="server" CssClass="d-block">+52-1-55-1234-7890</asp:Label>
              </div>
              <div class="col-md-6">
                <label class="form-label small text-muted">Sitio web</label>
                <asp:Label ID="lblWebsite" runat="server" CssClass="d-block text-primary">www.proveedora.com</asp:Label>
              </div>
              <div class="col-12">
                <label class="form-label small text-muted">Notas</label>
                <asp:Label ID="lblNotes" runat="server" CssClass="d-block">Datos de contacto</asp:Label>
              </div>
            </div>
          </div>
        </div>


        <div class="tab-pane fade" id="content-contactos" role="tabpanel" aria-labelledby="tab-contactos">
          <div class="card p-3 mb-3">
            <asp:GridView ID="gvContactos" runat="server" CssClass="table table-sm" AutoGenerateColumns="false">
              <Columns>
                <asp:BoundField DataField="Nombre" HeaderText="Nombre" />
                <asp:BoundField DataField="Puesto" HeaderText="Puesto" />
                <asp:BoundField DataField="Telefono" HeaderText="Teléfono" />
                <asp:BoundField DataField="Email" HeaderText="Email" />
              </Columns>
            </asp:GridView>
          </div>
        </div>


        <div class="tab-pane fade" id="content-productos" role="tabpanel" aria-labelledby="tab-productos">
          <div class="card p-3 mb-3">
            <asp:GridView ID="gvProductos" runat="server" CssClass="table table-sm" AutoGenerateColumns="false">
              <Columns>
                <asp:BoundField DataField="Nombre" HeaderText="Producto" />
                <asp:BoundField DataField="Categoria" HeaderText="Categoría" />
                <asp:BoundField DataField="Precio" HeaderText="Precio" DataFormatString="{0:C}" />
              </Columns>
            </asp:GridView>
          </div>
        </div>
      </div>

      <div class="card p-3 mb-3" style="max-width:420px;">
        <div class="d-flex justify-content-between align-items-center mb-2">
          <strong>Rank demo</strong>
          <asp:Button ID="btnLeaveRank" runat="server" CssClass="btn btn-sm btn-outline-danger" Text="leave" OnClick="btnLeaveRank_Click" />
        </div>
        <asp:Repeater ID="rptRankDemo" runat="server">
          <ItemTemplate>
            <div class="d-flex justify-content-between py-1">
              <div><%# (Container.ItemIndex + 1) + ". " + Eval("Name") %></div>
              <div class="text-muted small"><%# Eval("Score") %></div>
            </div>
          </ItemTemplate>
        </asp:Repeater>
        <div class="mt-2 text-end">
          <asp:Button ID="btnJoinRank" runat="server" CssClass="btn btn-sm btn-primary" Text="Join" OnClick="btnJoinRank_Click" />
        </div>
      </div>

    </main>
  </div>


  <script type="text/javascript">
      (function () {
          function activateTabFromQuery() {
              var params = new URLSearchParams(window.location.search);
              var tab = params.get('tab');
              if (!tab) return;
              var tabButton = document.querySelector('#tabsGestionProveedores #tab-' + tab);
              if (tabButton) {
                  var tabInstance = new bootstrap.Tab(tabButton);
                  tabInstance.show();
              }
          }

          document.querySelectorAll('#tabsGestionProveedores button[data-bs-toggle="tab"]').forEach(function (btn) {
              btn.addEventListener('shown.bs.tab', function (e) {
                  var id = e.target.id.replace('tab-', '');
                  var params = new URLSearchParams(window.location.search);
                  params.set('tab', id);
                  history.replaceState(null, '', window.location.pathname + '?' + params.toString());
              });
          });

          document.addEventListener('DOMContentLoaded', activateTabFromQuery);
      })();
  </script>

</asp:Content>
