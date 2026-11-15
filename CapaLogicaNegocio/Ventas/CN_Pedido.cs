using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
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

        /// <summary>Salon, Barra o Delivery.</summary>
        public string Canal { get; set; } = "Salon";

        /// <summary>Mesa: obligatoria cuando el canal es Salon.</summary>
        public string Mesa { get; set; }

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

        private static readonly string[] Canales = { "Salon", "Barra", "Delivery" };

        private void ValidarCanal()
        {
            if (!Canales.Contains(Canal))
            {
                throw new InvalidOperationException(Idioma.Texto("Msg_CanalInvalido"));
            }
            if (Canal == "Salon" && string.IsNullOrWhiteSpace(Mesa))
            {
                throw new InvalidOperationException(Idioma.Texto("Msg_MesaObligatoria"));
            }
            if ((Mesa ?? "").Trim().Length > 20)
            {
                throw new InvalidOperationException(Idioma.Texto("Msg_MesaLarga"));
            }
        }

        /// <summary>
        /// Confirma el pedido. Si el stock no alcanza lanza StockInsuficienteException con el detalle
        /// de lo que falta y lo que se puede producir; si alcanza descuenta los insumos.
        /// </summary>
        public int InsertarPedido()
        {
            ValidarCanal();
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
            ValidarCanal();
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

        /// <summary>Cobra el pedido. Hace falta una caja abierta: el cobro queda asociado a ella.</summary>
        public void CobrarPedido()
        {
            System.Data.DataRow caja = new CD_Caja().Abierta();
            if (caja == null)
            {
                throw new InvalidOperationException(Idioma.Texto("Msg_CajaCerrada"));
            }
            pedido.IdPedido = IdPedido;
            pedido.CobrarPedido(Convert.ToInt32(caja["IdCaja"]));
        }

        /// <summary>Cancela un pedido (CU-PED-003): solo los pendientes; devuelve los insumos al stock.</summary>
        public void CancelarPedido(int idPedido)
        {
            if (!EsEstado(pedido.EstadoDe(idPedido), "Pendiente"))
            {
                throw new InvalidOperationException(Idioma.Texto("Msg_SoloCancelarPendiente"));
            }
            pedido.CambiarEstado(idPedido, CD_Pedido.EstadoCancelado);
        }

        /// <summary>Comprobante de venta NO fiscal del pedido (el sistema no emite facturas).</summary>
        public string ArmarTicket(int idPedido)
        {
            System.Data.DataRow cab = pedido.Cabecera(idPedido);
            if (cab == null)
            {
                throw new InvalidOperationException(Idioma.Texto("Msg_SeleccioneRegistro"));
            }

            const int ancho = 40;
            Func<string, string> centrar = t => t.Length >= ancho ? t : new string(' ', (ancho - t.Length) / 2) + t;
            System.Globalization.CultureInfo cultura = System.Globalization.CultureInfo.CurrentCulture;
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.AppendLine(centrar("LA NUEVA MODERNA S.A."));
            sb.AppendLine(centrar(Idioma.Texto("Ticket_NoFiscal")));
            sb.AppendLine(new string('-', ancho));
            sb.AppendLine(Idioma.Texto("Ticket_Pedido") + ": " + cab["IdPedido"]);
            sb.AppendLine(Idioma.Texto("Fecha") + ": " + Convert.ToDateTime(cab["Fecha"]).ToString("dd/MM/yyyy"));
            sb.AppendLine(Idioma.Texto("Cliente") + ": " + cab["NombreCliente"]);
            sb.AppendLine(Idioma.Texto("Canal") + ": " + Idioma.Texto("Canal_" + cab["Canal"]) +
                (Convert.ToString(cab["Mesa"]).Length > 0 ? "  (" + Idioma.Texto("Mesa") + " " + cab["Mesa"] + ")" : ""));
            sb.AppendLine(Idioma.Texto("Ticket_Atendio") + ": " + cab["Empleado"]);
            sb.AppendLine(new string('-', ancho));
            foreach (System.Data.DataRow fila in pedido.Detalle(idPedido).Rows)
            {
                decimal sub = Convert.ToInt32(fila["Cantidad"]) * Convert.ToDecimal(fila["PrecioUnitario"]);
                string izq = fila["Cantidad"] + " x " + fila["Platillo"];
                string der = sub.ToString("N2", cultura);
                sb.AppendLine(izq.Length + der.Length + 1 > ancho
                    ? izq.Substring(0, Math.Max(1, ancho - der.Length - 2)) + " " + der
                    : izq + new string(' ', ancho - izq.Length - der.Length) + der);
            }
            sb.AppendLine(new string('-', ancho));
            string total = Convert.ToDecimal(cab["PrecioTotal"]).ToString("N2", cultura);
            string etiqueta = Idioma.Texto("PrecioTotal").ToUpper();
            sb.AppendLine(etiqueta + new string(' ', Math.Max(1, ancho - etiqueta.Length - total.Length - 2)) + "$ " + total);
            sb.AppendLine(Idioma.Texto("FormaPago") + ": " + cab["FormaPago"]);
            if (Convert.ToString(cab["Instrucciones"]).Length > 0)
            {
                sb.AppendLine(Idioma.Texto("Instrucciones") + ": " + cab["Instrucciones"]);
            }
            sb.AppendLine(new string('-', ancho));
            sb.AppendLine(centrar(Idioma.Texto("Ticket_Gracias")));
            return sb.ToString();
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
            pedido.Canal = Canal;
            pedido.Mesa = Mesa;
        }
        #endregion
    }
}
