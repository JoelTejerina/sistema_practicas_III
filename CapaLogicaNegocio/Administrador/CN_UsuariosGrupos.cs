using System.Data;
using CapaAccesoDatos.Administrador;

namespace CapaLogicaNegocio.Administrador
{
    public class CN_UsuariosGrupos
    {
        private CD_UsuariosGrupos datos = new CD_UsuariosGrupos();

        public int IdUsuario { get; set; }
        public int IdGrupo { get; set; }

        public DataTable MostrarUsuariosConRol()
        {
            return datos.MostrarUsuariosConRol();
        }

        public void AsignarGrupo()
        {
            datos.IdUsuario = IdUsuario;
            datos.IdGrupo = IdGrupo;
            datos.AsignarGrupo();
        }

        public int ObtenerIdGrupo(int idUsuario)
        {
            return datos.ObtenerIdGrupo(idUsuario);
        }
    }
}
