using CapaAccesoDatos.Administrador;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaLogicaNegocio.Administrador
{
    public class CN_Producto
    {
        private CD_Producto cdProducto = new CD_Producto();

        #region ATRIBUTOS
        private int idProducto;
        private string nombre;
        private string categoria;
        private string marca;
        #endregion

        #region PROPERTIES

        public int IdProducto
        {
            get => idProducto;
            set { idProducto = value; }
        }

        public string Nombre
        {
            get => nombre;
            set { nombre = value; }
        }

        public string Categoria
        {
            get => categoria;
            set { categoria = value; }
        }

        public string Marca
        {
            get => marca;
            set { marca = value; }
        }

        #endregion

        #region METODOS

        public DataTable MostrarProductos()
        {
            DataTable tabla = new DataTable();
            tabla = cdProducto.MostrarProductos();
            return tabla;
        }

        public void InsertarProducto()
        {
            PasarDatos();
            try
            {
                cdProducto.InsertarProducto();
                CL_clsBitacora Guardar = new CL_clsBitacora("Creado con exito", "Exitoso", "frmProducto");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                CL_clsBitacora Guardar = new CL_clsBitacora(ex.Message, "Error", "frmProducto");
            }
        }

        public void ModificarProducto()
        {
            PasarDatos();
            try
            {
                cdProducto.ModificarProducto();
                CL_clsBitacora Guardar = new CL_clsBitacora("Modificado con exito", "Exitoso", "frmProducto");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                CL_clsBitacora Guardar = new CL_clsBitacora(ex.Message, "Error", "frmProducto");
            }
        }

        public void EliminarProducto()
        {
            cdProducto.IdProducto = IdProducto;
            try
            {
                cdProducto.EliminarProducto();
                CL_clsBitacora Guardar = new CL_clsBitacora("Eliminado con exito", "Exitoso", "frmProducto");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                CL_clsBitacora Guardar = new CL_clsBitacora(ex.Message, "Error", "frmProducto");
            }
        }

        private void PasarDatos()
        {
            cdProducto.IdProducto = IdProducto;
            cdProducto.Nombre = Nombre;
            cdProducto.Categoria = Categoria;
            cdProducto.Marca = Marca;
        }

        #endregion
    }

}
