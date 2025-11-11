<%@ Page Title="Dashboard - Visión General del Negocio" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeBehind="PaneldeControl.aspx.cs" Inherits="tp_c_equipo_3B.PaneldeControl" %>

<asp:Content ID="HeadContent" ContentPlaceHolderID="head" runat="server">
    <link href="https://fonts.googleapis.com/css2?family=Inter:wght@400;500;700;900&display=swap" rel="stylesheet"/>
    <link href="https://fonts.googleapis.com/css2?family=Material+Symbols+Outlined" rel="stylesheet"/>
    <style>
        
        :root {
            --bs-primary: #1D7A5F; 
            --bs-success: #28a745; 
            --bs-info: #007bff; 
            --bs-warning: #ffc107; 
            --bs-danger: #dc3545; 
            --bs-background-light: #f8f9fa; 
        }

      
        .bg-primary-20 { background-color: rgba(29, 122, 95, 0.2); }
        .bg-success-20 { background-color: rgba(40, 167, 69, 0.2); }
        .bg-info-20 { background-color: rgba(0, 123, 255, 0.2); }
        .bg-gray-500-20 { background-color: rgba(108, 117, 125, 0.2); } 
        
        
        .fw-bolder { font-weight: 900 !important; }
        .rounded-xl { border-radius: 1rem !important; }
        
       
        .material-symbols-outlined {
            font-variation-settings: 'FILL' 0, 'wght' 400, 'GRAD' 0, 'opsz' 24;
        }

       
        .chart-bar { width: 50%; } 
    </style>
</asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="py-4 bg-background-light min-vh-100">
        <div class="container-fluid px-4 sm:px-6 lg:px-10">
            <div class="max-w-7xl mx-auto">
                
                <div class="d-flex flex-wrap justify-content-between align-items-center gap-3 mb-4">
                    <h1 class="text-dark fw-bolder mb-0" style="font-size: 2.5rem;">Visión General del Negocio</h1>
                    
                    <div class="d-flex gap-2 overflow-x-auto pb-2">
                        <button class="btn btn-light border fw-medium d-flex align-items-center gap-2" type="button">
                            <span class="material-symbols-outlined fs-6">today</span>
                            <span class="small">Hoy</span>
                        </button>
                        <button class="btn btn-light border fw-medium small" type="button">Últimos 7 días</button>
                        <button class="btn btn-primary fw-medium small" type="button">Este Mes</button>
                        <button class="btn btn-light border fw-medium small" type="button">Año Actual</button>
                        <button class="btn btn-light border fw-medium d-flex align-items-center gap-1" type="button">
                            <span class="small">Rango</span>
                            <span class="material-symbols-outlined fs-6">arrow_drop_down</span>
                        </button>
                    </div>
                </div>

                <div class="row g-3 g-md-4 mb-4">
                    
                    <div class="col-sm-6 col-lg-3">
                        <div class="card h-100 rounded-xl shadow-sm border">
                            <div class="card-body p-4 d-flex flex-column gap-2">
                                <div class="d-flex align-items-center gap-3">
                                    <div class="size-8 rounded-circle bg-success-20 d-flex justify-content-center align-items-center">
                                        <span class="material-symbols-outlined text-success">trending_up</span>
                                    </div>
                                    <p class="text-dark fw-semibold mb-0">Ingresos Totales</p>
                                </div>
                                <p class="text-dark fw-bolder mb-0" style="font-size: 2rem;">$15,230.50</p>
                                <p class="text-success small fw-semibold mb-0">+5.2% vs. periodo anterior</p>
                            </div>
                        </div>
                    </div>
                    
                    <div class="col-sm-6 col-lg-3">
                        <div class="card h-100 rounded-xl shadow-sm border">
                            <div class="card-body p-4 d-flex flex-column gap-2">
                                <div class="d-flex align-items-center gap-3">
                                    <div class="size-8 rounded-circle bg-info-20 d-flex justify-content-center align-items-center">
                                        <span class="material-symbols-outlined text-info">trending_down</span>
                                    </div>
                                    <p class="text-dark fw-semibold mb-0">Costos Totales</p>
                                </div>
                                <p class="text-dark fw-bolder mb-0" style="font-size: 2rem;">$8,120.00</p>
                                <p class="text-success small fw-semibold mb-0">+3.1% vs. periodo anterior</p>
                            </div>
                        </div>
                    </div>

                    <div class="col-sm-6 col-lg-3">
                        <div class="card h-100 rounded-xl shadow-sm border">
                            <div class="card-body p-4 d-flex flex-column gap-2">
                                <div class="d-flex align-items-center gap-3">
                                    <div class="size-8 rounded-circle bg-primary-20 d-flex justify-content-center align-items-center">
                                        <span class="material-symbols-outlined text-primary">paid</span>
                                    </div>
                                    <p class="text-dark fw-semibold mb-0">Ganancia Neta</p>
                                </div>
                                <p class="text-dark fw-bolder mb-0" style="font-size: 2rem;">$7,110.50</p>
                                <p class="text-success small fw-semibold mb-0">+8.7% vs. periodo anterior</p>
                            </div>
                        </div>
                    </div>

                    <div class="col-sm-6 col-lg-3">
                        <div class="card h-100 rounded-xl shadow-sm border">
                            <div class="card-body p-4 d-flex flex-column gap-2">
                                <div class="d-flex align-items-center gap-3">
                                    <div class="size-8 rounded-circle bg-gray-500-20 d-flex justify-content-center align-items-center">
                                        <span class="material-symbols-outlined text-secondary">receipt_long</span>
                                    </div>
                                    <p class="text-dark fw-semibold mb-0">Transacciones</p>
                                </div>
                                <p class="text-dark fw-bolder mb-0" style="font-size: 2rem;">432</p>
                                <p class="text-danger small fw-semibold mb-0">-1.2% vs. periodo anterior</p>
                            </div>
                        </div>
                    </div>
                </div>

                <div class="row g-4">
                    
                    <div class="col-lg-8">
                        <div class="card h-100 rounded-xl shadow-sm border">
                            <div class="card-body p-4 d-flex flex-column gap-3">
                                <p class="text-dark h5 fw-bold mb-0">Ventas vs. Compras</p>
                                
                                <div style="height: 20rem;" class="position-relative">
                                    <div class="position-absolute w-100 h-100 d-flex flex-column justify-content-between">
                                        <div class="border-bottom border-dashed border-secondary-subtle"></div>
                                        <div class="border-bottom border-dashed border-secondary-subtle"></div>
                                        <div class="border-bottom border-dashed border-secondary-subtle"></div>
                                        <div class="border-bottom border-dashed border-secondary-subtle"></div>
                                        <div class="border-bottom border-secondary-subtle"></div>
                                    </div>
                                    
                                    <div class="position-absolute w-100 h-100 d-flex align-items-end justify-content-around px-4 gap-4">
                                        
                                        <div class="d-flex flex-column align-items-center w-100">
                                            <div class="d-flex align-items-end w-100 justify-content-center gap-1">
                                                <div class="bg-success rounded-top chart-bar" style="height: 60%;"></div>
                                                <div class="bg-info rounded-top chart-bar" style="height: 40%;"></div>
                                            </div>
                                            <p class="text-muted small fw-medium mt-2">Semana 1</p>
                                        </div>

                                        <div class="d-flex flex-column align-items-center w-100">
                                            <div class="d-flex align-items-end w-100 justify-content-center gap-1">
                                                <div class="bg-success rounded-top chart-bar" style="height: 75%;"></div>
                                                <div class="bg-info rounded-top chart-bar" style="height: 50%;"></div>
                                            </div>
                                            <p class="text-muted small fw-medium mt-2">Semana 2</p>
                                        </div>

                                        <div class="d-flex flex-column align-items-center w-100">
                                            <div class="d-flex align-items-end w-100 justify-content-center gap-1">
                                                <div class="bg-success rounded-top chart-bar" style="height: 50%;"></div>
                                                <div class="bg-info rounded-top chart-bar" style="height: 60%;"></div>
                                            </div>
                                            <p class="text-muted small fw-medium mt-2">Semana 3</p>
                                        </div>

                                        <div class="d-flex flex-column align-items-center w-100">
                                            <div class="d-flex align-items-end w-100 justify-content-center gap-1">
                                                <div class="bg-success rounded-top chart-bar" style="height: 85%;"></div>
                                                <div class="bg-info rounded-top chart-bar" style="height: 65%;"></div>
                                            </div>
                                            <p class="text-muted small fw-medium mt-2">Semana 4</p>
                                        </div>
                                    </div>
                                </div>
                                
                                <div class="d-flex justify-content-center gap-4 small mt-3">
                                    <div class="d-flex align-items-center gap-2">
                                        <div class="size-3 rounded-sm bg-success"></div>
                                        <span>Ventas</span>
                                    </div>
                                    <div class="d-flex align-items-center gap-2">
                                        <div class="size-3 rounded-sm bg-info"></div>
                                        <span>Compras</span>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                    
                    <div class="col-lg-4">
                        <div class="card h-100 rounded-xl shadow-sm border">
                            <div class="card-body p-4 d-flex flex-column gap-4">
                                <p class="text-dark h5 fw-bold mb-0">Productos con Bajo Stock</p>
                                
                                <div class="space-y-3">
                                    <div class="d-flex items-center justify-content-between">
                                        <div>
                                            <p class="fw-semibold text-dark mb-0">Café Grano Molido 1kg</p>
                                            <p class="small text-muted mb-0">SKU: CF-001</p>
                                        </div>
                                        <div class="text-end">
                                            <p class="fw-bold text-danger mb-0">5 Unidades</p>
                                            <a class="small text-primary text-decoration-none hover-underline" href="#">Realizar Pedido</a>
                                        </div>
                                    </div>
                                    <div class="d-flex items-center justify-content-between">
                                        <div>
                                            <p class="fw-semibold text-dark mb-0">Té Verde Orgánico 50u</p>
                                            <p class="small text-muted mb-0">SKU: TE-012</p>
                                        </div>
                                        <div class="text-end">
                                            <p class="fw-bold text-danger mb-0">8 Unidades</p>
                                            <a class="small text-primary text-decoration-none hover-underline" href="#">Realizar Pedido</a>
                                        </div>
                                    </div>
                                    <div class="d-flex items-center justify-content-between">
                                        <div>
                                            <p class="fw-semibold text-dark mb-0">Leche de Almendras 1L</p>
                                            <p class="small text-muted mb-0">SKU: LA-003</p>
                                        </div>
                                        <div class="text-end">
                                            <p class="fw-bold text-warning mb-0">12 Unidades</p>
                                            <a class="small text-primary text-decoration-none hover-underline" href="#">Ver Producto</a>
                                        </div>
                                    </div>
                                    <div class="d-flex items-center justify-content-between">
                                        <div>
                                            <p class="fw-semibold text-dark mb-0">Chocolate Amargo 70%</p>
                                            <p class="small text-muted mb-0">SKU: CH-005</p>
                                        </div>
                                        <div class="text-end">
                                            <p class="fw-bold text-warning mb-0">15 Unidades</p>
                                            <a class="small text-primary text-decoration-none hover-underline" href="#">Ver Producto</a>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>

                <div class="mt-4">
                    <div class="card rounded-xl shadow-sm border">
                        <div class="card-body p-4">
                            <p class="text-dark h5 fw-bold mb-4">Actividad Reciente</p>
                            <ul class="list-unstyled space-y-4">
                                <li class="d-flex align-items-start gap-3">
                                    <div class="mt-1 size-8 rounded-circle bg-success-20 d-flex justify-content-center align-items-center flex-shrink-0">
                                        <span class="material-symbols-outlined small text-success">shopping_cart</span>
                                    </div>
                                    <div>
                                        <p class="fw-semibold text-dark mb-0">
                                            Venta <a class="text-primary text-decoration-none" href="#">#1023</a> realizada por $250.00
                                        </p>
                                        <p class="small text-muted mb-0">Hace 15 minutos</p>
                                    </div>
                                </li>
                                <li class="d-flex align-items-start gap-3">
                                    <div class="mt-1 size-8 rounded-circle bg-info-20 d-flex justify-content-center align-items-center flex-shrink-0">
                                        <span class="material-symbols-outlined small text-info">local_shipping</span>
                                    </div>
                                    <div>
                                        <p class="fw-semibold text-dark mb-0">
                                            Compra <a class="text-primary text-decoration-none" href="#">#C58</a> recibida de 'Proveedor XYZ'
                                        </p>
                                        <p class="small text-muted mb-0">Hace 1 hora</p>
                                    </div>
                                </li>
                                <li class="d-flex align-items-start gap-3">
                                    <div class="mt-1 size-8 rounded-circle bg-gray-500-20 d-flex justify-content-center align-items-center flex-shrink-0">
                                        <span class="material-symbols-outlined small text-secondary">add_box</span>
                                    </div>
                                    <div>
                                        <p class="fw-semibold text-dark mb-0">
                                            Nuevo producto 'Azúcar Morena 500g' agregado al inventario
                                        </p>
                                        <p class="small text-muted mb-0">Hace 3 horas</p>
                                    </div>
                                </li>
                                <li class="d-flex align-items-start gap-3">
                                    <div class="mt-1 size-8 rounded-circle bg-success-20 d-flex justify-content-center align-items-center flex-shrink-0">
                                        <span class="material-symbols-outlined small text-success">shopping_cart</span>
                                    </div>
                                    <div>
                                        <p class="fw-semibold text-dark mb-0">
                                            Venta <a class="text-primary text-decoration-none" href="#">#1022</a> realizada por $75.50
                                        </p>
                                        <p class="small text-muted mb-0">Hace 5 horas</p>
                                    </div>
                                </li>
                            </ul>
                        </div>
                    </div>
                </div>

            </div>
        </div>
    </div>
</asp:Content>