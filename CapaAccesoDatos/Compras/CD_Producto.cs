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
        public int CantidadInicial { get; set; }
        #endregion

        #region METODOS
        public DataTable Mostrar()
        {
            string sSql = "SELECT * FROM Producto WHERE Activo = 1 ORDER BY Nombre";
            clsEjecutarComando Ejecutar = new clsEjecutarComando();
            return Ejecutar.Ejecutar(sSql);
        }

        /// <summary>Hay otro producto activo con el mismo nombre, marca y medida.</summary>
        public bool Duplicado()
        {
            string sSql = "SELECT 1 FROM Producto WHERE Activo = 1 AND IdProducto <> " + IdProducto +
                " AND Nombre = N'" + T(Nombre) + "' AND ISNULL(Marca, N'') = N'" + T(Marca) +
                "' AND ISNULL(Medida, N'') = N'" + T(Medida) + "'";
            return new clsEjecutarComando().Ejecutar(sSql).Rows.Count > 0;
        }

        // Con cantidad inicial se crea, en la misma transaccion, el primer lote de stock del producto.
        public void InsertarProducto()
        {
            string sSql = "SET XACT_ABORT ON; BEGIN TRAN; " +
                "INSERT INTO Producto (Nombre, Descripcion, Marca, Categoria, Medida, IdProveedor, CantidadInicial) VALUES (" +
                "N'" + T(Nombre) + "',N'" + T(Descripcion) + "',N'" + T(Marca) + "',N'" + T(Categoria) + "',N'" +
                T(Medida) + "'," + IdProveedor + "," + CantidadInicial + "); " +
                "DECLARE @id INT = SCOPE_IDENTITY(); ";
            if (CantidadInicial > 0)
            {
                sSql +=
                    "DECLARE @lote NVARCHAR(50) = N'INI-' + CAST(@id AS NVARCHAR(10)); " +
                    "INSERT INTO Stock (IdProducto, NumeroLote, Cantidad, FechaVencimiento, Precio, StockMinimo, StockMaximo, IdProveedor) " +
                    "VALUES (@id, @lote, " + CantidadInicial + ", NULL, 0, 0, 0, " + IdProveedor + "); " +
                    "DECLARE @st INT = SCOPE_IDENTITY(); " +
                    "INSERT INTO MovimientoStock (Tipo, IdStock, IdProducto, NumeroLote, Cantidad, CantidadResultante, IdUsuario, Detalle) " +
                    "VALUES (N'Ingreso', @st, @id, @lote, " + CantidadInicial + ", " + CantidadInicial + ", " +
                    CapaComun.UserCache.IdUsuario + ", N'Cantidad inicial del producto'); ";
            }
            sSql += "COMMIT;";
            new clsEjecutarComando().EjecucionDirecta(sSql);
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
            string sSql = "UPDATE Producto SET Activo = 0 WHERE IdProducto=" + IdProducto;
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
