<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="GestionUsuarios.aspx.cs" Inherits="tp_c_equipo_3B.GestionUsuarios" %>

<!DOCTYPE html>

<html class="light" lang="es"><head>
<meta charset="utf-8"/>
<meta content="width=device-width, initial-scale=1.0" name="viewport"/>
<title>Administración de Usuarios</title>
<link href="https://fonts.googleapis.com/css2?family=Inter:wght@400;500;700;900&amp;display=swap" rel="stylesheet"/>
<link href="https://fonts.googleapis.com/css2?family=Material+Symbols+Outlined" rel="stylesheet"/>
<script src="https://cdn.tailwindcss.com?plugins=forms,container-queries"></script>
<script>
    tailwind.config = {
      darkMode: "class",
      theme: {
        extend: {
          colors: {
            "primary": "#4A90E2",
            "background-light": "#F4F5F7",
            "background-dark": "#101922",
            "text-light": "#333333",
            "text-dark": "#F4F5F7",
            "success": "#7ED321",
            "error": "#D0021B",
            "admin-role": "#9013FE",
            "sales-role": "#4A90E2"
          },
          fontFamily: {
            "display": ["Inter"]
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
<body class="bg-background-light dark:bg-background-dark font-display text-text-light dark:text-text-dark">
<div class="relative flex h-auto min-h-screen w-full flex-col group/design-root overflow-x-hidden">
<div class="layout-container flex h-full grow flex-col">
<header class="flex items-center justify-between whitespace-nowrap border-b border-solid border-b-gray-200 dark:border-b-gray-700 px-10 py-3 bg-white dark:bg-background-dark">
<div class="flex items-center gap-4">
<div class="size-8 text-primary">
<svg fill="none" viewbox="0 0 48 48" xmlns="http://www.w3.org/2000/svg">
<path d="M13.8261 17.4264C16.7203 18.1174 20.2244 18.5217 24 18.5217C27.7756 18.5217 31.2797 18.1174 34.1739 17.4264C36.9144 16.7722 39.9967 15.2331 41.3563 14.1648L24.8486 40.6391C24.4571 41.267 23.5429 41.267 23.1514 40.6391L6.64374 14.1648C8.00331 15.2331 11.0856 16.7722 13.8261 17.4264Z" fill="currentColor"></path>
<path clip-rule="evenodd" d="M39.998 12.236C39.9944 12.2537 39.9875 12.2845 39.9748 12.3294C39.9436 12.4399 39.8949 12.5741 39.8346 12.7175C39.8168 12.7597 39.7989 12.8007 39.7813 12.8398C38.5103 13.7113 35.9788 14.9393 33.7095 15.4811C30.9875 16.131 27.6413 16.5217 24 16.5217C20.3587 16.5217 17.0125 16.131 14.2905 15.4811C12.0012 14.9346 9.44505 13.6897 8.18538 12.8168C8.17384 12.7925 8.16216 12.767 8.15052 12.7408C8.09919 12.6249 8.05721 12.5114 8.02977 12.411C8.00356 12.3152 8.00039 12.2667 8.00004 12.2612C8.00004 12.261 8 12.2607 8.00004 12.2612C8.00004 12.2359 8.0104 11.9233 8.68485 11.3686C9.34546 10.8254 10.4222 10.2469 11.9291 9.72276C14.9242 8.68098 19.1919 8 24 8C28.8081 8 33.0758 8.68098 36.0709 9.72276C37.5778 10.2469 38.6545 10.8254 39.3151 11.3686C39.9006 11.8501 39.9857 12.1489 39.998 12.236ZM4.95178 15.2312L21.4543 41.6973C22.6288 43.5809 25.3712 43.5809 26.5457 41.6973L43.0534 15.223C43.0709 15.1948 43.0878 15.1662 43.104 15.1371L41.3563 14.1648C43.104 15.1371 43.1038 15.1374 43.104 15.1371L43.1051 15.135L43.1065 15.1325L43.1101 15.1261L43.1199 15.1082C43.1276 15.094 43.1377 15.0754 43.1497 15.0527C43.1738 15.0075 43.2062 14.9455 43.244 14.8701C43.319 14.7208 43.4196 14.511 43.5217 14.2683C43.6901 13.8679 44 13.0689 44 12.2609C44 10.5573 43.003 9.22254 41.8558 8.2791C40.6947 7.32427 39.1354 6.55361 37.385 5.94477C33.8654 4.72057 29.133 4 24 4C18.867 4 14.1346 4.72057 10.615 5.94478C8.86463 6.55361 7.30529 7.32428 6.14419 8.27911C4.99695 9.22255 3.99999 10.5573 3.99999 12.2609C3.99999 13.1275 4.29264 13.9078 4.49321 14.3607C4.60375 14.6102 4.71348 14.8196 4.79687 14.9689C4.83898 15.0444 4.87547 15.1065 4.9035 15.1529C4.91754 15.1762 4.92954 15.1957 4.93916 15.2111L4.94662 15.223L4.95178 15.2312ZM35.9868 18.996L24 38.22L12.0131 18.996C12.4661 19.1391 12.9179 19.2658 13.3617 19.3718C16.4281 20.1039 20.0901 20.5217 24 20.5217C27.9099 20.5217 31.5719 20.1039 34.6383 19.3718C35.082 19.2658 35.5339 19.1391 35.9868 18.996Z" fill="currentColor" fill-rule="evenodd"></path>
</svg>
</div>
<h2 class="text-text-light dark:text-text-dark text-lg font-bold leading-tight tracking-[-0.015em]">Administración de Usuarios</h2>
</div>
<div class="flex flex-1 justify-end gap-4 items-center">
<button class="flex min-w-[84px] max-w-[480px] cursor-pointer items-center justify-center overflow-hidden rounded-lg h-10 px-4 bg-primary text-white text-sm font-bold leading-normal tracking-[0.015em] hover:bg-primary/90 transition-colors">
<span class="truncate">Agregar Usuario</span>
</button>
<div class="relative">
<button class="bg-center bg-no-repeat aspect-square bg-cover rounded-full size-10" data-alt="User profile picture with initials JD" style='background-image: url("https://placeholder.pics/svg/300/F4F5F7/333333?text=JD");'></button>
</div>
</div>
</header>
<main class="flex-1 px-10 py-8">
<div class="layout-content-container flex flex-col max-w-[1280px] mx-auto">
<div class="flex flex-wrap justify-between items-center gap-4 mb-6">
<div class="flex flex-col gap-1">
<p class="text-3xl font-black leading-tight tracking-[-0.033em] text-text-light dark:text-text-dark">Usuarios</p>
<p class="text-gray-500 dark:text-gray-400 text-base font-normal leading-normal">Agrega, edita, y remueve usuarios, y asigna perfiles de seguridad.</p>
</div>
</div>
<div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm p-4 mb-6">
<div class="flex flex-col sm:flex-row gap-4">
<div class="flex-1">
<label class="flex flex-col min-w-40 h-12 w-full">
<div class="flex w-full flex-1 items-stretch rounded-lg h-full">
<div class="text-gray-500 dark:text-gray-400 flex border-none bg-background-light dark:bg-gray-700 items-center justify-center pl-4 rounded-l-lg border-r-0">
<span class="material-symbols-outlined">search</span>
</div>
<input class="form-input flex w-full min-w-0 flex-1 resize-none overflow-hidden rounded-r-lg text-text-light dark:text-text-dark focus:outline-0 focus:ring-0 border-none bg-background-light dark:bg-gray-700 h-full placeholder:text-gray-500 dark:placeholder:text-gray-400 px-4 text-base font-normal leading-normal" placeholder="Buscar por nombre o email" value=""/>
</div>
</label>
</div>
<div class="flex items-center gap-3 overflow-x-auto">
<button class="flex h-12 shrink-0 items-center justify-center gap-x-2 rounded-lg bg-background-light dark:bg-gray-700 px-4">
<p class="text-text-light dark:text-text-dark text-sm font-medium leading-normal">Todos</p>
<span class="material-symbols-outlined text-text-light dark:text-text-dark">expand_more</span>
</button>
<button class="flex h-12 shrink-0 items-center justify-center gap-x-2 rounded-lg bg-background-light dark:bg-gray-700 px-4">
<p class="text-text-light dark:text-text-dark text-sm font-medium leading-normal">Administrador</p>
</button>
<button class="flex h-12 shrink-0 items-center justify-center gap-x-2 rounded-lg bg-background-light dark:bg-gray-700 px-4">
<p class="text-text-light dark:text-text-dark text-sm font-medium leading-normal">Vendedor</p>
</button>
</div>
</div>
</div>
<div class="overflow-hidden rounded-lg border border-gray-200 dark:border-gray-700 bg-white dark:bg-gray-800 shadow-sm">
<table class="w-full">
<thead class="bg-background-light dark:bg-gray-700">
<tr>
<th class="p-4 text-left w-12"><input class="form-checkbox rounded text-primary focus:ring-primary/50" type="checkbox"/></th>
<th class="p-4 text-left text-sm font-medium text-text-light dark:text-text-dark">Nombre</th>
<th class="p-4 text-left text-sm font-medium text-text-light dark:text-text-dark">Correo Electrónico</th>
<th class="p-4 text-left text-sm font-medium text-text-light dark:text-text-dark">Rol</th>
<th class="p-4 text-left text-sm font-medium text-text-light dark:text-text-dark">Fecha de Creación</th>
<th class="p-4 text-left text-sm font-medium text-text-light dark:text-text-dark">Acciones</th>
</tr>
</thead>
<tbody class="divide-y divide-gray-200 dark:divide-gray-700">
<tr>
<td class="p-4"><input class="form-checkbox rounded text-primary focus:ring-primary/50" type="checkbox"/></td>
<td class="p-4 text-sm font-medium text-text-light dark:text-text-dark">Juan Pérez</td>
<td class="p-4 text-sm text-gray-500 dark:text-gray-400">juan.perez@example.com</td>
<td class="p-4"><span class="px-2 py-1 text-xs font-semibold rounded-full bg-admin-role/20 text-admin-role">Administrador</span></td>
<td class="p-4 text-sm text-gray-500 dark:text-gray-400">2023-01-15</td>
<td class="p-4 text-sm font-medium">
<div class="flex items-center gap-2">
<button class="text-primary hover:text-primary/80"><span class="material-symbols-outlined">edit</span></button>
<button class="text-error hover:text-error/80"><span class="material-symbols-outlined">delete</span></button>
<button class="text-gray-500 dark:text-gray-400 hover:text-gray-700 dark:hover:text-gray-200"><span class="material-symbols-outlined">vpn_key</span></button>
</div>
</td>
</tr>
<tr>
<td class="p-4"><input class="form-checkbox rounded text-primary focus:ring-primary/50" type="checkbox"/></td>
<td class="p-4 text-sm font-medium text-text-light dark:text-text-dark">Ana Gómez</td>
<td class="p-4 text-sm text-gray-500 dark:text-gray-400">ana.gomez@example.com</td>
<td class="p-4"><span class="px-2 py-1 text-xs font-semibold rounded-full bg-sales-role/20 text-sales-role">Vendedor</span></td>
<td class="p-4 text-sm text-gray-500 dark:text-gray-400">2023-02-20</td>
<td class="p-4 text-sm font-medium">
<div class="flex items-center gap-2">
<button class="text-primary hover:text-primary/80"><span class="material-symbols-outlined">edit</span></button>
<button class="text-error hover:text-error/80"><span class="material-symbols-outlined">delete</span></button>
<button class="text-gray-500 dark:text-gray-400 hover:text-gray-700 dark:hover:text-gray-200"><span class="material-symbols-outlined">vpn_key</span></button>
</div>
</td>
</tr>
<tr>
<td class="p-4"><input class="form-checkbox rounded text-primary focus:ring-primary/50" type="checkbox"/></td>
<td class="p-4 text-sm font-medium text-text-light dark:text-text-dark">Carlos Rodríguez</td>
<td class="p-4 text-sm text-gray-500 dark:text-gray-400">carlos.r@example.com</td>
<td class="p-4"><span class="px-2 py-1 text-xs font-semibold rounded-full bg-sales-role/20 text-sales-role">Vendedor</span></td>
<td class="p-4 text-sm text-gray-500 dark:text-gray-400">2023-03-10</td>
<td class="p-4 text-sm font-medium">
<div class="flex items-center gap-2">
<button class="text-primary hover:text-primary/80"><span class="material-symbols-outlined">edit</span></button>
<button class="text-error hover:text-error/80"><span class="material-symbols-outlined">delete</span></button>
<button class="text-gray-500 dark:text-gray-400 hover:text-gray-700 dark:hover:text-gray-200"><span class="material-symbols-outlined">vpn_key</span></button>
</div>
</td>
</tr>
<tr>
<td class="p-4"><input class="form-checkbox rounded text-primary focus:ring-primary/50" type="checkbox"/></td>
<td class="p-4 text-sm font-medium text-text-light dark:text-text-dark">Laura Martínez</td>
<td class="p-4 text-sm text-gray-500 dark:text-gray-400">laura.m@example.com</td>
<td class="p-4"><span class="px-2 py-1 text-xs font-semibold rounded-full bg-admin-role/20 text-admin-role">Administrador</span></td>
<td class="p-4 text-sm text-gray-500 dark:text-gray-400">2023-04-05</td>
<td class="p-4 text-sm font-medium">
<div class="flex items-center gap-2">
<button class="text-primary hover:text-primary/80"><span class="material-symbols-outlined">edit</span></button>
<button class="text-error hover:text-error/80"><span class="material-symbols-outlined">delete</span></button>
<button class="text-gray-500 dark:text-gray-400 hover:text-gray-700 dark:hover:text-gray-200"><span class="material-symbols-outlined">vpn_key</span></button>
</div>
</td>
</tr>
<tr>
<td class="p-4"><input class="form-checkbox rounded text-primary focus:ring-primary/50" type="checkbox"/></td>
<td class="p-4 text-sm font-medium text-text-light dark:text-text-dark">David Sánchez</td>
<td class="p-4 text-sm text-gray-500 dark:text-gray-400">david.s@example.com</td>
<td class="p-4"><span class="px-2 py-1 text-xs font-semibold rounded-full bg-sales-role/20 text-sales-role">Vendedor</span></td>
<td class="p-4 text-sm text-gray-500 dark:text-gray-400">2023-05-12</td>
<td class="p-4 text-sm font-medium">
<div class="flex items-center gap-2">
<button class="text-primary hover:text-primary/80"><span class="material-symbols-outlined">edit</span></button>
<button class="text-error hover:text-error/80"><span class="material-symbols-outlined">delete</span></button>
<button class="text-gray-500 dark:text-gray-400 hover:text-gray-700 dark:hover:text-gray-200"><span class="material-symbols-outlined">vpn_key</span></button>
</div>
</td>
</tr>
</tbody>
</table>
</div>
<div class="flex justify-between items-center mt-6">
<p class="text-sm text-gray-500 dark:text-gray-400">Mostrando 1-5 de 25 usuarios</p>
<div class="flex items-center gap-2">
<button class="p-2 rounded-lg hover:bg-gray-200 dark:hover:bg-gray-700 disabled:opacity-50" disabled=""><span class="material-symbols-outlined">chevron_left</span></button>
<button class="p-2 rounded-lg bg-primary/20 text-primary">1</button>
<button class="p-2 rounded-lg hover:bg-gray-200 dark:hover:bg-gray-700">2</button>
<button class="p-2 rounded-lg hover:bg-gray-200 dark:hover:bg-gray-700">3</button>
<span class="text-gray-500 dark:text-gray-400">...</span>
<button class="p-2 rounded-lg hover:bg-gray-200 dark:hover:bg-gray-700">5</button>
<button class="p-2 rounded-lg hover:bg-gray-200 dark:hover:bg-gray-700"><span class="material-symbols-outlined">chevron_right</span></button>
</div>
</div>
</div>
</main>
</div>
<!-- Modal para Agregar/Editar Usuario -->
<div class="fixed inset-0 bg-black/50 flex items-center justify-center hidden" id="user-modal">
<div class="bg-white dark:bg-gray-800 rounded-xl shadow-2xl p-8 w-full max-w-md m-4">
<h2 class="text-2xl font-bold mb-6 text-text-light dark:text-text-dark">Agregar Nuevo Usuario</h2>
<form>
<div class="grid grid-cols-1 md:grid-cols-2 gap-4 mb-4">
<div>
<label class="block text-sm font-medium text-gray-600 dark:text-gray-300 mb-1" for="firstName">Nombre</label>
<input class="form-input w-full rounded-lg border-gray-300 dark:border-gray-600 bg-background-light dark:bg-gray-700 text-text-light dark:text-text-dark focus:ring-primary focus:border-primary" id="firstName" type="text"/>
</div>
<div>
<label class="block text-sm font-medium text-gray-600 dark:text-gray-300 mb-1" for="lastName">Apellido</label>
<input class="form-input w-full rounded-lg border-gray-300 dark:border-gray-600 bg-background-light dark:bg-gray-700 text-text-light dark:text-text-dark focus:ring-primary focus:border-primary" id="lastName" type="text"/>
</div>
</div>
<div class="mb-4">
<label class="block text-sm font-medium text-gray-600 dark:text-gray-300 mb-1" for="email">Correo Electrónico</label>
<input class="form-input w-full rounded-lg border-gray-300 dark:border-gray-600 bg-background-light dark:bg-gray-700 text-text-light dark:text-text-dark focus:ring-primary focus:border-primary" id="email" type="email"/>
</div>
<div class="mb-4">
<label class="block text-sm font-medium text-gray-600 dark:text-gray-300 mb-1" for="password">Contraseña</label>
<input class="form-input w-full rounded-lg border-gray-300 dark:border-gray-600 bg-background-light dark:bg-gray-700 text-text-light dark:text-text-dark focus:ring-primary focus:border-primary" id="password" type="password"/>
</div>
<div class="mb-4">
<label class="block text-sm font-medium text-gray-600 dark:text-gray-300 mb-1" for="confirmPassword">Confirmar Contraseña</label>
<input class="form-input w-full rounded-lg border-gray-300 dark:border-gray-600 bg-background-light dark:bg-gray-700 text-text-light dark:text-text-dark focus:ring-primary focus:border-primary" id="confirmPassword" type="password"/>
</div>
<div class="mb-6">
<label class="block text-sm font-medium text-gray-600 dark:text-gray-300 mb-1" for="role">Rol</label>
<select class="form-select w-full rounded-lg border-gray-300 dark:border-gray-600 bg-background-light dark:bg-gray-700 text-text-light dark:text-text-dark focus:ring-primary focus:border-primary" id="role">
<option>Vendedor</option>
<option>Administrador</option>
</select>
</div>
<div class="flex justify-end gap-4">
<button class="px-6 py-2 rounded-lg text-sm font-medium bg-gray-200 dark:bg-gray-600 text-text-light dark:text-text-dark hover:bg-gray-300 dark:hover:bg-gray-500" type="button">Cancelar</button>
<button class="px-6 py-2 rounded-lg text-sm font-medium bg-primary text-white hover:bg-primary/90" type="submit">Guardar</button>
</div>
</form>
</div>
</div>
<!-- Modal de confirmación de eliminación -->
<div class="fixed inset-0 bg-black/50 flex items-center justify-center hidden" id="delete-confirm-modal">
<div class="bg-white dark:bg-gray-800 rounded-xl shadow-2xl p-8 w-full max-w-sm m-4 text-center">
<div class="w-16 h-16 rounded-full bg-error/10 flex items-center justify-center mx-auto mb-4">
<span class="material-symbols-outlined text-error text-4xl">delete</span>
</div>
<h2 class="text-xl font-bold mb-2 text-text-light dark:text-text-dark">¿Estás seguro?</h2>
<p class="text-gray-500 dark:text-gray-400 mb-6">¿Estás seguro de que quieres eliminar a <span class="font-bold">Juan Pérez</span>? Esta acción no se puede deshacer.</p>
<div class="flex justify-center gap-4">
<button class="px-6 py-2 rounded-lg text-sm font-medium bg-gray-200 dark:bg-gray-600 text-text-light dark:text-text-dark hover:bg-gray-300 dark:hover:bg-gray-500" type="button">Cancelar</button>
<button class="px-6 py-2 rounded-lg text-sm font-medium bg-error text-white hover:bg-error/90" type="button">Eliminar</button>
</div>
</div>
</div>
</div>
</body></html>