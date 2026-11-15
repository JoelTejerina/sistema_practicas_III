using System;
using System.Data;
using CapaAccesoDatos.Loguin;

namespace CapaAccesoDatos.Administrador
{
    /// <summary>
    /// Mantenimiento de usuarios: consulta, modificacion, baja, reactivacion y contraseñas temporales.
    /// Toda modificacion recalcula el digito verificador de Usuarios.
    /// </summary>
    public class CD_GestionUsuarios
    {
        private static string T(string valor)
        {
            return (valor ?? "").Replace("'", "''");
        }

        private static void Recalcular()
        {
            new CD_Seguridad().RecalcularDigitos();
        }

        public DataTable Listar()
        {
            string sSql =
                "SELECT u.IdUsuario, u.Usuario, p.Apellido, p.Nombres, ISNULL(g.Grupo, N'(sin rol)') AS Rol, " +
                "CASE WHEN u.FechaBaja IS NOT NULL THEN N'Baja' WHEN u.UsuarioDesactivado = 1 THEN N'Bloqueado' ELSE N'Activo' END AS Estado, " +
                "u.FechaAlta, u.FechaBaja, u.CambiaCada, u.DebeCambiarPassword " +
                "FROM Usuarios u INNER JOIN Personal p ON p.IdPersona = u.IdPersona " +
                "LEFT JOIN UsuariosGrupos ug ON ug.IdUsuario = u.IdUsuario " +
                "LEFT JOIN Grupos g ON g.IdGrupo = ug.IdGrupo ORDER BY u.Usuario";
            return new clsEjecutarComando().Ejecutar(sSql);
        }

        public bool ExisteUsuario(string usuario, int excluirIdUsuario)
        {
            DataTable dt = new clsEjecutarComando().Ejecutar(
                "SELECT 1 FROM Usuarios WHERE Usuario = N'" + T(usuario.Trim()) + "' AND IdUsuario <> " + excluirIdUsuario);
            return dt.Rows.Count > 0;
        }

        public void Modificar(int idUsuario, string usuario, int cambiaCada)
        {
            new clsEjecutarComando().EjecucionDirecta(
                "UPDATE Usuarios SET Usuario = N'" + T(usuario.Trim()) + "', CambiaCada = " + cambiaCada + " WHERE IdUsuario = " + idUsuario);
            Recalcular();
        }

        // Baja: el usuario queda desactivado con fecha de baja (no se borra para conservar su historial).
        public void DarDeBaja(int idUsuario)
        {
            new clsEjecutarComando().EjecucionDirecta(
                "UPDATE Usuarios SET UsuarioDesactivado = 1, FechaBaja = CAST(GETDATE() AS DATE), " +
                "FechaDesactivacion = CAST(GETDATE() AS DATE) WHERE IdUsuario = " + idUsuario + "; " +
                "DELETE FROM SesionActiva WHERE IdUsuario = " + idUsuario);
            Recalcular();
        }

        // Reactivar sirve tanto para un usuario bloqueado por intentos fallidos como para uno dado de baja.
        public void Reactivar(int idUsuario)
        {
            new clsEjecutarComando().EjecucionDirecta(
                "UPDATE Usuarios SET UsuarioDesactivado = 0, FechaBaja = NULL, FechaDesactivacion = NULL WHERE IdUsuario = " + idUsuario);
            Recalcular();
        }

        /// <summary>Deja una contraseña (ya cifrada) que el usuario esta obligado a cambiar en su proximo ingreso.</summary>
        public void EstablecerPasswordTemporal(int idUsuario, string hash)
        {
            new clsEjecutarComando().EjecucionDirecta(
                "UPDATE Usuarios SET [Password] = N'" + T(hash) + "', DebeCambiarPassword = 1, " +
                "FechaUltimoCambio = CAST(GETDATE() AS DATE) WHERE IdUsuario = " + idUsuario);
            Recalcular();
        }

        /// <summary>Datos para la recuperacion de contraseña de un usuario (vacio si no existe).</summary>
        public DataTable DatosParaRecuperacion(string usuario)
        {
            return new clsEjecutarComando().Ejecutar(
                "SELECT u.IdUsuario, u.[Password], u.UsuarioDesactivado, u.FechaBaja, u.DebeCambiarPassword, " +
                "ISNULL(p.Correo, N'') AS Correo, p.Apellido, p.Nombres " +
                "FROM Usuarios u INNER JOIN Personal p ON p.IdPersona = u.IdPersona " +
                "WHERE u.Usuario = N'" + T(usuario.Trim()) + "'");
        }

        /// <summary>Restaura el estado de un usuario (si fallo el envio del mail de recuperacion).</summary>
        public void RestaurarClave(int idUsuario, string hash, bool desactivado, bool debeCambiar)
        {
            new clsEjecutarComando().EjecucionDirecta(
                "UPDATE Usuarios SET [Password] = N'" + T(hash) + "', UsuarioDesactivado = " + (desactivado ? 1 : 0) +
                ", DebeCambiarPassword = " + (debeCambiar ? 1 : 0) + " WHERE IdUsuario = " + idUsuario);
            Recalcular();
        }

        /// <summary>Desbloquea a un usuario bloqueado por intentos fallidos (no a uno dado de baja).</summary>
        public void DesbloquearPorIntentos(int idUsuario)
        {
            new clsEjecutarComando().EjecucionDirecta(
                "UPDATE Usuarios SET UsuarioDesactivado = 0 WHERE IdUsuario = " + idUsuario + " AND FechaBaja IS NULL");
            Recalcular();
        }

        /// <summary>Cuantos usuarios activos tiene el rol Gerente (siempre tiene que quedar al menos uno).</summary>
        public int GerentesActivos()
        {
            DataTable dt = new clsEjecutarComando().Ejecutar(
                "SELECT COUNT(*) FROM Usuarios u INNER JOIN UsuariosGrupos ug ON ug.IdUsuario = u.IdUsuario " +
                "INNER JOIN Grupos g ON g.IdGrupo = ug.IdGrupo WHERE g.Grupo = N'Gerente' AND u.UsuarioDesactivado = 0 AND u.FechaBaja IS NULL");
            return Convert.ToInt32(dt.Rows[0][0]);
        }

        public string RolDelUsuario(int idUsuario)
        {
            DataTable dt = new clsEjecutarComando().Ejecutar(
                "SELECT g.Grupo FROM UsuariosGrupos ug INNER JOIN Grupos g ON g.IdGrupo = ug.IdGrupo WHERE ug.IdUsuario = " + idUsuario);
            return dt.Rows.Count == 0 ? "" : Convert.ToString(dt.Rows[0][0]);
        }
    }
}
