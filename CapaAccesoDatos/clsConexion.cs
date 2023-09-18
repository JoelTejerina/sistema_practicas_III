using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace CapaAccesoDatos
{
    public class CD_Conexion
    {
        private SqlConnection Conexion = new SqlConnection("Server=(local);DataBase=KUVO_GSTM;Integrated Security=true");

        public SqlConnection GetConexion()
        {
            if (Conexion.State == ConnectionState.Closed)
                Conexion.Open();
            return Conexion;
        }

        //public SqlConnection CerrarConexion()
        //{
        //    if (Conexion.State == ConnectionState.Open)
        //        Conexion.Close();
        //    return Conexion;
        //}
    }
}
