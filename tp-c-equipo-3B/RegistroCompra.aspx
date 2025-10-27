<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="RegistroCompra.aspx.cs" Inherits="tp_c_equipo_3B.RegistroCompra" %>

<!DOCTYPE html>

<html class="light" lang="es">
<head>
<meta charset="utf-8"/>
<meta content="width=device-width, initial-scale=1.0" name="viewport"/>
<title>Registro de Compras</title>
<script src="https://cdn.tailwindcss.com?plugins=forms,container-queries"></script>
<link href="https://fonts.googleapis.com/css2?family=Material+Symbols+Outlined" rel="stylesheet"/>
<link href="https://fonts.googleapis.com" rel="preconnect"/>
<link crossorigin="" href="https://fonts.gstatic.com" rel="preconnect"/>
<link href="https://fonts.googleapis.com/css2?family=Inter:wght@100..900&amp;display=swap" rel="stylesheet"/>
<script>
    tailwind.config = {
      darkMode: "class",
      theme: {
        extend: {
          colors: {
            "primary": "#2A52BE",
            "background-light": "#F0F2F5",
            "background-dark": "#101922",
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
<body class="bg-background-light dark:bg-background-dark font-display text-[#343A40] dark:text-white">
<div class="flex h-screen">
<aside class="w-64 bg-white dark:bg-gray-800 p-4 flex flex-col justify-between shadow-lg">
<div>
<div class="flex items-center gap-3 mb-8">
<div class="bg-center bg-no-repeat aspect-square bg-cover rounded-full size-10" data-alt="User avatar" style="background-image: url('https://lh3.googleusercontent.com/aida-public/AB6AXuBlV4XhU4SQQT7prUPmZCcrEn8nNGUCuekYggsil-jasBrRB1U7FK7jynkdusZwnukHzFxijXAJDizr_X1e-57jCQSbEZafmfYYgx201MLm-PAh13Xno1QoFlZfS59vmUVQF_zNjxdOeJJtZhgikmB1Y9AeGsagOwh2UI-Qo4Geb4jseC7CrZDk-8rqt1sTR_n43WSn1gh2VbOu7NJgiYd84PqmcGFOl1sugVtXcrNdoAhiHYiJLw1YDNcUgwOL87jPvdh6rv96uPMW');"></div>
<div class="flex flex-col">
<h1 class="text-[#111418] dark:text-gray-200 text-base font-medium leading-normal">Nombre de usuario</h1>
<p class="text-[#617589] dark:text-gray-400 text-sm font-normal leading-normal">Administrador</p>
</div>
</div>
<nav class="flex flex-col gap-2">
<a class="flex items-center gap-3 px-3 py-2 text-[#617589] dark:text-gray-400 hover:bg-primary/10 dark:hover:bg-primary/20 hover:text-primary dark:hover:text-primary rounded-lg" href="#">
<span class="material-symbols-outlined">dashboard</span>
<p class="text-sm font-medium leading-normal">Dashboard</p>
</a>
<a class="flex items-center gap-3 px-3 py-2 rounded-lg bg-primary/10 dark:bg-primary/20 text-primary" href="#">
<span class="material-symbols-outlined" style="font-variation-settings: 'FILL' 1;">shopping_cart</span>
<p class="text-sm font-medium leading-normal">Compras</p>
</a>
<a class="flex items-center gap-3 px-3 py-2 text-[#617589] dark:text-gray-400 hover:bg-primary/10 dark:hover:bg-primary/20 hover:text-primary dark:hover:text-primary rounded-lg" href="#">
<span class="material-symbols-outlined">receipt_long</span>
<p class="text-sm font-medium leading-normal">Ventas</p>
</a>
<a class="flex items-center gap-3 px-3 py-2 text-[#617589] dark:text-gray-400 hover:bg-primary/10 dark:hover:bg-primary/20 hover:text-primary dark:hover:text-primary rounded-lg" href="#">
<span class="material-symbols-outlined">inventory_2</span>
<p class="text-sm font-medium leading-normal">Productos</p>
</a>
</nav>
</div>
<button class="flex w-full items-center justify-center rounded-lg h-10 px-4 bg-[#DC3545] text-white text-sm font-bold leading-normal tracking-[0.015em] hover:bg-red-700">
<span class="truncate">Cerrar sesión</span>
</button>
</aside>
<main class="flex-1 p-8 overflow-y-auto">
<div class="max-w-7xl mx-auto">
<header class="mb-8">
<p class="text-[#111418] dark:text-white text-4xl font-black leading-tight tracking-[-0.033em]">Registro de Compras</p>
<p class="text-[#617589] dark:text-gray-400 text-base font-normal leading-normal mt-2">Completa los siguientes campos para registrar una nueva compra de inventario.</p>
</header>
<div class="grid grid-cols-1 lg:grid-cols-3 gap-8">
<div class="lg:col-span-2">
<div class="bg-white dark:bg-gray-800 p-6 rounded-xl shadow-md">
<h2 class="text-xl font-bold mb-6 text-[#111418] dark:text-white">Detalles de la Compra</h2>
<div class="grid grid-cols-1 md:grid-cols-2 gap-6">
<label class="flex flex-col">
<p class="text-[#343A40] dark:text-gray-300 text-base font-medium leading-normal pb-2">Proveedor</p>
<select class="form-select flex w-full min-w-0 flex-1 resize-none overflow-hidden rounded-lg text-[#343A40] dark:text-gray-300 focus:outline-0 focus:ring-2 focus:ring-primary/50 border border-[#dbe0e6] dark:border-gray-600 bg-white dark:bg-gray-700 h-12 placeholder:text-[#617589] px-4 text-base font-normal leading-normal">
<option>Seleccionar proveedor</option>
<option>Proveedor A</option>
<option>Proveedor B</option>
</select>
</label>
<div class="flex items-end">
<button class="flex items-center justify-center rounded-lg h-12 px-4 bg-primary/20 text-primary text-sm font-bold hover:bg-primary/30">
<span class="material-symbols-outlined mr-2">add</span>
<span>Nuevo Proveedor</span>
</button>
</div>
<label class="flex flex-col">
<p class="text-[#343A40] dark:text-gray-300 text-base font-medium leading-normal pb-2">Número de factura</p>
<input class="form-input flex w-full min-w-0 flex-1 resize-none overflow-hidden rounded-lg text-[#343A40] dark:text-gray-300 focus:outline-0 focus:ring-2 focus:ring-primary/50 border border-[#dbe0e6] dark:border-gray-600 bg-white dark:bg-gray-700 h-12 placeholder:text-[#617589] px-4 text-base font-normal leading-normal" placeholder="Ingresar número de factura" value=""/>
</label>
<label class="flex flex-col">
<p class="text-[#343A40] dark:text-gray-300 text-base font-medium leading-normal pb-2">Fecha de compra</p>
<div class="flex w-full flex-1 items-stretch rounded-lg">
<input class="form-input flex w-full min-w-0 flex-1 resize-none overflow-hidden rounded-lg text-[#343A40] dark:text-gray-300 focus:outline-0 focus:ring-2 focus:ring-primary/50 border border-[#dbe0e6] dark:border-gray-600 bg-white dark:bg-gray-700 h-12 placeholder:text-[#617589] px-4 text-base font-normal leading-normal" type="date"/>
</div>
</label>
</div>
<hr class="my-8 border-gray-200 dark:border-gray-700"/>
<div>
<h3 class="text-lg font-bold mb-4 text-[#111418] dark:text-white">Productos</h3>
<div class="flex items-end gap-4 mb-4">
<label class="flex flex-col flex-1">
<p class="text-[#343A40] dark:text-gray-300 text-sm font-medium leading-normal pb-2">Buscar producto</p>
<div class="relative">
<input class="form-input flex w-full min-w-0 resize-none overflow-hidden rounded-lg text-[#343A40] dark:text-gray-300 focus:outline-0 focus:ring-2 focus:ring-primary/50 border border-[#dbe0e6] dark:border-gray-600 bg-white dark:bg-gray-700 h-12 placeholder:text-[#617589] px-4 pl-10 text-base font-normal leading-normal" placeholder="Buscar por nombre o código..." value=""/>
<span class="material-symbols-outlined absolute left-3 top-1/2 -translate-y-1/2 text-[#617589]">search</span>
</div>
</label>
<button class="flex min-w-[84px] cursor-pointer items-center justify-center overflow-hidden rounded-lg h-12 px-6 bg-[#28A745] text-white text-sm font-bold leading-normal tracking-[0.015em] hover:bg-green-700">
<span class="truncate">Agregar</span>
</button>
</div>
<div class="overflow-x-auto rounded-lg border border-[#dbe0e6] dark:border-gray-700">
<table class="w-full text-left text-sm text-[#343A40] dark:text-gray-300">
<thead class="bg-gray-50 dark:bg-gray-700 text-xs uppercase">
<tr>
<th class="px-6 py-3" scope="col">Producto</th>
<th class="px-6 py-3" scope="col">Cantidad</th>
<th class="px-6 py-3" scope="col">Precio Unit.</th>
<th class="px-6 py-3" scope="col">Subtotal</th>
<th class="px-6 py-3" scope="col"></th>
</tr>
</thead>
<tbody>
<tr class="bg-white dark:bg-gray-800 border-b dark:border-gray-700">
<th class="px-6 py-4 font-medium whitespace-nowrap" scope="row">Laptop Pro 15"</th>
<td class="px-6 py-4">
<input class="w-20 form-input rounded-lg h-10 border-[#dbe0e6] dark:border-gray-600 bg-white dark:bg-gray-700 focus:ring-primary/50 focus:border-primary/50" type="number" value="2"/>
</td>
<td class="px-6 py-4">$1200.00</td>
<td class="px-6 py-4">$2400.00</td>
<td class="px-6 py-4 text-right">
<button class="text-[#DC3545] hover:text-red-700">
<span class="material-symbols-outlined">delete</span>
</button>
</td>
</tr>
<tr class="bg-white dark:bg-gray-800">
<th class="px-6 py-4 font-medium whitespace-nowrap" scope="row">Mouse Inalámbrico</th>
<td class="px-6 py-4">
<input class="w-20 form-input rounded-lg h-10 border-[#dbe0e6] dark:border-gray-600 bg-white dark:bg-gray-700 focus:ring-primary/50 focus:border-primary/50" type="number" value="5"/>
</td>
<td class="px-6 py-4">$25.00</td>
<td class="px-6 py-4">$125.00</td>
<td class="px-6 py-4 text-right">
<button class="text-[#DC3545] hover:text-red-700">
<span class="material-symbols-outlined">delete</span>
</button>
</td>
</tr>
</tbody>
</table>
</div>
</div>
</div>
</div>
<div class="lg:col-span-1">
<div class="bg-white dark:bg-gray-800 p-6 rounded-xl shadow-md sticky top-8">
<h2 class="text-xl font-bold mb-6 text-[#111418] dark:text-white">Resumen de la Compra</h2>
<div class="space-y-4">
<div class="flex justify-between text-base">
<p class="text-[#617589] dark:text-gray-400">Subtotal</p>
<p class="font-medium text-[#111418] dark:text-white">$2525.00</p>
</div>
<div class="flex justify-between items-center text-base">
<p class="text-[#617589] dark:text-gray-400">Impuestos (18%)</p>
<p class="font-medium text-[#111418] dark:text-white">$454.50</p>
</div>
<div class="flex justify-between items-center text-base">
<p class="text-[#617589] dark:text-gray-400">Otros Costos</p>
<input class="w-24 form-input text-right rounded-lg h-10 border-[#dbe0e6] dark:border-gray-600 bg-white dark:bg-gray-700 focus:ring-primary/50 focus:border-primary/50" type="number" value="0.00"/>
</div>
<div class="border-t border-gray-200 dark:border-gray-700 my-4"></div>
<div class="flex justify-between text-xl font-bold">
<p class="text-[#111418] dark:text-white">Total</p>
<p class="text-primary">$2979.50</p>
</div>
</div>
<div class="mt-8 flex flex-col gap-3">
<button class="w-full flex items-center justify-center rounded-lg h-12 px-6 bg-primary text-white text-sm font-bold hover:bg-blue-800">
<span class="material-symbols-outlined mr-2">save</span>
<span>Guardar Compra</span>
</button>
<button class="w-full flex items-center justify-center rounded-lg h-12 px-6 bg-gray-200 dark:bg-gray-600 text-[#343A40] dark:text-gray-300 text-sm font-bold hover:bg-gray-300 dark:hover:bg-gray-500">
<span class="truncate">Cancelar</span>
</button>
</div>
</div>
</div>
</div>
</div>
</main>
</div>
</body></html>
