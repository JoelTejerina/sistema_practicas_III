# sistema_practicas_III

Instalar SQL Server: https://www.youtube.com/watch?v=mA1qoWdNCOE&ab_channel=SergioAlejandroCampos-EXCELeINFO

SQL Server Express: https://www.microsoft.com/es-ar/sql-server/sql-server-downloads
SQL Server Management Studio: https://learn.microsoft.com/es-es/sql/ssms/download-sql-server-management-studio-ssms?view=sql-server-ver16

/**********************************************************************************************\

Github: https://github.com/
Crear cuenta: https://github.com/signup?ref_cta=Sign+up&ref_loc=header+logged+out&ref_page=%2F&source=header-home

/**********************************************************************************************\

Controlador de versionado: https://git-scm.com/downloads
Controlar que funcione correctamente: En simbolo del sistema (terminal) git version

/**********************************************************************************************\
Configurar usuario en git: TODO ESTO DESDE UNA CONSOLA (simbolo del sistema | ejemplo; C:\Users\Joel>git config --global user.name "John Doe") 

git config --global user.name "John Doe" (Nombre en github)
git config --global user.email johndoe@example.com (Email que utilizamos para crear cuenta en github)

/**********************************************************************************************\

1) Crearse una nueva rama en github:
https://docs.github.com/es/pull-requests/collaborating-with-pull-requests/proposing-changes-to-your-work-with-pull-requests/creating-and-deleting-branches-within-your-repository

ejemplo nombre de rama: joel/pedidos

/**********************************************************************************************\

2) Clonar/descargar el repositorio: ESTO DESDE UNA CONSOLA (simbolo del sistema | ejemplo: C:\Users\Joel>git clone <URLrepositorio>) 

git clone <URLrepositorio>

/**********************************************************************************************\
3) Subir cambios al repositorio: TODO ESTO DESDE UNA CONSOLA (simbolo del sistema |ejemplo: C:\Users\Joel\nuestroProyectoClonado>git add .) 

git add .
git commit -m "Nombre del commit";
git push origin <nombreDeLaRama>


/**********************************************************************************************\
Obtener los cambios del repositorio: ESTO DESDE UNA CONSOLA (simbolo del sistema | ejemplo: C:\Users\Joel\nuestroProyectoClonado>git pull origin <nombreDeLaRama>)  

git pull origin <nombreDeLaRama>


Conectarse a la base de datos using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace CapaAccesoDatos
{
    public class CD_Conexion
    {
        private SqlConnection Conexion = new SqlConnection("Server=(local);DataBase=KUVO_GSTM;Integrated Security=true");

        public SqlConnection GetConexion()
        {
            if (Conexion.State == ConnectionState.Closed)
                Conexion.Open();
            return Conexion;
        }

        //public SqlConnection CerrarConexion()
        //{
        //    if (Conexion.State == ConnectionState.Open)
        //        Conexion.Close();
        //    return Conexion;
        //}
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.OleDb;
using CapaComun;
using CapaAccesoDatos;

namespace CapaDatos
{
    public class clsPermisos : CD_Conexion
    {
        public bool Permisos(int IdUser)
        {
            string sSql = "SELECT DISTINCT P.idPermiso, P.funcionalidad " +
              "FROM Permisos P " +
              "INNER JOIN RolPermisos RP ON P.idPermiso = RP.idPermiso " +
              "INNER JOIN UsuarioRoles UR ON RP.idRol = UR.idRol " +
              "WHERE UR.idUsuario = " + IdUser;


            DataTable DT = new DataTable();
            clsEjecutarComando Ejecutar = new clsEjecutarComando();
            DT= Ejecutar.Ejecutar(sSql);

            if (DT.Rows.Count > 0)
            {
                foreach (DataRow row in DT.Rows)
                {
                    UserCache.PermisosUsuario.Add(Convert.ToInt32(row[0].ToString()), row[1].ToString());
                }
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}

