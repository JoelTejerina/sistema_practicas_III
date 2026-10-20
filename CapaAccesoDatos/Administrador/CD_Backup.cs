using System;
using System.Data.SqlClient;

namespace CapaAccesoDatos.Administrador
{
    /// <summary>Copia de respaldo (backup) de la base de datos del sistema.</summary>
    public class CD_Backup : clsConexion
    {
        /// <summary>Carpeta de respaldos por defecto de la instancia de SQL Server (la que ella puede escribir).</summary>
        public string RutaPredeterminada()
        {
            using (SqlConnection cn = GetConexion())
            {
                cn.Open();
                using (SqlCommand cmd = new SqlCommand("SELECT CAST(SERVERPROPERTY('InstanceDefaultBackupPath') AS NVARCHAR(500))", cn))
                {
                    object r = cmd.ExecuteScalar();
                    return r == null || r == DBNull.Value ? "" : Convert.ToString(r);
                }
            }
        }

        public string NombreBase()
        {
            using (SqlConnection cn = GetConexion())
            {
                cn.Open();
                return cn.Database;
            }
        }

        /// <summary>
        /// Genera el respaldo en la ruta indicada. La escribe el servicio de SQL Server, asi que la carpeta
        /// tiene que ser una a la que ese servicio tenga acceso (por ejemplo la que devuelve RutaPredeterminada).
        /// </summary>
        public void Respaldar(string rutaArchivo)
        {
            using (SqlConnection cn = GetConexion())
            {
                cn.Open();
                string baseDatos = cn.Database.Replace("]", "]]");
                using (SqlCommand cmd = new SqlCommand("BACKUP DATABASE [" + baseDatos + "] TO DISK = @ruta WITH INIT, NAME = @nombre", cn))
                {
                    cmd.CommandTimeout = 600;
                    cmd.Parameters.AddWithValue("@ruta", rutaArchivo);
                    cmd.Parameters.AddWithValue("@nombre", "Respaldo " + cn.Database + " " + DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
