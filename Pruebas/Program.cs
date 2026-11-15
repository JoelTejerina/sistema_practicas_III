using System;
using System.Collections.Generic;
using System.Linq;

namespace Pruebas
{
    /// <summary>Un caso de prueba: identificador, descripcion y la accion que lanza una excepcion si falla.</summary>
    public class Caso
    {
        public string Id;
        public string Nombre;
        public Action Accion;
    }

    /// <summary>Afirmaciones minimas (sin dependencias externas).</summary>
    public static class Afirmar
    {
        public static void Verdadero(bool condicion, string mensaje)
        {
            if (!condicion) throw new Exception(mensaje);
        }

        public static void Igual(object esperado, object actual, string mensaje)
        {
            if (!Equals(esperado, actual))
                throw new Exception(mensaje + " (esperado: " + esperado + ", obtenido: " + actual + ")");
        }

        public static void Lanza<T>(Action accion, string mensaje) where T : Exception
        {
            try { accion(); }
            catch (T) { return; }
            catch (Exception ex) { throw new Exception(mensaje + " (se esperaba " + typeof(T).Name + " y fue " + ex.GetType().Name + ")"); }
            throw new Exception(mensaje + " (no lanzo " + typeof(T).Name + ")");
        }
    }

    /// <summary>
    /// Ejecutor de pruebas unitarias. Uso:  Pruebas.exe [--sin-bd]
    /// Las pruebas de logica pura no necesitan base de datos. Las de negocio usan una base DESCARTABLE
    /// indicada en SISTEMA_DB_CONNECTION (el nombre debe contener "Prueba" o "Test"; Ejecutar-Pruebas.ps1 la crea).
    /// Codigo de salida: 0 = todo OK, 1 = alguna prueba fallo, 2 = no se pudo ejecutar.
    /// </summary>
    public static class Program
    {
        public static int Main(string[] args)
        {
            bool sinBd = args.Contains("--sin-bd");
            List<Caso> casos = new List<Caso>();
            casos.AddRange(PruebasComunes.Casos());
            if (!sinBd)
            {
                string cn = Environment.GetEnvironmentVariable("SISTEMA_DB_CONNECTION") ?? "";
                if (cn.IndexOf("Prueba", StringComparison.OrdinalIgnoreCase) < 0 && cn.IndexOf("Test", StringComparison.OrdinalIgnoreCase) < 0)
                {
                    Console.WriteLine("Las pruebas de negocio solo corren contra una base descartable (nombre con 'Prueba' o 'Test').");
                    Console.WriteLine("Defini SISTEMA_DB_CONNECTION o usa Pruebas\\Ejecutar-Pruebas.ps1 (o --sin-bd).");
                    return 2;
                }
                casos.AddRange(PruebasNegocio.Casos());
            }

            int ok = 0, fallas = 0;
            foreach (Caso c in casos)
            {
                try
                {
                    c.Accion();
                    ok++;
                    Console.WriteLine("OK     " + c.Id + "  " + c.Nombre);
                }
                catch (Exception ex)
                {
                    fallas++;
                    Console.WriteLine("FALLA  " + c.Id + "  " + c.Nombre + "  ->  " + ex.Message);
                }
            }
            Console.WriteLine();
            Console.WriteLine("Total: " + casos.Count + "   OK: " + ok + "   Fallas: " + fallas);
            return fallas == 0 ? 0 : 1;
        }
    }
}
