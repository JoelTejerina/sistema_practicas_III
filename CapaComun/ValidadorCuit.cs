using System;
using System.Linq;

namespace CapaComun
{
    /// <summary>Validacion del CUIT/CUIL argentino: 11 digitos, prefijo valido y digito verificador (modulo 11).</summary>
    public static class ValidadorCuit
    {
        private static readonly string[] Prefijos = { "20", "23", "24", "25", "26", "27", "30", "33", "34" };
        private static readonly int[] Pesos = { 5, 4, 3, 2, 7, 6, 5, 4, 3, 2 };

        /// <summary>Deja solo los digitos (admite guiones y espacios).</summary>
        public static string SoloDigitos(string cuit)
        {
            return new string((cuit ?? "").Where(char.IsDigit).ToArray());
        }

        public static bool EsValido(string cuit)
        {
            string d = SoloDigitos(cuit);
            if (d.Length != 11 || !Prefijos.Contains(d.Substring(0, 2)))
            {
                return false;
            }

            int suma = 0;
            for (int i = 0; i < 10; i++)
            {
                suma += (d[i] - '0') * Pesos[i];
            }
            int verificador = 11 - (suma % 11);
            if (verificador == 11) verificador = 0;
            return verificador != 10 && verificador == d[10] - '0';
        }
    }
}
