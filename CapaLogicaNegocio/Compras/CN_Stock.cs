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
        public int StockMinimo { get; set; }
        public int StockMaximo { get; set; }
        #endregion

        #region METODOS
        public DataTable MostrarStock()
        {
            return stock.Mostrar();
        }

        public void InsertarStock()
        {
            Validar();
            PasarDatos();
            stock.InsertarStock();
        }

        public void ModificarStock()
        {
            Validar();
            PasarDatos();
            stock.ModificarStock();
        }

        public void EliminarStock()
        {
            stock.IdStock = IdStock;
            stock.EliminarStock();
        }

        /// <summary>Lotes cuya existencia llego al minimo configurado (alerta de stock bajo).</summary>
        public DataTable LotesBajoMinimo()
        {
            return stock.LotesBajoMinimo();
        }

        /// <summary>Historial de movimientos (trazabilidad) de un producto; 0 trae todos.</summary>
        public DataTable Historial(int idProducto)
        {
            return stock.Historial(idProducto);
        }

        // El maximo, si se configura, no puede ser menor que el minimo.
        private void Validar()
        {
            if (StockMinimo < 0 || StockMaximo < 0)
            {
                throw new InvalidOperationException("El stock minimo y maximo no pueden ser negativos.");
            }
            if (StockMaximo > 0 && StockMaximo < StockMinimo)
            {
                throw new InvalidOperationException("El stock maximo no puede ser menor que el minimo.");
            }
        }

        private void PasarDatos()
        {
            stock.IdStock = IdStock;
            stock.IdProducto = IdProducto;
            stock.NumeroLote = NumeroLote;
            stock.Cantidad = Cantidad;
            stock.FechaVencimiento = FechaVencimiento;
            stock.Precio = Precio;
            stock.StockMinimo = StockMinimo;
            stock.StockMaximo = StockMaximo;
        }
        #endregion
    }
}
