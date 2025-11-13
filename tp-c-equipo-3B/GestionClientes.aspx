<%@ Page Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeBehind="GestionClientes.aspx.cs" Inherits="tp_c_equipo_3B.GestionClientes" %>

<asp:Content ID="HeadContent" ContentPlaceHolderID="head" runat="server">
    <title>Administración de Clientes</title>
</asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="MainContent" runat="server">
    
    <div class="d-flex justify-content-end gap-2 mb-3">
        <asp:Button ID="btnRefrescar" runat="server" Text="Refrescar" CssClass="btn btn-outline-info btn-sm" OnClick="btnRefrescar_Click" CausesValidation="false" />
        <asp:Button ID="btnNuevoCliente" runat="server" Text="Nuevo Cliente" CssClass="btn btn-primary btn-sm" OnClick="btnNuevoCliente_Click" CausesValidation="false" />
    </div>

    <div class="relative">
        
        <asp:Panel ID="pnlClienteForm" runat="server" CssClass="card p-3 mb-3" Visible="false">
            <h3>
                <asp:Literal ID="litTitulo" runat="server" Text="Nuevo Cliente"></asp:Literal>
            </h3>
            <hr />
            <asp:HiddenField ID="hfEditingId" runat="server" />
            <div class="row g-3">
                <div class="col-md-4">
                    <label class="form-label" for="txtDocumento">Documento *</label>
                    <asp:TextBox ID="txtDocumento" runat="server" CssClass="form-control" />
                    <asp:RequiredFieldValidator ErrorMessage="Documento requerido." ControlToValidate="txtDocumento" CssClass="text-danger" Display="Dynamic" runat="server" />
                </div>
                <div class="col-md-4">
                    <label class="form-label" for="txtNombre">Nombre *</label>
                    <asp:TextBox ID="txtNombre" runat="server" CssClass="form-control" />
                    <asp:RequiredFieldValidator ErrorMessage="Nombre requerido." ControlToValidate="txtNombre" CssClass="text-danger" Display="Dynamic" runat="server" />
                </div>
                <div class="col-md-4">
                    <label class="form-label" for="txtApellido">Apellido *</label>
                    <asp:TextBox ID="txtApellido" runat="server" CssClass="form-control" />
                    <asp:RequiredFieldValidator ErrorMessage="Apellido requerido." ControlToValidate="txtApellido" CssClass="text-danger" Display="Dynamic" runat="server" />
                </div>
                <div class="col-md-6">
                    <label class="form-label" for="txtEmail">Correo Electrónico</label>
                    <asp:TextBox ID="txtEmail" runat="server" TextMode="Email" CssClass="form-control" />
                </div>
                <div class="col-md-6">
                    <label class="form-label" for="txtTelefono">Teléfono</label>
                    <asp:TextBox ID="txtTelefono" runat="server" CssClass="form-control" />
                </div>
                <div class="col-md-6">
                    <label class="form-label" for="txtDireccion">Dirección</label>
                    <asp:TextBox ID="txtDireccion" runat="server" CssClass="form-control" />
                </div>
                <div class="col-md-4">
                    <label class="form-label" for="txtCiudad">Ciudad</label>
                    <asp:TextBox ID="txtCiudad" runat="server" CssClass="form-control" />
                </div>
                <div class="col-md-2">
                    <label class="form-label" for="txtCP">Cód. Postal</label>
                    <asp:TextBox ID="txtCP" runat="server" CssClass="form-control" TextMode="Number" />
                </div>
            </div>

            <div class="mt-3 d-flex justify-content-end gap-2">
                <asp:Button ID="btnCancelarCliente" runat="server" Text="Cancelar" CssClass="btn btn-secondary" OnClick="btnCancelarCliente_Click" CausesValidation="false" />
                <asp:Button ID="btnGuardar" runat="server" Text="Guardar" CssClass="btn btn-primary" OnClick="btnGuardar_Click" Visible="true" />
                <asp:Button ID="btnModificar" runat="server" Text="Modificar" CssClass="btn btn-warning" OnClick="btnModificar_Click" Visible="false" />
            </div>
        </asp:Panel>

        <asp:Label ID="lblMensaje" runat="server" CssClass="text-danger mb-2 d-block" />

        <div class="card p-3">
            <div class="d-flex justify-content-between align-items-center mb-2">
                 <div><h5>Lista de Clientes</h5></div>
                <div class="w-50">
                    <div class="input-group">
                        <asp:TextBox ID="txtBuscarClientes" runat="server" CssClass="form-control" Placeholder="Buscar por nombre, documento o email" />
                        <asp:Button ID="btnBuscarClientes" runat="server" Text="Buscar" CssClass="btn btn-outline-secondary" OnClick="btnBuscarClientes_Click" />
                    </div>
                </div>
            </div>

            <asp:GridView ID="gvClientes" runat="server" CssClass="table table-striped table-hover" 
                AutoGenerateColumns="false" 
                DataKeyNames="Id"
                EmptyDataText="No se encontraron clientes."
                OnRowCommand="gvClientes_RowCommand"
                AllowPaging="True" PageSize="10" OnPageIndexChanging="gvClientes_PageIndexChanging">
                <Columns>
                    <asp:BoundField DataField="NombreCompleto" HeaderText="Nombre Completo" />
                    <asp:BoundField DataField="Documento" HeaderText="Documento" />
                    <asp:BoundField DataField="Email" HeaderText="Correo Electrónico" />
                    <asp:BoundField DataField="Telefono" HeaderText="Teléfono" />
                    <asp:TemplateField HeaderText="Acciones">
                        <ItemTemplate>
                            <asp:LinkButton ID="lnkEditar" runat="server" CommandName="Editar" CommandArgument='<%# Eval("Id") %>' CssClass="me-2">Editar</asp:LinkButton>
                            <asp:LinkButton ID="lnkEliminar" runat="server" CommandName="Eliminar" CommandArgument='<%# Eval("Id") %>' CssClass="text-danger"
                                OnClientClick="return confirm('¿Está seguro que desea eliminar este cliente?');">Eliminar</asp:LinkButton>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
                 <PagerStyle CssClass="pagination-container" />
            </asp:GridView>
        </div>
    </div>

</asp:Content>