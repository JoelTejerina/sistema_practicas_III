using System;
using System.Data;
using System.Security.Cryptography;
using CapaAccesoDatos.Administrador;
using CapaComun;

namespace CapaLogicaNegocio.Administrador
{
    /// <summary>Mantenimiento de usuarios: modificar, dar de baja, reactivar y restablecer la contraseña.</summary>
    public class CN_GestionUsuarios
    {
        private readonly CD_GestionUsuarios datos = new CD_GestionUsuarios();

        public DataTable Listar()
        {
            return datos.Listar();
        }

        public void Modificar(int idUsuario, string usuario, int cambiaCada)
        {
            if (string.IsNullOrWhiteSpace(usuario))
            {
                throw new InvalidOperationException(Idioma.Texto("Msg_CompletarUsuario"));
            }
            if (cambiaCada < 0 || cambiaCada > 365)
            {
                throw new InvalidOperationException(Idioma.Texto("Msg_CambiaCadaInvalido"));
            }
            if (datos.ExisteUsuario(usuario, idUsuario))
            {
                throw new InvalidOperationException(Idioma.Texto("Msg_UsuarioExiste"));
            }
            datos.Modificar(idUsuario, usuario, cambiaCada);
        }

        /// <summary>Baja del usuario. No se puede dar de baja a uno mismo ni al ultimo Gerente activo.</summary>
        public void DarDeBaja(int idUsuario)
        {
            if (idUsuario == UserCache.IdUsuario)
            {
                throw new InvalidOperationException(Idioma.Texto("Msg_BajaPropioUsuario"));
            }
            if (datos.RolDelUsuario(idUsuario) == PermisosSistema.GrupoGerente && datos.GerentesActivos() <= 1)
            {
                throw new InvalidOperationException(Idioma.Texto("Msg_UltimoGerente"));
            }
            datos.DarDeBaja(idUsuario);
        }

        public void Reactivar(int idUsuario)
        {
            datos.Reactivar(idUsuario);
        }

        /// <summary>Genera una contraseña temporal, la guarda (cifrada) y la devuelve para entregarsela al usuario.</summary>
        public string RestablecerPassword(int idUsuario)
        {
            string temporal = GeneradorClave.Nueva(10);
            datos.EstablecerPasswordTemporal(idUsuario, clsSeguridad.SHA256(temporal));
            return temporal;
        }
    }

    /// <summary>Contraseñas temporales aleatorias (sin caracteres que se confunden: 0/O, 1/l/I).</summary>
    public static class GeneradorClave
    {
        private const string Caracteres = "ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz23456789";

        public static string Nueva(int largo)
        {
            byte[] azar = new byte[largo];
            using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(azar);
            }
            char[] clave = new char[largo];
            for (int i = 0; i < largo; i++)
            {
                clave[i] = Caracteres[azar[i] % Caracteres.Length];
            }
            return new string(clave);
        }
    }
}
