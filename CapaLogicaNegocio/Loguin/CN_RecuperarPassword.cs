using System;
using System.Data;
using CapaAccesoDatos.Administrador;
using CapaComun;
using CapaLogicaNegocio.Administrador;

namespace CapaLogicaNegocio.Loguin
{
    /// <summary>
    /// Recuperacion de contraseña por mail (CU-SEG-003): se genera una contraseña temporal, se la envia al
    /// correo de la persona y el usuario queda obligado a cambiarla al ingresar. Tambien destraba a un usuario
    /// bloqueado por intentos fallidos. Por seguridad el resultado es el mismo exista o no el usuario.
    /// </summary>
    public class CN_RecuperarPassword
    {
        private readonly CD_GestionUsuarios datos = new CD_GestionUsuarios();

        /// <summary>false si el envio de correo no esta configurado (no se hace ningun cambio).</summary>
        public bool Disponible
        {
            get { return CorreoSmtp.Configurado; }
        }

        public void Recuperar(string usuario)
        {
            if (string.IsNullOrWhiteSpace(usuario))
            {
                throw new InvalidOperationException(Idioma.Texto("Msg_CompletarUsuario"));
            }
            if (!Disponible)
            {
                throw new InvalidOperationException(Idioma.Texto("Msg_CorreoNoConfigurado"));
            }

            DataTable dt = datos.DatosParaRecuperacion(usuario);
            if (dt.Rows.Count == 0)
            {
                return;   // mismo resultado que si existiera: no se revela que usuarios hay
            }

            DataRow u = dt.Rows[0];
            string correo = Convert.ToString(u["Correo"]).Trim();
            bool dadoDeBaja = u["FechaBaja"] != DBNull.Value;
            if (dadoDeBaja || correo.Length == 0)
            {
                return;
            }

            int idUsuario = Convert.ToInt32(u["IdUsuario"]);
            string claveAnterior = Convert.ToString(u["Password"]);
            bool estabaBloqueado = Convert.ToBoolean(u["UsuarioDesactivado"]);
            bool debiaCambiar = Convert.ToBoolean(u["DebeCambiarPassword"]);

            string temporal = GeneradorClave.Nueva(10);
            datos.EstablecerPasswordTemporal(idUsuario, clsSeguridad.SHA256(temporal));
            if (estabaBloqueado)
            {
                datos.DesbloquearPorIntentos(idUsuario);
            }

            try
            {
                CorreoSmtp.Enviar(correo, Idioma.Texto("Mail_Asunto"),
                    string.Format(Idioma.Texto("Mail_Cuerpo"), Convert.ToString(u["Nombres"]), usuario.Trim(), temporal));
            }
            catch (Exception)
            {
                // Si el mail no sale, el usuario conserva su contraseña anterior.
                datos.RestaurarClave(idUsuario, claveAnterior, estabaBloqueado, debiaCambiar);
                throw;
            }

            new CL_clsBitacora("Recuperación de contraseña", "Se envió una contraseña temporal al usuario " + usuario.Trim(), "frmLoguin");
        }
    }
}
