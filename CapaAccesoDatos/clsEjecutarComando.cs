using System;
using System.Data;
using System.Data.OleDb;

namespace CapaAccesoDatos
{
    class clsEjecutarComando : clsConexion
    {
        [ThreadStatic]
        private static bool registrandoEnBitacora;

        private static void RegistrarErrorSql(Exception ex, string sSql)
        {
            if (registrandoEnBitacora)
            {
                return;
            }

            try
            {
                registrandoEnBitacora = true;
                string detalle = ex.Message;
                if (detalle.Length > 400)
                {
                    detalle = detalle.Substring(0, 400) + "...";
                }
                string origen = "clsEjecutarComando";
                if (!string.IsNullOrEmpty(sSql) && sSql.Length > 80)
                {
                    detalle += " | SQL: " + sSql.Substring(0, 80) + "...";
                }
                else if (!string.IsNullOrEmpty(sSql))
                {
                    detalle += " | SQL: " + sSql;
                }
                new CD_clsBitacora("Error SQL", detalle, origen);
            }
            catch
            {
            }
            finally
            {
                registrandoEnBitacora = false;
            }
        }
        OleDbDataReader DR;
        private DataTable DT = new DataTable();

        public DataTable Ejecutar(string sSql)
        {
            //La importancia de usar USING:
            //La declaración using garantiza que se llame a Dispose una vez termine de ejecutarse los códigos
            //dentro del bloque using, incluso si ocurre una excepción.
            //Para entender mejor, una vez que termine de ejecutarse el método Login,
            //se desechará los objetos OleDbConnection y OleDbCommand,
            
            try
            {
                using (OleDbConnection CNN = GetConexion())
                {
                    CNN.Open();
                    using (OleDbCommand comando = new OleDbCommand(sSql, CNN))
                    {
                        DR = comando.ExecuteReader();
                        DT.Load(DR);
                        return DT;
                    }
                }
            }
            catch (Exception ex)
            {
                RegistrarErrorSql(ex, sSql);
                throw;
            }
        }
        public void EjecucionDirecta(string sSql)
        {
            try
            {
                using (OleDbConnection CNN = GetConexion())
                {
                    CNN.Open();
                    using (OleDbCommand comando = new OleDbCommand(sSql, CNN))
                    {
                        comando.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                RegistrarErrorSql(ex, sSql);
                throw;
            }
        }
    }
}
