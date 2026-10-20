using System;
using System.Data;
using System.Globalization;
using CapaAccesoDatos;
using CapaComun;

namespace CapaAccesoDatos.Compras
{
    public class CD_Stock
    {
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
        public DataTable Mostrar()
        {
            string sSql = "SELECT * FROM Stock ORDER BY IdStock";
            clsEjecutarComando Ejecutar = new clsEjecutarComando();
            return Ejecutar.Ejecutar(sSql);
        }

        // Cada cambio de stock deja un movimiento (trazabilidad). Todo va en una transaccion.
        public void InsertarStock()
        {
            string sSql = "SET XACT_ABORT ON; BEGIN TRAN; " +
                "INSERT INTO Stock (IdProducto, NumeroLote, Cantidad, FechaVencimiento, Precio, StockMinimo, StockMaximo) VALUES (" +
                IdProducto + ",'" + T(NumeroLote) + "'," + Cantidad + ",'" + Fecha() + "'," + Num(Precio) + "," +
                StockMinimo + "," + StockMaximo + "); " +
                "DECLARE @id INT = SCOPE_IDENTITY(); " +
                "INSERT INTO MovimientoStock (Tipo, IdStock, IdProducto, NumeroLote, Cantidad, CantidadResultante, IdUsuario, Detalle) VALUES (" +
                "N'Ingreso', @id, " + IdProducto + ",'" + T(NumeroLote) + "'," + Cantidad + "," + Cantidad + "," +
                UserCache.IdUsuario + ", N'Alta de lote'); " +
                "COMMIT;";
            clsEjecutarComando Ejecutar = new clsEjecutarComando();
            Ejecutar.EjecucionDirecta(sSql);
        }

        public void ModificarStock()
        {
            string sSql = "SET XACT_ABORT ON; BEGIN TRAN; " +
                "DECLARE @ant INT = (SELECT Cantidad FROM Stock WITH (UPDLOCK) WHERE IdStock=" + IdStock + "); " +
                "UPDATE Stock SET " +
                "IdProducto=" + IdProducto + ", NumeroLote='" + T(NumeroLote) + "', Cantidad=" + Cantidad +
                ", FechaVencimiento='" + Fecha() + "', Precio=" + Num(Precio) +
                ", StockMinimo=" + StockMinimo + ", StockMaximo=" + StockMaximo + " " +
                "WHERE IdStock=" + IdStock + "; " +
                "IF @ant IS NOT NULL AND @ant <> " + Cantidad + " " +
                "INSERT INTO MovimientoStock (Tipo, IdStock, IdProducto, NumeroLote, Cantidad, CantidadResultante, IdUsuario, Detalle) VALUES (" +
                "N'Ajuste', " + IdStock + ", " + IdProducto + ",'" + T(NumeroLote) + "'," + Cantidad + " - @ant," + Cantidad + "," +
                UserCache.IdUsuario + ", N'Modificacion manual del lote'); " +
                "COMMIT;";
            clsEjecutarComando Ejecutar = new clsEjecutarComando();
            Ejecutar.EjecucionDirecta(sSql);
        }

        public void EliminarStock()
        {
            string sSql = "SET XACT_ABORT ON; BEGIN TRAN; " +
                "INSERT INTO MovimientoStock (Tipo, IdStock, IdProducto, NumeroLote, Cantidad, CantidadResultante, IdUsuario, Detalle) " +
                "SELECT N'Baja', IdStock, IdProducto, NumeroLote, -Cantidad, 0, " + UserCache.IdUsuario + ", N'Baja del lote' " +
                "FROM Stock WHERE IdStock=" + IdStock + " AND Cantidad > 0; " +
                "DELETE FROM Stock WHERE IdStock=" + IdStock + "; " +
                "COMMIT;";
            clsEjecutarComando Ejecutar = new clsEjecutarComando();
            Ejecutar.EjecucionDirecta(sSql);
        }

        /// <summary>Lotes con existencia igual o menor al minimo configurado (alerta de stock bajo).</summary>
        public DataTable LotesBajoMinimo()
        {
            string sSql = "SELECT s.IdStock, p.Nombre AS Producto, s.NumeroLote, s.Cantidad, s.StockMinimo " +
                "FROM Stock s INNER JOIN Producto p ON p.IdProducto = s.IdProducto " +
                "WHERE s.StockMinimo > 0 AND s.Cantidad <= s.StockMinimo AND p.Activo = 1 ORDER BY p.Nombre";
            clsEjecutarComando Ejecutar = new clsEjecutarComando();
            return Ejecutar.Ejecutar(sSql);
        }

        /// <summary>Historial de movimientos de un producto (todos si idProducto es 0).</summary>
        public DataTable Historial(int idProducto)
        {
            string sSql = "SELECT m.Fecha, p.Nombre AS Producto, m.NumeroLote AS Lote, m.Tipo, m.Cantidad, " +
                "m.CantidadResultante AS Resultante, ISNULL(u.Usuario, '') AS Usuario, " +
                "ISNULL(CAST(m.IdPedido AS NVARCHAR(20)), '') AS Pedido, ISNULL(m.Detalle, '') AS Detalle " +
                "FROM MovimientoStock m INNER JOIN Producto p ON p.IdProducto = m.IdProducto " +
                "LEFT JOIN Usuarios u ON u.IdUsuario = m.IdUsuario " +
                (idProducto > 0 ? "WHERE m.IdProducto = " + idProducto + " " : "") +
                "ORDER BY m.Fecha DESC, m.IdMovimiento DESC";
            clsEjecutarComando Ejecutar = new clsEjecutarComando();
            return Ejecutar.Ejecutar(sSql);
        }

        private string T(string valor)
        {
            return (valor ?? "").Replace("'", "''");
        }

        // Formato de fecha no ambiguo para SQL Server (yyyyMMdd)
        private string Fecha()
        {
            return FechaVencimiento.ToString("yyyyMMdd", CultureInfo.InvariantCulture);
        }

        // Numero con punto decimal, independiente de la cultura/idioma activo
        private string Num(decimal valor)
        {
            return valor.ToString(CultureInfo.InvariantCulture);
        }
        #endregion
    }
}
