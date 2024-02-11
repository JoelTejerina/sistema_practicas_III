using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CapaComun;

namespace CapaAccesoDatos
{
    public class CD_clsBitacora
    {

        public CD_clsBitacora(string evento, string detalle, string origen)
        {
            string fecha = DateTime.Now.ToString("dd-MM-yyyy");
            string hora = DateTime.Now.ToString("HH:mm");
            int IdUsuario;
            string usuario;

            if (UserCache.IdUsuario == 0)
            { IdUsuario = VerificCache.IdUsuario; }
            else
            { IdUsuario = UserCache.IdUsuario; }

            if (UserCache.Usuario == null)
            { usuario = UserCache.Usuario; }
            else
            { usuario = UserCache.Usuario; }

            string sSQL = "Insert into Bitacora (fecha, hora, idUsuario, usuario, evento, detalle, origen) " +
                "values ('" + fecha + "', '" + hora + "', '" + IdUsuario + "', '" + usuario + "', '" + evento + "', '" + detalle + "', '" + origen + "')";

            clsEjecutarComando Ejecutar = new clsEjecutarComando();
            Ejecutar.Ejecutar(sSQL);
        }
        public static DataTable verBitacoraFacturacion()
        {
            string sSql = "SELECT * FROM Bitacora WHERE origen = 'frmFacturacion'";
            clsEjecutarComando Ejecutar = new clsEjecutarComando();
            return Ejecutar.Ejecutar(sSql);
        }

        public static DataTable verBitacoraMenu()
        {
            string sSql = "SELECT * FROM Bitacora WHERE origen = 'frmMenu'";
            clsEjecutarComando Ejecutar = new clsEjecutarComando();
            return Ejecutar.Ejecutar(sSql);
        }

        public static DataTable verBitacoraPersonal()
        {
            string sSql = "SELECT * FROM Bitacora WHERE origen = 'frmPersonal'";
            clsEjecutarComando Ejecutar = new clsEjecutarComando();
            return Ejecutar.Ejecutar(sSql);
        }

        public static DataTable verBitacoraProducto()
        {
            string sSql = "SELECT * FROM Bitacora WHERE origen = 'frmProducto'";
            clsEjecutarComando Ejecutar = new clsEjecutarComando();
            return Ejecutar.Ejecutar(sSql);
        }

        public static DataTable verBitacoraProveedores()
        {
            string sSql = "SELECT * FROM Bitacora WHERE origen = 'frmProveedores'";
            clsEjecutarComando Ejecutar = new clsEjecutarComando();
           return Ejecutar.Ejecutar(sSql);
        }

        public static DataTable verBitacoraStock()
        {
            string sSql = "SELECT * FROM Bitacora WHERE origen = 'frmStock'";
            clsEjecutarComando Ejecutar = new clsEjecutarComando();
            return Ejecutar.Ejecutar(sSql);
        }

        public static DataTable verBitacoraLogin()
        {
            string sSql = "SELECT * FROM Bitacora WHERE origen = 'frmLoguin'";
            clsEjecutarComando Ejecutar = new clsEjecutarComando();
            return Ejecutar.Ejecutar(sSql);
        }
    }
}
