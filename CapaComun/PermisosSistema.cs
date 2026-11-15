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
        public const string Elaboracion = "Elaboracion";
        public const string Caja = "Caja";

        public const string GrupoGerente = "Gerente";
        public const string GrupoCajero = "Cajero";
        public const string GrupoJefeCocina = "Jefe de Cocina";
        public const string GrupoPropietario = "Propietario";
        public const string GrupoJefeSalon = "Jefe de Salon";

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

        // La administracion de usuarios, roles y personal la hacen el Gerente y el Propietario
        // (el "Administrador" de los casos de uso del documento de alcance).
        public static bool PuedeGestionarUsuarios()
        {
            return EsAdministrador() || TieneAlguno(AltaUsuarios, BajaUsuarios, ModificacionUsuarios)
                || EsGrupo(GrupoGerente, GrupoPropietario);
        }

        public static bool PuedeAsignarRoles()
        {
            return EsAdministrador() || EsGrupo(GrupoGerente, GrupoPropietario);
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
                    // Compras es del Gerente (ya retornado arriba): el resto, solo lectura.
                    return true;
                case Menu:
                    return grupo != GrupoJefeCocina;
                case Pedidos:
                    return grupo != GrupoCajero && grupo != GrupoJefeSalon;
                case Elaboracion:
                    return grupo != GrupoJefeCocina;
                case Caja:
                    return grupo != GrupoCajero;
                case Personal:
                    return grupo != GrupoPropietario;
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
