using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dominio;
using System.Data.SqlClient;

namespace SQL
{
    public class ClienteSQL
    {
        public List<Cliente> Listar()
        {
            List<Cliente> lista = new List<Cliente>();
            AccesoDatos datos = new AccesoDatos();
            try
            {
                
                datos.setearConsulta("SELECT Id, Documento, Nombre, Apellido, Email, Telefono, Direccion, Ciudad, CP FROM Cliente");
                datos.ejecutarLectura();
                while (datos.Lector.Read())
                {
                    lista.Add(new Cliente
                    {
                        Id = (int)datos.Lector["Id"],
                        Documento = datos.Lector["Documento"].ToString(),
                        Nombre = datos.Lector["Nombre"].ToString(),
                        Apellido = datos.Lector["Apellido"].ToString(),
                        Email = datos.Lector["Email"]?.ToString(),
                        Telefono = datos.Lector["Telefono"]?.ToString(),
                        Direccion = datos.Lector["Direccion"]?.ToString(),
                        Ciudad = datos.Lector["Ciudad"]?.ToString(),
                        CP = datos.Lector["CP"] != DBNull.Value ? (int)datos.Lector["CP"] : 0
                    });
                }
                return lista;
            }
            catch (Exception ex) { throw ex; }
            finally { datos.cerrarConexion(); }
        }

        public void Agregar(Cliente nuevo)
        {
            AccesoDatos datos = new AccesoDatos();
            try
            {
                
                datos.setearConsulta("INSERT INTO Cliente (Documento, Nombre, Apellido, Email, Telefono, Direccion, Ciudad, CP) VALUES (@Doc, @Nombre, @Apellido, @Email, @Tel, @Dir, @Ciudad, @CP)");
                datos.setearParametro("@Doc", nuevo.Documento);
                datos.setearParametro("@Nombre", nuevo.Nombre);
                datos.setearParametro("@Apellido", nuevo.Apellido);
                datos.setearParametro("@Email", nuevo.Email);
                datos.setearParametro("@Tel", nuevo.Telefono);
                datos.setearParametro("@Dir", nuevo.Direccion);
                datos.setearParametro("@Ciudad", nuevo.Ciudad);
                datos.setearParametro("@CP", nuevo.CP);
                datos.ejecutarAccion();
            }
            catch (Exception ex) { throw ex; }
            finally { datos.cerrarConexion(); }
        }

        public void Modificar(Cliente cliente)
        {
            AccesoDatos datos = new AccesoDatos();
            try
            {
                
                datos.setearConsulta("UPDATE Cliente SET Documento=@Doc, Nombre=@Nombre, Apellido=@Apellido, Email=@Email, Telefono=@Tel, Direccion=@Dir, Ciudad=@Ciudad, CP=@CP WHERE Id=@Id");
                datos.setearParametro("@Id", cliente.Id);
                datos.setearParametro("@Doc", cliente.Documento);
                datos.setearParametro("@Nombre", cliente.Nombre);
                datos.setearParametro("@Apellido", cliente.Apellido);
                datos.setearParametro("@Email", cliente.Email);
                datos.setearParametro("@Tel", cliente.Telefono);
                datos.setearParametro("@Dir", cliente.Direccion);
                datos.setearParametro("@Ciudad", cliente.Ciudad);
                datos.setearParametro("@CP", cliente.CP);
                datos.ejecutarAccion();
            }
            catch (Exception ex) { throw ex; }
            finally { datos.cerrarConexion(); }
        }

        public void Eliminar(int id)
        {
            AccesoDatos datos = new AccesoDatos();
            try
            {
                datos.setearConsulta("DELETE FROM Cliente WHERE Id = @Id");
                datos.setearParametro("@Id", id);
                datos.ejecutarAccion();
            }
            catch (Exception ex) { throw ex; }
            finally { datos.cerrarConexion(); }
        }
    }
}