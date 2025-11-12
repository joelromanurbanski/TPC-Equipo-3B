<%@ Page Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeBehind="GestionClientes.aspx.cs" Inherits="tp_c_equipo_3B.GestionClientes" %>

<asp:Content ID="HeadContent" ContentPlaceHolderID="head" runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>Administración de Clientes</title>

    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css" rel="stylesheet" crossorigin="anonymous" />
    <link href="https://fonts.googleapis.com/css2?family=Material+Symbols+Outlined" rel="stylesheet" />
    <style>
        .material-symbols-outlined { font-variation-settings: 'FILL' 0, 'wght' 400, 'GRAD' 0, 'opsz' 24; }
    </style>
</asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="MainContent" runat="server">

  
    <asp:ScriptManagerProxy ID="ScriptManagerProxyPage" runat="server"></asp:ScriptManagerProxy>

    <div class="d-flex justify-content-end gap-2 mb-3">
        <asp:Button ID="btnCerrarPanel" runat="server" Text="Cerrar" CssClass="btn btn-outline-secondary btn-sm" OnClick="btnCerrarPanel_Click" />
        <asp:Button ID="btnExportar" runat="server" Text="Exportar" CssClass="btn btn-outline-primary btn-sm" OnClick="btnExportar_Click" />
        <asp:Button ID="btnImportar" runat="server" Text="Importar" CssClass="btn btn-outline-success btn-sm" OnClick="btnImportar_Click" />
        <asp:Button ID="btnRefrescar" runat="server" Text="Refrescar" CssClass="btn btn-outline-info btn-sm" OnClick="btnRefrescar_Click" />
        <asp:Button ID="btnNuevoClienteTop" runat="server" Text="Nuevo Cliente" CssClass="btn btn-primary btn-sm" OnClick="btnNuevoCliente_Click" />
    </div>


    <ul class="nav nav-tabs mb-3" id="tabsGestionClientes" role="tablist">
      <li class="nav-item" role="presentation">
        <button class="nav-link active" id="tab-clientes" data-bs-toggle="tab" data-bs-target="#content-clientes" type="button" role="tab" aria-controls="content-clientes" aria-selected="true">Clientes</button>
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

        <div class="card p-3 mb-3">
          <div class="d-flex align-items-start justify-content-between">
            <div>
              <h5 class="mb-1">Gestión de Negocios <small class="text-muted">Pre-intermedio</small></h5>
              <div class="mb-2" role="list">
                <span class="badge bg-secondary me-1" role="listitem" aria-label="Verificado">Verificado</span>
                <span class="badge bg-success me-1" role="listitem" aria-label="Exito">Éxito</span>
                <span class="badge bg-info text-dark me-1" role="listitem" aria-label="Intermedio">Intermedio</span>
                <span class="badge bg-light text-dark me-1" role="listitem" aria-label="Curso">Curso</span>
                <span class="badge bg-light text-dark me-1" role="listitem" aria-label="Nivel">Nivel</span>
                <span class="badge bg-warning text-dark me-1" role="listitem" aria-label="Verifica">Verifica</span>
              </div>
            </div>
            <div>
              <asp:Button ID="btnExitoCard" runat="server" CssClass="btn btn-primary" Text="Éxito" OnClick="btnExitoCard_Click" />
            </div>
          </div>

          <hr />

          <div class="d-flex align-items-center justify-content-between">
            <div>
              <asp:Button ID="btnContinuarServer" runat="server" CssClass="btn btn-primary" Text="CONTINUAR" OnClick="btnContinuarServer_Click" />
              <div class="mt-2">
                <span class="badge bg-light text-dark me-1">Nivel bajo</span>
                <span class="badge bg-light text-dark me-1">Facilidades</span>
                <span class="badge bg-light text-dark me-1">Examen</span>
                <span class="badge bg-light text-dark me-1">Nivel</span>
                <span class="badge bg-light text-dark me-1">Gestión</span>
              </div>
            </div>
            <div class="text-muted small">Progreso: 35%</div>
          </div>
        </div>

        <div class="relative">
            <div class="d-flex justify-content-between align-items-center mb-3">
                <h3 class="mb-0">Administración de Clientes</h3>
                <asp:Button ID="btnNuevoCliente" runat="server" Text="Nuevo Cliente" CssClass="btn btn-primary" OnClick="btnNuevoCliente_Click" />
            </div>
            <asp:Panel ID="pnlClienteForm" runat="server" CssClass="card p-3 mb-3" Visible="false">
                <asp:HiddenField ID="hfEditingId" runat="server" />
                <div class="row g-3">
                    <div class="col-md-4">
                        <label class="form-label" for="txtNombre">Nombre</label>
                        <asp:TextBox ID="txtNombre" runat="server" CssClass="form-control" />
                    </div>
                    <div class="col-md-4">
                        <label class="form-label" for="txtApellido">Apellido</label>
                        <asp:TextBox ID="txtApellido" runat="server" CssClass="form-control" />
                    </div>
                    <div class="col-md-4">
                        <label class="form-label" for="txtCedula">Cédula/Identificación</label>
                        <asp:TextBox ID="txtCedula" runat="server" CssClass="form-control" />
                    </div>
                    <div class="col-md-4">
                        <label class="form-label" for="txtTelefono">Teléfono</label>
                        <asp:TextBox ID="txtTelefono" runat="server" CssClass="form-control" />
                    </div>
                    <div class="col-md-4">
                        <label class="form-label" for="txtEmail">Correo Electrónico</label>
                        <asp:TextBox ID="txtEmail" runat="server" TextMode="Email" CssClass="form-control" />
                    </div>
                    <div class="col-md-4">
                        <label class="form-label" for="txtDireccion">Dirección</label>
                        <asp:TextBox ID="txtDireccion" runat="server" CssClass="form-control" />
                    </div>
                </div>

                <div class="mt-3 d-flex justify-content-end gap-2">
                    <asp:Button ID="btnCancelarCliente" runat="server" Text="Cancelar" CssClass="btn btn-secondary" OnClick="btnCancelarCliente_Click" />
                    <asp:Button ID="btnGuardarCliente" runat="server" Text="Guardar" CssClass="btn btn-success" OnClick="btnGuardarCliente_Click" />
                </div>
            </asp:Panel>

            <asp:Label ID="lblMensaje" runat="server" CssClass="text-danger mb-2 d-block" />

            <div class="card p-3">
                <div class="d-flex justify-content-between align-items-center mb-2">
                    <div>Lista de Clientes</div>
                    <div class="w-50">
                        <div class="input-group">
                            <asp:TextBox ID="txtBuscarClientes" runat="server" CssClass="form-control" Placeholder="Buscar por nombre, teléfono o correo" />
                            <button class="btn btn-outline-secondary" type="button" onclick="document.getElementById('<%= btnBuscarClientes.ClientID %>').click()">Buscar</button>
                            <asp:Button ID="btnBuscarClientes" runat="server" CssClass="d-none" OnClick="btnBuscarClientes_Click" />
                        </div>
                    </div>
                </div>

                <asp:GridView ID="gvClientes" runat="server" CssClass="table table-striped" AutoGenerateColumns="false" EmptyDataText="No hay clientes." OnRowCommand="gvClientes_RowCommand">
                    <Columns>
                        <asp:BoundField DataField="NombreCompleto" HeaderText="Nombre Completo" />
                        <asp:BoundField DataField="Telefono" HeaderText="Número de Teléfono" />
                        <asp:BoundField DataField="Email" HeaderText="Correo Electrónico" />
                        <asp:TemplateField HeaderText="Acciones">
                            <ItemTemplate>
                                <asp:LinkButton ID="lnkEditar" runat="server" CommandName="Editar" CommandArgument='<%# Eval("Id") %>' CssClass="me-2">Editar</asp:LinkButton>
                                <asp:LinkButton ID="lnkEliminar" runat="server" CommandName="Eliminar" CommandArgument='<%# Eval("Id") %>' CssClass="text-danger">Eliminar</asp:LinkButton>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
            </div>
        </div>

      </div>

      <div class="tab-pane fade" id="content-historial" role="tabpanel" aria-labelledby="tab-historial">
        <div class="card p-3">
          <h5>Historial de cambios</h5>
          <p class="text-muted">Registros de alta/edición/eliminación de clientes.</p>
          <asp:GridView ID="gvHistorial" runat="server" CssClass="table table-sm" AutoGenerateColumns="false" EmptyDataText="No hay historial.">
            <Columns>
              <asp:BoundField DataField="Date" HeaderText="Fecha" DataFormatString="{0:G}" />
              <asp:BoundField DataField="Action" HeaderText="Acción" />
              <asp:BoundField DataField="User" HeaderText="Usuario" />
              <asp:BoundField DataField="Details" HeaderText="Detalles" />
            </Columns>
          </asp:GridView>
        </div>
      </div>

      <div class="tab-pane fade" id="content-estadisticas" role="tabpanel" aria-labelledby="tab-estadisticas">
        <div class="card p-3">
          <h5>Estadísticas</h5>
          <div class="row">
            <div class="col-md-4">
              <div class="p-3 border rounded text-center">
                <asp:Label ID="lblTotalClientes" runat="server" CssClass="h4 mb-0">0</asp:Label>
                <div class="text-muted small">Total clientes</div>
              </div>
            </div>
            <div class="col-md-4">
              <div class="p-3 border rounded text-center">
                <asp:Label ID="lblClientesActivos" runat="server" CssClass="h4 mb-0">0</asp:Label>
                <div class="text-muted small">Activos</div>
              </div>
            </div>
            <div class="col-md-4">
              <div class="p-3 border rounded text-center">
                <asp:Label ID="lblClientesRecientes" runat="server" CssClass="h4 mb-0">0</asp:Label>
                <div class="text-muted small">Nuevos (30d)</div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <script type="text/javascript">
        (function () {
            function activateTabFromQuery() {
                var params = new URLSearchParams(window.location.search);
                var tab = params.get('tab');
                if (!tab) return;
                var tabButton = document.querySelector('#tabsGestionClientes #tab-' + tab);
                if (tabButton) {
                    var tabInstance = new bootstrap.Tab(tabButton);
                    tabInstance.show();
                }
            }

            document.querySelectorAll('#tabsGestionClientes button[data-bs-toggle="tab"]').forEach(function (btn) {
                btn.addEventListener('shown.bs.tab', function (e) {
                    var id = e.target.id.replace('tab-', '');
                    var params = new URLSearchParams(window.location.search);
                    params.set('tab', id);
                    var newUrl = window.location.pathname + '?' + params.toString();
                    history.replaceState(null, '', newUrl);
                });
            });

            document.addEventListener('DOMContentLoaded', function () {
                activateTabFromQuery();
            });
        })();
    </script>

</asp:Content>
