using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.OleDb;

namespace CapaAccesoDatos
{
    public abstract class clsConexion
    {
        private readonly string cadena;

        public clsConexion()
        {
            cadena = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=|DataDirectory|Loguin.accdb";
        }

        protected OleDbConnection GetConexion()
        {
            return new OleDbConnection(cadena);
        }
    }
}
