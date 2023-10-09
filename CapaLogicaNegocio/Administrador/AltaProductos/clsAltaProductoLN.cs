using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using CapaAccesoDatos;
using CapaAccesoDatos.Administrador;
using CapaAccesoDatos.Imitacion_de_una_BD;
using System.Text.RegularExpressions;

namespace CapaLogicaNegocio.Administrador.AltaProductos
{
    public class clsAltaProductoLN
    {
        //El codigo que se vera en esta clase es el que sera utilizado en el programa original.
        
        private BDAltaProducto InstanciaImitacionBDAltaProducto = new BDAltaProducto();

        #region ATRIBUTOS
        private string nombreproducto;
        private string categoria;
        private string marca;
        private string tipocantidada;
        private string tipocantidadb;
        private int cantidada;
        private int idproducto;
        private int cantidadb;
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


        public void InsertarPersona()
        {
            PasarDatos();
            personas.InsertarPersona();
        }

        private void PasarDatos()
        {
            InstanciaImitacionBDAltaProducto.NombreProducto = this.nombreproducto;
            InstanciaImitacionBDAltaProducto.Categoria = this.categoria;
            InstanciaImitacionBDAltaProducto.Marca = this.marca;
            InstanciaImitacionBDAltaProducto.TipoCantidadA = this.tipocantidada;
            InstanciaImitacionBDAltaProducto.TipoCantidadB = this.tipocantidadb;
            InstanciaImitacionBDAltaProducto.CantidadA = Convert.ToInt32(this.cantidada);
            InstanciaImitacionBDAltaProducto.CantidadB = Convert.ToInt32(this.cantidadb);
        }

        #endregion
        
    }
}
