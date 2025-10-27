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
        private AccesoDatos datos = new AccesoDatos();

        public bool DniExiste(string dni)
        {
            try
            {
                datos.setearConsulta("SELECT COUNT(*) FROM Clientes WHERE Documento = @dni");
                datos.setearParametro("@dni", dni);
                object result = datos.ejecutarEscalar();
                return Convert.ToInt32(result) > 0;
            }
            finally { datos.cerrarConexion(); }
        }

        public void AgregarCliente(string dni, string nombre, string apellido, string email, string direccion, string ciudad, int cp)
        {
            try
            {
                datos.setearConsulta(@"INSERT INTO Clientes 
                    (Documento, Nombre, Apellido, Email, Direccion, Ciudad, CP) 
                    VALUES (@dni, @nombre, @apellido, @correo, @direccion, @ciudad, @cp)");

                datos.setearParametro("@dni", dni);
                datos.setearParametro("@nombre", nombre);
                datos.setearParametro("@apellido", apellido);
                datos.setearParametro("@correo", email);
                datos.setearParametro("@direccion", direccion);
                datos.setearParametro("@ciudad", ciudad);
                datos.setearParametro("@cp", cp);

                datos.ejecutarAccion();
            }
            finally { datos.cerrarConexion(); }
        }

        public Cliente PrellenarDatos(string dni)
        {
            Cliente cliente = null;
            try
            {
                datos.setearConsulta("SELECT * FROM Clientes WHERE Documento = @dni");
                datos.setearParametro("@dni", dni);
                datos.ejecutarLectura();

                if (datos.Lector.Read())
                {
                    cliente = new Cliente
                    {
                        Id = (int)datos.Lector["Id"],
                        Documento = datos.Lector["Documento"].ToString(),
                        Nombre = datos.Lector["Nombre"].ToString(),
                        Apellido = datos.Lector["Apellido"].ToString(),
                        Email = datos.Lector["Email"].ToString(),
                        Direccion = datos.Lector["Direccion"].ToString(),
                        Ciudad = datos.Lector["Ciudad"].ToString(),
                        CP = (int)datos.Lector["CP"]
                    };
                }
            }
            finally { datos.cerrarConexion(); }
            return cliente;
        }

        public int ObtenerIdCliente(string dni)
        {
            try
            {
                datos.setearConsulta("SELECT Id FROM Clientes WHERE Documento = @dni");
                datos.setearParametro("@dni", dni);
                object result = datos.ejecutarEscalar();
                return result != null ? Convert.ToInt32(result) : 0;
            }
            finally { datos.cerrarConexion(); }
        }
    }
}
