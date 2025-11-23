using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dominio;
using System.Data.SqlClient;


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

                datos.setearConsulta("INSERT INTO Venta (Fecha, IdCliente, NumeroFactura, TotalVenta, Estado) OUTPUT INSERTED.Id VALUES (@Fecha, @IdCliente, @NumeroFactura, @Total, @Estado)");
                datos.setearParametro("@Fecha", venta.Fecha);
                datos.setearParametro("@IdCliente", venta.Cliente.Id);
                datos.setearParametro("@NumeroFactura", venta.NumeroFactura);
                datos.setearParametro("@Total", venta.TotalVenta);
                datos.setearParametro("@Estado", "En Preparación");

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
        public List<Venta> ListarVentas(string busqueda = "", string estado = "Todos")
        {
            List<Venta> lista = new List<Venta>();
            AccesoDatos datos = new AccesoDatos();
            try
            {
                string consulta = @"
            SELECT DISTINCT V.Id, V.Fecha, V.NumeroFactura, V.TotalVenta, V.Estado,
                    C.Nombre, C.Apellido, C.Documento
            FROM Venta V
            INNER JOIN Cliente C ON V.IdCliente = C.Id
            LEFT JOIN DetalleVenta DV ON V.Id = DV.IdVenta
            LEFT JOIN Articulo A ON DV.IdArticulo = A.Id
            WHERE 1=1";

                if (!string.IsNullOrEmpty(busqueda))
                {
                    consulta += " AND (C.Nombre LIKE @Busqueda OR C.Apellido LIKE @Busqueda OR C.Documento LIKE @Busqueda OR V.NumeroFactura LIKE @Busqueda OR A.Nombre LIKE @Busqueda)";
                    datos.setearParametro("@Busqueda", "%" + busqueda + "%");
                }

                if (estado != "Todos")
                {
                    consulta += " AND V.Estado = @FiltroEstado";
                    datos.setearParametro("@FiltroEstado", estado);
                }

                consulta += " ORDER BY V.Fecha DESC";

                datos.setearConsulta(consulta);
                datos.setearConsulta(consulta); // Primero definimos la consulta (esto limpia los parámetros viejos)

                // agregamos los parámetros
                if (!string.IsNullOrEmpty(busqueda))
                {
                    datos.setearParametro("@Busqueda", "%" + busqueda + "%");
                }
                if (estado != "Todos")
                {
                    datos.setearParametro("@FiltroEstado", estado);
                }

                datos.ejecutarLectura();

                while (datos.Lector.Read())
                {
                    lista.Add(new Venta
                    {
                        Id = (int)datos.Lector["Id"],
                        Fecha = (DateTime)datos.Lector["Fecha"],
                        NumeroFactura = (string)datos.Lector["NumeroFactura"],
                        TotalVenta = (decimal)datos.Lector["TotalVenta"],
                        Estado = (string)datos.Lector["Estado"],
                        Cliente = new Cliente
                        {
                            Nombre = (string)datos.Lector["Nombre"],
                            Apellido = (string)datos.Lector["Apellido"],
                            Documento = (string)datos.Lector["Documento"]
                        }
                    });
                }
                return lista;
            }
            catch (Exception ex) { throw ex; }
            finally { datos.cerrarConexion(); }
        }
        public List<DetalleVenta> ListarDetallesPorVenta(int idVenta)
        {
            List<DetalleVenta> lista = new List<DetalleVenta>();
            AccesoDatos datos = new AccesoDatos();
            try
            {
                datos.setearConsulta("SELECT IdArticulo, Cantidad FROM DetalleVenta WHERE IdVenta = @IdVenta");
                datos.setearParametro("@IdVenta", idVenta);
                datos.ejecutarLectura();
                while (datos.Lector.Read())
                {
                    lista.Add(new DetalleVenta
                    {
                        Articulo = new Articulo { Id = (int)datos.Lector["IdArticulo"] },
                        Cantidad = (int)datos.Lector["Cantidad"]
                    });
                }
                return lista;
            }
            catch (Exception ex) { throw ex; }
            finally { datos.cerrarConexion(); }
        }

        public void ActualizarEstado(int idVenta, string nuevoEstado)
        {
            AccesoDatos datos = new AccesoDatos();
            try
            {
                datos.setearConsulta("UPDATE Venta SET Estado = @NuevoEstado WHERE Id = @IdVenta");
                datos.setearParametro("@NuevoEstado", nuevoEstado);
                datos.setearParametro("@IdVenta", idVenta);
                datos.ejecutarAccion();
            }
            catch (Exception ex) { throw ex; }
            finally { datos.cerrarConexion(); }
        }
    }
}