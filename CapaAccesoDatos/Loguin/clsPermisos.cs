using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.OleDb;
using CapaComun;
using CapaAccesoDatos;

namespace CapaDatos
{
    public class clsPermisos : CD_Conexion
    {
        public bool Permisos(int IdUser)
        {
            string sSql = "SELECT DISTINCT P.idPermiso, P.funcionalidad " +
              "FROM Permisos P " +
              "INNER JOIN RolPermisos RP ON P.idPermiso = RP.idPermiso " +
              "INNER JOIN UsuarioRoles UR ON RP.idRol = UR.idRol " +
              "WHERE UR.idUsuario = " + IdUser;


            DataTable DT = new DataTable();
            clsEjecutarComando Ejecutar = new clsEjecutarComando();
            DT = Ejecutar.Ejecutar(sSql);

            if (DT.Rows.Count > 0)
            {
                foreach (DataRow row in DT.Rows)
                {
                    UserCache.PermisosUsuario.Add(Convert.ToInt32(row[0].ToString()), row[1].ToString());
                }
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}