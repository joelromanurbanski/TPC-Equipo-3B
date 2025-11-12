using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dominio;


namespace SQL
{
    public class CompraSQL
    {
        public void RegistrarCompra(Compra compra)
        {
            AccesoDatos datos = new AccesoDatos();

            try
            {
                datos.iniciarTransaccion();

                datos.setearConsulta("INSERT INTO COMPRAS (Fecha, IdProveedor) OUTPUT INSERTED.Id VALUES (@Fecha, @IdProveedor)");
                datos.setearParametro("@Fecha", compra.Fecha);
                datos.setearParametro("@IdProveedor", compra.Proveedor.Id);
                int idCompra = (int)datos.ejecutarEscalar();

                foreach (var detalle in compra.Detalles)
                {
                    datos.setearConsulta(@"INSERT INTO DETALLECOMPRA (IdCompra, IdArticulo, Cantidad, PrecioCompra) 
                                           VALUES (@IdCompra, @IdArticulo, @Cantidad, @PrecioCompra)");
                    datos.setearParametro("@IdCompra", idCompra);
                    datos.setearParametro("@IdArticulo", detalle.Articulo.Id);
                    datos.setearParametro("@Cantidad", detalle.Cantidad);
                    datos.setearParametro("@PrecioCompra", detalle.PrecioCompra);
                    datos.ejecutarAccion();

                    datos.setearConsulta(@"UPDATE ARTICULOS 
                                           SET StockActual = StockActual + @Cantidad, 
                                               UltimoPrecioCompra = @PrecioCompra 
                                           WHERE Id = @IdArticulo");
                    datos.setearParametro("@Cantidad", detalle.Cantidad);
                    datos.setearParametro("@PrecioCompra", detalle.PrecioCompra);
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