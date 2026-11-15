using System;
using System.Collections.Generic;
using CapaComun;
using CapaLogicaNegocio.Administrador;

namespace Pruebas
{
    /// <summary>Pruebas unitarias de logica pura: no usan base de datos y son independientes entre si.</summary>
    public static class PruebasComunes
    {
        private static void Rol(string grupo)
        {
            UserCache.Limpiar();
            UserCache.NombreGrupo = grupo;
        }

        public static List<Caso> Casos()
        {
            List<Caso> c = new List<Caso>();
            Action<string, string, Action> add = (id, nombre, accion) => c.Add(new Caso { Id = id, Nombre = nombre, Accion = accion });

            // ---- ValidadorCuit
            add("PU-CMN-001", "CUIT valido (20-12345678-6) se acepta", () =>
                Afirmar.Verdadero(ValidadorCuit.EsValido("20123456786"), "deberia ser valido"));
            add("PU-CMN-002", "CUIT con guiones y espacios se acepta", () =>
                Afirmar.Verdadero(ValidadorCuit.EsValido("20-12345678-6"), "deberia ser valido"));
            add("PU-CMN-003", "CUIT con digito verificador incorrecto se rechaza", () =>
                Afirmar.Verdadero(!ValidadorCuit.EsValido("20123456785"), "deberia ser invalido"));
            add("PU-CMN-004", "CUIT con prefijo inexistente se rechaza", () =>
                Afirmar.Verdadero(!ValidadorCuit.EsValido("10123456786"), "deberia ser invalido"));
            add("PU-CMN-005", "CUIT de largo incorrecto o vacio se rechaza", () =>
            {
                Afirmar.Verdadero(!ValidadorCuit.EsValido("2012345678"), "10 digitos");
                Afirmar.Verdadero(!ValidadorCuit.EsValido(""), "vacio");
                Afirmar.Verdadero(!ValidadorCuit.EsValido(null), "nulo");
            });

            // ---- Cifrado
            add("PU-CMN-006", "SHA-256 es determinista y coincide con el valor conocido de '123'", () =>
            {
                Afirmar.Igual("a665a45920422f9d417e4867efdc4fb8a04a1f3fff1fa07e998e86f7f7a27ae3", clsSeguridad.SHA256("123"), "hash de 123");
                Afirmar.Igual(clsSeguridad.SHA256("clave"), clsSeguridad.SHA256("clave"), "mismo texto, mismo hash");
            });
            add("PU-CMN-007", "SHA-256 de textos distintos es distinto y no deja la clave en claro", () =>
            {
                Afirmar.Verdadero(clsSeguridad.SHA256("123") != clsSeguridad.SHA256("124"), "textos distintos");
                Afirmar.Verdadero(clsSeguridad.SHA256("123") != "123", "no debe guardar en claro");
            });

            // ---- Contrasenas temporales
            add("PU-CMN-008", "La contraseña temporal tiene el largo pedido y no usa caracteres ambiguos", () =>
            {
                string clave = GeneradorClave.Nueva(10);
                Afirmar.Igual(10, clave.Length, "largo");
                foreach (char ch in "0O1lI")
                    Afirmar.Verdadero(clave.IndexOf(ch) < 0, "caracter ambiguo " + ch);
            });
            add("PU-CMN-009", "Dos contraseñas temporales consecutivas son distintas", () =>
                Afirmar.Verdadero(GeneradorClave.Nueva(10) != GeneradorClave.Nueva(10), "deberian diferir"));

            // ---- Idioma
            add("PU-CMN-010", "Idioma: un texto se traduce al ingles y al portugues", () =>
            {
                try
                {
                    Idioma.Establecer(Idioma.Espanol);
                    string es = Idioma.Texto("Btn_CancelarPedido");
                    Idioma.Establecer(Idioma.Ingles);
                    string en = Idioma.Texto("Btn_CancelarPedido");
                    Idioma.Establecer(Idioma.Portugues);
                    string pt = Idioma.Texto("Btn_CancelarPedido");
                    Afirmar.Verdadero(es != en && en != pt && es != "Btn_CancelarPedido", "traducciones " + es + " / " + en + " / " + pt);
                }
                finally { Idioma.Establecer(Idioma.Espanol); }
            });
            add("PU-CMN-011", "Idioma: una clave inexistente devuelve la clave (no falla)", () =>
                Afirmar.Igual("Clave_Que_No_Existe", Idioma.Texto("Clave_Que_No_Existe"), "clave inexistente"));
            add("PU-CMN-012", "Idioma: un idioma no soportado vuelve a español", () =>
            {
                try
                {
                    Idioma.Establecer("xx");
                    Afirmar.Igual(Idioma.Espanol, Idioma.Actual, "idioma actual");
                }
                finally { Idioma.Establecer(Idioma.Espanol); }
            });

            // ---- Permisos por rol (solo lectura)
            add("PU-CMN-013", "Cajero: edita Pedidos y Caja; solo lee Carta y Compras", () =>
            {
                Rol(PermisosSistema.GrupoCajero);
                Afirmar.Verdadero(!PermisosSistema.SoloLectura(PermisosSistema.Pedidos), "Pedidos");
                Afirmar.Verdadero(!PermisosSistema.SoloLectura(PermisosSistema.Caja), "Caja");
                Afirmar.Verdadero(PermisosSistema.SoloLectura(PermisosSistema.Menu), "Menu");
                Afirmar.Verdadero(PermisosSistema.SoloLectura(PermisosSistema.Proveedores), "Proveedores");
                Afirmar.Verdadero(PermisosSistema.SoloLectura(PermisosSistema.Elaboracion), "Elaboracion");
            });
            add("PU-CMN-014", "Jefe de Cocina: edita Carta y Elaboración; solo lee Pedidos y Caja", () =>
            {
                Rol(PermisosSistema.GrupoJefeCocina);
                Afirmar.Verdadero(!PermisosSistema.SoloLectura(PermisosSistema.Menu), "Menu");
                Afirmar.Verdadero(!PermisosSistema.SoloLectura(PermisosSistema.Elaboracion), "Elaboracion");
                Afirmar.Verdadero(PermisosSistema.SoloLectura(PermisosSistema.Pedidos), "Pedidos");
                Afirmar.Verdadero(PermisosSistema.SoloLectura(PermisosSistema.Caja), "Caja");
            });
            add("PU-CMN-015", "Jefe de Salón: edita Pedidos; solo lee Carta, Elaboración y Caja", () =>
            {
                Rol(PermisosSistema.GrupoJefeSalon);
                Afirmar.Verdadero(!PermisosSistema.SoloLectura(PermisosSistema.Pedidos), "Pedidos");
                Afirmar.Verdadero(PermisosSistema.SoloLectura(PermisosSistema.Menu), "Menu");
                Afirmar.Verdadero(PermisosSistema.SoloLectura(PermisosSistema.Elaboracion), "Elaboracion");
                Afirmar.Verdadero(PermisosSistema.SoloLectura(PermisosSistema.Caja), "Caja");
            });
            add("PU-CMN-016", "Propietario: edita Personal; solo consulta Bitácora y Estadísticas", () =>
            {
                Rol(PermisosSistema.GrupoPropietario);
                Afirmar.Verdadero(!PermisosSistema.SoloLectura(PermisosSistema.Personal), "Personal");
                Afirmar.Verdadero(PermisosSistema.SoloLectura(PermisosSistema.Bitacora), "Bitacora");
                Afirmar.Verdadero(PermisosSistema.SoloLectura(PermisosSistema.Estadisticas), "Estadisticas");
                Afirmar.Verdadero(PermisosSistema.SoloLectura(PermisosSistema.Pedidos), "Pedidos");
            });
            add("PU-CMN-017", "Gerente: nunca es de solo lectura", () =>
            {
                Rol(PermisosSistema.GrupoGerente);
                foreach (string f in new[] { PermisosSistema.Proveedores, PermisosSistema.Productos, PermisosSistema.Stock,
                    PermisosSistema.Menu, PermisosSistema.Pedidos, PermisosSistema.Elaboracion, PermisosSistema.Caja, PermisosSistema.Personal })
                    Afirmar.Verdadero(!PermisosSistema.SoloLectura(f), "solo lectura en " + f);
            });
            add("PU-CMN-018", "Solo el Gerente y el Propietario gestionan usuarios y asignan roles", () =>
            {
                Rol(PermisosSistema.GrupoGerente);
                Afirmar.Verdadero(PermisosSistema.PuedeGestionarUsuarios() && PermisosSistema.PuedeAsignarRoles(), "Gerente");
                Rol(PermisosSistema.GrupoPropietario);
                Afirmar.Verdadero(PermisosSistema.PuedeGestionarUsuarios() && PermisosSistema.PuedeAsignarRoles(), "Propietario");
                Rol(PermisosSistema.GrupoCajero);
                Afirmar.Verdadero(!PermisosSistema.PuedeGestionarUsuarios() && !PermisosSistema.PuedeAsignarRoles(), "Cajero");
                Rol(PermisosSistema.GrupoJefeCocina);
                Afirmar.Verdadero(!PermisosSistema.PuedeGestionarUsuarios(), "Jefe de Cocina");
            });
            add("PU-CMN-019", "Un permiso que no esta asignado al usuario no se reconoce", () =>
            {
                UserCache.Limpiar();
                Afirmar.Verdadero(!PermisosSistema.Tiene(PermisosSistema.Bitacora), "sin permisos");
                Afirmar.Verdadero(!PermisosSistema.Tiene(""), "vacio");
            });
            add("PU-CMN-020", "StockInsuficienteException conserva el detalle del faltante", () =>
            {
                var ex = new StockInsuficienteException("Falta 2 kg de harina");
                Afirmar.Igual("Falta 2 kg de harina", ex.Message, "mensaje");
            });
            return c;
        }
    }
}
