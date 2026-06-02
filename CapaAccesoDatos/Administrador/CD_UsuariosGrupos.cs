using System.Data;
using CapaAccesoDatos;

namespace CapaAccesoDatos.Administrador
{
    public class CD_UsuariosGrupos
    {
        public int IdUsuario { get; set; }
        public int IdGrupo { get; set; }

        public DataTable MostrarUsuariosConRol()
        {
            string sSql = "SELECT u.IdUsuario, u.Usuario, g.IdGrupo, g.Grupo " +
                "FROM (Usuarios AS u LEFT JOIN UsuariosGrupos AS ug ON u.IdUsuario = ug.IdUsuario) " +
                "LEFT JOIN Grupos AS g ON ug.IdGrupo = g.IdGrupo " +
                "ORDER BY u.Usuario";
            clsEjecutarComando ejecutar = new clsEjecutarComando();
            return ejecutar.Ejecutar(sSql);
        }

        public void AsignarGrupo()
        {
            clsEjecutarComando ejecutar = new clsEjecutarComando();
            ejecutar.EjecucionDirecta("DELETE FROM UsuariosGrupos WHERE IdUsuario = " + IdUsuario);
            if (IdGrupo > 0)
            {
                ejecutar.EjecucionDirecta(
                    "INSERT INTO UsuariosGrupos (IdUsuario, IdGrupo) VALUES (" + IdUsuario + ", " + IdGrupo + ")");
            }
        }

        public int ObtenerIdGrupo(int idUsuario)
        {
            string sSql = "SELECT IdGrupo FROM UsuariosGrupos WHERE IdUsuario = " + idUsuario;
            clsEjecutarComando ejecutar = new clsEjecutarComando();
            DataTable dt = ejecutar.Ejecutar(sSql);
            if (dt.Rows.Count > 0 && dt.Rows[0][0] != System.DBNull.Value)
            {
                return System.Convert.ToInt32(dt.Rows[0][0]);
            }
            return 0;
        }
    }
}
