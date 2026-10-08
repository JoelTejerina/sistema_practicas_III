using CapaAccesoDatos.Administrador;
using System.Data;

namespace CapaLogicaNegocio.Administrador.ABMsUbicaciones
{
    public class CLN_Localidad
    {
        private CD_Localidades localidad = new CD_Localidades();

        public DataTable MostrarLocalidad()
        {
            return localidad.Mostrar();
        }
    }
}
