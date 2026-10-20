using System.Data;
using System.Globalization;
using CapaAccesoDatos;

namespace CapaAccesoDatos.Ventas
{
    /// <summary>Ingredientes (con cantidad) de cada platillo del menu.</summary>
    public class CD_Receta
    {
        // Stock utilizable de un producto: lotes no vencidos.
        private const string StockDisponible =
            "ISNULL((SELECT SUM(s.Cantidad) FROM Stock s WHERE s.IdProducto = r.IdProducto " +
            "AND (s.FechaVencimiento IS NULL OR s.FechaVencimiento >= CAST(GETDATE() AS DATE))), 0)";

        public DataTable Listar(int idMenu)
        {
            string sSql = "SELECT r.IdProducto, p.Nombre AS Ingrediente, ISNULL(p.Medida, '') AS Medida, " +
                "r.Cantidad, CASE WHEN r.EsPrincipal = 1 THEN 'Principal' ELSE 'Secundario' END AS Tipo, " +
                StockDisponible + " AS StockDisponible " +
                "FROM RecetaIngrediente r INNER JOIN Producto p ON p.IdProducto = r.IdProducto " +
                "WHERE r.IdMenu = " + idMenu + " ORDER BY r.EsPrincipal DESC, p.Nombre";
            clsEjecutarComando Ejecutar = new clsEjecutarComando();
            return Ejecutar.Ejecutar(sSql);
        }

        /// <summary>Productos activos que se pueden usar como ingrediente (con su medida).</summary>
        public DataTable ProductosDisponibles()
        {
            string sSql = "SELECT IdProducto, Nombre, ISNULL(Medida, '') AS Medida FROM Producto WHERE Activo = 1 ORDER BY Nombre";
            clsEjecutarComando Ejecutar = new clsEjecutarComando();
            return Ejecutar.Ejecutar(sSql);
        }

        // Agrega el ingrediente o, si ya estaba en la receta, actualiza su cantidad.
        public void Guardar(int idMenu, int idProducto, decimal cantidad, bool principal)
        {
            string cant = cantidad.ToString(CultureInfo.InvariantCulture);
            string sSql =
                "IF EXISTS (SELECT 1 FROM RecetaIngrediente WHERE IdMenu = " + idMenu + " AND IdProducto = " + idProducto + ") " +
                "UPDATE RecetaIngrediente SET Cantidad = " + cant + ", EsPrincipal = " + (principal ? 1 : 0) +
                " WHERE IdMenu = " + idMenu + " AND IdProducto = " + idProducto + " " +
                "ELSE INSERT INTO RecetaIngrediente (IdMenu, IdProducto, Cantidad, EsPrincipal) VALUES (" +
                idMenu + "," + idProducto + "," + cant + "," + (principal ? 1 : 0) + ")";
            clsEjecutarComando Ejecutar = new clsEjecutarComando();
            Ejecutar.EjecucionDirecta(sSql);
        }

        public void Quitar(int idMenu, int idProducto)
        {
            string sSql = "DELETE FROM RecetaIngrediente WHERE IdMenu = " + idMenu + " AND IdProducto = " + idProducto;
            clsEjecutarComando Ejecutar = new clsEjecutarComando();
            Ejecutar.EjecucionDirecta(sSql);
        }

        /// <summary>Cuantas unidades de cada platillo se pueden preparar con el stock actual.</summary>
        public DataTable Capacidad()
        {
            // El stock se calcula en una tabla derivada: SQL Server no permite MIN() sobre una subconsulta.
            string sSql = "SELECT m.IdMenu, m.Nombre AS Platillo, " +
                "CASE WHEN COUNT(x.IdProducto) = 0 THEN NULL " +
                "ELSE CAST(MIN(FLOOR(x.Disp / x.Cantidad)) AS INT) END AS Capacidad " +
                "FROM Menu m LEFT JOIN (" +
                "SELECT r.IdMenu, r.IdProducto, r.Cantidad, " + StockDisponible + " AS Disp FROM RecetaIngrediente r" +
                ") x ON x.IdMenu = m.IdMenu " +
                "WHERE m.Activo = 1 GROUP BY m.IdMenu, m.Nombre ORDER BY m.Nombre";
            clsEjecutarComando Ejecutar = new clsEjecutarComando();
            return Ejecutar.Ejecutar(sSql);
        }
    }
}
