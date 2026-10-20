using System;
using System.Collections.Generic;
using System.Data;
using CapaAccesoDatos.Ventas;
using CapaComun;

namespace CapaLogicaNegocio.Ventas
{
    public class CN_Pedido
    {
        private CD_Pedido pedido = new CD_Pedido();

        #region PROPERTIES
        public int IdPedido { get; set; }
        public string NombreCliente { get; set; }
        public string FormaPago { get; set; }
        public string InstruccionesEspeciales { get; set; }
        public string Estado { get; set; }
        public DateTime Fecha { get; set; }

        /// <summary>Estado que tenia el pedido antes de modificarlo (para aplicar la regla de cancelacion).</summary>
        public string EstadoAnterior { get; set; }

        /// <summary>Detalle del pedido: platillos y cantidades.</summary>
        public List<ItemPedido> Items { get; set; } = new List<ItemPedido>();
        #endregion

        #region METODOS
        public DataTable MostrarPedido()
        {
            return pedido.Mostrar();
        }

        public DataTable DetallePedido(int idPedido)
        {
            return pedido.Detalle(idPedido);
        }

        public DataTable MostrarEnElaboracion()
        {
            return pedido.MostrarEnElaboracion();
        }

        /// <summary>
        /// Confirma el pedido. Si el stock no alcanza lanza StockInsuficienteException con el detalle
        /// de lo que falta y lo que se puede producir; si alcanza descuenta los insumos.
        /// </summary>
        public int InsertarPedido()
        {
            PasarDatos();
            return pedido.InsertarPedido(Items);
        }

        public void ModificarPedido()
        {
            // Solo se pueden cancelar pedidos pendientes (CU-005-002).
            if (EsEstado(Estado, CD_Pedido.EstadoCancelado) && !EsEstado(EstadoAnterior, CD_Pedido.EstadoCancelado)
                && !EsEstado(EstadoAnterior, "Pendiente"))
            {
                throw new InvalidOperationException(Idioma.Texto("Msg_SoloCancelarPendiente"));
            }
            PasarDatos();
            pedido.ModificarPedido(Items);
        }

        /// <summary>Cambia el estado desde Elaboracion (iniciar, finalizar o cancelar la elaboracion).</summary>
        public void CambiarEstado(int idPedido, string nuevoEstado)
        {
            pedido.CambiarEstado(idPedido, nuevoEstado);
        }

        public void EliminarPedido()
        {
            pedido.IdPedido = IdPedido;
            pedido.EliminarPedido();
        }

        public void CobrarPedido()
        {
            pedido.IdPedido = IdPedido;
            pedido.CobrarPedido();
        }

        private static bool EsEstado(string actual, string esperado)
        {
            return string.Equals((actual ?? "").Trim(), esperado, StringComparison.OrdinalIgnoreCase);
        }

        private void PasarDatos()
        {
            pedido.IdPedido = IdPedido;
            pedido.NombreCliente = NombreCliente;
            pedido.FormaPago = FormaPago;
            pedido.InstruccionesEspeciales = InstruccionesEspeciales;
            pedido.Estado = Estado;
            pedido.Fecha = Fecha;
        }
        #endregion
    }
}
