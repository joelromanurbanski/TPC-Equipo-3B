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
        public void RegistrarVenta(Venta venta)
        {
            AccesoDatos datos = new AccesoDatos();

            try
            {
                datos.setearConsulta("INSERT INTO VENTAS (Fecha, IdCliente, NumeroFactura) OUTPUT INSERTED.Id VALUES (@Fecha, @IdCliente, @NumeroFactura)");
                datos.setearParametro("@Fecha", venta.Fecha);
                datos.setearParametro("@IdCliente", venta.Cliente.Id);
                datos.setearParametro("@NumeroFactura", venta.NumeroFactura);
                int idVenta = (int)datos.ejecutarEscalar();

                foreach (var detalle in venta.Detalles)
                {
                    datos.setearConsulta(@"INSERT INTO DETALLEVENTA (IdVenta, IdArticulo, Cantidad, PrecioUnitario, Subtotal) 
                                           VALUES (@IdVenta, @IdArticulo, @Cantidad, @PrecioUnitario, @Subtotal)");
                    datos.setearParametro("@IdVenta", idVenta);
                    datos.setearParametro("@IdArticulo", detalle.Articulo.Id);
                    datos.setearParametro("@Cantidad", detalle.Cantidad);
                    datos.setearParametro("@PrecioUnitario", detalle.PrecioUnitario);
                    datos.setearParametro("@Subtotal", detalle.Subtotal);
                    datos.ejecutarAccion();

                    // Descontar stock
                    datos.setearConsulta("UPDATE ARTICULOS SET StockActual = StockActual - @Cantidad WHERE Id = @IdArticulo");
                    datos.setearParametro("@Cantidad", detalle.Cantidad);
                    datos.setearParametro("@IdArticulo", detalle.Articulo.Id);
                    datos.ejecutarAccion();
                }
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

        public string GenerarNumeroFactura()
        {
            return $"FAC-{DateTime.Now:yyyyMMddHHmmss}-{Guid.NewGuid().ToString().Substring(0, 4)}";
        }
    }
}
