using MySqlX.XDevAPI.Relational;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaAccesoDatos.Imitacion_de_una_BD
{
    public class BDAltaProducto
    {


        #region ATRIBUTOS
        private string nombreproducto;
        private string categoria;
        private string marca;
        private string tipocantidada;
        private string tipocantidadb;
        private int cantidada;
        private int cantidadb;
        private int idproducto;
        #endregion

        #region PROPERTIES

        public string CLNNombreProducto
        {
            get => nombreproducto;
            set { nombreproducto = value; }
        }

        public string CLNCategoria
        {
            get => categoria;
            set { categoria = value; }
        }

        public string CLNMarca
        {
            get => marca;
            set { marca = value; }
        }

        public string CLNTipoCantidadA
        {
            get => tipocantidada;
            set { tipocantidada = value; }
        }

        public string CLNTipoCantidadB
        {
            get => tipocantidadb;
            set { tipocantidadb = value; }
        }

        public int CLNCantidadA
        {
            get => cantidada;
            set { cantidada = value; }
        }

        public int CLNCantidadB
        {
            get => cantidadb;
            set { cantidadb = value; }
        }

        public int IdProducto
        {
            get => idproducto;
            set { idproducto = value; }
        }
        #endregion

        #region METODOS

        public DataTable Mostrar()
        {
            string sSql;
            sSql = "Select * from Personal ";
            clsEjecutarComando Ejecutar = new clsEjecutarComando();
            return Ejecutar.Ejecutar(sSql);

        }

        public void InsertarPersona()
        {
            string sSql = "INSERT INTO Personal " +
               "(Apellido, Nombres, IdTDoc, NroDoc, Telefono, Correo, Calle, Nro, Piso, Dto, IdLocalidad, IdCargo) " +
                "values" +
                " ('" + apellido + "','" + nombre + "'," + tipodoc + "," + nrodoc +
                ",'" + telefono + "','" + correo + "','" + calle + "','" + nro +
                "','" + piso + "','" + dto + "'," + idlocalidad + ", 6 )";
            clsEjecutarComando Ejecutar = new clsEjecutarComando();
            Ejecutar.Ejecutar(sSql);
        }

        public void ModificarPersona()
        {
            string sSql = "UPDATE Personal set " +
                "Apellido='" + apellido + "', Nombres='" + nombre + "', IdTDoc =" + tipodoc +
                ", NroDoc = " + nrodoc + ", Telefono = '" + telefono + "', Correo = '" + correo +
                "', Calle = '" + calle + "', Nro = '" + nro + "', Piso = '" + piso + "', Dto = '" + dto +
                "', IdLocalidad = " + idlocalidad +
                " WHERE IdPersona =" + idpersona;
            clsEjecutarComando Ejecutar = new clsEjecutarComando();
            Ejecutar.Ejecutar(sSql);
        }

        public void EliminarPersona()
        {
            string sSql = "DELETE FROM Personal WHERE IdPersona =" + idpersona;
            clsEjecutarComando Ejecutar = new clsEjecutarComando();
            Ejecutar.Ejecutar(sSql);
        }

        #endregion
    }
}
