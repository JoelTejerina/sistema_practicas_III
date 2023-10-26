using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using CapaAccesoDatos;
using CapaAccesoDatos.Administrador;
using CapaAccesoDatos.ModuloProductos;
using System.Text.RegularExpressions;

namespace CapaLogicaNegocio.ModuloProducto
{
    public class clsProductosLN
    {
        private BDProducto InstanciaBDProducto = new BDProducto();

        #region ATRIBUTOS
        private string nombreproducto;
        private string categoria;
        private string marca;
        private string tipocantidad;
        private int cantidad;
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

        public string CLNTipoCantidad
        {
            get => tipocantidad;
            set { tipocantidad = value; }
        }

        public int CLNCantidad
        {
            get => cantidad;
            set { cantidad = value; }
        }

        public int CLNIdProducto
        {
            get => idproducto;
            set { idproducto = value; }
        }
        #endregion


        #region METODOS



        public DataTable MostrarProducto()
        {
            DataTable tabla = new DataTable();
            tabla = InstanciaBDProducto.Mostrar();
            return tabla;
        }
        public void InsertarProducto()
        {
            PasarDatos();
            InstanciaBDProducto.InsertarProducto();
        }

        public void ModificarProducto()
        {
            PasarDatos();
            InstanciaBDProducto.ModificarProducto();
        }

        public void EliminarProducto()
        {
            InstanciaBDProducto.CDIdProducto = idproducto;
            InstanciaBDProducto.EliminarProducto();
        }

        private void PasarDatos()
        {
            InstanciaBDProducto.CDNombreProducto = this.nombreproducto;
            InstanciaBDProducto.CDCategoria = this.categoria;
            InstanciaBDProducto.CDMarca = this.marca;
            InstanciaBDProducto.CDTipoCantidad = this.tipocantidad;
            InstanciaBDProducto.CDCantidad = Convert.ToInt32(this.cantidad);
            InstanciaBDProducto.CDIdProducto = Convert.ToInt32(this.idproducto);
        }

        #endregion

    }
}
