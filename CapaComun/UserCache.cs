using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace CapaComun
{
    public static class UserCache
    {
        public static int IdUsuario { get; set; }
        public static string Usuario { get; set; }
        public static string Password { get; set; }
        public static int IdPersona { get; set; }
        public static DateTime FechaAlta { get; set; }
        public static DateTime FechaBaja { get; set; }
        public static int CambiaCada { get; set; }
        public static DateTime FechaUltimoCambio { get; set; }
        public static bool UsuarioDesactivado { get; set; }
        public static DateTime FechaDesactivacion { get; set; }
        public static string Apellido { get; set; }
        public static string Nombres { get; set; }
        public static string Cargo { get; set; }
        public static int IdGrupo { get; set; }
        public static string NombreGrupo { get; set; }
        public static bool DebeCambiarPassword { get; set; }

        /// <summary>Deja la sesion en blanco (cierre de sesion): el siguiente usuario empieza de cero.</summary>
        public static void Limpiar()
        {
            IdUsuario = 0; Usuario = null; Password = null; IdPersona = 0;
            FechaAlta = default(DateTime); FechaBaja = default(DateTime); CambiaCada = 0;
            FechaUltimoCambio = default(DateTime); UsuarioDesactivado = false; FechaDesactivacion = default(DateTime);
            Apellido = null; Nombres = null; Cargo = null; IdGrupo = 0; NombreGrupo = null; DebeCambiarPassword = false;
            PermisosUsuario.Clear();
        }

        public static Hashtable PermisosUsuario = new Hashtable();

    }
}
