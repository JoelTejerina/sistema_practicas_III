using CapaAccesoDatos.Loguin;
using CapaComun;

namespace CapaLogicaNegocio.Loguin
{
    /// <summary>Guarda y recupera el idioma elegido por cada usuario.</summary>
    public class CN_IdiomaUsuario
    {
        private readonly CD_IdiomaUsuario datos = new CD_IdiomaUsuario();

        /// <summary>Aplica el idioma guardado del usuario (si tiene uno).</summary>
        public void AplicarIdiomaDelUsuario(int idUsuario)
        {
            string idioma = datos.Obtener(idUsuario);
            if (idioma == Idioma.Espanol || idioma == Idioma.Ingles || idioma == Idioma.Portugues)
            {
                Idioma.Establecer(idioma);
            }
        }

        public void GuardarIdioma(int idUsuario, string idioma)
        {
            datos.Guardar(idUsuario, idioma);
        }
    }
}
