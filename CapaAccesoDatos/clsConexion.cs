using System;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;

namespace CapaAccesoDatos
{
    /// <summary>
    /// Base de las clases de acceso a datos. Resuelve la cadena de conexion a SQL Server.
    ///
    /// Como somos varios desarrolladores y cada uno tiene su propio servidor/instancia,
    /// la cadena NO esta fija en el codigo. Se busca, en este orden:
    ///   1) Variable de entorno SISTEMA_DB_CONNECTION
    ///   2) Archivo conexion.local.txt (se busca junto al .exe y en las carpetas padre,
    ///      asi que alcanza con tenerlo en la raiz del repo). Lo crea
    ///      Database\Instalar-BaseDeDatos.bat y esta ignorado por git.
    ///   3) Cadena "SistemaDB" de App.config (valor por defecto: .\SQLEXPRESS)
    /// </summary>
    public abstract class clsConexion
    {
        private const string VariableEntorno = "SISTEMA_DB_CONNECTION";
        private const string ArchivoLocal = "conexion.local.txt";
        private const string NombreConfig = "SistemaDB";

        private static string cadenaCache;
        private static readonly object bloqueo = new object();

        protected SqlConnection GetConexion()
        {
            return new SqlConnection(ObtenerCadena());
        }

        /// <summary>Cadena de conexion vigente (se resuelve una sola vez por ejecucion).</summary>
        public static string ObtenerCadena()
        {
            if (cadenaCache != null)
            {
                return cadenaCache;
            }

            lock (bloqueo)
            {
                if (cadenaCache != null)
                {
                    return cadenaCache;
                }

                string cadena = Environment.GetEnvironmentVariable(VariableEntorno);

                if (string.IsNullOrWhiteSpace(cadena))
                {
                    cadena = LeerArchivoLocal();
                }

                if (string.IsNullOrWhiteSpace(cadena))
                {
                    ConnectionStringSettings cfg = ConfigurationManager.ConnectionStrings[NombreConfig];
                    if (cfg != null)
                    {
                        cadena = cfg.ConnectionString;
                    }
                }

                if (string.IsNullOrWhiteSpace(cadena))
                {
                    throw new InvalidOperationException(
                        "No se encontro la cadena de conexion a la base de datos. " +
                        "Ejecute Database\\Instalar-BaseDeDatos.bat o defina " + VariableEntorno + ".");
                }

                cadenaCache = cadena.Trim();
                return cadenaCache;
            }
        }

        private static string LeerArchivoLocal()
        {
            try
            {
                DirectoryInfo dir = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
                for (int nivel = 0; dir != null && nivel < 8; nivel++, dir = dir.Parent)
                {
                    string ruta = Path.Combine(dir.FullName, ArchivoLocal);
                    if (!File.Exists(ruta))
                    {
                        continue;
                    }

                    foreach (string linea in File.ReadAllLines(ruta))
                    {
                        string t = linea.Trim();
                        if (t.Length > 0 && !t.StartsWith("#") && !t.StartsWith("//"))
                        {
                            return t;
                        }
                    }
                }
            }
            catch (Exception)
            {
                // Si el archivo no se puede leer se sigue con App.config.
            }

            return null;
        }
    }
}
