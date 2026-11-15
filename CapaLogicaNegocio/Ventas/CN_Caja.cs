using System;
using System.Data;
using CapaAccesoDatos.Ventas;
using CapaComun;

namespace CapaLogicaNegocio.Ventas
{
    /// <summary>Apertura y cierre de caja, y los datos que se ven mientras esta abierta.</summary>
    public class CN_Caja
    {
        private readonly CD_Caja datos = new CD_Caja();

        public DataRow Abierta()
        {
            return datos.Abierta();
        }

        public int Abrir(decimal montoInicial)
        {
            if (montoInicial < 0)
            {
                throw new InvalidOperationException(Idioma.Texto("Msg_MontoInvalido"));
            }
            if (datos.Abierta() != null)
            {
                throw new InvalidOperationException(Idioma.Texto("Msg_CajaYaAbierta"));
            }
            try
            {
                return datos.Abrir(montoInicial, UserCache.IdUsuario);
            }
            catch (System.Data.SqlClient.SqlException)
            {
                // Dos personas abrieron a la vez: el indice unico de la base deja pasar solo a una.
                throw new InvalidOperationException(Idioma.Texto("Msg_CajaYaAbierta"));
            }
        }

        /// <summary>Efectivo que deberia haber en la caja: monto inicial + lo cobrado en efectivo.</summary>
        public decimal EfectivoEsperado(int idCaja, decimal montoInicial)
        {
            return montoInicial + datos.CobradoEnEfectivo(idCaja);
        }

        public void Cerrar(int idCaja, decimal montoContado, string observaciones)
        {
            if (montoContado < 0)
            {
                throw new InvalidOperationException(Idioma.Texto("Msg_MontoInvalido"));
            }
            datos.Cerrar(idCaja, montoContado, observaciones, UserCache.IdUsuario);
        }

        public DataTable CobrosPorFormaPago(int idCaja)
        {
            return datos.CobrosPorFormaPago(idCaja);
        }

        public decimal TotalCobrado(int idCaja)
        {
            return datos.TotalCobrado(idCaja);
        }

        public DataTable CuentasPorMesa()
        {
            return datos.CuentasPorMesa();
        }

        public int PedidosSinCobrar()
        {
            return datos.PedidosSinCobrar();
        }

        public DataTable Historial()
        {
            return datos.Historial();
        }
    }
}
