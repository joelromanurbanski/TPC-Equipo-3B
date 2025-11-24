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
        // --- REGISTRAR VENTA (Sin cambios) ---
        public int RegistrarVenta(Venta venta)
        {
            AccesoDatos datos = new AccesoDatos();
            try
            {
                datos.iniciarTransaccion();

                datos.setearConsulta("INSERT INTO Venta (Fecha, IdCliente, IdUsuario, NumeroFactura, TotalVenta, Estado) OUTPUT INSERTED.Id VALUES (@Fecha, @IdCliente, @IdUsuario, @NumeroFactura, @Total, @Estado)");
                datos.setearParametro("@Fecha", venta.Fecha);
                datos.setearParametro("@IdCliente", venta.Cliente.Id);
                datos.setearParametro("@IdUsuario", venta.Usuario != null ? (object)venta.Usuario.Id : DBNull.Value);
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

        // --- ¡MÉTODO DE LISTADO CORREGIDO! ---
        public List<Venta> ListarVentas(string busqueda = "", string estado = "Todos", bool ordenAscendente = false, DateTime? fechaInicio = null, DateTime? fechaFin = null, int? idUsuario = null)
        {
            List<Venta> lista = new List<Venta>();
            AccesoDatos datos = new AccesoDatos();
            try
            {
                // 1. Construir el texto de la consulta
                string consulta = @"
                    SELECT DISTINCT V.Id, V.Fecha, V.NumeroFactura, V.TotalVenta, V.Estado,
                           C.Nombre, C.Apellido, C.Documento,
                           U.Nombre AS VendedorNombre, U.Apellido AS VendedorApellido
                    FROM Venta V
                    INNER JOIN Cliente C ON V.IdCliente = C.Id
                    LEFT JOIN Usuario U ON V.IdUsuario = U.Id
                    LEFT JOIN DetalleVenta DV ON V.Id = DV.IdVenta
                    LEFT JOIN Articulo A ON DV.IdArticulo = A.Id
                    WHERE 1=1";

                if (!string.IsNullOrEmpty(busqueda))
                    consulta += " AND (C.Nombre LIKE @Busqueda OR C.Apellido LIKE @Busqueda OR C.Documento LIKE @Busqueda OR V.NumeroFactura LIKE @Busqueda OR A.Nombre LIKE @Busqueda)";

                if (estado != "Todos")
                    consulta += " AND V.Estado = @FiltroEstado";

                if (fechaInicio.HasValue)
                    consulta += " AND V.Fecha >= @FechaInicio";

                if (fechaFin.HasValue)
                    consulta += " AND V.Fecha <= @FechaFin";

                if (idUsuario.HasValue && idUsuario.Value > 0)
                    consulta += " AND V.IdUsuario = @IdUsuario";

                if (ordenAscendente)
                    consulta += " ORDER BY V.Fecha ASC";
                else
                    consulta += " ORDER BY V.Fecha DESC";

                // 2. Setear la consulta (Esto limpia los parámetros viejos)
                datos.setearConsulta(consulta);

                // 3. Agregar los parámetros AHORA (después de setearConsulta)
                if (!string.IsNullOrEmpty(busqueda))
                    datos.setearParametro("@Busqueda", "%" + busqueda + "%");

                if (estado != "Todos")
                    datos.setearParametro("@FiltroEstado", estado);

                if (fechaInicio.HasValue)
                    datos.setearParametro("@FechaInicio", fechaInicio.Value);

                if (fechaFin.HasValue)
                {
                    DateTime finDia = fechaFin.Value.Date.AddHours(23).AddMinutes(59).AddSeconds(59);
                    datos.setearParametro("@FechaFin", finDia);
                }

                if (idUsuario.HasValue && idUsuario.Value > 0)
                    datos.setearParametro("@IdUsuario", idUsuario.Value);

                // 4. Ejecutar
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
                        },
                        Usuario = new Usuario
                        {
                            Nombre = datos.Lector["VendedorNombre"] != DBNull.Value ? (string)datos.Lector["VendedorNombre"] : "Sistema",
                            Apellido = datos.Lector["VendedorApellido"] != DBNull.Value ? (string)datos.Lector["VendedorApellido"] : ""
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
                datos.setearConsulta(@"
                    SELECT D.IdArticulo, D.Cantidad, D.PrecioUnitario, D.Subtotal, A.Nombre
                    FROM DetalleVenta D
                    INNER JOIN Articulo A ON D.IdArticulo = A.Id
                    WHERE D.IdVenta = @IdVenta");

                datos.setearParametro("@IdVenta", idVenta);
                datos.ejecutarLectura();
                while (datos.Lector.Read())
                {
                    lista.Add(new DetalleVenta
                    {
                        Articulo = new Articulo
                        {
                            Id = (int)datos.Lector["IdArticulo"],
                            Nombre = (string)datos.Lector["Nombre"]
                        },
                        Cantidad = (int)datos.Lector["Cantidad"],
                        PrecioUnitario = (decimal)datos.Lector["PrecioUnitario"],
                        Subtotal = (decimal)datos.Lector["Subtotal"]
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