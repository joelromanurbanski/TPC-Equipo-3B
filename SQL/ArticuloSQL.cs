using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dominio;


namespace SQL
{
    public class ArticuloSQL
    {
        // 1. LISTAR (Para GestionProducto)
        public List<Articulo> Listar()
        {
            List<Articulo> lista = new List<Articulo>();
            AccesoDatos datos = new AccesoDatos();
            try
            {
                datos.setearConsulta(@"
                    SELECT A.Id, Codigo, Nombre, A.Descripcion, 
                           A.UltimoPrecioCompra, A.PorcentajeGanancia, 
                           A.StockActual, A.StockMinimo,
                           A.IdMarca, A.IdCategoria, A.UrlImagen,
                           M.Descripcion Marca, C.Descripcion Categoria,
                           (
                               SELECT STRING_AGG(P.Nombre, ', ')
                               FROM ArticuloProveedor AP
                               INNER JOIN Proveedor P ON P.Id = AP.IdProveedor
                               WHERE AP.IdArticulo = A.Id
                           ) AS ProveedoresString
                    FROM Articulo A
                    LEFT JOIN Marca M ON A.IdMarca = M.Id
                    LEFT JOIN Categoria C ON A.IdCategoria = C.Id");

                datos.ejecutarLectura();

                while (datos.Lector.Read())
                {
                    Articulo art = new Articulo
                    {
                        Id = (int)datos.Lector["Id"],
                        Codigo = datos.Lector["Codigo"].ToString(),
                        Nombre = datos.Lector["Nombre"].ToString(),
                        Descripcion = datos.Lector["Descripcion"].ToString(),
                        UltimoPrecioCompra = datos.Lector["UltimoPrecioCompra"] != DBNull.Value ? (decimal)datos.Lector["UltimoPrecioCompra"] : 0,
                        PorcentajeGanancia = datos.Lector["PorcentajeGanancia"] != DBNull.Value ? (decimal)datos.Lector["PorcentajeGanancia"] : 0,
                        StockActual = datos.Lector["StockActual"] != DBNull.Value ? (int)datos.Lector["StockActual"] : 0,
                        StockMinimo = datos.Lector["StockMinimo"] != DBNull.Value ? (int)datos.Lector["StockMinimo"] : 0,
                        IdMarca = datos.Lector["IdMarca"] != DBNull.Value ? (int)datos.Lector["IdMarca"] : 0,
                        IdCategoria = datos.Lector["IdCategoria"] != DBNull.Value ? (int)datos.Lector["IdCategoria"] : 0,
                        Marca = new Marca { Descripcion = datos.Lector["Marca"].ToString() },
                        Categoria = new Categoria { Descripcion = datos.Lector["Categoria"].ToString() },
                        UrlImagen = datos.Lector["UrlImagen"] != DBNull.Value ? datos.Lector["UrlImagen"].ToString() : "",
                        ProveedoresString = datos.Lector["ProveedoresString"] != DBNull.Value ? datos.Lector["ProveedoresString"].ToString() : "Sin asignar"
                    };
                    lista.Add(art);
                }
                return lista;
            }
            catch (Exception ex) { throw ex; }
            finally { datos.cerrarConexion(); }
        }

        // 2. AGREGAR (Para GestionProducto)
        public int AgregarYDevolverId(Articulo nuevo)
        {
            AccesoDatos datos = new AccesoDatos();
            try
            {
                datos.setearConsulta(@"INSERT INTO Articulo 
                                     (Codigo, Nombre, Descripcion, IdMarca, IdCategoria, 
                                      PorcentajeGanancia, StockMinimo, StockActual, UltimoPrecioCompra, UrlImagen) 
                                     OUTPUT INSERTED.Id 
                                     VALUES (@Codigo, @Nombre, @Descripcion, @IdMarca, @IdCategoria, 
                                             @PorcentajeGanancia, @StockMinimo, @StockActual, @UltimoPrecioCompra, @UrlImagen)");

                datos.setearParametro("@Codigo", nuevo.Codigo);
                datos.setearParametro("@Nombre", nuevo.Nombre);
                datos.setearParametro("@Descripcion", nuevo.Descripcion);
                datos.setearParametro("@IdMarca", nuevo.IdMarca);
                datos.setearParametro("@IdCategoria", nuevo.IdCategoria);
                datos.setearParametro("@UrlImagen", (object)nuevo.UrlImagen ?? DBNull.Value);
                datos.setearParametro("@PorcentajeGanancia", nuevo.PorcentajeGanancia);
                datos.setearParametro("@StockMinimo", nuevo.StockMinimo);
                datos.setearParametro("@StockActual", nuevo.StockActual);
                datos.setearParametro("@UltimoPrecioCompra", nuevo.UltimoPrecioCompra);

                return (int)datos.ejecutarEscalar();
            }
            catch (Exception ex) { throw ex; }
            finally { datos.cerrarConexion(); }
        }

        // 3. MODIFICAR (Para GestionProducto)
        public void Modificar(Articulo art)
        {
            AccesoDatos datos = new AccesoDatos();
            try
            {
                datos.setearConsulta(@"UPDATE Articulo SET Codigo=@Codigo, Nombre=@Nombre, 
                                       Descripcion=@Descripcion, IdMarca=@IdMarca, 
                                       IdCategoria=@IdCategoria, UrlImagen=@UrlImagen,
                                       PorcentajeGanancia=@PorcentajeGanancia, 
                                       StockMinimo=@StockMinimo,
                                       StockActual=@StockActual, 
                                       UltimoPrecioCompra=@UltimoPrecioCompra
                                       WHERE Id=@Id");

                datos.setearParametro("@Codigo", art.Codigo);
                datos.setearParametro("@Nombre", art.Nombre);
                datos.setearParametro("@Descripcion", art.Descripcion);
                datos.setearParametro("@IdMarca", art.IdMarca);
                datos.setearParametro("@IdCategoria", art.IdCategoria);
                datos.setearParametro("@UrlImagen", (object)art.UrlImagen ?? DBNull.Value);
                datos.setearParametro("@PorcentajeGanancia", art.PorcentajeGanancia);
                datos.setearParametro("@StockMinimo", art.StockMinimo);
                datos.setearParametro("@Id", art.Id);
                datos.setearParametro("@StockActual", art.StockActual);
                datos.setearParametro("@UltimoPrecioCompra", art.UltimoPrecioCompra);

                datos.ejecutarAccion();
            }
            catch (Exception ex) { throw ex; }
            finally { datos.cerrarConexion(); }
        }

        // 4. ELIMINAR (Para GestionProducto)
        public void Eliminar(int id)
        {
            AccesoDatos datos = new AccesoDatos();
            try
            {
                datos.setearConsulta("DELETE FROM Articulo WHERE Id = @Id");
                datos.setearParametro("@Id", id);
                datos.ejecutarAccion();
            }
            catch (Exception ex) { throw ex; }
            finally { datos.cerrarConexion(); }
        }

        // 5. GET BY ID (Para RegistroVenta)
        public Articulo GetById(int id)
        {
            AccesoDatos datos = new AccesoDatos();
            try
            {
                datos.setearConsulta(@"
                    SELECT Id, Codigo, Nombre, Descripcion, 
                           UltimoPrecioCompra, PorcentajeGanancia, 
                           StockActual, StockMinimo,
                           IdMarca, IdCategoria, UrlImagen
                    FROM Articulo
                    WHERE Id = @Id");

                datos.setearParametro("@Id", id);
                datos.ejecutarLectura();

                if (datos.Lector.Read())
                {
                    return new Articulo
                    {
                        Id = (int)datos.Lector["Id"],
                        Codigo = datos.Lector["Codigo"].ToString(),
                        Nombre = datos.Lector["Nombre"].ToString(),
                        Descripcion = datos.Lector["Descripcion"].ToString(),
                        UltimoPrecioCompra = datos.Lector["UltimoPrecioCompra"] != DBNull.Value ? (decimal)datos.Lector["UltimoPrecioCompra"] : 0,
                        PorcentajeGanancia = datos.Lector["PorcentajeGanancia"] != DBNull.Value ? (decimal)datos.Lector["PorcentajeGanancia"] : 0,
                        StockActual = datos.Lector["StockActual"] != DBNull.Value ? (int)datos.Lector["StockActual"] : 0,
                        StockMinimo = datos.Lector["StockMinimo"] != DBNull.Value ? (int)datos.Lector["StockMinimo"] : 0,
                        IdMarca = datos.Lector["IdMarca"] != DBNull.Value ? (int)datos.Lector["IdMarca"] : 0,
                        IdCategoria = datos.Lector["IdCategoria"] != DBNull.Value ? (int)datos.Lector["IdCategoria"] : 0,
                        UrlImagen = datos.Lector["UrlImagen"] != DBNull.Value ? datos.Lector["UrlImagen"].ToString() : ""
                    };
                }
                return null;
            }
            catch (Exception ex) { throw ex; }
            finally { datos.cerrarConexion(); }
        }

        // 6. LISTAR POR PROVEEDOR (Para GestionProveedores)
        public List<Articulo> ListarPorProveedor(int idProveedor)
        {
            List<Articulo> lista = new List<Articulo>();
            AccesoDatos datos = new AccesoDatos();
            try
            {
                datos.setearConsulta(@"
                    SELECT A.Id, Codigo, Nombre, A.Descripcion, 
                           A.UltimoPrecioCompra, A.PorcentajeGanancia, 
                           A.StockActual, A.StockMinimo,
                           A.IdMarca, A.IdCategoria, A.UrlImagen,
                           M.Descripcion Marca, C.Descripcion Categoria
                    FROM Articulo A
                    INNER JOIN ArticuloProveedor AP ON A.Id = AP.IdArticulo
                    LEFT JOIN Marca M ON A.IdMarca = M.Id
                    LEFT JOIN Categoria C ON A.IdCategoria = C.Id
                    WHERE AP.IdProveedor = @IdProveedor");

                datos.setearParametro("@IdProveedor", idProveedor);
                datos.ejecutarLectura();

                while (datos.Lector.Read())
                {
                    lista.Add(new Articulo
                    {
                        Id = (int)datos.Lector["Id"],
                        Codigo = datos.Lector["Codigo"].ToString(),
                        Nombre = datos.Lector["Nombre"].ToString(),
                        StockActual = datos.Lector["StockActual"] != DBNull.Value ? (int)datos.Lector["StockActual"] : 0
                    });
                }
                return lista;
            }
            catch (Exception ex) { throw ex; }
            finally { datos.cerrarConexion(); }
        }

        // 7. BAJO STOCK (Para PaneldeControl)
        public List<Articulo> ListarBajoStock(int top = 5)
        {
            List<Articulo> lista = new List<Articulo>();
            AccesoDatos datos = new AccesoDatos();
            try
            {
                datos.setearConsulta(@"
                    SELECT TOP (@Top) Id, Codigo, Nombre, StockActual, StockMinimo
                    FROM Articulo
                    WHERE StockActual <= StockMinimo AND StockActual > 0
                    ORDER BY StockActual ASC");

                datos.setearParametro("@Top", top);
                datos.ejecutarLectura();

                while (datos.Lector.Read())
                {
                    lista.Add(new Articulo
                    {
                        Id = (int)datos.Lector["Id"],
                        Codigo = datos.Lector["Codigo"].ToString(),
                        Nombre = datos.Lector["Nombre"].ToString(),
                        StockActual = (int)datos.Lector["StockActual"],
                        StockMinimo = (int)datos.Lector["StockMinimo"]
                    });
                }
                return lista;
            }
            catch (Exception ex) { throw ex; }
            finally { datos.cerrarConexion(); }
        }

        // 8. REPONER STOCK (Para GestionPedidos)
        public void ReponerStock(List<DetalleVenta> detalles)
        {
            AccesoDatos datos = new AccesoDatos();
            try
            {
                datos.iniciarTransaccion();
                foreach (var detalle in detalles)
                {
                    datos.setearConsulta("UPDATE Articulo SET StockActual = StockActual + @Cantidad WHERE Id = @IdArticulo");
                    datos.setearParametro("@Cantidad", detalle.Cantidad);
                    datos.setearParametro("@IdArticulo", detalle.Articulo.Id);
                    datos.ejecutarAccion();
                }
                datos.commitTransaccion();
            }
            catch (Exception ex)
            {
                datos.rollbackTransaccion();
                throw ex;
            }
            finally
            {
                datos.cerrarConexionTransaccional();
            }
        }
    }
}