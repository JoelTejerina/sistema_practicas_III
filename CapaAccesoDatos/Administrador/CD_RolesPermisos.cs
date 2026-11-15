using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace CapaAccesoDatos.Administrador
{
    /// <summary>Roles (grupos) y los permisos de cada uno.</summary>
    public class CD_RolesPermisos
    {
        private static string T(string valor)
        {
            return (valor ?? "").Replace("'", "''");
        }

        public DataTable Roles()
        {
            return new clsEjecutarComando().Ejecutar(
                "SELECT g.IdGrupo, g.Grupo, (SELECT COUNT(*) FROM UsuariosGrupos ug WHERE ug.IdGrupo = g.IdGrupo) AS Usuarios " +
                "FROM Grupos g ORDER BY g.IdGrupo");
        }

        /// <summary>Todos los permisos que se pueden asignar (se oculta el permiso heredado "Sin cargo").</summary>
        public DataTable Permisos()
        {
            return new clsEjecutarComando().Ejecutar(
                "SELECT IdPermiso, Funcionalidad FROM Permisos WHERE Funcionalidad <> N'Sin cargo' ORDER BY IdPermiso");
        }

        public List<int> PermisosDelRol(int idGrupo)
        {
            DataTable dt = new clsEjecutarComando().Ejecutar("SELECT IdPermiso FROM PermisosGrupos WHERE IdGrupo = " + idGrupo);
            return dt.Rows.Cast<DataRow>().Select(r => Convert.ToInt32(r[0])).ToList();
        }

        /// <summary>Reemplaza los permisos del rol por la lista indicada (en una sola transaccion).</summary>
        public void GuardarPermisos(int idGrupo, IEnumerable<int> permisos)
        {
            string sSql = "SET XACT_ABORT ON; BEGIN TRAN; DELETE FROM PermisosGrupos WHERE IdGrupo = " + idGrupo + "; ";
            foreach (int p in permisos.Distinct())
            {
                sSql += "INSERT INTO PermisosGrupos (IdPermiso, IdGrupo) VALUES (" + p + ", " + idGrupo + "); ";
            }
            sSql += "COMMIT;";
            new clsEjecutarComando().EjecucionDirecta(sSql);
        }

        public bool ExisteRol(string nombre)
        {
            return new clsEjecutarComando().Ejecutar("SELECT 1 FROM Grupos WHERE Grupo = N'" + T(nombre.Trim()) + "'").Rows.Count > 0;
        }

        public int CrearRol(string nombre)
        {
            object id = new clsEjecutarComando().EjecutarEscalar(
                "INSERT INTO Grupos (Grupo) VALUES (N'" + T(nombre.Trim()) + "'); SELECT CAST(SCOPE_IDENTITY() AS INT)");
            return Convert.ToInt32(id);
        }

        public void EliminarRol(int idGrupo)
        {
            new clsEjecutarComando().EjecucionDirecta(
                "SET XACT_ABORT ON; BEGIN TRAN; DELETE FROM PermisosGrupos WHERE IdGrupo = " + idGrupo +
                "; DELETE FROM Grupos WHERE IdGrupo = " + idGrupo + "; COMMIT;");
        }

        public int UsuariosDelRol(int idGrupo)
        {
            return Convert.ToInt32(new clsEjecutarComando()
                .Ejecutar("SELECT COUNT(*) FROM UsuariosGrupos WHERE IdGrupo = " + idGrupo).Rows[0][0]);
        }
    }
}
