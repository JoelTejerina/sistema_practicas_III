using System;
using System.Data;
using CapaAccesoDatos.Administrador;

namespace CapaLogicaNegocio.Administrador
{
    public class CN_Bitacora
    {
        private CD_Bitacora bitacora = new CD_Bitacora();

        public DataTable MostrarBitacora()
        {
            return bitacora.Mostrar();
        }

        public DataTable MostrarBitacoraFiltrada(string evento, string usuario, DateTime? fechaDesde, DateTime? fechaHasta)
        {
            return bitacora.MostrarFiltrada(evento, usuario, fechaDesde, fechaHasta);
        }
    }
}
