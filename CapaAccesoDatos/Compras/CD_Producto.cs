using System.Data;
using CapaAccesoDatos;

namespace CapaAccesoDatos.Compras
{
    public class CD_Producto
    {
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
        public DataTable Mostrar()
        {
            string sSql = "SELECT * FROM Producto ORDER BY Nombre";
            clsEjecutarComando Ejecutar = new clsEjecutarComando();
            return Ejecutar.Ejecutar(sSql);
        }

        public void InsertarProducto()
        {
            string sSql = "INSERT INTO Producto " +
                "(Nombre, Descripcion, Marca, Categoria, Medida, IdProveedor) VALUES (" +
                "'" + T(Nombre) + "','" + T(Descripcion) + "','" + T(Marca) + "','" + T(Categoria) + "','" +
                T(Medida) + "'," + IdProveedor + ")";
            clsEjecutarComando Ejecutar = new clsEjecutarComando();
            Ejecutar.Ejecutar(sSql);
        }

        public void ModificarProducto()
        {
            string sSql = "UPDATE Producto SET " +
                "Nombre='" + T(Nombre) + "', Descripcion='" + T(Descripcion) + "', Marca='" + T(Marca) +
                "', Categoria='" + T(Categoria) + "', Medida='" + T(Medida) + "', IdProveedor=" + IdProveedor + " " +
                "WHERE IdProducto=" + IdProducto;
            clsEjecutarComando Ejecutar = new clsEjecutarComando();
            Ejecutar.Ejecutar(sSql);
        }

        public void EliminarProducto()
        {
            string sSql = "DELETE FROM Producto WHERE IdProducto=" + IdProducto;
            clsEjecutarComando Ejecutar = new clsEjecutarComando();
            Ejecutar.Ejecutar(sSql);
        }

        private string T(string valor)
        {
            return (valor ?? "").Replace("'", "''");
        }
        #endregion
    }
}
