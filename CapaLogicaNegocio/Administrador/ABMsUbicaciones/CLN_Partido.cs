using CapaAccesoDatos.Administrador;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaLogicaNegocio.Administrador.ABMsUbicaciones
{
    public class CLN_Partido
    {
        private CD_Partidos partido = new CD_Partidos();

        #region ATRIBUTOS
        private string provincia;
        private int idprovincia;
        #endregion

        #region PROPERTIES

        public string Partido
        {
            get => provincia;
            set { provincia = value; }
        }
        public string Provincia
        {
            get => provincia;
            set { provincia = value; }
        }

        public int IdPartido
        {
            get => idprovincia;
            set { idprovincia = value; }
        }
        #endregion

        #region METODOS

        public DataTable MostrarPartido()
        {
            DataTable tabla = new DataTable();
            PasarDatos();
            tabla = partido.Mostrar();
            return tabla;
        }

        public void InsertarPartido()
        {
            PasarDatos();
            //partido.InsertarPartido();
        }

        public void ModificarPartido()
        {
            PasarDatos();
            //partido.ModificarPartido();
        }

        public void EliminarPartido()
        {
            //partido.IdPartido = idpartido;
            //partido.EliminarPartido();
        }

        public void PasarDatos()
        {
            //partido.Provincia = this.partido;
            //partido.IdPartido = Convert.ToInt32(this.idpartido);
        }
        #endregion
    }
}
