using System.Data;
using CapaAccesoDatos.Ventas;

namespace CapaLogicaNegocio.Ventas
{
    public class CN_Estadisticas
    {
        private CD_Estadisticas datos = new CD_Estadisticas();

        public DataTable ResumenPedidosPorEstado()
        {
            return datos.ResumenPedidosPorEstado();
        }

        public DataTable VentasPorPeriodo(System.DateTime desde, System.DateTime hasta, string agrupar)
        {
            if (desde.Date > hasta.Date)
            {
                throw new System.InvalidOperationException(CapaComun.Idioma.Texto("Msg_RangoFechasInvalido"));
            }
            return datos.VentasPorPeriodo(desde.Date, hasta.Date, agrupar);
        }

        public DataTable PlatosMasVendidos(System.DateTime desde, System.DateTime hasta, int cuantos)
        {
            if (desde.Date > hasta.Date)
            {
                throw new System.InvalidOperationException(CapaComun.Idioma.Texto("Msg_RangoFechasInvalido"));
            }
            return datos.PlatosMasVendidos(desde.Date, hasta.Date, cuantos);
        }

        public DataTable PedidosDelDia()
        {
            return datos.PedidosDelDia();
        }
    }
}
