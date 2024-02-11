using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CapaAccesoDatos;

namespace CapaLogicaNegocio
{
    public class CL_clsBitacora
    {
        private string evento;
        private string detalle;
        private string origen;

        public CL_clsBitacora(string evento, string detalle, string origen)
        {
            this.evento = evento;
            this.detalle = detalle;
            this.origen = origen;

            CD_clsBitacora Guardar = new CD_clsBitacora(evento, detalle, origen);
        }

        public CL_clsBitacora()
        {
        }

        public DataTable MostrarFacturacion()
        {
            DataTable tabla = new DataTable();
            tabla = CD_clsBitacora.verBitacoraFacturacion();
            return tabla;
        }

        public DataTable MostrarMenu()
        {
            DataTable tabla = new DataTable();
            tabla = CD_clsBitacora.verBitacoraMenu();
            return tabla;
        }

        public DataTable MostrarPersonal()
        {
            DataTable tabla = new DataTable();
            tabla = CD_clsBitacora.verBitacoraPersonal();
            return tabla;
        }

        public DataTable MostrarProducto()
        {
            DataTable tabla = new DataTable();
            tabla = CD_clsBitacora.verBitacoraProducto();
            return tabla;
        }

        public DataTable MostrarProveedores()
        {
            DataTable tabla = new DataTable();
            tabla = CD_clsBitacora.verBitacoraProveedores();
            return tabla;
        }

        public DataTable MostrarStock()
        {
            DataTable tabla = new DataTable();
            tabla = CD_clsBitacora.verBitacoraStock();
            return tabla;
        }
        public DataTable MostrarLogin()
        {
            DataTable tabla = new DataTable();
            tabla = CD_clsBitacora.verBitacoraLogin();
            return tabla;
        }
    }
}
