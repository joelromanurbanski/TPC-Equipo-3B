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


        public void ActualizarProveedores(int idArticulo, List<int> idsProveedores)
        {
            AccesoDatos datos = new AccesoDatos();
            try
            {
                datos.iniciarTransaccion();
                datos.setearConsulta("DELETE FROM ArticuloProveedor WHERE IdArticulo = @IdArticulo");
                datos.setearParametro("@IdArticulo", idArticulo);
                datos.ejecutarAccion();
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