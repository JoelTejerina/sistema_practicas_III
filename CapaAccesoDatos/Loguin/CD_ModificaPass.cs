using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaAccesoDatos.Loguin
{
    public class CD_ModificaPass
    {
        
        clsEjecutarComando Ejecutar = new clsEjecutarComando();
        public void ModificaPass(int IdUser, String Pass )
        {
            string SSql = "UPDATE Usuarios SET Usuarios.Password ='" + (Pass ?? "").Replace("'", "''") + "' WHERE Usuarios.IdUsuario =" + IdUser ;
            Ejecutar.EjecucionDirecta(SSql);
        }
        

    }
}
