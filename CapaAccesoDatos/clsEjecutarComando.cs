using System;
using System.Data;
using System.Data.SqlClient;

namespace CapaAccesoDatos
{
    class clsEjecutarComando : CD_Conexion
    {
        SqlDataReader DR;
        private DataTable DT = new DataTable();

        public DataTable Ejecutar(string sSql)
        {
            using (SqlConnection CNN = GetConexion())
            {
                CNN.Close();
                CNN.Open();
                using (SqlCommand comando = new SqlCommand(sSql, CNN))
                {
                    SqlDataReader DR = comando.ExecuteReader();
                    DT.Load(DR);
                }
                // Cierra la conexión explícitamente después de su uso.
                CNN.Close();
                return DT;
            }
        }
        public void EjecucionDirecta(string sSql)
        {
            using (SqlConnection CNN = GetConexion())
            {
                CNN.Open();
                using (SqlCommand comando = new SqlCommand(sSql, CNN))
                {
                    comando.ExecuteNonQuery();
                }
            }
        }
    }
}
