using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dominio;

namespace SQL
{
    public class VentaSQL
    {

        public int RegistrarVenta(Venta venta)
        {
            AccesoDatos datos = new AccesoDatos();

            try
            {
                datos.iniciarTransaccion();

        
                datos.setearConsulta("INSERT INTO Venta (Fecha, IdCliente, NumeroFactura, TotalVenta) OUTPUT INSERTED.Id VALUES (@Fecha, @IdCliente, @NumeroFactura, @Total)");
                datos.setearParametro("@Fecha", venta.Fecha);
                datos.setearParametro("@IdCliente", venta.Cliente.Id);
                datos.setearParametro("@NumeroFactura", venta.NumeroFactura);
                datos.setearParametro("@Total", venta.TotalVenta);
                int idVenta = (int)datos.ejecutarEscalar();

                foreach (var detalle in venta.Detalles)
                {

                    datos.setearConsulta(@"INSERT INTO DetalleVenta (IdVenta, IdArticulo, Cantidad, PrecioUnitario, Subtotal) 
                                           VALUES (@IdVenta, @IdArticulo, @Cantidad, @PrecioUnitario, @Subtotal)");
                    datos.setearParametro("@IdVenta", idVenta);
                    datos.setearParametro("@IdArticulo", detalle.Articulo.Id);
                    datos.setearParametro("@Cantidad", detalle.Cantidad);
                    datos.setearParametro("@PrecioUnitario", detalle.PrecioUnitario);
                    datos.setearParametro("@Subtotal", detalle.Subtotal);
                    datos.ejecutarAccion();

              
                    datos.setearConsulta("UPDATE Articulo SET StockActual = StockActual - @Cantidad WHERE Id = @IdArticulo");
                    datos.setearParametro("@Cantidad", detalle.Cantidad);
                    datos.setearParametro("@IdArticulo", detalle.Articulo.Id);
                    datos.ejecutarAccion();
                }

                datos.commitTransaccion();
                return idVenta;
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

        public string GenerarNumeroFactura()
        {
          
            return $"FAC-{DateTime.Now:yyyyMMddHHmmss}-{Guid.NewGuid().ToString().Substring(0, 4)}";
        }
    }
}