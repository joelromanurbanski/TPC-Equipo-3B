<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="GestionClientes.aspx.cs" Inherits="tp_c_equipo_3B.GestionClientes" %>

<!DOCTYPE html>

<html class="light" lang="es"><head>
<meta charset="utf-8"/>
<meta content="width=device-width, initial-scale=1.0" name="viewport"/>
<title>Administración de Clientes</title>
<script src="https://cdn.tailwindcss.com?plugins=forms,container-queries"></script>
<link href="https://fonts.googleapis.com/css2?family=Material+Symbols+Outlined" rel="stylesheet"/>
<link href="https://fonts.googleapis.com" rel="preconnect"/>
<link crossorigin="" href="https://fonts.gstatic.com" rel="preconnect"/>
<link href="https://fonts.googleapis.com/css2?family=Inter:wght@400;500;600;700;800;900&amp;display=swap" rel="stylesheet"/>
<style>
    .material-symbols-outlined {
      font-variation-settings:
      'FILL' 0,
      'wght' 400,
      'GRAD' 0,
      'opsz' 24
    }
  </style>
<script id="tailwind-config">
    tailwind.config = {
      darkMode: "class",
      theme: {
        extend: {
          colors: {
            "primary": "#4A90E2",
            "background-light": "#F4F4F4",
            "background-dark": "#101922",
          },
          fontFamily: {
            "display": ["Inter", "sans-serif"]
          },
          borderRadius: {"DEFAULT": "0.25rem", "lg": "0.5rem", "xl": "0.75rem", "full": "9999px"},
        },
      },
    }
</script>
</head>
<body class="bg-background-light dark:bg-background-dark font-display">
<div class="relative flex h-auto min-h-screen w-full flex-col group/design-root overflow-x-hidden">
<div class="layout-container flex h-full grow flex-col">
<header class="flex items-center justify-between whitespace-nowrap border-b border-solid border-b-[#dbe0e6] dark:border-b-gray-700 px-10 py-3 bg-white dark:bg-background-dark">
<div class="flex items-center gap-8">
<div class="flex items-center gap-4 text-[#111418] dark:text-white">
<div class="size-8 text-primary">
<span class="material-symbols-outlined !text-4xl">
                business_center
              </span>
</div>
<h2 class="text-[#111418] dark:text-white text-lg font-bold leading-tight tracking-[-0.015em]">Business Management App</h2>
</div>
<div class="flex items-center gap-9">
<a class="text-[#4A4A4A] dark:text-gray-300 text-sm font-medium leading-normal" href="#">Compras</a>
<a class="text-[#4A4A4A] dark:text-gray-300 text-sm font-medium leading-normal" href="#">Ventas</a>
<a class="text-primary dark:text-primary text-sm font-medium leading-normal" href="#">Dashboard</a>
</div>
</div>
<div class="flex flex-1 justify-end items-center gap-4">
<div class="bg-center bg-no-repeat aspect-square bg-cover rounded-full size-10" data-alt="User avatar" style='background-image: url("https://lh3.googleusercontent.com/aida-public/AB6AXuA8ncVdFrEiatAbWsHAGuUxYhujCk8qazQQh9ZwSJ0K0QOKFu4rYNuHY3Z3gAMgyyIAKmyyYffz6zNU520DU1FWld4rJZFawTLGKjPvNES8ADYamO96ZtaiBUFjLpF_05l56-AycjeJKefAo21szpi7ToPrjLrvaPRrGYtvyi0HgC6JhbuKhlKXrNtCg-OXKgMPtzHKUoDF36SfXGgUrz7Dd1STZrrjs7YkXFsf03LVw8eXS4IqavAcOC47CKy1q-aeWk-8VUgNmkie");'></div>
</div>
</header>
<div class="px-10 md:px-20 lg:px-40 flex flex-1 justify-center py-5">
<div class="layout-content-container flex flex-col max-w-7xl flex-1 gap-6">
<div class="flex flex-wrap justify-between items-center gap-4">
<p class="text-[#4A4A4A] dark:text-white text-4xl font-black leading-tight tracking-[-0.033em]">Administración de Clientes</p>
<button class="flex min-w-[84px] max-w-[480px] cursor-pointer items-center justify-center overflow-hidden rounded-lg h-12 px-5 bg-primary text-white gap-2 pl-4 text-base font-bold leading-normal tracking-[0.015em] hover:bg-blue-500 transition-colors">
<span class="material-symbols-outlined">
                add
              </span>
<span class="truncate">Nuevo Cliente</span>
</button>
</div>
<div class="bg-white dark:bg-gray-800 p-6 rounded-xl shadow-sm">
<div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
<div>
<label class="block text-sm font-medium text-[#4A4A4A] dark:text-gray-300" for="nombre">Nombre</label>
<input class="mt-1 block w-full rounded-lg border-gray-300 dark:border-gray-600 bg-gray-50 dark:bg-gray-700 text-[#4A4A4A] dark:text-gray-200 focus:border-primary focus:ring-primary-light" id="nombre" name="nombre" placeholder="John" type="text"/>
</div>
<div>
<label class="block text-sm font-medium text-[#4A4A4A] dark:text-gray-300" for="apellido">Apellido</label>
<input class="mt-1 block w-full rounded-lg border-gray-300 dark:border-gray-600 bg-gray-50 dark:bg-gray-700 text-[#4A4A4A] dark:text-gray-200 focus:border-primary focus:ring-primary-light" id="apellido" name="apellido" placeholder="Doe" type="text"/>
</div>
<div>
<label class="block text-sm font-medium text-[#4A4A4A] dark:text-gray-300" for="cedula">Cédula/Identificación</label>
<input class="mt-1 block w-full rounded-lg border-gray-300 dark:border-gray-600 bg-gray-50 dark:bg-gray-700 text-[#4A4A4A] dark:text-gray-200 focus:border-primary focus:ring-primary-light" id="cedula" name="cedula" placeholder="123456789" type="text"/>
</div>
<div>
<label class="block text-sm font-medium text-[#4A4A4A] dark:text-gray-300" for="telefono">Teléfono</label>
<input class="mt-1 block w-full rounded-lg border-gray-300 dark:border-gray-600 bg-gray-50 dark:bg-gray-700 text-[#4A4A4A] dark:text-gray-200 focus:border-primary focus:ring-primary-light" id="telefono" name="telefono" placeholder="555-123-4567" type="tel"/>
</div>
<div>
<label class="block text-sm font-medium text-[#4A4A4A] dark:text-gray-300" for="email">Correo Electrónico</label>
<input class="mt-1 block w-full rounded-lg border-gray-300 dark:border-gray-600 bg-gray-50 dark:bg-gray-700 text-[#4A4A4A] dark:text-gray-200 focus:border-primary focus:ring-primary-light" id="email" name="email" placeholder="john.doe@example.com" type="email"/>
</div>
<div class="lg:col-span-1">
<label class="block text-sm font-medium text-[#4A4A4A] dark:text-gray-300" for="direccion">Dirección</label>
<input class="mt-1 block w-full rounded-lg border-gray-300 dark:border-gray-600 bg-gray-50 dark:bg-gray-700 text-[#4A4A4A] dark:text-gray-200 focus:border-primary focus:ring-primary-light" id="direccion" name="direccion" placeholder="123 Main St, Anytown" type="text"/>
</div>
</div>
<div class="flex justify-end gap-4 mt-6">
<button class="flex min-w-[84px] max-w-[480px] cursor-pointer items-center justify-center overflow-hidden rounded-lg h-12 px-5 bg-gray-200 dark:bg-gray-600 text-[#4A4A4A] dark:text-gray-200 gap-2 text-base font-bold leading-normal tracking-[0.015em] hover:bg-gray-300 dark:hover:bg-gray-500 transition-colors">
<span class="truncate">Cancelar</span>
</button>
<button class="flex min-w-[84px] max-w-[480px] cursor-pointer items-center justify-center overflow-hidden rounded-lg h-12 px-5 bg-green-500 text-white gap-2 text-base font-bold leading-normal tracking-[0.015em] hover:bg-green-600 transition-colors">
<span class="material-symbols-outlined">
                        save
                    </span>
<span class="truncate">Guardar</span>
</button>
</div>
</div>
<div class="bg-white dark:bg-gray-800 p-6 rounded-xl shadow-sm">
<div class="flex flex-col md:flex-row justify-between items-center gap-4 mb-4">
<h3 class="text-xl font-bold text-[#4A4A4A] dark:text-white">Lista de Clientes</h3>
<div class="w-full md:w-1/2 lg:w-1/3">
<label class="flex flex-col min-w-40 h-12 w-full">
<div class="flex w-full flex-1 items-stretch rounded-lg h-full">
<div class="text-gray-400 flex border border-gray-300 dark:border-gray-600 bg-gray-50 dark:bg-gray-700 items-center justify-center pl-4 rounded-l-lg border-r-0">
<span class="material-symbols-outlined">
                        search
                      </span>
</div>
<input class="form-input flex w-full min-w-0 flex-1 resize-none overflow-hidden rounded-r-lg text-[#4A4A4A] dark:text-gray-200 focus:outline-0 focus:ring-primary-light border border-gray-300 dark:border-gray-600 bg-gray-50 dark:bg-gray-700 focus:border-primary h-full placeholder:text-gray-400 px-4 border-l-0 text-base font-normal leading-normal" placeholder="Buscar por nombre, teléfono o correo" value=""/>
</div>
</label>
</div>
</div>
<div class="overflow-x-auto">
<div class="flex overflow-hidden rounded-lg border border-[#dbe0e6] dark:border-gray-700 bg-white dark:bg-gray-800">
<table class="min-w-full divide-y divide-[#dbe0e6] dark:divide-gray-700">
<thead class="bg-gray-50 dark:bg-gray-700">
<tr>
<th class="px-6 py-3 text-left text-xs font-medium text-[#4A4A4A] dark:text-gray-300 uppercase tracking-wider" scope="col">Nombre Completo</th>
<th class="px-6 py-3 text-left text-xs font-medium text-[#4A4A4A] dark:text-gray-300 uppercase tracking-wider" scope="col">Número de Teléfono</th>
<th class="px-6 py-3 text-left text-xs font-medium text-[#4A4A4A] dark:text-gray-300 uppercase tracking-wider" scope="col">Correo Electrónico</th>
<th class="px-6 py-3 text-left text-xs font-medium text-[#4A4A4A] dark:text-gray-300 uppercase tracking-wider" scope="col">Acciones</th>
</tr>
</thead>
<tbody class="bg-white dark:bg-gray-800 divide-y divide-[#dbe0e6] dark:divide-gray-700">
<tr>
<td class="px-6 py-4 whitespace-nowrap text-sm font-medium text-[#4A4A4A] dark:text-white">Juan Pérez</td>
<td class="px-6 py-4 whitespace-nowrap text-sm text-gray-500 dark:text-gray-400">555-1234</td>
<td class="px-6 py-4 whitespace-nowrap text-sm text-gray-500 dark:text-gray-400">juan.perez@example.com</td>
<td class="px-6 py-4 whitespace-nowrap text-sm font-medium">
<a class="text-primary hover:text-blue-700 mr-4" href="#">Editar</a>
<a class="text-red-600 hover:text-red-800" href="#">Eliminar</a>
</td>
</tr>
<tr>
<td class="px-6 py-4 whitespace-nowrap text-sm font-medium text-[#4A4A4A] dark:text-white">Ana Gómez</td>
<td class="px-6 py-4 whitespace-nowrap text-sm text-gray-500 dark:text-gray-400">555-5678</td>
<td class="px-6 py-4 whitespace-nowrap text-sm text-gray-500 dark:text-gray-400">ana.gomez@example.com</td>
<td class="px-6 py-4 whitespace-nowrap text-sm font-medium">
<a class="text-primary hover:text-blue-700 mr-4" href="#">Editar</a>
<a class="text-red-600 hover:text-red-800" href="#">Eliminar</a>
</td>
</tr>
<tr>
<td class="px-6 py-4 whitespace-nowrap text-sm font-medium text-[#4A4A4A] dark:text-white">Carlos Ruiz</td>
<td class="px-6 py-4 whitespace-nowrap text-sm text-gray-500 dark:text-gray-400">555-8765</td>
<td class="px-6 py-4 whitespace-nowrap text-sm text-gray-500 dark:text-gray-400">carlos.ruiz@example.com</td>
<td class="px-6 py-4 whitespace-nowrap text-sm font-medium">
<a class="text-primary hover:text-blue-700 mr-4" href="#">Editar</a>
<a class="text-red-600 hover:text-red-800" href="#">Eliminar</a>
</td>
</tr>
<tr>
<td class="px-6 py-4 whitespace-nowrap text-sm font-medium text-[#4A4A4A] dark:text-white">María Fernández</td>
<td class="px-6 py-4 whitespace-nowrap text-sm text-gray-500 dark:text-gray-400">555-4321</td>
<td class="px-6 py-4 whitespace-nowrap text-sm text-gray-500 dark:text-gray-400">maria.fernandez@example.com</td>
<td class="px-6 py-4 whitespace-nowrap text-sm font-medium">
<a class="text-primary hover:text-blue-700 mr-4" href="#">Editar</a>
<a class="text-red-600 hover:text-red-800" href="#">Eliminar</a>
</td>
</tr>
<tr>
<td class="px-6 py-4 whitespace-nowrap text-sm font-medium text-[#4A4A4A] dark:text-white">Luis Torres</td>
<td class="px-6 py-4 whitespace-nowrap text-sm text-gray-500 dark:text-gray-400">555-9876</td>
<td class="px-6 py-4 whitespace-nowrap text-sm text-gray-500 dark:text-gray-400">luis.torres@example.com</td>
<td class="px-6 py-4 whitespace-nowrap text-sm font-medium">
<a class="text-primary hover:text-blue-700 mr-4" href="#">Editar</a>
<a class="text-red-600 hover:text-red-800" href="#">Eliminar</a>
</td>
</tr>
</tbody>
</table>
</div>
</div>
</div>
</div>
</div>
</div>
</div>
</body></html>