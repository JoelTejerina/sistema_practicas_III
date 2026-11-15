using System;
using System.Data;

namespace CapaAccesoDatos.Loguin
{
    /// <summary>
    /// Seguridad de acceso: digito verificador de Usuarios, bloqueo de la terminal y sesion unica.
    /// Los controles usan la hora del servidor de base de datos para no depender del reloj de cada PC.
    /// </summary>
    public class CD_Seguridad
    {
        private static string T(string valor)
        {
            return (valor ?? "").Replace("'", "''");
        }

        // ------------------------------------------------------------ digito verificador
        /// <summary>Recalcula el DVH de cada usuario y el DVV de la tabla (se llama despues de toda modificacion).</summary>
        public void RecalcularDigitos()
        {
            new clsEjecutarComando().EjecucionDirecta("EXEC dbo.usp_RecalcularDigitosUsuarios");
        }

        /// <summary>true si ningun usuario fue alterado fuera de la aplicacion.</summary>
        public bool DigitosValidos()
        {
            DataTable dt = new clsEjecutarComando().Ejecutar("EXEC dbo.usp_VerificarDigitosUsuarios");
            return dt.Rows.Count > 0 && Convert.ToBoolean(dt.Rows[0]["Valido"]);
        }

        // ------------------------------------------------------------ bloqueo de terminal
        /// <summary>Hasta cuando esta bloqueada la terminal, o null si no lo esta.</summary>
        public DateTime? TerminalBloqueadaHasta(string terminal)
        {
            DataTable dt = new clsEjecutarComando().Ejecutar(
                "SELECT BloqueadaHasta FROM BloqueoTerminal WHERE Terminal = N'" + T(terminal) +
                "' AND BloqueadaHasta > SYSDATETIME()");
            return dt.Rows.Count == 0 ? (DateTime?)null : Convert.ToDateTime(dt.Rows[0][0]);
        }

        public void BloquearTerminal(string terminal, int minutos, string motivo)
        {
            string sSql =
                "MERGE BloqueoTerminal AS t USING (SELECT N'" + T(terminal) + "' AS Terminal) AS s ON t.Terminal = s.Terminal " +
                "WHEN MATCHED THEN UPDATE SET BloqueadaHasta = DATEADD(MINUTE, " + minutos + ", SYSDATETIME()), Motivo = N'" + T(motivo) + "' " +
                "WHEN NOT MATCHED THEN INSERT (Terminal, BloqueadaHasta, Motivo) VALUES (N'" + T(terminal) +
                "', DATEADD(MINUTE, " + minutos + ", SYSDATETIME()), N'" + T(motivo) + "');";
            new clsEjecutarComando().EjecucionDirecta(sSql);
        }

        public void LiberarTerminal(string terminal)
        {
            new clsEjecutarComando().EjecucionDirecta("DELETE FROM BloqueoTerminal WHERE Terminal = N'" + T(terminal) + "'");
        }

        // ------------------------------------------------------------ sesion unica
        /// <summary>Sesion abierta del usuario: terminal, proceso y minutos sin actividad (null si no tiene).</summary>
        public DataRow SesionDelUsuario(int idUsuario)
        {
            DataTable dt = new clsEjecutarComando().Ejecutar(
                "SELECT Terminal, IdProceso, DATEDIFF(MINUTE, UltimaActividad, SYSDATETIME()) AS MinutosInactiva " +
                "FROM SesionActiva WHERE IdUsuario = " + idUsuario);
            return dt.Rows.Count == 0 ? null : dt.Rows[0];
        }

        public void AbrirSesion(int idUsuario, string terminal, int idProceso)
        {
            string sSql =
                "MERGE SesionActiva AS t USING (SELECT " + idUsuario + " AS IdUsuario) AS s ON t.IdUsuario = s.IdUsuario " +
                "WHEN MATCHED THEN UPDATE SET Terminal = N'" + T(terminal) + "', IdProceso = " + idProceso +
                ", Inicio = SYSDATETIME(), UltimaActividad = SYSDATETIME() " +
                "WHEN NOT MATCHED THEN INSERT (IdUsuario, Terminal, IdProceso) VALUES (" + idUsuario + ", N'" + T(terminal) + "', " + idProceso + ");";
            new clsEjecutarComando().EjecucionDirecta(sSql);
        }

        public void LatidoSesion(int idUsuario, int idProceso)
        {
            new clsEjecutarComando().EjecucionDirecta(
                "UPDATE SesionActiva SET UltimaActividad = SYSDATETIME() WHERE IdUsuario = " + idUsuario + " AND IdProceso = " + idProceso);
        }

        public void CerrarSesion(int idUsuario, int idProceso)
        {
            new clsEjecutarComando().EjecucionDirecta(
                "DELETE FROM SesionActiva WHERE IdUsuario = " + idUsuario + " AND IdProceso = " + idProceso);
        }
    }
}
