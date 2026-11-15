using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using CapaAccesoDatos.Administrador;
using CapaComun;

namespace CapaLogicaNegocio.Administrador
{
    /// <summary>Gestion de roles y permisos (CU-USU-004).</summary>
    public class CN_RolesPermisos
    {
        private readonly CD_RolesPermisos datos = new CD_RolesPermisos();

        // Roles que trae el sistema: no se pueden eliminar.
        private static readonly string[] RolesBase =
        {
            PermisosSistema.GrupoGerente, PermisosSistema.GrupoPropietario, PermisosSistema.GrupoCajero,
            PermisosSistema.GrupoJefeCocina, PermisosSistema.GrupoJefeSalon
        };

        public DataTable Roles()
        {
            return datos.Roles();
        }

        public DataTable Permisos()
        {
            return datos.Permisos();
        }

        public List<int> PermisosDelRol(int idGrupo)
        {
            return datos.PermisosDelRol(idGrupo);
        }

        public static bool EsRolBase(string rol)
        {
            return RolesBase.Contains(rol);
        }

        /// <summary>El Gerente siempre tiene todos los permisos: sus permisos no se editan.</summary>
        public void GuardarPermisos(int idGrupo, string rol, IEnumerable<int> permisos)
        {
            if (rol == PermisosSistema.GrupoGerente)
            {
                throw new InvalidOperationException(Idioma.Texto("Msg_PermisosGerente"));
            }
            datos.GuardarPermisos(idGrupo, permisos);
        }

        public int CrearRol(string nombre)
        {
            if (string.IsNullOrWhiteSpace(nombre))
            {
                throw new InvalidOperationException(Idioma.Texto("Msg_CampoObligatorio"));
            }
            if (nombre.Trim().Length > 50)
            {
                throw new InvalidOperationException(Idioma.Texto("Msg_RolLargo"));
            }
            if (datos.ExisteRol(nombre))
            {
                throw new InvalidOperationException(Idioma.Texto("Msg_RolExiste"));
            }
            return datos.CrearRol(nombre);
        }

        public void EliminarRol(int idGrupo, string rol)
        {
            if (EsRolBase(rol))
            {
                throw new InvalidOperationException(Idioma.Texto("Msg_RolBase"));
            }
            if (datos.UsuariosDelRol(idGrupo) > 0)
            {
                throw new InvalidOperationException(Idioma.Texto("Msg_RolConUsuarios"));
            }
            datos.EliminarRol(idGrupo);
        }
    }
}
