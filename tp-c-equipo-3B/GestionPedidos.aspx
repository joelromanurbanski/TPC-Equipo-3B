<%@ Page Title="Gestión de Pedidos" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeBehind="GestionPedidos.aspx.cs" Inherits="tp_c_equipo_3B.GestionPedidos" %>

<asp:Content ID="HeadContent" ContentPlaceHolderID="head" runat="server">
    <style>
        .gv-estado-EnPreparacion { background-color: #fff3cd; color: #664d03; }
        .gv-estado-Enviado { background-color: #cfe2ff; color: #0a58ca; }
        .gv-estado-Listo { background-color: #cfe2ff; color: #0a58ca; }
        .gv-estado-Entregado { background-color: #d1e7dd; color: #0f5132; }
        .gv-estado-Cancelado { background-color: #f8d7da; color: #842029; }
    </style>
</asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="MainContent" runat="server">
    
    <div class="container my-4">
        <div class="d-flex justify-content-between align-items-center mb-4">
            <h1 class="h2 mb-0">Gestión de Pedidos de Venta</h1>
        </div>
        
        <div class="card mb-4 border-0 shadow-sm">
            <div class="card-body">
                <div class="row g-3">
                    <div class="col-md-6">
                        <div class="input-group">
                             <span class="input-group-text bg-white"><i class="material-symbols-outlined">search</i></span>
                             <asp:TextBox ID="txtBuscar" runat="server" CssClass="form-control" Placeholder="Buscar por Cliente, DNI, Factura o Producto..." />
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
                    <div class="col-md-3 d-flex gap-2">
                        <asp:Button ID="btnBuscar" runat="server" Text="Buscar" CssClass="btn btn-primary w-100" OnClick="btnBuscar_Click" />
                        <asp:Button ID="btnRefrescar" runat="server" Text="Limpiar" CssClass="btn btn-outline-secondary" OnClick="btnRefrescar_Click" />
                    </div>
                </div>
            </div>
        </div>

        <asp:Label ID="lblMensaje" runat="server" CssClass="alert" Visible="false" />

        <div class="card shadow-sm border-0 rounded-xl">
            <div class="card-body p-0">
                <asp:GridView ID="gvVentas" runat="server" 
                    CssClass="table table-striped table-hover mb-0"
                    AutoGenerateColumns="False" 
                    DataKeyNames="Id"
                    OnRowCommand="gvVentas_RowCommand"
                    OnRowDataBound="gvVentas_RowDataBound"
                    EmptyDataText="No se encontraron ventas con esos criterios."
                    AllowPaging="True" PageSize="15" OnPageIndexChanging="gvVentas_PageIndexChanging">
                    
                    <Columns>
                        <asp:BoundField DataField="NumeroFactura" HeaderText="N° Factura" ItemStyle-Wrap="false" />
                        <asp:BoundField DataField="Cliente.NombreCompleto" HeaderText="Cliente" />
                        <asp:BoundField DataField="Fecha" HeaderText="Fecha" DataFormatString="{0:g}" ItemStyle-Wrap="false" />
                        <asp:BoundField DataField="TotalVenta" HeaderText="Total" DataFormatString="{0:C}" />
                        
                        <asp:TemplateField HeaderText="Estado Actual">
                            <ItemTemplate>
                                <span class="badge rounded-pill <%# GetEstadoClass(Eval("Estado").ToString()) %>">
                                    <%# Eval("Estado") %>
                                </span>
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="Cambiar Estado">
                            <ItemTemplate>
                                <div class="d-flex gap-2">
                                    <asp:DropDownList ID="ddlEstado" runat="server" CssClass="form-select form-select-sm" style="width: 170px;">
                                        <asp:ListItem Value="En Preparación">En Preparación</asp:ListItem>
                                        <asp:ListItem Value="Listo para Despachar">Listo para Despachar</asp:ListItem>
                                        <asp:ListItem Value="Enviado">Enviado</asp:ListItem>
                                        <asp:ListItem Value="Entregado">Entregado</asp:ListItem>
                                        <asp:ListItem Value="Cancelado">Cancelado</asp:ListItem>
                                    </asp:DropDownList>
                                    <asp:Button ID="btnActualizarEstado" runat="server" Text="OK" 
                                        CssClass="btn btn-secondary btn-sm"
                                        CommandName="ActualizarEstado" 
                                        CommandArgument='<%# Eval("Id") %>' />
                                </div>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <PagerStyle CssClass="pagination-container" />
                </asp:GridView>
            </div>
        </div>
    </div>

</asp:Content>