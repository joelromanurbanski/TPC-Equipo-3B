using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQL
{
    public class ArticuloProveedorSQL
    {
        public List<int> ListarIdsPorArticulo(int idArticulo)
        {
            List<int> ids = new List<int>();
            AccesoDatos datos = new AccesoDatos();
            try
            {
                datos.setearConsulta("SELECT IdProveedor FROM ArticuloProveedor WHERE IdArticulo = @IdArticulo");
                datos.setearParametro("@IdArticulo", idArticulo);
                datos.ejecutarLectura();
                while (datos.Lector.Read())
                {
                    ids.Add((int)datos.Lector["IdProveedor"]);
                }
                return ids;
            }
            catch (Exception ex) { throw ex; }
            finally { datos.cerrarConexion(); }
        }

        // Trae todos los IDs de artículos asociados a un proveedor
        public List<int> ListarIdsArticuloPorProveedor(int idProveedor)
        {
            List<int> ids = new List<int>();
            AccesoDatos datos = new AccesoDatos();
            try
            {
                datos.setearConsulta("SELECT IdArticulo FROM ArticuloProveedor WHERE IdProveedor = @IdProveedor");
                datos.setearParametro("@IdProveedor", idProveedor);
                datos.ejecutarLectura();
                while (datos.Lector.Read())
                {
                    ids.Add((int)datos.Lector["IdArticulo"]);
                }
                return ids;
            }
            catch (Exception ex) { throw ex; }
            finally { datos.cerrarConexion(); }
        }


        // Borra y re-asigna los proveedores de un artículo (usa transacción)
        public void ActualizarProveedores(int idArticulo, List<int> idsProveedores)
        {
            AccesoDatos datos = new AccesoDatos();
            try
            {
                datos.iniciarTransaccion();

                // 1. Borrar todas las asociaciones viejas
                datos.setearConsulta("DELETE FROM ArticuloProveedor WHERE IdArticulo = @IdArticulo");
                datos.setearParametro("@IdArticulo", idArticulo);
                datos.ejecutarAccion();

                // 2. Insertar las nuevas asociaciones
                if (idsProveedores != null && idsProveedores.Count > 0)
                {
                    foreach (int idProveedor in idsProveedores)
                    {
                        datos.setearConsulta("INSERT INTO ArticuloProveedor (IdArticulo, IdProveedor) VALUES (@IdArticulo, @IdProveedor)");
                        datos.setearParametro("@IdArticulo", idArticulo);
                        datos.setearParametro("@IdProveedor", idProveedor);
                        datos.ejecutarAccion();
                    }
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