using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dominio;

namespace SQL
{
    public class ProveedorSQL
    {
        public List<Proveedor> Listar()
        {
            List<Proveedor> lista = new List<Proveedor>();
            AccesoDatos datos = new AccesoDatos();

            try
            {
                datos.setearConsulta("SELECT Id, Nombre, Email, Telefono, Direccion FROM PROVEEDORES");
                datos.ejecutarLectura();

                while (datos.Lector.Read())
                {
                    lista.Add(new Proveedor
                    {
                        Id = (int)datos.Lector["Id"],
                        Nombre = datos.Lector["Nombre"].ToString(),
                        Email = datos.Lector["Email"].ToString(),
                        Telefono = datos.Lector["Telefono"].ToString(),
                        Direccion = datos.Lector["Direccion"].ToString()
                    });
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

        public void Agregar(Proveedor proveedor)
        {
            AccesoDatos datos = new AccesoDatos();

            try
            {
                datos.setearConsulta("INSERT INTO PROVEEDORES (Nombre, Email, Telefono, Direccion) VALUES (@Nombre, @Email, @Telefono, @Direccion)");
                datos.setearParametro("@Nombre", proveedor.Nombre);
                datos.setearParametro("@Email", proveedor.Email);
                datos.setearParametro("@Telefono", proveedor.Telefono);
                datos.setearParametro("@Direccion", proveedor.Direccion);
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
