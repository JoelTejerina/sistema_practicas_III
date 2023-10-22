using MySqlX.XDevAPI.Relational;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaAccesoDatos.ModuloProductos
{
    public class BDProducto
    {


        #region ATRIBUTOS
        private string nombreproducto;
        private string categoria;
        private string marca;
        private string tipocantidad;
        private int cantidad;
        private int idproducto;
        #endregion

        #region PROPERTIES

        public string CDNombreProducto
        {
            get => nombreproducto;
            set { nombreproducto = value; }
        }

        public string CDCategoria
        {
            get => categoria;
            set { categoria = value; }
        }

        public string CDMarca
        {
            get => marca;
            set { marca = value; }
        }

        public string CDTipoCantidad
        {
            get => tipocantidad;
            set { tipocantidad = value; }
        }

        public int CDCantidad
        {
            get => cantidad;
            set { cantidad = value; }
        }

        public int CDIdProducto
        {
            get => idproducto;
            set { idproducto = value; }
        }
        #endregion

        #region METODOS

        public DataTable Mostrar()
        {
            string sSql;
            sSql = "Select * from Productos ";
            clsEjecutarComando Ejecutar = new clsEjecutarComando();
            return Ejecutar.Ejecutar(sSql);

        }

        public void InsertarProducto()
        {
            string sSql = "INSERT INTO Productos " +
               "(nombreProducto, categoria, marca, tipoMedia, cantidad) " +
                "values" +
                " ('" + nombreproducto + "','" + categoria + "'," + marca + "," + tipocantidad +
                ",'" + cantidad + "')";
            clsEjecutarComando Ejecutar = new clsEjecutarComando();
            Ejecutar.Ejecutar(sSql);
        }

        public void ModificarProducto()
        {
            string sSql = "UPDATE Productos set " +
                "nombreProducto='" + nombreproducto + "', categoria='" + categoria + "', marca =" + marca +
                ", tipoMedia = " + tipocantidad + ", cantidad = '" + cantidad +
                " WHERE IdProducto =" + idproducto;
            clsEjecutarComando Ejecutar = new clsEjecutarComando();
            Ejecutar.Ejecutar(sSql);
        }

        public void EliminarProducto()
        {
            string sSql = "DELETE FROM Productos WHERE IdProducto =" + idproducto;
            clsEjecutarComando Ejecutar = new clsEjecutarComando();
            Ejecutar.Ejecutar(sSql);
        }

        #endregion
    }
}
