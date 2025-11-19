using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dominio;

namespace SQL
{
    public class DashboardSQL
    {
        public DashboardStats GetDashboardStats()
        {
            AccesoDatos datos = new AccesoDatos();
            DashboardStats stats = new DashboardStats();
            try
            {
                // Ingresos Totales
                datos.setearConsulta("SELECT ISNULL(SUM(TotalVenta), 0) FROM Venta");
                stats.IngresosTotales = (decimal)datos.ejecutarEscalar();

                // Costos Totales
                datos.setearConsulta("SELECT ISNULL(SUM(TotalCompra), 0) FROM Compra");
                stats.CostosTotales = (decimal)datos.ejecutarEscalar();

                // Transacciones
                datos.setearConsulta("SELECT (SELECT COUNT(*) FROM Venta) + (SELECT COUNT(*) FROM Compra)");
                stats.TotalTransacciones = (int)datos.ejecutarEscalar();

                return stats;
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

        public List<ActividadReciente> ListarActividadReciente(int top = 5)
        {
            List<ActividadReciente> lista = new List<ActividadReciente>();
            AccesoDatos datos = new AccesoDatos();
            try
            {
                // Consulta que une Ventas y Compras
                datos.setearConsulta($@"
                    SELECT TOP (@Top) 'Venta' AS Tipo, v.Id, v.Fecha, v.TotalVenta AS Monto, c.Nombre + ' ' + c.Apellido AS NombreClienteOProveedor
                    FROM Venta v
                    INNER JOIN Cliente c ON v.IdCliente = c.Id
                    UNION ALL
                    SELECT 'Compra' AS Tipo, co.Id, co.Fecha, co.TotalCompra AS Monto, p.Nombre AS NombreClienteOProveedor
                    FROM Compra co
                    INNER JOIN Proveedor p ON co.IdProveedor = p.Id
                    ORDER BY Fecha DESC");

                datos.setearParametro("@Top", top);
                datos.ejecutarLectura();

                while (datos.Lector.Read())
                {
                    lista.Add(new ActividadReciente
                    {
                        Tipo = (string)datos.Lector["Tipo"],
                        Id = (int)datos.Lector["Id"],
                        Fecha = (DateTime)datos.Lector["Fecha"],
                        Monto = (decimal)datos.Lector["Monto"],
                        NombreClienteOProveedor = (string)datos.Lector["NombreClienteOProveedor"]
                    });
                }
                return lista;
            }
            catch (Exception ex) { throw ex; }
            finally { datos.cerrarConexion(); }
        }

        public List<ChartData> GetChartData()
        {
            List<ChartData> lista = new List<ChartData>();
            AccesoDatos datos = new AccesoDatos();
            try
            {
                // Consulta todas las transacciones por semana (para el mes y año actual).
                datos.setearConsulta(@"
            SELECT 
                'Semana ' + CAST(DATEPART(week, Fecha) - DATEPART(week, DATEADD(month, DATEDIFF(month, 0, GETDATE()), 0)) + 1 AS VARCHAR) AS Label,
                SUM(Ventas) AS TotalVentas,
                SUM(Compras) AS TotalCompras
            FROM (
                SELECT Fecha, TotalVenta AS Ventas, 0 AS Compras FROM Venta
                UNION ALL
                SELECT Fecha, 0 AS Ventas, TotalCompra AS Compras FROM Compra
            ) AS Transacciones
            WHERE DATEPART(month, Fecha) = DATEPART(month, GETDATE())
              AND DATEPART(year, Fecha) = DATEPART(year, GETDATE())
            GROUP BY DATEPART(week, Fecha)
            ORDER BY DATEPART(week, Fecha)
        ");

                datos.ejecutarLectura();

                while (datos.Lector.Read())
                {
                    lista.Add(new ChartData
                    {
                        Label = (string)datos.Lector["Label"],
                        TotalVentas = (decimal)datos.Lector["TotalVentas"],
                        TotalCompras = (decimal)datos.Lector["TotalCompras"]
                    });
                }
                return lista;
            }
            catch (Exception ex) { throw ex; }
            finally { datos.cerrarConexion(); }
        }
    }
}