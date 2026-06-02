using System;
using System.Collections;

namespace CapaComun
{
    public static class PermisosSistema
    {
        public const string AltaUsuarios = "Alta de Usuarios";
        public const string BajaUsuarios = "Baja de Usuarios";
        public const string ModificacionUsuarios = "Modificacion de Usuarios";
        public const string AdministracionSistema = "Administracion del Sistema";
        public const string CambioPassword = "Cambios de Password";
        public const string Proveedores = "Proveedores";
        public const string Productos = "Productos";
        public const string Stock = "Stock";
        public const string Menu = "Menu";
        public const string Pedidos = "Pedidos";
        public const string Bitacora = "Bitacora";
        public const string Personal = "Personal";
        public const string Ubicaciones = "Ubicaciones";
        public const string Estadisticas = "Estadisticas";

        public const string GrupoGerente = "Gerente";
        public const string GrupoSubgerente = "Subgerente";
        public const string GrupoCajero = "Cajero";
        public const string GrupoJefeCocina = "Jefe de Cocina";
        public const string GrupoPropietario = "Propietario";
        public const string GrupoRecursosHumanos = "Recursos Humanos";

        public static bool EsAdministrador()
        {
            return Tiene(AdministracionSistema);
        }

        public static bool Tiene(string funcionalidad)
        {
            if (string.IsNullOrWhiteSpace(funcionalidad))
            {
                return false;
            }

            foreach (DictionaryEntry item in UserCache.PermisosUsuario)
            {
                string permiso = item.Value as string;
                if (permiso != null &&
                    string.Equals(permiso.Trim(), funcionalidad.Trim(), StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }

        public static bool TieneAlguno(params string[] funcionalidades)
        {
            if (EsAdministrador())
            {
                return true;
            }
            foreach (string f in funcionalidades)
            {
                if (Tiene(f))
                {
                    return true;
                }
            }
            return false;
        }

        public static bool PuedeGestionarUsuarios()
        {
            return EsAdministrador() || TieneAlguno(AltaUsuarios, BajaUsuarios, ModificacionUsuarios)
                || EsGrupo(GrupoGerente, GrupoRecursosHumanos);
        }

        public static bool PuedeAsignarRoles()
        {
            return EsAdministrador() || EsGrupo(GrupoGerente, GrupoRecursosHumanos);
        }

        public static bool PuedeVerEstadisticas()
        {
            return EsAdministrador() || Tiene(Estadisticas)
                || EsGrupo(GrupoGerente, GrupoPropietario);
        }

        /// <summary>
        /// Si devuelve true, el usuario puede abrir el módulo pero no Alta/Baja/Modificar.
        /// </summary>
        public static bool SoloLectura(string funcionalidad)
        {
            if (EsAdministrador() || EsGrupo(GrupoGerente))
            {
                return false;
            }

            string grupo = UserCache.NombreGrupo ?? "";

            switch (funcionalidad)
            {
                case Proveedores:
                case Productos:
                case Stock:
                    return grupo != GrupoSubgerente;
                case Menu:
                    return grupo != GrupoJefeCocina;
                case Pedidos:
                    return grupo != GrupoCajero;
                case Personal:
                    return grupo != GrupoRecursosHumanos;
                case Bitacora:
                case Estadisticas:
                    return grupo == GrupoPropietario;
                default:
                    return grupo == GrupoPropietario;
            }
        }

        private static bool EsGrupo(params string[] grupos)
        {
            if (string.IsNullOrEmpty(UserCache.NombreGrupo))
            {
                return false;
            }
            foreach (string g in grupos)
            {
                if (string.Equals(UserCache.NombreGrupo, g, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
