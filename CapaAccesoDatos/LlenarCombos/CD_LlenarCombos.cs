﻿using System.Data;
using System.Data.OleDb;

/* Esta clase me permite cargar cualquier comboBox desde una tabla
   recibiendo  el Nombre de la tabala, el campo Id de la tabla relacionado al dato que mostrara el ComboBox 
   y el campo que mostrara el ComboBox.
   como opcional podra recibir una condicion .
*/
namespace CapaAccesoDatos
{
    public class CD_LlenarCombos : CD_Conexion
    {

        #region ATRIBUTOS
        private string tabla;
        private string campoid;
        private string campodescrip;
        private string campoAdicional1;
        private string campoAdicional2;
        private string condicion;
        #endregion

        #region PROPERTIES
        public string Tabla
        {
            set { tabla = value; }
        }
        public string CampoId
        {
            set { campoid = value; }
        }
        public string CampoDescrip
        {
            set { campodescrip = value; }
        }
        public string CampoAdicional1
        {
            set { campoAdicional1 = value; }
        }

        public string CampoAdicional2
        {
            set { campoAdicional2 = value; }
        }

        public string Condicion
        {
            set { condicion = value; }
        }
        #endregion


        public DataTable CargarCMB()
        {
            string sSql;
            if (condicion == "")
            {
                if (campoAdicional1 != null  && campoAdicional2 != null)
                {
                    sSql = "SELECT " + campoid + ", " + campodescrip + ", " + campoAdicional1 + ", " + campoAdicional2 + " FROM " + tabla + " ORDER BY " + campodescrip;
                } else
                {
                    sSql = "SELECT " + campoid + ", " + campodescrip + " FROM " + tabla + " ORDER BY " + campodescrip;
                }
            }
            else
            {
                sSql = "SELECT " + campoid + ", " + campodescrip + " FROM " + tabla + " Where  " + condicion + " ORDER BY " + campodescrip;
            }

            clsEjecutarComando Ejecutar = new clsEjecutarComando();
            return Ejecutar.Ejecutar(sSql);
        }

        public DataTable CargarCMBPersonal()
        {         
            string  sSql = "SELECT IdPersona, Apellido, Nombre  FROM Personal ORDER BY Apellido";
            clsEjecutarComando Ejecutar = new clsEjecutarComando();
            return Ejecutar.Ejecutar(sSql);
        }


    }
}

