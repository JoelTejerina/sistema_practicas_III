using System;
using System.Data;
using CapaAccesoDatos.Compras;

namespace CapaLogicaNegocio.Compras
{
    public class CN_Stock
    {
        private CD_Stock stock = new CD_Stock();

        #region PROPERTIES
        public int IdStock { get; set; }
        public int IdProducto { get; set; }
        public string NumeroLote { get; set; }
        public int Cantidad { get; set; }
        public DateTime FechaVencimiento { get; set; }
        public decimal Precio { get; set; }
        #endregion

        #region METODOS
        public DataTable MostrarStock()
        {
            return stock.Mostrar();
        }

        public void InsertarStock()
        {
            PasarDatos();
            stock.InsertarStock();
        }

        public void ModificarStock()
        {
            PasarDatos();
            stock.ModificarStock();
        }

        public void EliminarStock()
        {
            stock.IdStock = IdStock;
            stock.EliminarStock();
        }

        private void PasarDatos()
        {
            stock.IdStock = IdStock;
            stock.IdProducto = IdProducto;
            stock.NumeroLote = NumeroLote;
            stock.Cantidad = Cantidad;
            stock.FechaVencimiento = FechaVencimiento;
            stock.Precio = Precio;
        }
        #endregion
    }
}
