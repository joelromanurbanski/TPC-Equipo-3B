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
        public List<Articulo> Listar()
        {
            List<Articulo> lista = new List<Articulo>();
            AccesoDatos datos = new AccesoDatos();
            ImagenSQL imagenSQL = new ImagenSQL();

            try
            {
                datos.setearConsulta(@"SELECT A.Id, Codigo, Nombre, A.Descripcion, A.Precio, 
                            A.IdMarca, A.IdCategoria, 
                            M.Descripcion Marca, C.Descripcion Categoria,
                            (SELECT TOP 1 ImagenUrl FROM IMAGENES WHERE IdArticulo = A.Id) UrlImagen
                            FROM ARTICULOS A
                            LEFT JOIN MARCAS M ON A.IdMarca = M.Id
                            LEFT JOIN CATEGORIAS C ON A.IdCategoria = C.Id");
                datos.ejecutarLectura();

                while (datos.Lector.Read())
                {
                    Articulo art = new Articulo
                    {
                        Id = (int)datos.Lector["Id"],
                        Codigo = datos.Lector["Codigo"].ToString(),
                        Nombre = datos.Lector["Nombre"].ToString(),
                        Descripcion = datos.Lector["Descripcion"].ToString(),
                        Precio = datos.Lector["Precio"] != DBNull.Value ? (decimal)datos.Lector["Precio"] : 0,
                        IdMarca = datos.Lector["IdMarca"] != DBNull.Value ? (int)datos.Lector["IdMarca"] : 0,
                        IdCategoria = datos.Lector["IdCategoria"] != DBNull.Value ? (int)datos.Lector["IdCategoria"] : 0,
                        Marca = new Marca { Descripcion = datos.Lector["Marca"].ToString() },
                        Categoria = new Categoria { Descripcion = datos.Lector["Categoria"].ToString() },
                        UrlImagen = datos.Lector["UrlImagen"] != DBNull.Value ? datos.Lector["UrlImagen"].ToString() : ""
                    };

                    // Cargar todas las imágenes asociadas
                    art.Imagenes = imagenSQL.ListarPorArticulo(art.Id);

                    lista.Add(art);
                }

                return lista;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                datos.cerrarConexion();
            }
        }

        public int AgregarYDevolverId(Articulo nuevo)
        {
            AccesoDatos datos = new AccesoDatos();

            try
            {
                datos.setearConsulta("INSERT INTO ARTICULOS (Codigo, Nombre, Descripcion, Precio, IdMarca, IdCategoria) " +
                                     "OUTPUT INSERTED.Id VALUES (@Codigo, @Nombre, @Descripcion, @Precio, @IdMarca, @IdCategoria)");

                datos.setearParametro("@Codigo", nuevo.Codigo);
                datos.setearParametro("@Nombre", nuevo.Nombre);
                datos.setearParametro("@Descripcion", nuevo.Descripcion);
                datos.setearParametro("@Precio", nuevo.Precio);
                datos.setearParametro("@IdMarca", nuevo.IdMarca);
                datos.setearParametro("@IdCategoria", nuevo.IdCategoria);

                return (int)datos.ejecutarEscalar();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                datos.cerrarConexion();
            }
        }

        public void Modificar(Articulo art)
        {
            AccesoDatos datos = new AccesoDatos();
            try
            {
                datos.setearConsulta(@"UPDATE ARTICULOS SET Codigo=@Codigo, Nombre=@Nombre, Descripcion=@Descripcion, 
                                       IdMarca=@IdMarca, IdCategoria=@IdCategoria, Precio=@Precio 
                                       WHERE Id=@Id");
                datos.setearParametro("@Codigo", art.Codigo);
                datos.setearParametro("@Nombre", art.Nombre);
                datos.setearParametro("@Descripcion", art.Descripcion);
                datos.setearParametro("@IdMarca", art.IdMarca);
                datos.setearParametro("@IdCategoria", art.IdCategoria);
                datos.setearParametro("@Precio", art.Precio);
                datos.setearParametro("@Id", art.Id);
                datos.ejecutarAccion();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                datos.cerrarConexion();
            }
        }

        public void Eliminar(int id)
        {
            AccesoDatos datos = new AccesoDatos();
            try
            {
                datos.setearConsulta("DELETE FROM ARTICULOS WHERE Id = @Id");
                datos.setearParametro("@Id", id);
                datos.ejecutarAccion();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                datos.cerrarConexion();
            }
        }
    }
}
