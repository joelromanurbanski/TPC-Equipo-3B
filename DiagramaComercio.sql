--Script SQL — Base de datos

-- Crear base de datos
CREATE DATABASE GestionNegocio;
GO
USE GestionNegocio;
GO

-- Tabla Marca
CREATE TABLE Marca (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Descripcion NVARCHAR(100) NOT NULL
);

-- Tabla Categoria
CREATE TABLE Categoria (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Descripcion NVARCHAR(100) NOT NULL
);

-- Tabla Articulo (Producto)
CREATE TABLE Articulo (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Codigo NVARCHAR(50) NOT NULL,
    Nombre NVARCHAR(100) NOT NULL,
    Descripcion NVARCHAR(500),
    Precio DECIMAL(18,2) NOT NULL,
    IdMarca INT NOT NULL,
    IdCategoria INT NOT NULL,
   
    FOREIGN KEY (IdMarca) REFERENCES Marca(Id),
    FOREIGN KEY (IdCategoria) REFERENCES Categoria(Id)
);
--agrego relacion de proveedor a articulo en 2 pasos : 
--paso 1:
ALTER TABLE Articulo ADD IdProveedor INT NULL;--ya que tenemos datos cargados
--paso 2: 
UPDATE Articulo SET IdProveedor = 1; -- por ahora con este valor .
ALTER TABLE Articulo ALTER COLUMN IdProveedor INT NOT NULL;
-- Agrego una urlimagen al id 3 : 
UPDATE Articulo
SET UrlImagen = 'https://www.xt-pc.com.ar/img/productos/32/NOT3292.jpg'
WHERE Id = 3 ;
SELECT * FROM Articulo;
--agrego nuevas tablas a articulo:

ALTER TABLE Articulo ADD UrlImagen NVARCHAR(500);
ALTER TABLE Articulo ADD Imagenes NVARCHAR(MAX);
ALTER TABLE Articulo ADD StockActual INT NOT NULL DEFAULT 0;
ALTER TABLE Articulo ADD StockClass NVARCHAR(100);
ALTER TABLE Articulo ADD StockDisplay NVARCHAR(100);
ALTER TABLE Articulo ADD ProveedorNombre NVARCHAR(200);
-- borre esta que agregue por error
ALTER TABLE Articulo DROP COLUMN FirstImage;

 

-- Tabla Proveedor
CREATE TABLE Proveedor (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Nombre NVARCHAR(100) NOT NULL,
    Email NVARCHAR(100),
    Telefono NVARCHAR(50),
    Direccion NVARCHAR(200),
    

);


-- Tabla Imagen
CREATE TABLE Imagen (
    Id INT PRIMARY KEY IDENTITY(1,1),
    UrlImagen NVARCHAR(250) NOT NULL,
    IdArticulo INT NOT NULL,
    FOREIGN KEY (IdArticulo) REFERENCES Articulo(Id)
);

---Datos de ejemplo

-- Marcas
INSERT INTO Marca (Descripcion) VALUES ('Samsung'), ('Apple'), ('Sony');
INSERT INTO Marca (Descripcion) VALUES ('Blue'),('Motorola'),('Xiaomi');
SELECT * FROM Marca;

-- Categorías
INSERT INTO Categoria (Descripcion) VALUES ('Celulares'), ('Auriculares'), ('Televisores');
INSERT INTO Categoria(Descripcion) VALUES  ('PC'),('NETBOOK'),('NOTEBOOK');
    -- asi se verifica que aparezcan todos :
SELECT * FROM Categoria;
-- Artículo
INSERT INTO Articulo (Codigo, Nombre, Descripcion, Precio, IdMarca, IdCategoria)
VALUES ('A001', 'Galaxy S23', 'Smartphone de alta gama', 899.99, 1, 1);
inseRT INTO Articulo (Codigo, Nombre, Descripcion, Precio, IdMarca, IdCategoria)
values('A003','ASUS VivoBook 15.6','Notebook gama media ','400',2,3),('A004','GigaByte G5','Notebook gama alta ','500',2,4);
    -- asi se verifica que aparezcan todos :
 
SELECT * FROM Articulo;
-- Imagen
INSERT INTO Imagen (UrlImagen, IdArticulo)
VALUES ('https://www.gigabyte.com/FileUpload/Global/WebPage/805/img/1.png', 4);
UPDATE Imagen
SET UrlImagen= 'https://http2.mlstatic.com/D_NQ_NP_2X_635757-MLA93493521199_092025-F.webp'
WHERE Id = 1 ;


SELECT * FROM Imagen;
SELECT * FROM Articulo;

-- Script SQL — Tabla de Usuarios + Usuario Inicial

-- Crear tabla de usuarios
CREATE TABLE Usuario (
    Id INT PRIMARY KEY IDENTITY(1,1),
    NombreUsuario NVARCHAR(50) NOT NULL UNIQUE,
    Contrasenia NVARCHAR(255) NOT NULL,
    EsAdmin BIT NOT NULL DEFAULT 0
);

-- Insertar usuario inicial
INSERT INTO Usuario (NombreUsuario, Contrasenia, EsAdmin)
VALUES ('ADMIN', 'ADMIN123!', 1);
