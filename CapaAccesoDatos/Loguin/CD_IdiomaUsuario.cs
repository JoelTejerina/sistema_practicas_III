using System;
using System.Data;

namespace CapaAccesoDatos.Loguin
{
    /// <summary>Idioma elegido por cada usuario (se carga al iniciar sesion).</summary>
    public class CD_IdiomaUsuario
    {
        /// <summary>Codigo de idioma guardado (es, en, pt) o null si el usuario no eligio ninguno.</summary>
        public string Obtener(int idUsuario)
        {
            clsEjecutarComando ejecutar = new clsEjecutarComando();
            DataTable dt = ejecutar.Ejecutar("SELECT Idioma FROM Usuarios WHERE IdUsuario = " + idUsuario);
            if (dt.Rows.Count == 0 || dt.Rows[0][0] == DBNull.Value)
            {
                return null;
            }
            return Convert.ToString(dt.Rows[0][0]).Trim();
        }

        public void Guardar(int idUsuario, string idioma)
        {
            clsEjecutarComando ejecutar = new clsEjecutarComando();
            ejecutar.EjecucionDirecta("UPDATE Usuarios SET Idioma = '" + (idioma ?? "").Replace("'", "''") +
                "' WHERE IdUsuario = " + idUsuario);
        }
    }
}
