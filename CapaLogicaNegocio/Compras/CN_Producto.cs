using System.Data;
using CapaAccesoDatos.Compras;

namespace CapaLogicaNegocio.Compras
{
    public class CN_Producto
    {
        private CD_Producto producto = new CD_Producto();

        #region PROPERTIES
        public int IdProducto { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public string Marca { get; set; }
        public string Categoria { get; set; }
        public string Medida { get; set; }
        public int IdProveedor { get; set; }
        #endregion

        #region METODOS
        public DataTable MostrarProducto()
        {
            return producto.Mostrar();
        }

        public void InsertarProducto()
        {
            PasarDatos();
            producto.InsertarProducto();
        }

        public void ModificarProducto()
        {
            PasarDatos();
            producto.ModificarProducto();
        }

        public void EliminarProducto()
        {
            producto.IdProducto = IdProducto;
            producto.EliminarProducto();
        }

        private void PasarDatos()
        {
            producto.IdProducto = IdProducto;
            producto.Nombre = Nombre;
            producto.Descripcion = Descripcion;
            producto.Marca = Marca;
            producto.Categoria = Categoria;
            producto.Medida = Medida;
            producto.IdProveedor = IdProveedor;
        }
        #endregion
    }
}
