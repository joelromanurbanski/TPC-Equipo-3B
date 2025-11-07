<!DOCTYPE html>
<html class="light" lang="es"><head>
<meta charset="utf-8"/>
<meta content="width=device-width, initial-scale=1.0" name="viewport"/>
<title>Gestión de Proveedores</title>
<link href="https://fonts.googleapis.com/css2?family=Inter:wght@400;500;700;900&amp;display=swap" rel="stylesheet"/>
<link href="https://fonts.googleapis.com/css2?family=Material+Symbols+Outlined" rel="stylesheet"/>
<script src="https://cdn.tailwindcss.com?plugins=forms,container-queries"></script>
<script id="tailwind-config">
    tailwind.config = {
        darkMode: "class",
        theme: {
            extend: {
                colors: {
                    "primary": "#4A90E2",
                    "background-light": "#F5F7FA",
                    "background-dark": "#101922",
                    "success": "#50E3C2",
                    "danger": "#D0021B",
                    "text-primary": "#4A4A4A",
                    "text-secondary": "#9B9B9B"
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
      font-variation-settings: 'FILL' 0, 'wght' 400, 'GRAD' 0, 'opsz' 24;
    }
  </style>
</head>
<body class="bg-background-light dark:bg-background-dark font-display text-text-primary">
<div class="relative flex h-screen w-full flex-col group/design-root overflow-hidden">
<div class="layout-container flex h-full grow">
<div class="flex flex-1">
<aside class="w-1/3 flex flex-col bg-white dark:bg-background-dark border-r border-gray-200 dark:border-gray-700 p-4">
<div class="flex flex-col gap-4 h-full">
<div class="flex justify-between items-center">
<h2 class="text-xl font-bold text-text-primary dark:text-white">Proveedores</h2>
</div>
<div class="flex gap-2">
<div class="relative flex-grow">
<label class="flex flex-col min-w-40 h-12 w-full">
<div class="flex w-full flex-1 items-stretch rounded-lg h-full">
<div class="text-text-secondary flex border-none bg-gray-100 dark:bg-gray-800 items-center justify-center pl-4 rounded-l-lg border-r-0">
<span class="material-symbols-outlined">search</span>
</div>
<input class="form-input flex w-full min-w-0 flex-1 resize-none overflow-hidden rounded-lg text-text-primary dark:text-white focus:outline-0 focus:ring-0 border-none bg-gray-100 dark:bg-gray-800 focus:border-none h-full placeholder:text-text-secondary px-4 rounded-l-none border-l-0 pl-2 text-base font-normal leading-normal" placeholder="Buscar proveedor" value=""/>
</div>
</label>
</div>
<button class="flex min-w-[48px] cursor-pointer items-center justify-center overflow-hidden rounded-lg h-12 w-12 bg-primary text-white gap-2">
<span class="material-symbols-outlined">add</span>
</button>
</div>
<div class="flex-grow overflow-y-auto">
<div class="flex overflow-hidden rounded-lg bg-white dark:bg-background-dark">
<table class="w-full">
<thead>
<tr class="bg-white dark:bg-background-dark">
<th class="px-4 py-3 text-left text-text-primary dark:text-white text-sm font-medium">Nombre</th>
<th class="px-4 py-3 text-left text-text-primary dark:text-white text-sm font-medium">Teléfono</th>
<th class="px-4 py-3 text-left text-text-primary dark:text-white text-sm font-medium">Estado</th>
</tr>
</thead>
<tbody>
<tr class="border-t border-gray-200 dark:border-gray-700 bg-primary/10 dark:bg-primary/20">
<td class="h-[60px] px-4 py-2 text-text-primary dark:text-white text-sm font-normal">Proveedor A</td>
<td class="h-[60px] px-4 py-2 text-text-secondary text-sm font-normal">123-456-7890</td>
<td class="h-[60px] px-4 py-2 text-sm font-normal">
<span class="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium bg-success/20 text-green-800 dark:text-green-300">Activo</span>
</td>
</tr>
<tr class="border-t border-gray-200 dark:border-gray-700 hover:bg-gray-50 dark:hover:bg-gray-800/50">
<td class="h-[60px] px-4 py-2 text-text-primary dark:text-white text-sm font-normal">Proveedor B</td>
<td class="h-[60px] px-4 py-2 text-text-secondary text-sm font-normal">234-567-8901</td>
<td class="h-[60px] px-4 py-2 text-sm font-normal">
<span class="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium bg-success/20 text-green-800 dark:text-green-300">Activo</span>
</td>
</tr>
<tr class="border-t border-gray-200 dark:border-gray-700 hover:bg-gray-50 dark:hover:bg-gray-800/50">
<td class="h-[60px] px-4 py-2 text-text-primary dark:text-white text-sm font-normal">Proveedor C</td>
<td class="h-[60px] px-4 py-2 text-text-secondary text-sm font-normal">345-678-9012</td>
<td class="h-[60px] px-4 py-2 text-sm font-normal">
<span class="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium bg-danger/20 text-red-800 dark:text-red-300">Inactivo</span>
</td>
</tr>
</tbody>
</table>
</div>
</div>
</div>
</aside>
<main class="flex-1 p-6">
<div class="flex flex-wrap justify-between gap-3 p-4">
<div class="flex min-w-72 flex-col gap-3">
<p class="text-text-primary dark:text-white text-4xl font-black leading-tight tracking-[-0.033em]">Proveedor A</p>
<p class="text-text-secondary text-base font-normal leading-normal">Información detallada del proveedor</p>
</div>
<div class="flex gap-2">
<button class="flex min-w-[84px] max-w-[480px] cursor-pointer items-center justify-center overflow-hidden rounded-lg h-12 px-5 bg-gray-200 dark:bg-gray-700 text-text-primary dark:text-white gap-2 text-base font-bold leading-normal">
<span class="material-symbols-outlined">edit</span>
<span class="truncate">Editar</span>
</button>
<button class="flex min-w-[84px] max-w-[480px] cursor-pointer items-center justify-center overflow-hidden rounded-lg h-12 px-5 bg-danger text-white gap-2 text-base font-bold leading-normal">
<span class="material-symbols-outlined">delete</span>
<span class="truncate">Eliminar</span>
</button>
</div>
</div>
<div class="mt-6">
<div class="border-b border-gray-200 dark:border-gray-700">
<nav aria-label="Tabs" class="-mb-px flex space-x-8">
<a class="border-primary text-primary whitespace-nowrap py-4 px-1 border-b-2 font-medium text-sm" href="#">Datos Generales</a>
<a class="border-transparent text-text-secondary hover:text-text-primary dark:hover:text-white hover:border-gray-300 dark:hover:border-gray-600 whitespace-nowrap py-4 px-1 border-b-2 font-medium text-sm" href="#">Contactos</a>
<a class="border-transparent text-text-secondary hover:text-text-primary dark:hover:text-white hover:border-gray-300 dark:hover:border-gray-600 whitespace-nowrap py-4 px-1 border-b-2 font-medium text-sm" href="#">Productos Suministrados</a>
</nav>
</div>
<div class="py-6">
<div class="grid grid-cols-1 md:grid-cols-2 gap-6">
<div>
<label class="block text-sm font-medium text-text-secondary" for="company-name">Nombre de la empresa</label>
<p class="mt-1 text-base text-text-primary dark:text-white">Proveedor A</p>
</div>
<div>
<label class="block text-sm font-medium text-text-secondary" for="tax-id">Número de identificación fiscal</label>
<p class="mt-1 text-base text-text-primary dark:text-white">XYZ-12345678</p>
</div>
<div class="md:col-span-2">
<label class="block text-sm font-medium text-text-secondary" for="address">Dirección</label>
<p class="mt-1 text-base text-text-primary dark:text-white">Av. Principal 123, Ciudad, Estado, 12345</p>
</div>
<div>
<label class="block text-sm font-medium text-text-secondary" for="phone">Teléfono principal</label>
<p class="mt-1 text-base text-text-primary dark:text-white">123-456-7890</p>
</div>
<div>
<label class="block text-sm font-medium text-text-secondary" for="website">Sitio web</label>
<p class="mt-1 text-base text-primary">www.proveedora.com</p>
</div>
<div class="md:col-span-2">
<label class="block text-sm font-medium text-text-secondary" for="notes">Notas generales</label>
<p class="mt-1 text-base text-text-primary dark:text-white">Este proveedor ofrece descuentos por volumen.</p>
</div>
</div>
</div>
</div>
</main>
</div>
</div>
</div>

</body></html>