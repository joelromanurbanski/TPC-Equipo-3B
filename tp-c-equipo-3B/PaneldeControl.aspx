<%@ Page Title="Dashboard - Visión General del Negocio" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeBehind="PaneldeControl.aspx.cs" Inherits="tp_c_equipo_3B.PaneldeControl" %>

<asp:Content ID="HeadContent" ContentPlaceHolderID="head" runat="server">
    <link href="https://fonts.googleapis.com/css2?family=Inter:wght@400;500;700;900&display=swap" rel="stylesheet"/>
    <link href="https://fonts.googleapis.com/css2?family=Material+Symbols+Outlined" rel="stylesheet"/>
    <style>
        :root { --bs-primary: #1D7A5F; --bs-success: #28a745; --bs-info: #007bff; --bs-warning: #ffc107; --bs-danger: #dc3545; --bs-background-light: #f8f9fa; }
        .bg-primary-20 { background-color: rgba(29, 122, 95, 0.2); }
        .bg-success-20 { background-color: rgba(40, 167, 69, 0.2); }
        .bg-info-20 { background-color: rgba(0, 123, 255, 0.2); }
        .bg-gray-500-20 { background-color: rgba(108, 117, 125, 0.2); } 
        .fw-bolder { font-weight: 900 !important; }
        .rounded-xl { border-radius: 1rem !important; }
        .material-symbols-outlined { font-variation-settings: 'FILL' 0, 'wght' 400, 'GRAD' 0, 'opsz' 24; }
        .chart-bar { width: 50%; } 
    </style>
</asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="py-4 bg-background-light min-vh-100">
        <div class="container-fluid px-4 sm:px-6 lg:px-10">
            <div class="max-w-7xl mx-auto">
                
                <div class="d-flex flex-wrap justify-content-between align-items-center gap-3 mb-4">
                    <h1 class="text-dark fw-bolder mb-0" style="font-size: 2.5rem;">Visión General del Negocio</h1>
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
                                <asp:Label ID="litIngresosTotales" runat="server" Text="$0.00" CssClass="text-dark fw-bolder mb-0" style="font-size: 2rem;" />
                                <asp:Label ID="litIngresosStats" runat="server" Text="--" CssClass="text-success small fw-semibold mb-0" />
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
                                <asp:Label ID="litCostosTotales" runat="server" Text="$0.00" CssClass="text-dark fw-bolder mb-0" style="font-size: 2rem;" />
                                <asp:Label ID="litCostosStats" runat="server" Text="--" CssClass="text-success small fw-semibold mb-0" />
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
                                <asp:Label ID="litGananciaNeta" runat="server" Text="$0.00" CssClass="text-dark fw-bolder mb-0" style="font-size: 2rem;" />
                                <asp:Label ID="litGananciaStats" runat="server" Text="--" CssClass="text-success small fw-semibold mb-0" />
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
                                <asp:Label ID="litTransacciones" runat="server" Text="0" CssClass="text-dark fw-bolder mb-0" style="font-size: 2rem;" />
                                <asp:Label ID="litTransaccionesStats" runat="server" Text="--" CssClass="text-danger small fw-semibold mb-0" />
                            </div>
                        </div>
                    </div>
                </div>

                <div class="row g-4">
                    
                    <div class="col-lg-8">
                        <div class="card h-100 rounded-xl shadow-sm border">
                            <div class="card-body p-4 d-flex flex-column gap-3">
                                <p class="text-dark h5 fw-bold mb-0">Ventas vs. Compras (Este Mes)</p>
                                <div style="height: 20rem;" class="position-relative">
                                    <canvas id="salesChart"></canvas>
                                </div>
                                <div class="d-flex justify-content-center gap-4 small mt-3">
                                    <div class="d-flex align-items-center gap-2">
                                        <div class="size-3 rounded-sm" style="background-color: #28a745;"></div>
                                        <span>Ventas</span>
                                    </div>
                                    <div class="d-flex align-items-center gap-2">
                                        <div class="size-3 rounded-sm" style="background-color: #007bff;"></div>
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
                                    <asp:Repeater ID="rptBajoStock" runat="server">
                                        <ItemTemplate>
                                            <div class="d-flex items-center justify-content-between">
                                                <div>
                                                    <p class="fw-semibold text-dark mb-0"><%# Eval("Nombre") %></p>
                                                    <p class="small text-muted mb-0">SKU: <%# Eval("Codigo") %></p>
                                                </div>
                                                <div class="text-end">
                                                    <p class="fw-bold text-danger mb-0"><%# Eval("StockActual") %> Unidades</p>
                                                    <a class="small text-primary text-decoration-none hover-underline" href='GestionProducto.aspx'>Ver Producto</a>
                                                </div>
                                            </div>
                                        </ItemTemplate>
                                        <SeparatorTemplate>
                                            <hr class="my-2" />
                                        </SeparatorTemplate>
                                    </asp:Repeater>
                                    <asp:Panel ID="pnlNoBajoStock" runat="server" Visible="false" CssClass="text-center text-muted p-3">
                                        ¡Sin problemas de stock!
                                    </asp:Panel>
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
                                <asp:Repeater ID="rptActividadReciente" runat="server">
                                    <ItemTemplate>
                                        <li class="d-flex align-items-start gap-3">
                                            <div class="mt-1 size-8 rounded-circle <%# GetIconBgClass(Eval("Tipo").ToString()) %> d-flex justify-content-center align-items-center flex-shrink-0">
                                                <span class="material-symbols-outlined small <%# GetIconClass(Eval("Tipo").ToString()) %>">
                                                    <%# GetIconName(Eval("Tipo").ToString()) %>
                                                </span>
                                            </div>
                                            <div>
                                                <p class="fw-semibold text-dark mb-0">
                                                    <%# GetActivityText(Eval("Tipo").ToString(), Eval("Id"), Eval("Monto"), Eval("NombreClienteOProveedor")) %>
                                                </p>
                                                <p class="small text-muted mb-0"><%# Eval("Fecha", "{0:g}") %></p>
                                            </div>
                                        </li>
                                    </ItemTemplate>
                                </asp:Repeater>
                                <asp:Panel ID="pnlNoActividad" runat="server" Visible="false" CssClass="text-center text-muted p-3">
                                    No hay actividad reciente.
                                </asp:Panel>
                            </ul>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
    
    <script type="text/javascript">
        function initializeSalesChart(chartLabels, chartVentasData, chartComprasData) {
            if (typeof Chart === 'undefined') {
                console.error('Chart.js no está cargado. Asegúrate de incluirlo en tu Site.master.');
                return;
            }
            try {
                var ctx = document.getElementById('salesChart').getContext('2d');
                var myChart = new Chart(ctx, {
                    type: 'bar',
                    data: {
                        labels: chartLabels,
                        datasets: [
                            {
                                label: 'Ventas',
                                data: chartVentasData,
                                backgroundColor: '#28a745',
                                borderRadius: 4
                            },
                            {
                                label: 'Compras',
                                data: chartComprasData,
                                backgroundColor: '#007bff',
                                borderRadius: 4
                            }
                        ]
                    },
                    options: {
                        responsive: true,
                        maintainAspectRatio: false,
                        scales: {
                            y: {
                                beginAtZero: true,
                                ticks: {
                                    callback: function(value, index, values) {
                                        return '$' + value.toLocaleString('es-AR');
                                    }
                                }
                            }
                        },
                        plugins: {
                            tooltip: {
                                callbacks: {
                                    label: function(context) {
                                        let label = context.dataset.label || '';
                                        if (label) { label += ': '; }
                                        if (context.parsed.y !== null) {
                                            label += '$' + context.parsed.y.toLocaleString('es-AR');
                                        }
                                        return label;
                                    }
                                }
                            }
                        }
                    }
                });
            } catch (e) {
                console.error("Error al cargar el gráfico:", e);
                document.getElementById('salesChart').parentElement.innerHTML = '<div class="text-center text-muted">Error al cargar el gráfico.</div>';
            }
        }
    </script>
</asp:Content>