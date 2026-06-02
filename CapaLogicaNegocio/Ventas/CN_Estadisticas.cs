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

        public DataTable PedidosDelDia()
        {
            return datos.PedidosDelDia();
        }
    }
}
