<%@ Page Title="Gestión de Usuarios" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeBehind="GestionUsuarios.aspx.cs" Inherits="tp_c_equipo_3B.GestionUsuarios" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server"></asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    
    <div class="container my-4">
        <div class="d-flex justify-content-between align-items-center mb-4">
            <h2>Gestión de Usuarios</h2>
            <asp:Button ID="btnNuevo" runat="server" Text="Nuevo Usuario" CssClass="btn btn-primary" OnClick="btnNuevo_Click" />
        </div>

        <asp:Label ID="lblMensaje" runat="server" CssClass="alert d-block mb-3" Visible="false" />

        <asp:Panel ID="pnlFormulario" runat="server" Visible="false" CssClass="card p-4 mb-4 shadow-sm">
            <h4>Datos del Usuario</h4>
            <asp:HiddenField ID="hfIdUsuario" runat="server" />
            
            <div class="row g-3">
                <div class="col-md-6">
                    <label class="form-label">Email (Usuario)</label>
                    <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control" TextMode="Email" />
                </div>
                <div class="col-md-6">
                    <label class="form-label">Contraseña</label>
                    <asp:TextBox ID="txtPass" runat="server" CssClass="form-control" />
                </div>
                <div class="col-md-6">
                    <label class="form-label">Nombre</label>
                    <asp:TextBox ID="txtNombre" runat="server" CssClass="form-control" />
                </div>
                <div class="col-md-6">
                    <label class="form-label">Apellido</label>
                    <asp:TextBox ID="txtApellido" runat="server" CssClass="form-control" />
                </div>
                <div class="col-12">
                    <div class="form-check">
                        <asp:CheckBox ID="chkAdmin" runat="server" CssClass="form-check-input" />
                        <label class="form-check-label">Es Administrador</label>
                    </div>
                </div>
            </div>

            <div class="mt-3 d-flex justify-content-end gap-2">
                <asp:Button ID="btnGuardar" runat="server" Text="Guardar" CssClass="btn btn-success" OnClick="btnGuardar_Click" />
                <asp:Button ID="btnCancelar" runat="server" Text="Cancelar" CssClass="btn btn-secondary" OnClick="btnCancelar_Click" />
            </div>
        </asp:Panel>

        <div class="card shadow-sm">
            <div class="card-body p-0">
                <asp:GridView ID="gvUsuarios" runat="server" CssClass="table table-striped mb-0" AutoGenerateColumns="false"
                    DataKeyNames="Id" OnRowCommand="gvUsuarios_RowCommand">
                    <Columns>
                        <asp:BoundField DataField="Id" HeaderText="ID" />
                        <asp:BoundField DataField="Email" HeaderText="Usuario" />
                        <asp:BoundField DataField="NombreCompleto" HeaderText="Nombre" />
                        <asp:BoundField DataField="RolTexto" HeaderText="Rol" />
                        <asp:TemplateField HeaderText="Acciones">
                            <ItemTemplate>
                                <asp:Button Text="Editar" CssClass="btn btn-sm btn-outline-primary" CommandName="Editar" CommandArgument='<%# Eval("Id") %>' runat="server" />
                                <asp:Button Text="Eliminar" CssClass="btn btn-sm btn-outline-danger" CommandName="Eliminar" CommandArgument='<%# Eval("Id") %>' runat="server" OnClientClick="return confirm('¿Seguro?');" />
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
            </div>
        </div>
    </div>
</asp:Content>