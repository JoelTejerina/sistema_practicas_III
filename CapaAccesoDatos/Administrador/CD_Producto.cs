using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaAccesoDatos.Administrador
{
    public class CD_Producto
    {
        #region ATRIBUTOS
        private int idProducto;
        private string nombre;
        private string categoria;
        private string marca;
        #endregion

        #region PROPIEDADES

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

        public DataTable MostrarProductos()
        {
            string sSql = "SELECT * FROM Productos";
            clsEjecutarComando Ejecutar = new clsEjecutarComando();
            return Ejecutar.Ejecutar(sSql);
        }

        public void InsertarProducto()
        {
            string sSql = $"INSERT INTO Productos (nombre, categoria, marca) " +
                $"VALUES ('{Nombre}', '{Categoria}', '{Marca}')";
            clsEjecutarComando Ejecutar = new clsEjecutarComando();
            Ejecutar.Ejecutar(sSql);
        }

        public void ModificarProducto()
        {
            string sSql = $"UPDATE Productos SET " +
                $"nombre = '{Nombre}', categoria = '{Categoria}', marca = '{Marca}' " +
                $"WHERE idProducto = {IdProducto}";
            clsEjecutarComando Ejecutar = new clsEjecutarComando();
            Ejecutar.Ejecutar(sSql);
        }

        public void EliminarProducto()
        {
            string sSql = $"DELETE FROM Productos WHERE idProducto = {IdProducto}";
            clsEjecutarComando Ejecutar = new clsEjecutarComando();
            Ejecutar.Ejecutar(sSql);
        }
    }

}
