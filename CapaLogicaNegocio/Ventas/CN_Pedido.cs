using System;
using System.Data;
using CapaAccesoDatos.Ventas;

namespace CapaLogicaNegocio.Ventas
{
    public class CN_Pedido
    {
        private CD_Pedido pedido = new CD_Pedido();

        #region PROPERTIES
        public int IdPedido { get; set; }
        public string NombreCliente { get; set; }
        public int IdMenu { get; set; }
        public int Cantidad { get; set; }
        public string FormaPago { get; set; }
        public string InstruccionesEspeciales { get; set; }
        public string Estado { get; set; }
        public decimal PrecioTotal { get; set; }
        public DateTime Fecha { get; set; }
        #endregion

        #region METODOS
        public DataTable MostrarPedido()
        {
            return pedido.Mostrar();
        }

        public void InsertarPedido()
        {
            PasarDatos();
            pedido.InsertarPedido();
        }

        public void ModificarPedido()
        {
            PasarDatos();
            pedido.ModificarPedido();
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

        private void PasarDatos()
        {
            pedido.IdPedido = IdPedido;
            pedido.NombreCliente = NombreCliente;
            pedido.IdMenu = IdMenu;
            pedido.Cantidad = Cantidad;
            pedido.FormaPago = FormaPago;
            pedido.InstruccionesEspeciales = InstruccionesEspeciales;
            pedido.Estado = Estado;
            pedido.PrecioTotal = PrecioTotal;
            pedido.Fecha = Fecha;
        }
        #endregion
    }
}
