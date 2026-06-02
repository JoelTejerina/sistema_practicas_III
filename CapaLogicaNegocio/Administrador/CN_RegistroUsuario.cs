using CapaAccesoDatos.Administrador;
using CapaComun;

namespace CapaLogicaNegocio.Administrador
{
    public class CN_RegistroUsuario
    {
        private readonly CD_RegistroUsuario datos = new CD_RegistroUsuario();

        public string Apellido { get; set; }
        public string Nombres { get; set; }
        public int IdTipoDoc { get; set; }
        public int NroDoc { get; set; }
        public string Telefono { get; set; }
        public string Correo { get; set; }
        public int IdLocalidad { get; set; }
        public int IdCargo { get; set; }
        public string Usuario { get; set; }
        public string Password { get; set; }
        public int IdGrupo { get; set; }

        public bool ExisteUsuario(string nombreUsuario)
        {
            return datos.ExisteUsuario(nombreUsuario);
        }

        public bool ExisteDocumento(int idTipoDoc, int nroDoc)
        {
            return datos.ExisteDocumento(idTipoDoc, nroDoc);
        }

        public int ObtenerIdPersonaPorDocumento(int idTipoDoc, int nroDoc)
        {
            return datos.ObtenerIdPersonaPorDocumento(idTipoDoc, nroDoc);
        }

        public bool ExisteUsuarioPorPersona(int idPersona)
        {
            return datos.ExisteUsuarioPorPersona(idPersona);
        }

        public int Registrar(int idPersonaExistente = 0)
        {
            datos.Apellido = Apellido;
            datos.Nombres = Nombres;
            datos.IdTipoDoc = IdTipoDoc;
            datos.NroDoc = NroDoc;
            datos.Telefono = Telefono ?? "";
            datos.Correo = Correo ?? "";
            datos.IdLocalidad = IdLocalidad;
            datos.IdCargo = IdCargo;
            datos.Usuario = Usuario;
            datos.PasswordHash = clsSeguridad.SHA256(Password);
            datos.IdGrupo = IdGrupo;
            return datos.Registrar(idPersonaExistente);
        }
    }
}
