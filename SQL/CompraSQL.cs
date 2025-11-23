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

                datos.setearConsulta("INSERT INTO Compra (Fecha, IdProveedor, TotalCompra, Estado) OUTPUT INSERTED.Id VALUES (@Fecha, @IdProveedor, @Total, @Estado)");
                datos.setearParametro("@Fecha", compra.Fecha);
                datos.setearParametro("@IdProveedor", compra.Proveedor.Id);
                datos.setearParametro("@Total", compra.TotalCompra);
                datos.setearParametro("@Estado", "Solicitado"); // Estado inicial

                int idCompra = (int)datos.ejecutarEscalar();

                foreach (var detalle in compra.Detalles)
                {
                    datos.setearConsulta(@"INSERT INTO DetalleCompra (IdCompra, IdArticulo, Cantidad, PrecioCompra) 
                                           VALUES (@IdCompra, @IdArticulo, @Cantidad, @PrecioCompra)");
                    datos.setearParametro("@IdCompra", idCompra);
                    datos.setearParametro("@IdArticulo", detalle.Articulo.Id);
                    datos.setearParametro("@Cantidad", detalle.Cantidad);
                    datos.setearParametro("@PrecioCompra", detalle.PrecioCompra);
                    datos.ejecutarAccion();

                    datos.setearConsulta(@"UPDATE Articulo 
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
        /// Lista todas las compras, uniendo con Proveedor para obtener el nombre.
        public List<Compra> ListarCompras()
        {
            List<Compra> lista = new List<Compra>();
            AccesoDatos datos = new AccesoDatos();
            try
            {
                datos.setearConsulta(@"
                    SELECT C.Id, C.Fecha, C.TotalCompra, C.Estado,
                           P.Nombre AS ProveedorNombre
                    FROM Compra C
                    INNER JOIN Proveedor P ON C.IdProveedor = P.Id
                    ORDER BY C.Fecha DESC
                ");
                datos.ejecutarLectura();
                while (datos.Lector.Read())
                {
                    lista.Add(new Compra
                    {
                        Id = (int)datos.Lector["Id"],
                        Fecha = (DateTime)datos.Lector["Fecha"],
                        TotalCompra = (decimal)datos.Lector["TotalCompra"],
                        Estado = (string)datos.Lector["Estado"],
                        Proveedor = new Proveedor
                        {
                            Nombre = (string)datos.Lector["ProveedorNombre"]
                        }
                    });
                }
                return lista;
            }
            catch (Exception ex) { throw ex; }
            finally { datos.cerrarConexion(); }
        }
        /// Actualiza el estado de una compra específica.
        public void ActualizarEstadoCompra(int idCompra, string nuevoEstado)
        {
            AccesoDatos datos = new AccesoDatos();
            try
            {
                datos.setearConsulta("UPDATE Compra SET Estado = @NuevoEstado WHERE Id = @IdCompra");
                datos.setearParametro("@NuevoEstado", nuevoEstado);
                datos.setearParametro("@IdCompra", idCompra);
                datos.ejecutarAccion();
            }
            catch (Exception ex) { throw ex; }
            finally { datos.cerrarConexion(); }
        }
    }
}