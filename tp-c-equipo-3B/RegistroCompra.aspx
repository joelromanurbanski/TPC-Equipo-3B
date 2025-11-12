<%@ Page Title="Registrar Compra" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeBehind="RegistroCompra.aspx.cs" Inherits="tp_c_equipo_3B.RegistroCompra" %>

<asp:Content ID="HeadContent" ContentPlaceHolderID="head" runat="server">
    <link href="https://fonts.googleapis.com/css2?family=Inter:wght@100..900&amp;display=swap" rel="stylesheet"/>
    <link href="https://fonts.googleapis.com/css2?family=Material+Symbols+Outlined" rel="stylesheet"/>
    <style>
        
        :root {
            --bs-primary: #2A52BE; 
            --bs-background-light: #F0F2F5; 
            --bs-gray-700: #495057; 
        }
        
      
        .bg-background-light { background-color: var(--bs-background-light) !important; }
        .text-primary { color: var(--bs-primary) !important; }
        .bg-primary-light { background-color: rgba(42, 82, 190, 0.2); } 
        .hover-bg-primary-light:hover { background-color: rgba(42, 82, 190, 0.3) !important; } 
        
       
        .form-control, .form-select, .btn { border-radius: 0.5rem; }
        .form-control:not(.form-control-sm), .form-select:not(.form-select-sm), .btn:not(.btn-sm) { height: 3rem; }

        
        .material-symbols-outlined {
            font-variation-settings: 'FILL' 0, 'wght' 400, 'GRAD' 0, 'opsz' 24;
        }

      
       
        .card.dark { background-color: #343A40; color: white; }
        .card.dark .text-dark { color: white !important; }
        .card.dark .bg-light { background-color: #495057 !important; }
    </style>
</asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="py-5 bg-background-light min-vh-100"> 
        <div class="container my-4">
            <header class="mb-5">
                <h1 class="text-dark fw-bolder mb-1" style="font-size: 2.5rem;">Registrar nueva compra</h1>
                <p class="text-muted small mt-2">Completa los siguientes campos para registrar una nueva compra de inventario.</p>
            </header>

            <div class="row g-4">
                <div class="col-lg-8">
                    <div class="card shadow-sm border-0 rounded-xl"> <div class="card-body p-4 p-md-5">
                            <h2 class="card-title h5 fw-bold mb-4 text-dark">Detalles de la Compra</h2>
                            <div class="row g-4">
                                <div class="col-12 col-md-6">
                                    <label class="form-label fw-semibold mb-2">Proveedor</label>
                                    <asp:DropDownList ID="ddlProveedor" runat="server" CssClass="form-select">
                                        <asp:ListItem Text="Seleccionar proveedor" Value="" />
                                        <asp:ListItem Text="Proveedor A" Value="A" />
                                        <asp:ListItem Text="Proveedor B" Value="B" />
                                    </asp:DropDownList>
                                </div>
                                
                                <div class="col-12 col-md-6 d-flex align-items-end">
                                    <button class="btn btn-sm bg-primary-light text-primary fw-bold hover-bg-primary-light d-flex align-items-center justify-content-center w-100 h-100" type="button">
                                        <span class="material-symbols-outlined me-2">add</span>
                                        <span>Nuevo Proveedor</span>
                                    </button>
                                </div>
                                
                                <div class="col-12 col-md-6">
                                    <label class="form-label fw-semibold mb-2">Número de factura</label>
                                    <asp:TextBox ID="txtFactura" runat="server" CssClass="form-control" Placeholder="Ingresar número de factura" />
                                </div>
                                
                                <div class="col-12 col-md-6">
                                    <label class="form-label fw-semibold mb-2">Fecha de compra</label>
                                    <asp:TextBox ID="txtFecha" runat="server" CssClass="form-control" TextMode="Date" />
                                </div>
                            </div>

                            <hr class="my-5 border-secondary" />

                            <h3 class="h6 fw-bold mb-3 text-dark">Productos</h3>
                            <div class="d-flex align-items-end gap-3 mb-4">
                                <div class="flex-grow-1">
                                    <label class="form-label small fw-semibold mb-2">Buscar producto</label>
                                    <div class="input-group">
                                        <span class="input-group-text border-end-0 bg-white">
                                            <span class="material-symbols-outlined text-muted">search</span>
                                        </span>
                                        <input type="text" class="form-control border-start-0" placeholder="Buscar por nombre o código..." />
                                    </div>
                                </div>
                                <button class="btn btn-success fw-bold flex-shrink-0 px-4" type="button" style="min-width: 84px;">
                                    <span>Agregar</span>
                                </button>
                            </div>

                            <div class="table-responsive border rounded-lg">
                                <table class="table table-striped table-hover mb-0">
                                    <thead class="bg-light small text-uppercase">
                                        <tr>
                                            <th class="px-4 py-3" scope="col">Producto</th>
                                            <th class="px-4 py-3" scope="col">Cantidad</th>
                                            <th class="px-4 py-3" scope="col">Precio Unit.</th>
                                            <th class="px-4 py-3" scope="col">Subtotal</th>
                                            <th class="px-4 py-3 text-end" scope="col"></th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        <tr>
                                            <th class="px-4 py-3 fw-normal text-nowrap" scope="row">Laptop Pro 15"</th>
                                            <td class="px-4 py-3">
                                                <input class="form-control form-control-sm w-auto" type="number" value="2" style="width: 80px; height: 32px;" />
                                            </td>
                                            <td class="px-4 py-3">$1200.00</td>
                                            <td class="px-4 py-3">$2400.00</td>
                                            <td class="px-4 py-3 text-end">
                                                <button class="btn btn-link p-0 text-danger" type="button">
                                                    <span class="material-symbols-outlined small">delete</span>
                                                </button>
                                            </td>
                                        </tr>
                                        <tr>
                                            <th class="px-4 py-3 fw-normal text-nowrap" scope="row">Mouse Inalámbrico</th>
                                            <td class="px-4 py-3">
                                                <input class="form-control form-control-sm w-auto" type="number" value="5" style="width: 80px; height: 32px;" />
                                            </td>
                                            <td class="px-4 py-3">$25.00</td>
                                            <td class="px-4 py-3">$125.00</td>
                                            <td class="px-4 py-3 text-end">
                                                <button class="btn btn-link p-0 text-danger" type="button">
                                                    <span class="material-symbols-outlined small">delete</span>
                                                </button>
                                            </td>
                                        </tr>
                                    </tbody>
                                </table>
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
                                    <p class="fw-semibold mb-0 text-dark">$2525.00</p>
                                </div>
                                <div class="d-flex justify-content-between align-items-center">
                                    <p class="text-muted mb-0">Impuestos (18%)</p>
                                    <p class="fw-semibold mb-0 text-dark">$454.50</p>
                                </div>
                                <div class="d-flex justify-content-between align-items-center">
                                    <p class="text-muted mb-0">Otros Costos</p>
                                    <input class="form-control text-end" type="number" value="0.00" style="width: 100px; height: 3rem;" />
                                </div>

                                <div class="border-top my-3"></div>

                                <div class="d-flex justify-content-between h5 fw-bold mb-0">
                                    <p class="text-dark mb-0">Total</p>
                                    <p class="text-primary mb-0">$2979.50</p>
                                </div>
                            </div>

                            <div class="mt-4 d-grid gap-3">
                                <asp:Button ID="btnGuardarCompra" runat="server" Text="Guardar Compra" 
                                    CssClass="btn btn-primary d-flex align-items-center justify-content-center fw-bold h-12" />
                                <asp:Button ID="btnCancelarCompra" runat="server" Text="Cancelar" 
                                    CssClass="btn btn-secondary d-flex align-items-center justify-content-center fw-bold h-12" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>