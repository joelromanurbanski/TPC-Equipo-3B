-- 1. Crear base de datos
CREATE DATABASE GestionNegocio;
GO
USE GestionNegocio;
GO

-- 2. Tablas de Catálogo
CREATE TABLE Marca (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Descripcion NVARCHAR(100) NOT NULL
);

CREATE TABLE Categoria (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Descripcion NVARCHAR(100) NOT NULL
);

CREATE TABLE Proveedor (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Nombre NVARCHAR(100) NOT NULL,
    Email NVARCHAR(100),
    Telefono NVARCHAR(50),
    Direccion NVARCHAR(200)
);

-- 3. Tabla Articulo
CREATE TABLE Articulo (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Codigo NVARCHAR(50) NOT NULL,
    Nombre NVARCHAR(100) NOT NULL,
    Descripcion NVARCHAR(500),
    UltimoPrecioCompra DECIMAL(18,2) NOT NULL DEFAULT 0,
    PorcentajeGanancia DECIMAL(18,2) NOT NULL DEFAULT 0,
    StockActual INT NOT NULL DEFAULT 0,
    StockMinimo INT NOT NULL DEFAULT 0,
    IdMarca INT NOT NULL,
    IdCategoria INT NOT NULL,
    UrlImagen NVARCHAR(500), -- Imagen principal
   
    FOREIGN KEY (IdMarca) REFERENCES Marca(Id),
    FOREIGN KEY (IdCategoria) REFERENCES Categoria(Id)
);
GO

-- 4. Tabla Imagen (Galería secundaria)
CREATE TABLE Imagen (
    Id INT PRIMARY KEY IDENTITY(1,1),
    UrlImagen NVARCHAR(250) NOT NULL,
    IdArticulo INT NOT NULL,
    FOREIGN KEY (IdArticulo) REFERENCES Articulo(Id)
);
GO

-- 5. Tabla ArticuloProveedor (Muchos-a-Muchos)
CREATE TABLE ArticuloProveedor (
    IdArticulo INT NOT NULL,
    IdProveedor INT NOT NULL,
    
    PRIMARY KEY (IdArticulo, IdProveedor),
    FOREIGN KEY (IdArticulo) REFERENCES Articulo(Id),
    FOREIGN KEY (IdProveedor) REFERENCES Proveedor(Id)
);
GO

-- 6. Tabla Cliente
CREATE TABLE Cliente (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Documento NVARCHAR(50) UNIQUE NOT NULL,
    Nombre NVARCHAR(100) NOT NULL,
    Apellido NVARCHAR(100) NOT NULL,
    Email NVARCHAR(100),
    Telefono NVARCHAR(100) NULL,
    Direccion NVARCHAR(200),
    Ciudad NVARCHAR(100),
    CP INT
);

-- 7. Tablas Transaccionales (Ventas y Compras)
CREATE TABLE Compra (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Fecha DATETIME NOT NULL DEFAULT GETDATE(),
    IdProveedor INT NOT NULL,
    TotalCompra DECIMAL(18,2),
    Estado NVARCHAR(50) NOT NULL DEFAULT 'Solicitado' -- ¡COLUMNA DE ESTADO!
);

CREATE TABLE DetalleCompra (
    Id INT PRIMARY KEY IDENTITY(1,1),
    IdCompra INT NOT NULL,
    IdArticulo INT NOT NULL,
    Cantidad INT NOT NULL,
    PrecioCompra DECIMAL(18,2) NOT NULL,
    FOREIGN KEY (IdCompra) REFERENCES Compra(Id),
    FOREIGN KEY (IdArticulo) REFERENCES Articulo(Id)
);

CREATE TABLE Venta (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Fecha DATETIME NOT NULL DEFAULT GETDATE(),
    IdCliente INT NOT NULL,
    NumeroFactura NVARCHAR(100) NOT NULL,
    TotalVenta DECIMAL(18,2),
    Estado NVARCHAR(50) NOT NULL DEFAULT 'En Preparación' -- ¡COLUMNA DE ESTADO!
);

CREATE TABLE DetalleVenta (
    Id INT PRIMARY KEY IDENTITY(1,1),
    IdVenta INT NOT NULL,
    IdArticulo INT NOT NULL,
    Cantidad INT NOT NULL,
    PrecioUnitario DECIMAL(18,2) NOT NULL,
    Subtotal DECIMAL(18,2) NOT NULL,
    FOREIGN KEY (IdVenta) REFERENCES Venta(Id),
    FOREIGN KEY (IdArticulo) REFERENCES Articulo(Id)
);
GO

-- 8. Tabla Usuario (Para login manual)
CREATE TABLE Usuario (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Email NVARCHAR(100) NOT NULL UNIQUE,
    Contrasenia NVARCHAR(255) NOT NULL,
    Nombre NVARCHAR(100),
    Apellido NVARCHAR(100),
    EsAdmin BIT NOT NULL DEFAULT 0 -- 1 = Admin, 0 = Vendedor
);
GO

-- ---------------------------------
-- INSERCIÓN DE DATOS DE EJEMPLO
-- ---------------------------------
PRINT 'Insertando Datos de Ejemplo...';
GO

-- Usuarios
INSERT INTO Usuario (Email, Contrasenia, Nombre, Apellido, EsAdmin)
VALUES ('admin@admin.com', 'admin', 'Admin', 'User', 1),
       ('vendedor@vendedor.com', 'vendedor', 'Vendedor', 'User', 0);
GO

-- Marcas y Categorías
INSERT INTO Marca (Descripcion) VALUES ('Samsung'), ('Apple'), ('Sony'), ('Lenovo'), ('Asus'), ('Gigabyte');
INSERT INTO Categoria (Descripcion) VALUES ('Celulares'), ('Auriculares'), ('Televisores'), ('Notebooks');
GO

-- 7 Proveedores
INSERT INTO Proveedor (Nombre, Email, Telefono, Direccion) 
VALUES 
('Argentina Color', 'ventas@argentinacolor.com.ar', '011 7700-4288', 'Juramento 5079, C1431 Cdad. Autónoma de Buenos Aires'),
('Biosistemas', 'ventas@biosistemas.com.ar', '011 4977-7538', 'CTH, Mariano Moreno 746, B6700 Luján, Provincia de Buenos Aires'),
('Fravega', 'cobranzas@fravega.com.ar', '0810-333-8700', 'Valentín Gomez 2813, CABA'),
('Compra Gamer', 'info@compragamer.com', 'No informado', 'No informado (Venta Online)'),
('Maximus Gaming', 'empresas@maximus.com.ar', '11 2357-5522', 'Florida 537 PB, Local 379, Microcentro, CABA'),
('XT-PC', 'ventas@XT-PC.com.ar', '011 5263-2796', 'Florida 537, Galería Jardín, Local 382, CABA'),
('HyperGaming', 'ventas@hypergaming.com.ar', '11 5725-9722', 'Av. Dr. Ricardo Balbín 3319, CABA');
GO

-- 4 Artículos (Stock 0, se carga con las compras)
INSERT INTO Articulo (Codigo, Nombre, Descripcion, IdMarca, IdCategoria, PorcentajeGanancia, StockMinimo, UrlImagen, StockActual, UltimoPrecioCompra)
VALUES 
('A001', 'Galaxy S23', 'Smartphone alta gama', 1, 1, 30.0, 5, 'galaxys23.webp', 0, 0),
('A002', 'Lenovo Thinkpad', 'Notebook gama media', 4, 4, 25.0, 3, 'LenovoThinkPad.jpg', 0, 0),
('A003', 'ASUS VivoBook 15.6', 'Notebook gama media', 5, 4, 25.0, 3, 'AsusVivoBook.jpg', 0, 0),
('A004', 'Poco X7 Pro', 'Celular gama media', 1, 1, 20.0, 7, 'pocox7.webp', 0, 0);
GO

-- Relaciones Articulo-Proveedor
INSERT INTO ArticuloProveedor (IdArticulo, IdProveedor) VALUES (1, 1), (1, 2), (2, 4), (3, 6), (4, 2);
GO

-- 10 Clientes
INSERT INTO Cliente (Documento, Nombre, Apellido, Email, Telefono, Direccion, Ciudad, CP)
VALUES
('28.123.456', 'Carlos', 'Gomez', 'carlos.gomez@email.com', '11 4567-8901', 'Av. Rivadavia 2030', 'CABA', 1033),
('34.567.890', 'Laura', 'Martinez', 'laura.martinez@email.com', '11 3322-1144', 'Calle 12 N° 567', 'La Plata', 1900),
('25.987.654', 'Miguel', 'Rodriguez', 'miguel.r@email.com', '0351 488-9021', 'Av. Colón 1500', 'Córdoba', 5000),
('30.111.222', 'Ana', 'Fernandez', 'ana.fernandez@email.com', '0341 455-6789', 'Bv. Oroño 300', 'Rosario', 2000),
('38.765.432', 'Diego', 'Alvarez', 'diego.alvarez@email.com', '11 5566-7788', 'Av. Maipú 850', 'Vicente López', 1638),
('29.333.444', 'Sofia', 'Torres', 'sofia.torres@email.com', '0223 472-3344', 'Av. Luro 3010', 'Mar del Plata', 7600),
('32.888.999', 'Julieta', 'Diaz', 'julieta.diaz@email.com', '11 6789-0123', 'Av. Santa Fe 2500', 'CABA', 1425),
('27.555.666', 'Martin', 'Sanchez', 'martin.sanchez@email.com', '0381 430-1122', '25 de Mayo 550', 'San Miguel de Tucumán', 4000),
('36.222.111', 'Valentina', 'Romero', 'valentina.romero@email.com', '0261 425-9988', 'Av. San Martín 1020', 'Mendoza', 5500),
('24.777.888', 'Lucia', 'Perez', 'lucia.perez@email.com', '11 4999-0011', 'Av. Corrientes 4500', 'CABA', 1195);
GO

-- ---------------------------------
-- 10. TRANSACCIONES (TODAS JUNTAS)
-- ---------------------------------
PRINT 'Insertando Transacciones...';
GO

DECLARE @LastCompraID int;
DECLARE @LastVentaID int;

-- COMPRAS (7 en total)
INSERT INTO Compra (Fecha, IdProveedor, TotalCompra, Estado) VALUES ('2025-10-01', 1, 7400000.00, 'Recibido');
SET @LastCompraID = SCOPE_IDENTITY();
INSERT INTO DetalleCompra (IdCompra, IdArticulo, Cantidad, PrecioCompra) VALUES (@LastCompraID, 1, 10, 740000.00);
UPDATE Articulo SET StockActual = StockActual + 10, UltimoPrecioCompra = 740000.00 WHERE Id = 1;

INSERT INTO Compra (Fecha, IdProveedor, TotalCompra, Estado) VALUES ('2025-10-05', 4, 1950000.00, 'Recibido');
SET @LastCompraID = SCOPE_IDENTITY();
INSERT INTO DetalleCompra (IdCompra, IdArticulo, Cantidad, PrecioCompra) VALUES (@LastCompraID, 2, 5, 390000.00);
UPDATE Articulo SET StockActual = StockActual + 5, UltimoPrecioCompra = 390000.00 WHERE Id = 2;

INSERT INTO Compra (Fecha, IdProveedor, TotalCompra, Estado) VALUES ('2025-11-13', 2, 5920089.9, 'Recibido');
SET @LastCompraID = SCOPE_IDENTITY();
INSERT INTO DetalleCompra (IdCompra, IdArticulo, Cantidad, PrecioCompra) VALUES (@LastCompraID, 2, 15, 394672.66);
UPDATE Articulo SET StockActual = StockActual + 15, UltimoPrecioCompra = 394672.66 WHERE Id = 2;

INSERT INTO Compra (Fecha, IdProveedor, TotalCompra, Estado) VALUES ('2025-11-14', 1, 15368281.2, 'Recibido');
SET @LastCompraID = SCOPE_IDENTITY();
INSERT INTO DetalleCompra (IdCompra, IdArticulo, Cantidad, PrecioCompra) VALUES (@LastCompraID, 1, 20, 768414.06);
UPDATE Articulo SET StockActual = StockActual + 20, UltimoPrecioCompra = 768414.06 WHERE Id = 1;

INSERT INTO Compra (Fecha, IdProveedor, TotalCompra, Estado) VALUES ('2025-11-15', 6, 11168500.64, 'Recibido');
SET @LastCompraID = SCOPE_IDENTITY();
INSERT INTO DetalleCompra (IdCompra, IdArticulo, Cantidad, PrecioCompra) VALUES (@LastCompraID, 4, 16, 698031.29);
UPDATE Articulo SET StockActual = StockActual + 16, UltimoPrecioCompra = 698031.29 WHERE Id = 4;

INSERT INTO Compra (Fecha, IdProveedor, TotalCompra, Estado) VALUES ('2025-11-16', 3, 10124293.92, 'Recibido');
SET @LastCompraID = SCOPE_IDENTITY();
INSERT INTO DetalleCompra (IdCompra, IdArticulo, Cantidad, PrecioCompra) VALUES (@LastCompraID, 1, 13, 778791.84);
UPDATE Articulo SET StockActual = StockActual + 13, UltimoPrecioCompra = 778791.84 WHERE Id = 1;

INSERT INTO Compra (Fecha, IdProveedor, TotalCompra, Estado) VALUES ('2025-11-17', 7, 10692402.3, 'Recibido');
SET @LastCompraID = SCOPE_IDENTITY();
INSERT INTO DetalleCompra (IdCompra, IdArticulo, Cantidad, PrecioCompra) VALUES (@LastCompraID, 4, 15, 712826.82);
UPDATE Articulo SET StockActual = StockActual + 15, UltimoPrecioCompra = 712826.82 WHERE Id = 4;

-- VENTAS (50 en total)
INSERT INTO Venta (Fecha, IdCliente, NumeroFactura, TotalVenta, Estado) VALUES ('2025-11-01', 1, 'F-0001', 975000.00, 'Entregado'); 
SET @LastVentaID = SCOPE_IDENTITY();
INSERT INTO DetalleVenta (IdVenta, IdArticulo, Cantidad, PrecioUnitario, Subtotal) VALUES (@LastVentaID, 1, 1, 975000.00, 975000.00);
UPDATE Articulo SET StockActual = StockActual - 1 WHERE Id = 1;

INSERT INTO Venta (Fecha, IdCliente, NumeroFactura, TotalVenta, Estado) VALUES ('2025-11-02', 2, 'F-0002', 1125000.00, 'Entregado'); 
SET @LastVentaID = SCOPE_IDENTITY();
INSERT INTO DetalleVenta (IdVenta, IdArticulo, Cantidad, PrecioUnitario, Subtotal) VALUES (@LastVentaID, 3, 2, 562500.00, 1125000.00);
UPDATE Articulo SET StockActual = StockActual - 2 WHERE Id = 3;

INSERT INTO Venta (Fecha, IdCliente, NumeroFactura, TotalVenta, Estado) VALUES ('2025-11-13', 1, 'FAC-20251113-3590', 1035024.54, 'Enviado');
SET @LastVentaID = SCOPE_IDENTITY();
INSERT INTO DetalleVenta (IdVenta, IdArticulo, Cantidad, PrecioUnitario, Subtotal) VALUES (@LastVentaID, 4, 1, 855392.18, 855392.18);
UPDATE Articulo SET StockActual = StockActual - 1 WHERE Id = 4;

INSERT INTO Venta (Fecha, IdCliente, NumeroFactura, TotalVenta, Estado) VALUES ('2025-11-14', 1, 'FAC-20251114-3629', 1035024.54, 'Enviado');
SET @LastVentaID = SCOPE_IDENTITY();
INSERT INTO DetalleVenta (IdVenta, IdArticulo, Cantidad, PrecioUnitario, Subtotal) VALUES (@LastVentaID, 4, 1, 855392.18, 855392.18);
UPDATE Articulo SET StockActual = StockActual - 1 WHERE Id = 4;

INSERT INTO Venta (Fecha, IdCliente, NumeroFactura, TotalVenta, Estado) VALUES ('2025-11-15', 2, 'FAC-20251115-7701', 3105073.61, 'Entregado');
SET @LastVentaID = SCOPE_IDENTITY();
INSERT INTO DetalleVenta (IdVenta, IdArticulo, Cantidad, PrecioUnitario, Subtotal) VALUES (@LastVentaID, 4, 3, 855392.18, 2566176.54);
UPDATE Articulo SET StockActual = StockActual - 3 WHERE Id = 4;

INSERT INTO Venta (Fecha, IdCliente, NumeroFactura, TotalVenta, Estado) VALUES ('2025-11-16', 1, 'FAC-20251116-3119', 1225039.56, 'Cancelado');
SET @LastVentaID = SCOPE_IDENTITY();
INSERT INTO DetalleVenta (IdVenta, IdArticulo, Cantidad, PrecioUnitario, Subtotal) VALUES (@LastVentaID, 1, 1, 1012429.39, 1012429.39);
UPDATE Articulo SET StockActual = StockActual - 1 WHERE Id = 1;

INSERT INTO Venta (Fecha, IdCliente, NumeroFactura, TotalVenta, Estado) VALUES ('2025-11-17', 7, 'FAC-20251117-2841', 1306800.0, 'Entregado');
SET @LastVentaID = SCOPE_IDENTITY();
INSERT INTO DetalleVenta (IdVenta, IdArticulo, Cantidad, PrecioUnitario, Subtotal) VALUES (@LastVentaID, 3, 2, 540000.0, 1080000.0);
UPDATE Articulo SET StockActual = StockActual - 2 WHERE Id = 3;

INSERT INTO Venta (Fecha, IdCliente, NumeroFactura, TotalVenta, Estado) VALUES ('2025-11-18', 7, 'FAC-20251118-3864', 1193884.78, 'Enviado');
SET @LastVentaID = SCOPE_IDENTITY();
INSERT INTO DetalleVenta (IdVenta, IdArticulo, Cantidad, PrecioUnitario, Subtotal) VALUES (@LastVentaID, 2, 2, 493340.82, 986681.64);
UPDATE Articulo SET StockActual = StockActual - 2 WHERE Id = 2;

INSERT INTO Venta (Fecha, IdCliente, NumeroFactura, TotalVenta, Estado) VALUES ('2025-11-19', 3, 'FAC-20251119-6656', 3675118.69, 'Listo para Despachar');
SET @LastVentaID = SCOPE_IDENTITY();
INSERT INTO DetalleVenta (IdVenta, IdArticulo, Cantidad, PrecioUnitario, Subtotal) VALUES (@LastVentaID, 1, 3, 1012429.39, 3037288.17);
UPDATE Articulo SET StockActual = StockActual - 3 WHERE Id = 1;

INSERT INTO Venta (Fecha, IdCliente, NumeroFactura, TotalVenta, Estado) VALUES ('2025-11-20', 5, 'FAC-20251120-2893', 2070049.08, 'En Preparación');
SET @LastVentaID = SCOPE_IDENTITY();
INSERT INTO DetalleVenta (IdVenta, IdArticulo, Cantidad, PrecioUnitario, Subtotal) VALUES (@LastVentaID, 4, 2, 855392.18, 1710784.36);
UPDATE Articulo SET StockActual = StockActual - 2 WHERE Id = 4;

INSERT INTO Venta (Fecha, IdCliente, NumeroFactura, TotalVenta, Estado) VALUES ('2025-11-21', 9, 'FAC-20251121-7406', 1193884.78, 'En Preparación');
SET @LastVentaID = SCOPE_IDENTITY();
INSERT INTO DetalleVenta (IdVenta, IdArticulo, Cantidad, PrecioUnitario, Subtotal) VALUES (@LastVentaID, 2, 2, 493340.82, 986681.64);
UPDATE Articulo SET StockActual = StockActual - 2 WHERE Id = 2;

INSERT INTO Venta (Fecha, IdCliente, NumeroFactura, TotalVenta, Estado) VALUES ('2025-11-22', 1, 'FAC-20251122-4868', 1193884.78, 'En Preparación');
SET @LastVentaID = SCOPE_IDENTITY();
INSERT INTO DetalleVenta (IdVenta, IdArticulo, Cantidad, PrecioUnitario, Subtotal) VALUES (@LastVentaID, 2, 2, 493340.82, 986681.64);
UPDATE Articulo SET StockActual = StockActual - 2 WHERE Id = 2;

INSERT INTO Venta (Fecha, IdCliente, NumeroFactura, TotalVenta, Estado) VALUES ('2025-11-23', 7, 'FAC-20251123-9108', 1306800.0, 'En Preparación');
SET @LastVentaID = SCOPE_IDENTITY();
INSERT INTO DetalleVenta (IdVenta, IdArticulo, Cantidad, PrecioUnitario, Subtotal) VALUES (@LastVentaID, 3, 2, 540000.0, 1080000.0);
UPDATE Articulo SET StockActual = StockActual - 2 WHERE Id = 3;

INSERT INTO Venta (Fecha, IdCliente, NumeroFactura, TotalVenta, Estado) VALUES ('2025-11-24', 4, 'FAC-20251124-7360', 3675118.69, 'En Preparación');
SET @LastVentaID = SCOPE_IDENTITY();
INSERT INTO DetalleVenta (IdVenta, IdArticulo, Cantidad, PrecioUnitario, Subtotal) VALUES (@LastVentaID, 1, 3, 1012429.39, 3037288.17);
UPDATE Articulo SET StockActual = StockActual - 3 WHERE Id = 1;

INSERT INTO Venta (Fecha, IdCliente, NumeroFactura, TotalVenta, Estado) VALUES ('2025-11-25', 4, 'FAC-20251125-2034', 3675118.69, 'En Preparación');
SET @LastVentaID = SCOPE_IDENTITY();
INSERT INTO DetalleVenta (IdVenta, IdArticulo, Cantidad, PrecioUnitario, Subtotal) VALUES (@LastVentaID, 1, 3, 1012429.39, 3037288.17);
UPDATE Articulo SET StockActual = StockActual - 3 WHERE Id = 1;

INSERT INTO Venta (Fecha, IdCliente, NumeroFactura, TotalVenta, Estado) VALUES ('2025-11-26', 4, 'FAC-20251126-1870', 596942.39, 'En Preparación');
SET @LastVentaID = SCOPE_IDENTITY();
INSERT INTO DetalleVenta (IdVenta, IdArticulo, Cantidad, PrecioUnitario, Subtotal) VALUES (@LastVentaID, 2, 1, 493340.82, 493340.82);
UPDATE Articulo SET StockActual = StockActual - 1 WHERE Id = 2;

INSERT INTO Venta (Fecha, IdCliente, NumeroFactura, TotalVenta, Estado) VALUES ('2025-11-27', 5, 'FAC-20251127-3694', 653400.0, 'En Preparación');
SET @LastVentaID = SCOPE_IDENTITY();
INSERT INTO DetalleVenta (IdVenta, IdArticulo, Cantidad, PrecioUnitario, Subtotal) VALUES (@LastVentaID, 3, 1, 540000.0, 540000.0);
UPDATE Articulo SET StockActual = StockActual - 1 WHERE Id = 3;

INSERT INTO Venta (Fecha, IdCliente, NumeroFactura, TotalVenta, Estado) VALUES ('2025-11-28', 10, 'FAC-20251128-3745', 2450079.12, 'En Preparación');
SET @LastVentaID = SCOPE_IDENTITY();
INSERT INTO DetalleVenta (IdVenta, IdArticulo, Cantidad, PrecioUnitario, Subtotal) VALUES (@LastVentaID, 1, 2, 1012429.39, 2024858.78);
UPDATE Articulo SET StockActual = StockActual - 2 WHERE Id = 1;

INSERT INTO Venta (Fecha, IdCliente, NumeroFactura, TotalVenta, Estado) VALUES ('2025-11-28', 7, 'FAC-20251128-7138', 1035024.54, 'En Preparación');
SET @LastVentaID = SCOPE_IDENTITY();
INSERT INTO DetalleVenta (IdVenta, IdArticulo, Cantidad, PrecioUnitario, Subtotal) VALUES (@LastVentaID, 4, 1, 855392.18, 855392.18);
UPDATE Articulo SET StockActual = StockActual - 1 WHERE Id = 4;

INSERT INTO Venta (Fecha, IdCliente, NumeroFactura, TotalVenta, Estado) VALUES ('2025-11-28', 4, 'FAC-20251128-4572', 1035024.54, 'En Preparación');
SET @LastVentaID = SCOPE_IDENTITY();
INSERT INTO DetalleVenta (IdVenta, IdArticulo, Cantidad, PrecioUnitario, Subtotal) VALUES (@LastVentaID, 4, 1, 855392.18, 855392.18);
UPDATE Articulo SET StockActual = StockActual - 1 WHERE Id = 4;

INSERT INTO Venta (Fecha, IdCliente, NumeroFactura, TotalVenta, Estado) VALUES ('2025-11-29', 2, 'FAC-20251129-2258', 3675118.69, 'En Preparación');
SET @LastVentaID = SCOPE_IDENTITY();
INSERT INTO DetalleVenta (IdVenta, IdArticulo, Cantidad, PrecioUnitario, Subtotal) VALUES (@LastVentaID, 1, 3, 1012429.39, 3037288.17);
UPDATE Articulo SET StockActual = StockActual - 3 WHERE Id = 1;

INSERT INTO Venta (Fecha, IdCliente, NumeroFactura, TotalVenta, Estado) VALUES ('2025-11-29', 7, 'FAC-20251129-8338', 1960200.0, 'En Preparación');
SET @LastVentaID = SCOPE_IDENTITY();
INSERT INTO DetalleVenta (IdVenta, IdArticulo, Cantidad, PrecioUnitario, Subtotal) VALUES (@LastVentaID, 3, 3, 540000.0, 1620000.0);
UPDATE Articulo SET StockActual = StockActual - 3 WHERE Id = 3;

INSERT INTO Venta (Fecha, IdCliente, NumeroFactura, TotalVenta, Estado) VALUES ('2025-11-29', 6, 'FAC-20251129-7865', 1193884.78, 'En Preparación');
SET @LastVentaID = SCOPE_IDENTITY();
INSERT INTO DetalleVenta (IdVenta, IdArticulo, Cantidad, PrecioUnitario, Subtotal) VALUES (@LastVentaID, 2, 2, 493340.82, 986681.64);
UPDATE Articulo SET StockActual = StockActual - 2 WHERE Id = 2;

INSERT INTO Venta (Fecha, IdCliente, NumeroFactura, TotalVenta, Estado) VALUES ('2025-11-30', 9, 'FAC-20251130-3086', 3675118.69, 'En Preparación');
SET @LastVentaID = SCOPE_IDENTITY();
INSERT INTO DetalleVenta (IdVenta, IdArticulo, Cantidad, PrecioUnitario, Subtotal) VALUES (@LastVentaID, 1, 3, 1012429.39, 3037288.17);
UPDATE Articulo SET StockActual = StockActual - 3 WHERE Id = 1;

INSERT INTO Venta (Fecha, IdCliente, NumeroFactura, TotalVenta, Estado) VALUES ('2025-11-30', 10, 'FAC-20251130-8721', 1193884.78, 'En Preparación');
SET @LastVentaID = SCOPE_IDENTITY();
INSERT INTO DetalleVenta (IdVenta, IdArticulo, Cantidad, PrecioUnitario, Subtotal) VALUES (@LastVentaID, 2, 2, 493340.82, 986681.64);
UPDATE Articulo SET StockActual = StockActual - 2 WHERE Id = 2;

INSERT INTO Venta (Fecha, IdCliente, NumeroFactura, TotalVenta, Estado) VALUES ('2025-11-30', 2, 'FAC-20251130-9231', 2450079.12, 'En Preparación');
SET @LastVentaID = SCOPE_IDENTITY();
INSERT INTO DetalleVenta (IdVenta, IdArticulo, Cantidad, PrecioUnitario, Subtotal) VALUES (@LastVentaID, 1, 2, 1012429.39, 2024858.78);
UPDATE Articulo SET StockActual = StockActual - 2 WHERE Id = 1;

INSERT INTO Venta (Fecha, IdCliente, NumeroFactura, TotalVenta, Estado) VALUES ('2025-12-01', 7, 'FAC-20251201-8869', 1193884.78, 'En Preparación');
SET @LastVentaID = SCOPE_IDENTITY();
INSERT INTO DetalleVenta (IdVenta, IdArticulo, Cantidad, PrecioUnitario, Subtotal) VALUES (@LastVentaID, 2, 2, 493340.82, 986681.64);
UPDATE Articulo SET StockActual = StockActual - 2 WHERE Id = 2;

INSERT INTO Venta (Fecha, IdCliente, NumeroFactura, TotalVenta, Estado) VALUES ('2025-12-01', 7, 'FAC-20251201-2129', 1790827.18, 'En Preparación');
SET @LastVentaID = SCOPE_IDENTITY();
INSERT INTO DetalleVenta (IdVenta, IdArticulo, Cantidad, PrecioUnitario, Subtotal) VALUES (@LastVentaID, 2, 3, 493340.82, 1480022.46);
UPDATE Articulo SET StockActual = StockActual - 3 WHERE Id = 2;

INSERT INTO Venta (Fecha, IdCliente, NumeroFactura, TotalVenta, Estado) VALUES ('2025-12-01', 8, 'FAC-20251201-1652', 1193884.78, 'En Preparación');
SET @LastVentaID = SCOPE_IDENTITY();
INSERT INTO DetalleVenta (IdVenta, IdArticulo, Cantidad, PrecioUnitario, Subtotal) VALUES (@LastVentaID, 2, 2, 493340.82, 986681.64);
UPDATE Articulo SET StockActual = StockActual - 2 WHERE Id = 2;

INSERT INTO Venta (Fecha, IdCliente, NumeroFactura, TotalVenta, Estado) VALUES ('2025-12-02', 8, 'FAC-20251202-1854', 2450079.12, 'En Preparación');
SET @LastVentaID = SCOPE_IDENTITY();
INSERT INTO DetalleVenta (IdVenta, IdArticulo, Cantidad, PrecioUnitario, Subtotal) VALUES (@LastVentaID, 1, 2, 1012429.39, 2024858.78);
UPDATE Articulo SET StockActual = StockActual - 2 WHERE Id = 1;

INSERT INTO Venta (Fecha, IdCliente, NumeroFactura, TotalVenta, Estado) VALUES ('2025-12-02', 8, 'FAC-20251202-6824', 1790827.18, 'En Preparación');
SET @LastVentaID = SCOPE_IDENTITY();
INSERT INTO DetalleVenta (IdVenta, IdArticulo, Cantidad, PrecioUnitario, Subtotal) VALUES (@LastVentaID, 2, 3, 493340.82, 1480022.46);
UPDATE Articulo SET StockActual = StockActual - 3 WHERE Id = 2;

INSERT INTO Venta (Fecha, IdCliente, NumeroFactura, TotalVenta, Estado) VALUES ('2025-12-02', 9, 'FAC-20251202-6399', 3675118.69, 'En Preparación');
SET @LastVentaID = SCOPE_IDENTITY();
INSERT INTO DetalleVenta (IdVenta, IdArticulo, Cantidad, PrecioUnitario, Subtotal) VALUES (@LastVentaID, 1, 3, 1012429.39, 3037288.17);
UPDATE Articulo SET StockActual = StockActual - 3 WHERE Id = 1;

INSERT INTO Venta (Fecha, IdCliente, NumeroFactura, TotalVenta, Estado) VALUES ('2025-12-03', 1, 'FAC-20251203-8064', 1225039.56, 'En Preparación');
SET @LastVentaID = SCOPE_IDENTITY();
INSERT INTO DetalleVenta (IdVenta, IdArticulo, Cantidad, PrecioUnitario, Subtotal) VALUES (@LastVentaID, 1, 1, 1012429.39, 1012429.39);
UPDATE Articulo SET StockActual = StockActual - 1 WHERE Id = 1;

INSERT INTO Venta (Fecha, IdCliente, NumeroFactura, TotalVenta, Estado) VALUES ('2025-12-03', 4, 'FAC-20251203-3858', 1306800.0, 'En Preparación');
SET @LastVentaID = SCOPE_IDENTITY();
INSERT INTO DetalleVenta (IdVenta, IdArticulo, Cantidad, PrecioUnitario, Subtotal) VALUES (@LastVentaID, 3, 2, 540000.0, 1080000.0);
UPDATE Articulo SET StockActual = StockActual - 2 WHERE Id = 3;

INSERT INTO Venta (Fecha, IdCliente, NumeroFactura, TotalVenta, Estado) VALUES ('2025-12-03', 8, 'FAC-20251203-4991', 3105073.61, 'En Preparación');
SET @LastVentaID = SCOPE_IDENTITY();
INSERT INTO DetalleVenta (IdVenta, IdArticulo, Cantidad, PrecioUnitario, Subtotal) VALUES (@LastVentaID, 4, 3, 855392.18, 2566176.54);
UPDATE Articulo SET StockActual = StockActual - 3 WHERE Id = 4;

INSERT INTO Venta (Fecha, IdCliente, NumeroFactura, TotalVenta, Estado) VALUES ('2025-12-04', 7, 'FAC-20251204-5498', 653400.0, 'En Preparación');
SET @LastVentaID = SCOPE_IDENTITY();
INSERT INTO DetalleVenta (IdVenta, IdArticulo, Cantidad, PrecioUnitario, Subtotal) VALUES (@LastVentaID, 3, 1, 540000.0, 540000.0);
UPDATE Articulo SET StockActual = StockActual - 1 WHERE Id = 3;

INSERT INTO Venta (Fecha, IdCliente, NumeroFactura, TotalVenta, Estado) VALUES ('2025-12-04', 4, 'FAC-20251204-2955', 653400.0, 'En Preparación');
SET @LastVentaID = SCOPE_IDENTITY();
INSERT INTO DetalleVenta (IdVenta, IdArticulo, Cantidad, PrecioUnitario, Subtotal) VALUES (@LastVentaID, 3, 1, 540000.0, 540000.0);
UPDATE Articulo SET StockActual = StockActual - 1 WHERE Id = 3;

INSERT INTO Venta (Fecha, IdCliente, NumeroFactura, TotalVenta, Estado) VALUES ('2025-12-05', 7, 'FAC-20251205-3325', 596942.39, 'En Preparación');
SET @LastVentaID = SCOPE_IDENTITY();
INSERT INTO DetalleVenta (IdVenta, IdArticulo, Cantidad, PrecioUnitario, Subtotal) VALUES (@LastVentaID, 2, 1, 493340.82, 493340.82);
UPDATE Articulo SET StockActual = StockActual - 1 WHERE Id = 2;

INSERT INTO Venta (Fecha, IdCliente, NumeroFactura, TotalVenta, Estado) VALUES ('2025-12-05', 6, 'FAC-20251205-6835', 596942.39, 'En Preparación');
SET @LastVentaID = SCOPE_IDENTITY();
INSERT INTO DetalleVenta (IdVenta, IdArticulo, Cantidad, PrecioUnitario, Subtotal) VALUES (@LastVentaID, 2, 1, 493340.82, 493340.82);
UPDATE Articulo SET StockActual = StockActual - 1 WHERE Id = 2;

INSERT INTO Venta (Fecha, IdCliente, NumeroFactura, TotalVenta, Estado) VALUES ('2025-12-06', 6, 'FAC-20251206-7933', 1035024.54, 'En Preparación');
SET @LastVentaID = SCOPE_IDENTITY();
INSERT INTO DetalleVenta (IdVenta, IdArticulo, Cantidad, PrecioUnitario, Subtotal) VALUES (@LastVentaID, 4, 1, 855392.18, 855392.18);
UPDATE Articulo SET StockActual = StockActual - 1 WHERE Id = 4;

INSERT INTO Venta (Fecha, IdCliente, NumeroFactura, TotalVenta, Estado) VALUES ('2025-12-06', 4, 'FAC-20251206-7554', 653400.0, 'En Preparación');
SET @LastVentaID = SCOPE_IDENTITY();
INSERT INTO DetalleVenta (IdVenta, IdArticulo, Cantidad, PrecioUnitario, Subtotal) VALUES (@LastVentaID, 3, 1, 540000.0, 540000.0);
UPDATE Articulo SET StockActual = StockActual - 1 WHERE Id = 3;

INSERT INTO Venta (Fecha, IdCliente, NumeroFactura, TotalVenta, Estado) VALUES ('2025-12-07', 3, 'FAC-20251207-9606', 596942.39, 'En Preparación');
SET @LastVentaID = SCOPE_IDENTITY();
INSERT INTO DetalleVenta (IdVenta, IdArticulo, Cantidad, PrecioUnitario, Subtotal) VALUES (@LastVentaID, 2, 1, 493340.82, 493340.82);
UPDATE Articulo SET StockActual = StockActual - 1 WHERE Id = 2;

INSERT INTO Venta (Fecha, IdCliente, NumeroFactura, TotalVenta, Estado) VALUES ('2025-12-07', 10, 'FAC-20251207-1944', 1225039.56, 'En Preparación');
SET @LastVentaID = SCOPE_IDENTITY();
INSERT INTO DetalleVenta (IdVenta, IdArticulo, Cantidad, PrecioUnitario, Subtotal) VALUES (@LastVentaID, 1, 1, 1012429.39, 1012429.39);
UPDATE Articulo SET StockActual = StockActual - 1 WHERE Id = 1;

INSERT INTO Venta (Fecha, IdCliente, NumeroFactura, TotalVenta, Estado) VALUES ('2025-12-08', 7, 'FAC-20251208-4250', 1225039.56, 'En Preparación');
SET @LastVentaID = SCOPE_IDENTITY();
INSERT INTO DetalleVenta (IdVenta, IdArticulo, Cantidad, PrecioUnitario, Subtotal) VALUES (@LastVentaID, 1, 1, 1012429.39, 1012429.39);
UPDATE Articulo SET StockActual = StockActual - 1 WHERE Id = 1;

INSERT INTO Venta (Fecha, IdCliente, NumeroFactura, TotalVenta, Estado) VALUES ('2025-12-08', 3, 'FAC-20251208-1338', 596942.39, 'En Preparación');
SET @LastVentaID = SCOPE_IDENTITY();
INSERT INTO DetalleVenta (IdVenta, IdArticulo, Cantidad, PrecioUnitario, Subtotal) VALUES (@LastVentaID, 2, 1, 493340.82, 493340.82);
UPDATE Articulo SET StockActual = StockActual - 1 WHERE Id = 2;

INSERT INTO Venta (Fecha, IdCliente, NumeroFactura, TotalVenta, Estado) VALUES ('2025-12-09', 9, 'FAC-20251209-7005', 653400.0, 'En Preparación');
SET @LastVentaID = SCOPE_IDENTITY();
INSERT INTO DetalleVenta (IdVenta, IdArticulo, Cantidad, PrecioUnitario, Subtotal) VALUES (@LastVentaID, 3, 1, 540000.0, 540000.0);
UPDATE Articulo SET StockActual = StockActual - 1 WHERE Id = 3;

INSERT INTO Venta (Fecha, IdCliente, NumeroFactura, TotalVenta, Estado) VALUES ('2025-12-09', 10, 'FAC-20251209-9122', 653400.0, 'En Preparación');
SET @LastVentaID = SCOPE_IDENTITY();
INSERT INTO DetalleVenta (IdVenta, IdArticulo, Cantidad, PrecioUnitario, Subtotal) VALUES (@LastVentaID, 3, 1, 540000.0, 540000.0);
UPDATE Articulo SET StockActual = StockActual - 1 WHERE Id = 3;

INSERT INTO Venta (Fecha, IdCliente, NumeroFactura, TotalVenta, Estado) VALUES ('2025-12-10', 4, 'FAC-20251210-8329', 653400.0, 'En Preparación');
SET @LastVentaID = SCOPE_IDENTITY();
INSERT INTO DetalleVenta (IdVenta, IdArticulo, Cantidad, PrecioUnitario, Subtotal) VALUES (@LastVentaID, 3, 1, 540000.0, 540000.0);
UPDATE Articulo SET StockActual = StockActual - 1 WHERE Id = 3;

INSERT INTO Venta (Fecha, IdCliente, NumeroFactura, TotalVenta, Estado) VALUES ('2025-12-10', 6, 'FAC-20251210-8152', 1225039.56, 'En Preparación');
SET @LastVentaID = SCOPE_IDENTITY();
INSERT INTO DetalleVenta (IdVenta, IdArticulo, Cantidad, PrecioUnitario, Subtotal) VALUES (@LastVentaID, 1, 1, 1012429.39, 1012429.39);
UPDATE Articulo SET StockActual = StockActual - 1 WHERE Id = 1;

INSERT INTO Venta (Fecha, IdCliente, NumeroFactura, TotalVenta, Estado) VALUES ('2025-12-10', 1, 'FAC-20251210-5299', 596942.39, 'En Preparación');
SET @LastVentaID = SCOPE_IDENTITY();
INSERT INTO DetalleVenta (IdVenta, IdArticulo, Cantidad, PrecioUnitario, Subtotal) VALUES (@LastVentaID, 2, 1, 493340.82, 493340.82);
UPDATE Articulo SET StockActual = StockActual - 1 WHERE Id = 2;

INSERT INTO Venta (Fecha, IdCliente, NumeroFactura, TotalVenta, Estado) VALUES ('2025-12-10', 8, 'FAC-20251210-2384', 596942.39, 'En Preparación');
SET @LastVentaID = SCOPE_IDENTITY();
INSERT INTO DetalleVenta (IdVenta, IdArticulo, Cantidad, PrecioUnitario, Subtotal) VALUES (@LastVentaID, 2, 1, 493340.82, 493340.82);
UPDATE Articulo SET StockActual = StockActual - 1 WHERE Id = 2;

INSERT INTO Venta (Fecha, IdCliente, NumeroFactura, TotalVenta, Estado) VALUES ('2025-12-10', 10, 'FAC-20251210-6449', 1225039.56, 'En Preparación');
SET @LastVentaID = SCOPE_IDENTITY();
INSERT INTO DetalleVenta (IdVenta, IdArticulo, Cantidad, PrecioUnitario, Subtotal) VALUES (@LastVentaID, 1, 1, 1012429.39, 1012429.39);
UPDATE Articulo SET StockActual = StockActual - 1 WHERE Id = 1;

GO

PRINT 'Base de datos GestionNegocio creada y poblada con éxito.'


-- 1. Agregar columna IdUsuario (permitimos NULL al principio para las ventas viejas)
ALTER TABLE Venta ADD IdUsuario INT NULL;
GO

-- 2. Asignar un usuario por defecto a las ventas viejas (ej: el usuario con ID 1, el Admin)
-- (Si no tienes ventas viejas, este paso no hace nada, pero es seguro correrlo)
UPDATE Venta SET IdUsuario = 1 WHERE IdUsuario IS NULL;
GO

-- 3. Crear la clave foránea para relacionarlo con la tabla Usuario
ALTER TABLE Venta ADD CONSTRAINT FK_Venta_Usuario FOREIGN KEY (IdUsuario) REFERENCES Usuario(Id);
GO
-- Agregar columna "Estado" a la tabla Venta
-- 'En Preparación' será el estado por defecto cuando se cree una nueva venta.
ALTER TABLE Venta
ADD Estado NVARCHAR(50) NOT NULL DEFAULT 'En Preparación';
GO

-- Agregar columna "Estado" a la tabla Compra
-- 'Solicitado' será el estado por defecto.
ALTER TABLE Compra
ADD Estado NVARCHAR(50) NOT NULL DEFAULT 'Solicitado';
GO



EXEC sp_help 'Venta';
EXEC sp_help 'Compra';
SELECT * 
FROM sys.columns 
WHERE name = 'Estado';
