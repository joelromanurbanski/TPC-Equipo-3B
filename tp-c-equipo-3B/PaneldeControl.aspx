<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PaneldeControl.aspx.cs" Inherits="tp_c_equipo_3B.PaneldeControl" %>

<!DOCTYPE html>

<html class="light" lang="es"><head>
<meta charset="utf-8"/>
<meta content="width=device-width, initial-scale=1.0" name="viewport"/>
<title>Dashboard - Visión General del Negocio</title>
<script src="https://cdn.tailwindcss.com?plugins=forms,container-queries"></script>
<link href="https://fonts.googleapis.com/css2?family=Inter:wght@400;500;700;900&amp;display=swap" rel="stylesheet"/>
<link href="https://fonts.googleapis.com/css2?family=Material+Symbols+Outlined" rel="stylesheet"/>
<script>
        tailwind.config = {
            darkMode: "class",
            theme: {
                extend: {
                    colors: {
                        "primary": "#1D7A5F",
                        "background-light": "#f8f9fa",
                        "background-dark": "#101922",
                        "success": "#28a745",
                        "info": "#007bff",
                        "warning": "#ffc107",
                        "danger": "#dc3545"
                    },
                    fontFamily: {
                        "display": ["Inter", "sans-serif"]
                    },
                    borderRadius: {
                        "DEFAULT": "0.5rem",
                        "lg": "0.75rem",
                        "xl": "1rem",
                        "full": "9999px"
                    },
                },
            },
        }
</script>
<style>
        .material-symbols-outlined {
            font-variation-settings:
            'FILL' 0,
            'wght' 400,
            'GRAD' 0,
            'opsz' 24
        }
    </style>
</head>
<body class="bg-background-light dark:bg-background-dark font-display text-[#111418] dark:text-gray-200">
<div class="relative flex h-auto min-h-screen w-full flex-col group/design-root overflow-x-hidden">
<div class="layout-container flex h-full grow flex-col">
<header class="flex items-center justify-between whitespace-nowrap border-b border-solid border-gray-200 dark:border-gray-700 px-6 md:px-10 py-3 bg-white dark:bg-background-dark">
<div class="flex items-center gap-8">
<div class="flex items-center gap-3 text-primary">
<span class="material-symbols-outlined text-3xl">store</span>
<h2 class="text-[#111418] dark:text-white text-xl font-bold leading-tight tracking-[-0.015em]">BusinessCo</h2>
</div>
<div class="hidden md:flex items-center gap-8">
<a class="text-[#111418] dark:text-gray-300 text-sm font-medium leading-normal" href="#">Ventas</a>
<a class="text-[#111418] dark:text-gray-300 text-sm font-medium leading-normal" href="#">Compras</a>
<a class="text-[#111418] dark:text-gray-300 text-sm font-medium leading-normal" href="#">Productos</a>
<a class="text-[#111418] dark:text-gray-300 text-sm font-medium leading-normal" href="#">Reportes</a>
</div>
</div>
<div class="flex flex-1 justify-end gap-4">
<button class="flex items-center justify-center overflow-hidden rounded-full h-10 w-10 bg-background-light dark:bg-gray-700/50 text-[#111418] dark:text-gray-300">
<span class="material-symbols-outlined">notifications</span>
</button>
<div class="bg-center bg-no-repeat aspect-square bg-cover rounded-full size-10" data-alt="User profile picture" style='background-image: url("https://lh3.googleusercontent.com/aida-public/AB6AXuDNiiq308-PQxdtjwhv4PTWViYFp2y9LMDsUgy-FUZAJ5LfTUfrHu10Qh7vulCX2hEL8NIkrXZe1t02RXW10D2ZPD6WVfUisIHTJ1yrW8eZMs9PsbHPy7oytwmL5R0fTzUVHWrox3q1YrvdHlVVEzZBsLwn6dYinw85UdUxM8v9fRNgqageBIGCW7nz6aTLQtI1N4po5Kwm_tdNX7bGjolpdfV369XJ98yoeUQMcBd1K7uFhp5LWIP-3myGa2rZQJmsS9cDNjU9gg1i");'></div>
</div>
</header>
<main class="flex-1 px-4 sm:px-6 lg:px-10 py-8">
<div class="max-w-7xl mx-auto">
<div class="flex flex-wrap justify-between items-center gap-4 mb-6">
<p class="text-[#111418] dark:text-white text-3xl md:text-4xl font-black leading-tight tracking-[-0.033em]">Visión General del Negocio</p>
<div class="flex gap-2 overflow-x-auto pb-2">
<button class="flex h-9 shrink-0 items-center justify-center gap-x-2 rounded-lg bg-white dark:bg-gray-800 border border-gray-200 dark:border-gray-700 px-3">
<span class="material-symbols-outlined text-base">today</span>
<p class="text-[#111418] dark:text-gray-300 text-sm font-medium leading-normal">Hoy</p>
</button>
<button class="flex h-9 shrink-0 items-center justify-center gap-x-2 rounded-lg bg-white dark:bg-gray-800 border border-gray-200 dark:border-gray-700 px-3">
<p class="text-[#111418] dark:text-gray-300 text-sm font-medium leading-normal">Últimos 7 días</p>
</button>
<button class="flex h-9 shrink-0 items-center justify-center gap-x-2 rounded-lg bg-primary text-white px-3">
<p class="text-sm font-medium leading-normal">Este Mes</p>
</button>
<button class="flex h-9 shrink-0 items-center justify-center gap-x-2 rounded-lg bg-white dark:bg-gray-800 border border-gray-200 dark:border-gray-700 px-3">
<p class="text-[#111418] dark:text-gray-300 text-sm font-medium leading-normal">Año Actual</p>
</button>
<button class="flex h-9 shrink-0 items-center justify-center gap-x-2 rounded-lg bg-white dark:bg-gray-800 border border-gray-200 dark:border-gray-700 pl-3 pr-2">
<p class="text-[#111418] dark:text-gray-300 text-sm font-medium leading-normal">Rango</p>
<span class="material-symbols-outlined text-base">arrow_drop_down</span>
</button>
</div>
</div>
<div class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-6 mb-8">
<div class="flex flex-col gap-2 rounded-xl p-6 bg-white dark:bg-gray-800 border border-gray-200 dark:border-gray-700">
<div class="flex items-center gap-3">
<div class="w-8 h-8 rounded-full bg-success/20 flex items-center justify-center"><span class="material-symbols-outlined text-success">trending_up</span></div>
<p class="text-[#111418] dark:text-gray-300 text-base font-medium leading-normal">Ingresos Totales</p>
</div>
<p class="text-[#111418] dark:text-white tracking-tight text-3xl font-bold leading-tight">$15,230.50</p>
<p class="text-success text-sm font-medium leading-normal">+5.2% vs. periodo anterior</p>
</div>
<div class="flex flex-col gap-2 rounded-xl p-6 bg-white dark:bg-gray-800 border border-gray-200 dark:border-gray-700">
<div class="flex items-center gap-3">
<div class="w-8 h-8 rounded-full bg-info/20 flex items-center justify-center"><span class="material-symbols-outlined text-info">trending_down</span></div>
<p class="text-[#111418] dark:text-gray-300 text-base font-medium leading-normal">Costos Totales</p>
</div>
<p class="text-[#111418] dark:text-white tracking-tight text-3xl font-bold leading-tight">$8,120.00</p>
<p class="text-success text-sm font-medium leading-normal">+3.1% vs. periodo anterior</p>
</div>
<div class="flex flex-col gap-2 rounded-xl p-6 bg-white dark:bg-gray-800 border border-gray-200 dark:border-gray-700">
<div class="flex items-center gap-3">
<div class="w-8 h-8 rounded-full bg-primary/20 flex items-center justify-center"><span class="material-symbols-outlined text-primary">paid</span></div>
<p class="text-[#111418] dark:text-gray-300 text-base font-medium leading-normal">Ganancia Neta</p>
</div>
<p class="text-[#111418] dark:text-white tracking-tight text-3xl font-bold leading-tight">$7,110.50</p>
<p class="text-success text-sm font-medium leading-normal">+8.7% vs. periodo anterior</p>
</div>
<div class="flex flex-col gap-2 rounded-xl p-6 bg-white dark:bg-gray-800 border border-gray-200 dark:border-gray-700">
<div class="flex items-center gap-3">
<div class="w-8 h-8 rounded-full bg-gray-500/20 flex items-center justify-center"><span class="material-symbols-outlined text-gray-500 dark:text-gray-400">receipt_long</span></div>
<p class="text-[#111418] dark:text-gray-300 text-base font-medium leading-normal">Transacciones</p>
</div>
<p class="text-[#111418] dark:text-white tracking-tight text-3xl font-bold leading-tight">432</p>
<p class="text-danger text-sm font-medium leading-normal">-1.2% vs. periodo anterior</p>
</div>
</div>
<div class="grid grid-cols-1 lg:grid-cols-3 gap-6">
<div class="lg:col-span-2 flex flex-col gap-4 rounded-xl border border-gray-200 dark:border-gray-700 p-6 bg-white dark:bg-gray-800">
<p class="text-[#111418] dark:text-white text-lg font-bold leading-normal">Ventas vs. Compras</p>
<div class="h-80 relative">
<div class="absolute inset-0 grid grid-rows-5">
<div class="border-b border-dashed border-gray-200 dark:border-gray-700"></div>
<div class="border-b border-dashed border-gray-200 dark:border-gray-700"></div>
<div class="border-b border-dashed border-gray-200 dark:border-gray-700"></div>
<div class="border-b border-dashed border-gray-200 dark:border-gray-700"></div>
<div class="border-b border-gray-200 dark:border-gray-700"></div>
</div>
<div class="absolute inset-0 px-4 grid grid-cols-4 gap-4 items-end">
<div class="flex flex-col gap-2 items-center">
<div class="flex items-end w-full gap-1 justify-center">
<div class="bg-success w-1/2 rounded-t-md" style="height: 60%;"></div>
<div class="bg-info w-1/2 rounded-t-md" style="height: 40%;"></div>
</div>
<p class="text-gray-500 dark:text-gray-400 text-xs font-medium">Semana 1</p>
</div>
<div class="flex flex-col gap-2 items-center">
<div class="flex items-end w-full gap-1 justify-center">
<div class="bg-success w-1/2 rounded-t-md" style="height: 75%;"></div>
<div class="bg-info w-1/2 rounded-t-md" style="height: 50%;"></div>
</div>
<p class="text-gray-500 dark:text-gray-400 text-xs font-medium">Semana 2</p>
</div>
<div class="flex flex-col gap-2 items-center">
<div class="flex items-end w-full gap-1 justify-center">
<div class="bg-success w-1/2 rounded-t-md" style="height: 50%;"></div>
<div class="bg-info w-1/2 rounded-t-md" style="height: 60%;"></div>
</div>
<p class="text-gray-500 dark:text-gray-400 text-xs font-medium">Semana 3</p>
</div>
<div class="flex flex-col gap-2 items-center">
<div class="flex items-end w-full gap-1 justify-center">
<div class="bg-success w-1/2 rounded-t-md" style="height: 85%;"></div>
<div class="bg-info w-1/2 rounded-t-md" style="height: 65%;"></div>
</div>
<p class="text-gray-500 dark:text-gray-400 text-xs font-medium">Semana 4</p>
</div>
</div>
</div>
<div class="flex justify-center gap-6 text-sm">
<div class="flex items-center gap-2"><div class="w-3 h-3 rounded-sm bg-success"></div><span>Ventas</span></div>
<div class="flex items-center gap-2"><div class="w-3 h-3 rounded-sm bg-info"></div><span>Compras</span></div>
</div>
</div>
<div class="flex flex-col gap-4 rounded-xl border border-gray-200 dark:border-gray-700 p-6 bg-white dark:bg-gray-800">
<p class="text-[#111418] dark:text-white text-lg font-bold leading-normal">Productos con Bajo Stock</p>
<div class="space-y-4">
<div class="flex items-center justify-between">
<div>
<p class="font-medium text-[#111418] dark:text-white">Café Grano Molido 1kg</p>
<p class="text-sm text-gray-500 dark:text-gray-400">SKU: CF-001</p>
</div>
<div class="text-right">
<p class="font-bold text-danger">5 Unidades</p>
<a class="text-sm text-primary hover:underline" href="#">Realizar Pedido</a>
</div>
</div>
<div class="flex items-center justify-between">
<div>
<p class="font-medium text-[#111418] dark:text-white">Té Verde Orgánico 50u</p>
<p class="text-sm text-gray-500 dark:text-gray-400">SKU: TE-012</p>
</div>
<div class="text-right">
<p class="font-bold text-danger">8 Unidades</p>
<a class="text-sm text-primary hover:underline" href="#">Realizar Pedido</a>
</div>
</div>
<div class="flex items-center justify-between">
<div>
<p class="font-medium text-[#111418] dark:text-white">Leche de Almendras 1L</p>
<p class="text-sm text-gray-500 dark:text-gray-400">SKU: LA-003</p>
</div>
<div class="text-right">
<p class="font-bold text-warning">12 Unidades</p>
<a class="text-sm text-primary hover:underline" href="#">Ver Producto</a>
</div>
</div>
<div class="flex items-center justify-between">
<div>
<p class="font-medium text-[#111418] dark:text-white">Chocolate Amargo 70%</p>
<p class="text-sm text-gray-500 dark:text-gray-400">SKU: CH-005</p>
</div>
<div class="text-right">
<p class="font-bold text-warning">15 Unidades</p>
<a class="text-sm text-primary hover:underline" href="#">Ver Producto</a>
</div>
</div>
</div>
</div>
</div>
<div class="mt-6 rounded-xl border border-gray-200 dark:border-gray-700 p-6 bg-white dark:bg-gray-800">
<p class="text-[#111418] dark:text-white text-lg font-bold leading-normal mb-4">Actividad Reciente</p>
<ul class="space-y-4">
<li class="flex items-start gap-4">
<div class="mt-1 flex h-8 w-8 items-center justify-center rounded-full bg-success/20 text-success">
<span class="material-symbols-outlined text-base">shopping_cart</span>
</div>
<div>
<p class="font-medium text-[#111418] dark:text-white">Venta <a class="text-primary" href="#">#1023</a> realizada por $250.00</p>
<p class="text-sm text-gray-500 dark:text-gray-400">Hace 15 minutos</p>
</div>
</li>
<li class="flex items-start gap-4">
<div class="mt-1 flex h-8 w-8 items-center justify-center rounded-full bg-info/20 text-info">
<span class="material-symbols-outlined text-base">local_shipping</span>
</div>
<div>
<p class="font-medium text-[#111418] dark:text-white">Compra <a class="text-primary" href="#">#C58</a> recibida de 'Proveedor XYZ'</p>
<p class="text-sm text-gray-500 dark:text-gray-400">Hace 1 hora</p>
</div>
</li>
<li class="flex items-start gap-4">
<div class="mt-1 flex h-8 w-8 items-center justify-center rounded-full bg-gray-500/20 text-gray-500 dark:text-gray-400">
<span class="material-symbols-outlined text-base">add_box</span>
</div>
<div>
<p class="font-medium text-[#111418] dark:text-white">Nuevo producto 'Azúcar Morena 500g' agregado al inventario</p>
<p class="text-sm text-gray-500 dark:text-gray-400">Hace 3 horas</p>
</div>
</li>
<li class="flex items-start gap-4">
<div class="mt-1 flex h-8 w-8 items-center justify-center rounded-full bg-success/20 text-success">
<span class="material-symbols-outlined text-base">shopping_cart</span>
</div>
<div>
<p class="font-medium text-[#111418] dark:text-white">Venta <a class="text-primary" href="#">#1022</a> realizada por $75.50</p>
<p class="text-sm text-gray-500 dark:text-gray-400">Hace 5 horas</p>
</div>
</li>
</ul>
</div>
</div>
</main>
</div>
</div>
</body></html>