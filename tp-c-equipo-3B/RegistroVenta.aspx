<%@ Page Title="Registrar Venta" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeBehind="RegistroVenta.aspx.cs" Inherits="tp_c_equipo_3B.RegistroVenta" %>

<asp:Content ID="HeadContent" ContentPlaceHolderID="head" runat="server">
    <link href="https://fonts.googleapis.com/css2?family=Inter:wght@400;500;600;700;900&amp;display=swap" rel="stylesheet"/>
    <link href="https://fonts.googleapis.com/css2?family=Material+Symbols+Outlined" rel="stylesheet"/>
    <style>
        
        :root {
            --bs-primary: #1173d4; 
            --bs-danger: #DC3545; 
            --bs-warning: #FFC107; 
            --bs-background-light: #f6f7f8; 
        }
        
       
        .bg-background-light { background-color: var(--bs-background-light) !important; }
        .text-primary { color: var(--bs-primary) !important; }
        .bg-primary-light { background-color: rgba(17, 115, 212, 0.1); } 
        .hover-bg-primary-light:hover { background-color: rgba(17, 115, 212, 0.2) !important; } 
        
       
        .form-control, .form-select { border-radius: 0.5rem; height: 3.5rem; }
        .btn { border-radius: 0.5rem; }

     
        .material-symbols-outlined {
            font-variation-settings: 'FILL' 0, 'wght' 400, 'GRAD' 0, 'opsz' 24;
        }

      
        .input-warning-icon { 
            position: absolute; 
            right: -1.5rem; 
            top: 50%; 
            transform: translateY(-50%); 
            color: var(--bs-warning);
        }
    </style>
</asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="py-4 py-lg-5 bg-background-light min-vh-100">
        <div class="container-fluid max-w-7xl mx-auto">
            
            <header class="d-flex flex-wrap justify-content-between align-items-center gap-3 mb-5">
                <h1 class="text-dark fw-bolder mb-0" style="font-size: 2.5rem;">Registrar Nueva Venta</h1>
                
                <asp:Button ID="btnCancelar" runat="server" Text="Cancelar" 
                    CssClass="btn btn-secondary fw-bold" style="height: 2.5rem; min-width: 84px;" />
            </header>
            
            <div class="row g-4 g-lg-5">
                <div class="col-lg-8 space-y-5">
                    
                    <section class="card shadow-sm border-0 rounded-xl">
                        <div class="card-body p-4 p-md-5">
                            <h2 class="h4 fw-bold text-dark mb-4">Cliente</h2>
                            <div class="d-flex align-items-start gap-3">
                                <div class="flex-grow-1">
                                    <label class="form-label sr-only" for="customer-search">Buscar por nombre o DNI...</label>
                                    <div class="input-group">
                                        <span class="input-group-text border-end-0 bg-white" style="height: 3.5rem;">
                                            <span class="material-symbols-outlined text-muted">search</span>
                                        </span>
                                        <asp:TextBox ID="txtBuscarCliente" runat="server" CssClass="form-control border-start-0" 
                                            Placeholder="Buscar por nombre o DNI..." />
                                    </div>
                                </div>
                                <button class="flex-shrink-0 btn bg-primary-light hover-bg-primary-light text-primary d-flex align-items-center justify-content-center" 
                                    type="button" style="width: 3.5rem; height: 3.5rem;">
                                    <span class="material-symbols-outlined fs-4">add</span>
                                </button>
                            </div>

                            <div class="mt-4 p-3 border border-dashed border-secondary rounded-lg d-none" id="customer-details">
                                <h3 class="fw-semibold text-dark">Datos del Cliente</h3>
                                <div class="row g-2 mt-2 small">
                                    <div class="col-12 col-sm-4">
                                        <span class="fw-medium text-muted">Nombre:</span> 
                                        <span class="text-dark" id="customer-name">Nombre del Cliente</span>
                                    </div>
                                    <div class="col-12 col-sm-4">
                                        <span class="fw-medium text-muted">DNI/RUC:</span> 
                                        <span class="text-dark" id="customer-id">12345678X</span>
                                    </div>
                                    <div class="col-12 col-sm-4">
                                        <span class="fw-medium text-muted">Teléfono:</span> 
                                        <span class="text-dark" id="customer-phone">+54 9 11 XXXX-XXXX</span>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </section>
                    
                    <section class="card shadow-sm border-0 rounded-xl">
                        <div class="card-body p-4 p-md-5">
                            <h2 class="h4 fw-bold text-dark mb-4">Productos</h2>
                            <div class="d-flex align-items-start gap-3 mb-4">
                                <div class="flex-grow-1">
                                    <label class="form-label sr-only" for="product-search">Buscar por nombre o código...</label>
                                    <div class="input-group">
                                        <span class="input-group-text border-end-0 bg-white" style="height: 3.5rem;">
                                            <span class="material-symbols-outlined text-muted">search</span>
                                        </span>
                                        <asp:TextBox ID="txtBuscarProducto" runat="server" CssClass="form-control border-start-0" 
                                            Placeholder="Buscar por nombre o código..." />
                                    </div>
                                </div>
                                <button class="btn bg-primary-light hover-bg-primary-light text-primary fw-bold flex-shrink-0 px-4" 
                                    type="button" style="height: 3.5rem;">
                                    Agregar
                                </button>
                            </div>

                            <div class="table-responsive">
                                <table class="table table-borderless table-striped mb-0 text-start">
                                    <thead class="border-bottom border-secondary-subtle small text-uppercase">
                                        <tr>
                                            <th class="p-3 fw-semibold text-muted" scope="col">Producto</th>
                                            <th class="p-3 fw-semibold text-muted text-center" scope="col">Stock</th>
                                            <th class="p-3 fw-semibold text-muted text-center" scope="col">Cantidad</th>
                                            <th class="p-3 fw-semibold text-muted text-end" scope="col">Precio Unit.</th>
                                            <th class="p-3 fw-semibold text-muted text-end" scope="col">Precio Total</th>
                                            <th class="p-3 fw-semibold text-muted text-center" scope="col">Acciones</th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        <tr class="border-bottom border-secondary-subtle">
                                            <td class="p-3 text-dark fw-medium">Laptop Gamer Pro</td>
                                            <td class="p-3 text-muted text-center">15</td>
                                            <td class="p-3">
                                                <input class="form-control text-center mx-auto" type="number" value="1" style="width: 80px; height: 2.5rem;"/>
                                            </td>
                                            <td class="p-3 text-dark text-end">$1200.00</td>
                                            <td class="p-3 text-dark fw-bold text-end">$1200.00</td>
                                            <td class="p-3 text-center">
                                                <button class="btn btn-link p-0 text-danger" type="button">
                                                    <span class="material-symbols-outlined small">delete</span>
                                                </button>
                                            </td>
                                        </tr>
                                        <tr class="border-bottom border-secondary-subtle">
                                            <td class="p-3 text-dark fw-medium">Mouse Inalámbrico</td>
                                            <td class="p-3 text-center">
                                                <span class="text-danger fw-bold">5</span>
                                            </td>
                                            <td class="p-3">
                                                <div class="position-relative d-inline-block">
                                                    <input class="form-control text-center mx-auto border-danger" type="number" value="6" style="width: 80px; height: 2.5rem;"/>
                                                    <span class="material-symbols-outlined small input-warning-icon" title="Stock insuficiente">warning</span>
                                                </div>
                                            </td>
                                            <td class="p-3 text-dark text-end">$25.50</td>
                                            <td class="p-3 text-dark fw-bold text-end">$153.00</td>
                                            <td class="p-3 text-center">
                                                <button class="btn btn-link p-0 text-danger" type="button">
                                                    <span class="material-symbols-outlined small">delete</span>
                                                </button>
                                            </td>
                                        </tr>
                                    </tbody>
                                </table>
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
                                        <span class="fw-semibold text-dark">$1353.00</span>
                                    </div>
                                    <div class="d-flex justify-content-between align-items-center text-muted">
                                        <span>Descuento (%)</span>
                                        <input class="form-control text-end" type="number" value="0" style="width: 80px; height: 2.5rem;"/>
                                    </div>
                                    <div class="d-flex justify-content-between align-items-center text-muted">
                                        <span>IVA (%)</span>
                                        <input class="form-control text-end" type="number" value="21" style="width: 80px; height: 2.5rem;"/>
                                    </div>
                                    
                                    <div class="border-top border-secondary-subtle my-3"></div>

                                    <div class="d-flex justify-content-between h5 fw-bold text-dark mb-0">
                                        <span>TOTAL</span>
                                        <span style="font-size: 1.5rem;">$1637.13</span>
                                    </div>
                                </div>
                                
                                <div class="mt-4">
                                    <label class="form-label fw-semibold text-dark" for="payment-method">Método de Pago</label>
                                    <asp:DropDownList ID="ddlMetodoPago" runat="server" CssClass="form-select mt-2" style="height: 3.5rem;">
                                        <asp:ListItem Text="Efectivo" Value="Efectivo" />
                                        <asp:ListItem Text="Tarjeta de Crédito" Value="Tarjeta" />
                                        <asp:ListItem Text="Transferencia Bancaria" Value="Transferencia" />
                                    </asp:DropDownList>
                                </div>
                                
                                <div class="mt-4 d-grid gap-3">
                                    <asp:Button ID="btnGenerarVenta" runat="server" Text="Generar Venta y Factura" 
                                        CssClass="btn btn-primary fw-bold" style="height: 3rem;" />
                                    <asp:Button ID="btnGuardarBorrador" runat="server" Text="Guardar Borrador" 
                                        CssClass="btn btn-secondary fw-bold" style="height: 3rem;" />
                                </div>
                            </div>
                        </section>

                        <section class="card shadow-sm border-0 rounded-xl">
                            <div class="card-body p-4">
                                <h3 class="h6 fw-bold text-dark mb-4">Vista Previa de Factura</h3>
                                <div class="border border-secondary-subtle rounded-lg p-4 bg-white" style="aspect-ratio: 210 / 297; font-size: 0.75rem;">
                                    <div class="d-flex justify-content-between align-items-start pb-3 border-bottom border-secondary-subtle">
                                        <div>
                                            <h4 class="fw-bold fs-6">Mi Negocio</h4>
                                            <p class="mb-0">Calle Falsa 123</p>
                                            <p class="mb-0">Ciudad, País</p>
                                        </div>
                                        <div class="text-end">
                                            <h4 class="fw-bold fs-6">FACTURA</h4>
                                            <p class="mb-0">#FAC-00123</p>
                                            <p class="mb-0">Fecha: 24/07/2024</p>
                                        </div>
                                    </div>
                                    <div class="py-3 border-bottom border-secondary-subtle">
                                        <h4 class="fw-bold mb-1">Cliente:</h4>
                                        <p class="mb-0">Nombre del Cliente</p>
                                        <p class="mb-0">DNI: 12345678X</p>
                                    </div>
                                    <div class="flex-grow-1 py-3">
                                        <table class="w-100">
                                            <thead>
                                                <tr class="border-bottom border-secondary-subtle">
                                                    <th class="text-start py-1">Item</th>
                                                    <th class="text-center py-1">Cant.</th>
                                                    <th class="text-end py-1">P. Unit</th>
                                                    <th class="text-end py-1">Total</th>
                                                </tr>
                                            </thead>
                                            <tbody>
                                                <tr>
                                                    <td class="py-1">Laptop Gamer Pro</td>
                                                    <td class="text-center py-1">1</td>
                                                    <td class="text-end py-1">$1200.00</td>
                                                    <td class="text-end py-1">$1200.00</td>
                                                </tr>
                                                <tr>
                                                    <td class="py-1">Mouse Inalámbrico</td>
                                                    <td class="text-center py-1">6</td>
                                                    <td class="text-end py-1">$25.50</td>
                                                    <td class="text-end py-1">$153.00</td>
                                                </tr>
                                            </tbody>
                                        </table>
                                    </div>
                                    <div class="pt-3 border-top border-secondary-subtle text-end">
                                        <p class="mb-0">Subtotal: <span class="fw-semibold">$1353.00</span></p>
                                        <p class="mb-0">IVA (21%): <span class="fw-semibold">$284.13</span></p>
                                        <p class="fw-bold fs-6 mt-2">TOTAL: <span class="fw-bold">$1637.13</span></p>
                                    </div>
                                </div>
                            </div>
                        </section>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>