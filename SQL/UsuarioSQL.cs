using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dominio;

namespace SQL
{
    public class UsuarioSQL
    {
        // LOGIN
        public Usuario Loguear(string email, string pass)
        {
            AccesoDatos datos = new AccesoDatos();
            try
            {
                datos.setearConsulta("SELECT Id, Email, EsAdmin, Nombre, Apellido FROM Usuario WHERE Email = @email AND Contrasenia = @pass");
                datos.setearParametro("@email", email);
                datos.setearParametro("@pass", pass);
                datos.ejecutarLectura();

                if (datos.Lector.Read())
                {
                    return new Usuario
                    {
                        Id = (int)datos.Lector["Id"],
                        Email = (string)datos.Lector["Email"],
                        EsAdmin = (bool)datos.Lector["EsAdmin"],
                        Nombre = datos.Lector["Nombre"] != DBNull.Value ? (string)datos.Lector["Nombre"] : "",
                        Apellido = datos.Lector["Apellido"] != DBNull.Value ? (string)datos.Lector["Apellido"] : ""
                    };
                }
                return null;
            }
            catch (Exception ex) { throw ex; }
            finally { datos.cerrarConexion(); }
        }

        // LISTAR
        public List<Usuario> Listar()
        {
            List<Usuario> lista = new List<Usuario>();
            AccesoDatos datos = new AccesoDatos();
            try
            {
                datos.setearConsulta("SELECT Id, Email, EsAdmin, Nombre, Apellido, Contrasenia FROM Usuario");
                datos.ejecutarLectura();
                while (datos.Lector.Read())
                {
                    lista.Add(new Usuario
                    {
                        Id = (int)datos.Lector["Id"],
                        Email = (string)datos.Lector["Email"],
                        EsAdmin = (bool)datos.Lector["EsAdmin"],
                        Nombre = datos.Lector["Nombre"] != DBNull.Value ? (string)datos.Lector["Nombre"] : "",
                        Apellido = datos.Lector["Apellido"] != DBNull.Value ? (string)datos.Lector["Apellido"] : "",
                        Contrasenia = (string)datos.Lector["Contrasenia"]
                    });
                }
                return lista;
            }
            catch (Exception ex) { throw ex; }
            finally { datos.cerrarConexion(); }
        }

        // AGREGAR
        public void Agregar(Usuario user)
        {
            AccesoDatos datos = new AccesoDatos();
            try
            {
                datos.setearConsulta("INSERT INTO Usuario (Email, Contrasenia, EsAdmin, Nombre, Apellido) VALUES (@email, @pass, @admin, @nom, @ape)");
                datos.setearParametro("@email", user.Email);
                datos.setearParametro("@pass", user.Contrasenia);
                datos.setearParametro("@admin", user.EsAdmin);
                datos.setearParametro("@nom", user.Nombre);
                datos.setearParametro("@ape", user.Apellido);
                datos.ejecutarAccion();
            }
            catch (Exception ex) { throw ex; }
            finally { datos.cerrarConexion(); }
        }

        // MODIFICAR
        public void Modificar(Usuario user)
        {
            AccesoDatos datos = new AccesoDatos();
            try
            {
                string consulta = "UPDATE Usuario SET Email=@email, EsAdmin=@admin, Nombre=@nom, Apellido=@ape";
                if (!string.IsNullOrEmpty(user.Contrasenia))
                    consulta += ", Contrasenia=@pass";
                consulta += " WHERE Id=@id";

                datos.setearConsulta(consulta);
                datos.setearParametro("@email", user.Email);
                datos.setearParametro("@admin", user.EsAdmin);
                datos.setearParametro("@nom", user.Nombre);
                datos.setearParametro("@ape", user.Apellido);
                datos.setearParametro("@id", user.Id);
                if (!string.IsNullOrEmpty(user.Contrasenia))
                    datos.setearParametro("@pass", user.Contrasenia);

                datos.ejecutarAccion();
            }
            catch (Exception ex) { throw ex; }
            finally { datos.cerrarConexion(); }
        }

        // ELIMINAR
        public void Eliminar(int id)
        {
            AccesoDatos datos = new AccesoDatos();
            try
            {
                datos.setearConsulta("DELETE FROM Usuario WHERE Id = @id");
                datos.setearParametro("@id", id);
                datos.ejecutarAccion();
            }
            catch (Exception ex) { throw ex; }
            finally { datos.cerrarConexion(); }
        }

        // GET BY ID
        public Usuario GetById(int id)
        {
            AccesoDatos datos = new AccesoDatos();
            try
            {
                datos.setearConsulta("SELECT * FROM Usuario WHERE Id = @id");
                datos.setearParametro("@id", id);
                datos.ejecutarLectura();
                if (datos.Lector.Read())
                {
                    return new Usuario
                    {
                        Id = (int)datos.Lector["Id"],
                        Email = (string)datos.Lector["Email"],
                        Contrasenia = (string)datos.Lector["Contrasenia"],
                        Nombre = datos.Lector["Nombre"] != DBNull.Value ? (string)datos.Lector["Nombre"] : "",
                        Apellido = datos.Lector["Apellido"] != DBNull.Value ? (string)datos.Lector["Apellido"] : "",
                        EsAdmin = (bool)datos.Lector["EsAdmin"]
                    };
                }
                return null;
            }
            finally { datos.cerrarConexion(); }
        }
    }
}