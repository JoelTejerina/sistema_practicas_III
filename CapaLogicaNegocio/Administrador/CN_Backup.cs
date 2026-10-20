using CapaAccesoDatos.Administrador;

namespace CapaLogicaNegocio.Administrador
{
    /// <summary>Copia de respaldo de la base de datos.</summary>
    public class CN_Backup
    {
        private readonly CD_Backup backup = new CD_Backup();

        public string RutaPredeterminada()
        {
            return backup.RutaPredeterminada();
        }

        public string NombreBase()
        {
            return backup.NombreBase();
        }

        public void Respaldar(string rutaArchivo)
        {
            backup.Respaldar(rutaArchivo);
        }
    }
}
