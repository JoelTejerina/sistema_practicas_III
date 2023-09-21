using CapaAccesoDatos.Administrador;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CapaAccesoDatos;
using CapaAccesoDatos.Administrador;

namespace CapaLogicaNegocio.Administrador.ABMsUbicaciones
{
    public class CLN_Provincia
    {
        private CD_Provincias provincias = new CD_Provincias();

        #region ATRIBUTOS
        private string provincia;
        private int idprovincia;
        #endregion

        #region PROPERTIES

        public string Provincia
        {
            get => provincia;
            set { provincia = value; }
        }

        public int IdProvincia
        {
            get => idprovincia;
            set { idprovincia = value; }
        }
        #endregion

        #region METODOS

        public DataTable MostrarProvincia()
        {
            DataTable tabla = new DataTable();
            PasarDatos();
            tabla = provincias.Mostrar();
            return tabla;
        }

        public void InsertarProvincia()
        {
            PasarDatos();
            provincias.InsertarProvincia();
        }

        public void ModificarProvincia()
        {
            PasarDatos();
            provincias.ModificarProvincia();
        }

        public void EliminarProvincia()
        {
            provincias.IdProvincia = idprovincia;
            provincias.EliminarProvincia();
        }

        public void PasarDatos()
        {
            provincias.Provincia = this.provincia;
            provincias.IdProvincia = Convert.ToInt32(this.idprovincia);
        }

        #endregion
    }
}
