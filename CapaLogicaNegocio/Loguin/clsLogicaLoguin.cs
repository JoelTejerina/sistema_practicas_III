using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CapaAccesoDatos;
using CapaComun;
using CapaDatos;

namespace CapaLogicaNegocio
{
    

    public class clsLogicaLoguin
    {
        private bool existe;
        private string mensaje = "";
 
        clsConectarUsuario userLoguin = new clsConectarUsuario();
        Loguin.CL_clsLogicaBloqueoUser checkUser = new Loguin.CL_clsLogicaBloqueoUser();
        clsPermisos Permisos = new clsPermisos();

        public Tuple<bool, string, int> LoginUser(string user, string pass)
        {
            if (pass != null)
            {   // si password carga como null, busca coincidancia de usuario y contraseña
                existe = userLoguin.Login(user, clsSeguridad.SHA256(pass));
                //existe = userLoguin.Login(user, pass);
                if (existe)
                {
                    UserCache.PermisosUsuario.Clear();
                    Permisos.Permisos(UserCache.IdUsuario);
                }
            }
            else
            {   // si password == null
                existe = userLoguin.UserVerific(user);
                if (existe)
                {// si es true

                    mensaje = checkUser.BlokUser(user);//Bloqueo de usuario
                    ////***
                    ///if(user....
                }
            }


            var result = Tuple.Create<bool, string, int>(existe, mensaje, 250);
            return result;
            //return existe;
        }


        /*public bool LoginUser2(string user, string pass)
        {
            
           
            
            
        }*/
    }
}
