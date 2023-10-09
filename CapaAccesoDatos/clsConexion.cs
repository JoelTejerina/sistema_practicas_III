using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

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
    }
}