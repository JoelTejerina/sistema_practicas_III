using System;
using System.Diagnostics;
using CapaAccesoDatos.Loguin;
using CapaComun;

namespace CapaLogicaNegocio.Loguin
{
    /// <summary>
    /// Controles de seguridad del ingreso (CU-SEG-001): bloqueo de la terminal, digito verificador
    /// y sesion unica por usuario.
    /// </summary>
    public class CN_Seguridad
    {
        public const int MinutosBloqueoTerminal = 10;
        public const int MinutosSesionInactiva = 15;

        private readonly CD_Seguridad datos = new CD_Seguridad();

        public static string Terminal
        {
            get { return Environment.MachineName; }
        }

        private static int ProcesoActual
        {
            get { return Process.GetCurrentProcess().Id; }
        }

        /// <summary>Hasta cuando esta bloqueada esta terminal (null si no lo esta).</summary>
        public DateTime? TerminalBloqueadaHasta()
        {
            return datos.TerminalBloqueadaHasta(Terminal);
        }

        /// <summary>false si algun usuario fue modificado fuera de la aplicacion.</summary>
        public bool IntegridadValida()
        {
            return datos.DigitosValidos();
        }

        /// <summary>Bloquea esta terminal 10 minutos por una falla del digito verificador.</summary>
        public void BloquearTerminalPorIntegridad()
        {
            datos.BloquearTerminal(Terminal, MinutosBloqueoTerminal, "Digito verificador invalido");
            new CL_clsBitacora("Seguridad",
                "Digito verificador invalido: terminal " + Terminal + " bloqueada " + MinutosBloqueoTerminal + " minutos",
                "frmLoguin");
        }

        public void LiberarTerminal()
        {
            datos.LiberarTerminal(Terminal);
        }

        /// <summary>
        /// Abre la sesion del usuario. Devuelve false (y la terminal donde esta abierta) si ya tiene
        /// otra sesion activa. Una sesion de la misma PC cuyo programa ya no esta abierto, o de otra
        /// PC inactiva hace mas de 15 minutos, se considera cerrada.
        /// </summary>
        public bool AbrirSesion(int idUsuario, out string terminalAjena)
        {
            terminalAjena = null;
            System.Data.DataRow sesion = datos.SesionDelUsuario(idUsuario);
            if (sesion != null)
            {
                string terminal = Convert.ToString(sesion["Terminal"]);
                int proceso = Convert.ToInt32(sesion["IdProceso"]);
                int inactiva = Convert.ToInt32(sesion["MinutosInactiva"]);
                // Misma PC: la sesion sigue viva solo si el programa que la abrio todavia existe (por si se cerro de golpe).
                // Otra PC: no podemos saber si el proceso existe, asi que se mira hace cuanto no marca actividad.
                bool mismaTerminal = string.Equals(terminal, Terminal, StringComparison.OrdinalIgnoreCase);
                bool vigente = mismaTerminal ? ProcesoVivo(proceso) : inactiva < MinutosSesionInactiva;
                // Si es nuestra propia sesion (mismo proceso) se permite reabrirla; si no, "vigente" quiere decir
                // que el usuario ya esta conectado en otro lado y no lo dejamos entrar.
                if (vigente && !(mismaTerminal && proceso == ProcesoActual))
                {
                    terminalAjena = terminal;
                    return false;
                }
            }
            // Llegamos aca si no tenia sesion o la que tenia estaba abandonada: la pisamos con la nueva.
            datos.AbrirSesion(idUsuario, Terminal, ProcesoActual);
            return true;
        }

        /// <summary>Marca actividad de la sesion actual (evita que se la considere abandonada).</summary>
        public void Latido()
        {
            if (UserCache.IdUsuario > 0)
            {
                datos.LatidoSesion(UserCache.IdUsuario, ProcesoActual);
            }
        }

        public void CerrarSesion()
        {
            if (UserCache.IdUsuario > 0)
            {
                datos.CerrarSesion(UserCache.IdUsuario, ProcesoActual);
            }
        }

        // GetProcessById tira ArgumentException cuando no existe un proceso con ese numero.
        private static bool ProcesoVivo(int idProceso)
        {
            try
            {
                Process.GetProcessById(idProceso);
                return true;
            }
            catch (ArgumentException)
            {
                return false;
            }
        }
    }
}
