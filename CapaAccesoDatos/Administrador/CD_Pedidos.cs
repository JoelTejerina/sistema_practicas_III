using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using CapaComun;
using System.Data.OleDb;
using CapaAccesoDatos;

namespace CapaAccesoDatos.Administrador
{
    public class CD_Pedidos
    {
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
             string sSql;
             sSql = "Select * from Pedidos";
             clsEjecutarComando Ejecutar = new clsEjecutarComando();
             return Ejecutar.Ejecutar(sSql);  
            
        }

            public void InsertarPedido()
        {
            string sSql = "INSERT INTO Pedidos" +
               "(nombreCliente, apellido, idMenu, mesa, instruccionesEspeciales, opcionDePago) " +
                "values" + " ('" + NombreCliente + "','" + Apellido + "'," + IdMenu + "," + Mesa + ",'" + InstruccionesEspeciales + ",'" + OpcionDePago + "')";
            clsEjecutarComando Ejecutar = new clsEjecutarComando();
            Ejecutar.Ejecutar(sSql);
        }

        public void ModificarPedido()
        {
            string sSql = "UPDATE Pedidos set " +
                "nombreCliente='" + NombreCliente + "', apellido='" + Apellido + "', idMenu =" + IdMenu +
                ", mesa = " + Mesa + ", instruccionesEspeciales = '" + InstruccionesEspeciales + "', " + OpcionDePago + "', " +
                " WHERE idPedido =" + IdPedido;
            clsEjecutarComando Ejecutar = new clsEjecutarComando();
            Ejecutar.Ejecutar(sSql);
        }

        public void EliminarPedido()
        {
            string sSql = "DELETE FROM Pedidos WHERE idPedido =" + IdPedido;
            clsEjecutarComando Ejecutar = new clsEjecutarComando();
            Ejecutar.Ejecutar(sSql);
        }        
        #endregion
    }
}
