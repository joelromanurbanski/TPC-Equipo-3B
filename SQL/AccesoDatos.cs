using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using Dominio;

namespace SQL
{
    public class AccesoDatos
    {
        private SqlConnection conexion;
        private SqlCommand comando;
        private SqlDataReader lector;
        private SqlTransaction transaccion;

        public SqlDataReader Lector => lector;

        public AccesoDatos()
        {
            conexion = new SqlConnection("server=.\\SQLEXPRESS; database=GestionNegocio; integrated security=true");
            comando = new SqlCommand();
        }
        public void iniciarTransaccion()
        {
            try
            {
                if (conexion.State != ConnectionState.Open)
                    conexion.Open();
                transaccion = conexion.BeginTransaction();
                comando.Transaction = transaccion;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al iniciar la transacción.", ex);
            }
        }

        public void commitTransaccion()
        {
            transaccion?.Commit();
        }

        public void rollbackTransaccion()
        {
            transaccion?.Rollback();
        }
        public void cerrarConexionTransaccional()
        {
            lector?.Close();
            transaccion = null; // Limpiar la transacción
            if (conexion.State != System.Data.ConnectionState.Closed)
                conexion.Close();
        }

        public void setearConsulta(string consulta)
        {
            comando.CommandType = System.Data.CommandType.Text;
            comando.CommandText = consulta;
            comando.Parameters.Clear();
            comando.Connection = conexion;
            if (transaccion != null)
                comando.Transaction = transaccion; // Asignar transacción si existe
        }

        public void setearParametro(string nombre, object valor)
        {
            comando.Parameters.AddWithValue(nombre, valor ?? DBNull.Value);
        }

        public void ejecutarLectura()
        {
            comando.Connection = conexion;
            try
            {
                if (transaccion == null && conexion.State != ConnectionState.Open)
                    conexion.Open();
                lector = comando.ExecuteReader();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void ejecutarAccion()
        {
            comando.Connection = conexion;
            try
            {
                if (transaccion == null && conexion.State != ConnectionState.Open)
                    conexion.Open();
                comando.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public object ejecutarEscalar()
        {
            comando.Connection = conexion;
            try
            {
                if (transaccion == null && conexion.State != ConnectionState.Open)
                    conexion.Open();
                return comando.ExecuteScalar();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void cerrarConexion()
        {
            try
            {
                lector?.Close();
            }
            catch { }
            finally
            {
                // No cerrar la conexión si estamos en una transacción
                if (transaccion == null && conexion.State != System.Data.ConnectionState.Closed)
                    conexion.Close();
            }
        }

        internal int ejecutarScalar()
        {
            throw new NotImplementedException();
        }
    }
}