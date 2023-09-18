using System;
using System.Collections.Generic;
using System.Data;
using CapaAccesoDatos;
using CapaAccesoDatos.Administrador;

namespace CapaLogicaNegocio.Administrador
{
    public class CN_Pedidos
    {
        private CD_Pedidos cdPedido = new CD_Pedidos();

        #region ATRIBUTOS
        private int idPedido;
        private string nombreCliente;
        private string apellido;
        private string idMenu;
        private string cantidad;
        private string mesa;
        private string instruccionesEspeciales;
        private string opcionDePago;
        #endregion

        #region PROPERTIES

        public int IdPedido
        {
            get => idPedido;
            set { idPedido = value; }
        }

        public string NombreCliente
        {
            get => nombreCliente; 
            set { nombreCliente = value; }
        }
        public string Mesa
        {
            get => mesa;
            set { mesa = value; }
        }

        public string Apellido
        {
            get => apellido;
            set { apellido = value; }
        }
        public string IdMenu
        {
            get => idMenu;
            set { idMenu = value; }
        }

        public string Cantidad
        {
            get => cantidad;
            set { cantidad = value; }
        }

        public string InstruccionesEspeciales
        {
            get => instruccionesEspeciales;
            set { instruccionesEspeciales = value; }
        }

        public string OpcionDePago
        {
            get => opcionDePago;
            set { opcionDePago = value; }
        }

        #endregion

        #region METODOS

        public DataTable MostrarPedido()
        {
            DataTable tabla = new DataTable();
            tabla = cdPedido.MostrarPedido();
            return tabla;
        }

        public void InsertarPedido()
        {
            PasarDatos();
            cdPedido.InsertarPedido();
        }

        public void ModificarPedido()
        {
            PasarDatos();
            cdPedido.ModificarPedido();
        }

        public void EliminarPedido()
        {
            cdPedido.IdPedido = idPedido;
            cdPedido.EliminarPedido();
        }

        private void PasarDatos()
        {
            cdPedido.IdPedido = IdPedido;
            cdPedido.NombreCliente = NombreCliente;
            cdPedido.Apellido = Apellido;
            cdPedido.Cantidad = Cantidad;
            cdPedido.InstruccionesEspeciales = InstruccionesEspeciales;
            cdPedido.OpcionDePago = OpcionDePago;
            cdPedido.IdMenu = IdMenu;
        }

        #endregion
    }
}
