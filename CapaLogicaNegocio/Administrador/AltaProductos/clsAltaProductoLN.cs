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
        //El codigo que se vera en esta clase es el que sera utilizado en el programa original. En el video de
        // muestra no se dicha clase debido a que requiere de haber terminado mi Base de Datos.
        
        private ImitacionBDAltaProducto InstanciaImitacionBDAltaProducto = new ImitacionBDAltaProducto();

        #region ATRIBUTOS
        private string nombreproducto;
        private string categoria;
        private string marca;
        private string tipocantidada;
        private string tipocantidadb;
        private int cantidada;
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
        #endregion


        #region METODOS


        public void InsertarPersona()
        {
            PasarDatos();
            // InstanciaImitacionBDAltaProducto.InsertarProducto();
            // Tuve que poner la linea de arriba en modo de comentario debido a que utiliza un metodo que
            // requiere de tener su tabla terminada en la base de datos. Si no lo comentaba, no podria
            // depurar el programa para testearlo. Disculpen las molestias.
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
