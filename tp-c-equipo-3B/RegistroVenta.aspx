<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="RegistroVenta.aspx.cs" Inherits="tp_c_equipo_3B.RegistroVenta" %>

<!DOCTYPE html>
<html class="light" lang="es"><head>
<meta charset="utf-8"/>
<meta content="width=device-width, initial-scale-1.0" name="viewport"/>
<title>Registrar Nueva Venta</title>
<script src="https://cdn.tailwindcss.com?plugins=forms,container-queries"></script>
<link href="https://fonts.googleapis.com/css2?family=Material+Symbols+Outlined" rel="stylesheet"/>
<link href="https://fonts.googleapis.com" rel="preconnect"/>
<link crossorigin="" href="https://fonts.gstatic.com" rel="preconnect"/>
<link href="https://fonts.googleapis.com/css2?family=Inter:wght@400;500;600;700;900&amp;display=swap" rel="stylesheet"/>
<script>
    tailwind.config = {
        darkMode: "class",
        theme: {
            extend: {
                colors: {
                    "primary": "#1173d4",
                    "background-light": "#f6f7f8",
                    "background-dark": "#101922",
                    "danger": "#DC3545",
                    "warning": "#FFC107",
                },
                fontFamily: {
                    "display": ["Inter", "sans-serif"]
                },
                borderRadius: {
                    "DEFAULT": "0.25rem",
                    "lg": "0.5rem",
                    "xl": "0.75rem",
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
<body class="bg-background-light dark:bg-background-dark font-display">
<div class="min-h-screen">
<main class="p-6 lg:p-8">
<div class="max-w-7xl mx-auto">
<header class="flex flex-wrap justify-between items-center gap-4 mb-8">
<h1 class="text-4xl font-black tracking-tight text-gray-800 dark:text-white">Registrar Nueva Venta</h1>
<button class="flex min-w-[84px] items-center justify-center rounded-lg h-10 px-4 bg-gray-200 dark:bg-gray-700 text-gray-800 dark:text-gray-200 text-sm font-bold">
<span class="truncate">Cancelar</span>
</button>
</header>
<div class="grid grid-cols-1 lg:grid-cols-3 gap-8">
<div class="lg:col-span-2 space-y-8">
<section class="bg-white dark:bg-background-dark/50 p-6 rounded-xl shadow-sm">
<h2 class="text-2xl font-bold tracking-tight text-gray-800 dark:text-white mb-4">Cliente</h2>
<div class="flex items-start gap-4">
<div class="flex-1">
<label class="text-base font-medium text-gray-800 dark:text-gray-300 sr-only" for="customer-search">Buscar por nombre o DNI...</label>
<div class="relative">
<span class="material-symbols-outlined absolute left-3 top-1/2 -translate-y-1/2 text-gray-400">search</span>
<input class="form-input w-full rounded-lg border-gray-300 dark:border-gray-600 bg-white dark:bg-gray-800 h-14 pl-10 text-gray-800 dark:text-gray-200 placeholder:text-gray-500 dark:placeholder:text-gray-400 focus:ring-primary focus:border-primary" id="customer-search" placeholder="Buscar por nombre o DNI..." type="text"/>
</div>
</div>
<button class="flex-shrink-0 size-14 flex items-center justify-center rounded-lg bg-primary/10 hover:bg-primary/20 text-primary">
<span class="material-symbols-outlined text-3xl">add</span>
</button>
</div>
<div class="mt-4 p-4 border border-dashed border-gray-300 dark:border-gray-600 rounded-lg hidden" id="customer-details">
<h3 class="font-semibold text-gray-800 dark:text-white">Datos del Cliente</h3>
<div class="grid grid-cols-1 sm:grid-cols-3 gap-4 mt-2 text-sm">
<div><span class="font-medium text-gray-500 dark:text-gray-400">Nombre:</span> <span class="text-gray-800 dark:text-gray-200" id="customer-name"></span></div>
<div><span class="font-medium text-gray-500 dark:text-gray-400">DNI/RUC:</span> <span class="text-gray-800 dark:text-gray-200" id="customer-id"></span></div>
<div><span class="font-medium text-gray-500 dark:text-gray-400">Teléfono:</span> <span class="text-gray-800 dark:text-gray-200" id="customer-phone"></span></div>
</div>
</div>
</section>
<section class="bg-white dark:bg-background-dark/50 p-6 rounded-xl shadow-sm">
<h2 class="text-2xl font-bold tracking-tight text-gray-800 dark:text-white mb-4">Productos</h2>
<div class="flex items-start gap-4 mb-6">
<div class="flex-1">
<label class="text-base font-medium text-gray-800 dark:text-gray-300 sr-only" for="product-search">Buscar por nombre o código...</label>
<div class="relative">
<span class="material-symbols-outlined absolute left-3 top-1/2 -translate-y-1/2 text-gray-400">search</span>
<input class="form-input w-full rounded-lg border-gray-300 dark:border-gray-600 bg-white dark:bg-gray-800 h-14 pl-10 text-gray-800 dark:text-gray-200 placeholder:text-gray-500 dark:placeholder:text-gray-400 focus:ring-primary focus:border-primary" id="product-search" placeholder="Buscar por nombre o código..." type="text"/>
</div>
</div>
<button class="flex-shrink-0 h-14 px-6 flex items-center justify-center rounded-lg bg-primary/10 hover:bg-primary/20 text-primary text-sm font-bold">
                Agregar
              </button>
</div>
<div class="overflow-x-auto">
<table class="w-full text-left">
<thead class="border-b border-gray-200 dark:border-gray-700">
<tr>
<th class="p-3 text-sm font-semibold text-gray-500 dark:text-gray-400">Producto</th>
<th class="p-3 text-sm font-semibold text-gray-500 dark:text-gray-400 text-center">Stock</th>
<th class="p-3 text-sm font-semibold text-gray-500 dark:text-gray-400 text-center">Cantidad</th>
<th class="p-3 text-sm font-semibold text-gray-500 dark:text-gray-400 text-right">Precio Unit.</th>
<th class="p-3 text-sm font-semibold text-gray-500 dark:text-gray-400 text-right">Precio Total</th>
<th class="p-3 text-sm font-semibold text-gray-500 dark:text-gray-400 text-center">Acciones</th>
</tr>
</thead>
<tbody>
<tr class="border-b border-gray-200 dark:border-gray-700">
<td class="p-3 text-gray-800 dark:text-gray-200 font-medium">Laptop Gamer Pro</td>
<td class="p-3 text-gray-500 dark:text-gray-400 text-center">15</td>
<td class="p-3">
<input class="form-input w-20 text-center rounded-lg border-gray-300 dark:border-gray-600 bg-white dark:bg-gray-800 h-10 text-gray-800 dark:text-gray-200 focus:ring-primary focus:border-primary" type="number" value="1"/>
</td>
<td class="p-3 text-gray-800 dark:text-gray-200 text-right">$1200.00</td>
<td class="p-3 text-gray-800 dark:text-gray-200 font-bold text-right">$1200.00</td>
<td class="p-3 text-center">
<button class="text-danger hover:text-danger/80">
<span class="material-symbols-outlined">delete</span>
</button>
</td>
</tr>
<tr class="border-b border-gray-200 dark:border-gray-700">
<td class="p-3 text-gray-800 dark:text-gray-200 font-medium">Mouse Inalámbrico</td>
<td class="p-3 text-center">
<span class="text-danger font-bold">5</span>
</td>
<td class="p-3">
<div class="relative">
<input class="form-input w-20 text-center rounded-lg border-danger dark:border-danger bg-white dark:bg-gray-800 h-10 text-gray-800 dark:text-gray-200 focus:ring-danger focus:border-danger" type="number" value="6"/>
<span class="material-symbols-outlined text-warning absolute -right-6 top-1/2 -translate-y-1/2" title="Stock insuficiente">warning</span>
</div>
</td>
<td class="p-3 text-gray-800 dark:text-gray-200 text-right">$25.50</td>
<td class="p-3 text-gray-800 dark:text-gray-200 font-bold text-right">$153.00</td>
<td class="p-3 text-center">
<button class="text-danger hover:text-danger/80">
<span class="material-symbols-outlined">delete</span>
</button>
</td>
</tr>
</tbody>
</table>
</div>
</section>
</div>
<div class="lg:col-span-1 space-y-8">
<div class="sticky top-8">
<section class="bg-white dark:bg-background-dark/50 p-6 rounded-xl shadow-sm">
<h2 class="text-2xl font-bold tracking-tight text-gray-800 dark:text-white mb-6">Resumen de Venta</h2>
<div class="space-y-4">
<div class="flex justify-between text-gray-600 dark:text-gray-300">
<span>Subtotal</span>
<span class="font-medium text-gray-800 dark:text-gray-200">$1353.00</span>
</div>
<div class="flex justify-between items-center text-gray-600 dark:text-gray-300">
<span>Descuento (%)</span>
<input class="form-input w-20 text-right rounded-lg border-gray-300 dark:border-gray-600 bg-white dark:bg-gray-800 h-10 text-gray-800 dark:text-gray-200 focus:ring-primary focus:border-primary" type="number" value="0"/>
</div>
<div class="flex justify-between items-center text-gray-600 dark:text-gray-300">
<span>IVA (%)</span>
<input class="form-input w-20 text-right rounded-lg border-gray-300 dark:border-gray-600 bg-white dark:bg-gray-800 h-10 text-gray-800 dark:text-gray-200 focus:ring-primary focus:border-primary" type="number" value="21"/>
</div>
<div class="border-t border-gray-200 dark:border-gray-700 my-4"></div>
<div class="flex justify-between text-2xl font-bold text-gray-900 dark:text-white">
<span>TOTAL</span>
<span>$1637.13</span>
</div>
</div>
<div class="mt-6">
<label class="text-base font-medium text-gray-800 dark:text-gray-300" for="payment-method">Método de Pago</label>
<select class="form-select mt-2 w-full rounded-lg border-gray-300 dark:border-gray-600 bg-white dark:bg-gray-800 h-14 text-gray-800 dark:text-gray-200 focus:ring-primary focus:border-primary" id="payment-method">
<option>Efectivo</option>
<option>Tarjeta de Crédito</option>
<option>Transferencia Bancaria</option>
</select>
</div>
<div class="mt-8 space-y-3">
<button class="w-full flex items-center justify-center rounded-lg h-12 px-6 bg-primary text-white text-base font-bold">
                  Generar Venta y Factura
                </button>
<button class="w-full flex items-center justify-center rounded-lg h-12 px-6 bg-gray-200 dark:bg-gray-700 text-gray-800 dark:text-gray-200 text-base font-bold">
                  Guardar Borrador
                </button>
</div>
</section>
<section class="bg-white dark:bg-background-dark/50 p-6 rounded-xl shadow-sm mt-8">
<h3 class="text-lg font-bold text-gray-800 dark:text-white mb-4">Vista Previa de Factura</h3>
<div class="border border-gray-200 dark:border-gray-600 rounded-lg p-4 aspect-[210/297] flex flex-col text-xs text-gray-700 dark:text-gray-300">
<div class="flex justify-between items-start pb-4 border-b border-gray-200 dark:border-gray-700">
<div>
<h4 class="font-bold text-base">Mi Negocio</h4>
<p>Calle Falsa 123</p>
<p>Ciudad, País</p>
</div>
<div class="text-right">
<h4 class="font-bold text-base">FACTURA</h4>
<p>#FAC-00123</p>
<p>Fecha: 24/07/2024</p>
</div>
</div>
<div class="py-4 border-b border-gray-200 dark:border-gray-700">
<h4 class="font-bold">Cliente:</h4>
<p>Nombre del Cliente</p>
<p>DNI: 12345678X</p>
</div>
<div class="flex-1 py-4">
<table class="w-full">
<thead>
<tr class="border-b border-gray-200 dark:border-gray-700">
<th class="text-left py-1">Item</th>
<th class="text-center py-1">Cant.</th>
<th class="text-right py-1">P. Unit</th>
<th class="text-right py-1">Total</th>
</tr>
</thead>
<tbody>
<tr>
<td class="py-1">Laptop Gamer Pro</td>
<td class="text-center py-1">1</td>
<td class="text-right py-1">$1200.00</td>
<td class="text-right py-1">$1200.00</td>
</tr>
<tr>
<td class="py-1">Mouse Inalámbrico</td>
<td class="text-center py-1">6</td>
<td class="text-right py-1">$25.50</td>
<td class="text-right py-1">$153.00</td>
</tr>
</tbody>
</table>
</div>
<div class="pt-4 border-t border-gray-200 dark:border-gray-700 text-right">
<p>Subtotal: <span class="font-medium">$1353.00</span></p>
<p>IVA (21%): <span class="font-medium">$284.13</span></p>
<p class="font-bold text-base mt-2">TOTAL: <span class="font-bold">$1637.13</span></p>
</div>
</div>
</section>
</div>
</div>
</div>
</div></main>
</div>
</body></html>