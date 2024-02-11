using CapaAccesoDatos.Administrador;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaLogicaNegocio.Administrador
{
    public class CN_Personal
    {
        private CD_Personal cdPersonal = new CD_Personal();

        #region ATRIBUTOS
        private int idPersonal;
        private string apellido;
        private string nombre;
        private string nroDoc;
        private string cuil;
        private string calle;
        private int nro;
        #endregion

        #region PROPERTIES

        public int IdPersonal
        {
            get => idPersonal;
            set { idPersonal = value; }
        }

        public string Apellido
        {
            get => apellido;
            set { apellido = value; }
        }

        public string Nombre
        {
            get => nombre;
            set { nombre = value; }
        }

        public string NroDoc
        {
            get => nroDoc;
            set { nroDoc = value; }
        }

        public string Cuil
        {
            get => cuil;
            set { cuil = value; }
        }

        public string Calle
        {
            get => calle;
            set { calle = value; }
        }

        public int Nro
        {
            get => nro;
            set { nro = value; }
        }
        #endregion

        #region METODOS

        public DataTable MostrarPersonal()
        {
            DataTable tabla = new DataTable();
            tabla = cdPersonal.MostrarPersonal();
            return tabla;
        }

        public void InsertarPersonal()
        {
            PasarDatos();       
            try
            {
                cdPersonal.InsertarPersonal();
                CL_clsBitacora Guardar = new CL_clsBitacora("Creado con exito", "Exitoso", "frmPersonal");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                CL_clsBitacora Guardar = new CL_clsBitacora(ex.Message, "Error", "frmPersonal");
            }
        }

        public void ModificarPersonal()
        {
            PasarDatos();
            try
            {
                cdPersonal.ModificarPersonal();
                CL_clsBitacora Guardar = new CL_clsBitacora("Modificado con exito", "Exitoso", "frmPersonal");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                CL_clsBitacora Guardar = new CL_clsBitacora(ex.Message, "Error", "frmPersonal");
            }
        }

        public void EliminarPersonal()
        {
            cdPersonal.IdPersonal = IdPersonal;
            try
            {
                cdPersonal.EliminarPersonal();
                CL_clsBitacora Guardar = new CL_clsBitacora("Eliminado con exito", "Exitoso", "frmPersonal");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                CL_clsBitacora Guardar = new CL_clsBitacora(ex.Message, "Error", "frmPersonal");
            }
        }

        private void PasarDatos()
        {
            cdPersonal.IdPersonal = IdPersonal;
            cdPersonal.Apellido = Apellido;
            cdPersonal.Nombre = Nombre;
            cdPersonal.NroDoc = NroDoc;
            cdPersonal.Cuil = Cuil;
            cdPersonal.Calle = Calle;
            cdPersonal.Nro = Nro;
        }

        #endregion

    }
}
