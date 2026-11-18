using System.Globalization;
using System;
using System.Data;
using CapaAccesoDatos;

namespace CapaAccesoDatos.Ventas
{
    public class CD_Estadisticas
    {
        public DataTable ResumenPedidosPorEstado()
        {
            string sSql = "SELECT Estado, COUNT(*) AS Cantidad, SUM(PrecioTotal) AS Total " +
                "FROM Pedido GROUP BY Estado ORDER BY Estado";
            clsEjecutarComando ejecutar = new clsEjecutarComando();
            return ejecutar.Ejecutar(sSql);
        }

        /// <summary>Ventas cobradas agrupadas por dia, semana o mes entre dos fechas.</summary>
        public DataTable VentasPorPeriodo(DateTime desde, DateTime hasta, string agrupar)
        {
            // Segun como se agrupe cambia la expresion que arma el "periodo"; el GROUP BY usa esa misma expresion.
            string periodo;
            string orden;
            switch (agrupar)
            {
                case "Semana":
                    // La semana se identifica por su lunes (1900-01-01 fue lunes: no depende de la configuracion del servidor).
                    periodo = "CONCAT(N'Sem. ', CONVERT(NVARCHAR(10), DATEADD(DAY, -(DATEDIFF(DAY, '19000101', Fecha) % 7), Fecha), 23))";
                    orden = "MIN(Fecha)";
                    break;
                case "Mes":
                    periodo = "CONCAT(DATENAME(YEAR, Fecha), N'-', RIGHT(CONCAT(N'0', MONTH(Fecha)), 2))";
                    orden = "MIN(Fecha)";
                    break;
                default:
                    periodo = "CONVERT(NVARCHAR(10), Fecha, 23)";
                    orden = "MIN(Fecha)";
                    break;
            }
            string d = desde.ToString("yyyyMMdd", CultureInfo.InvariantCulture);
            string h = hasta.ToString("yyyyMMdd", CultureInfo.InvariantCulture);
            // Solo cuentan los pedidos Cobrados; las fechas van en formato yyyyMMdd para que no dependan de la configuracion regional.
            string sSql = "SELECT " + periodo + " AS Periodo, COUNT(*) AS Pedidos, SUM(PrecioTotal) AS Total " +
                "FROM Pedido WHERE Estado = N'Cobrado' AND Fecha >= '" + d + "' AND Fecha <= '" + h + "' " +
                "GROUP BY " + periodo + " ORDER BY " + orden;
            return new clsEjecutarComando().Ejecutar(sSql);
        }

        /// <summary>Platillos mas pedidos (sin contar pedidos cancelados) entre dos fechas.</summary>
        public DataTable PlatosMasVendidos(DateTime desde, DateTime hasta, int cuantos)
        {
            string d = desde.ToString("yyyyMMdd", CultureInfo.InvariantCulture);
            string h = hasta.ToString("yyyyMMdd", CultureInfo.InvariantCulture);
            // Se suma por platillo desde el detalle (un pedido puede tener varios); los cancelados no cuentan.
            string sSql = "SELECT TOP (" + cuantos + ") m.Nombre AS Platillo, SUM(d.Cantidad) AS Unidades, SUM(d.Cantidad * d.PrecioUnitario) AS Importe " +
                "FROM PedidoDetalle d INNER JOIN Pedido p ON p.IdPedido = d.IdPedido INNER JOIN Menu m ON m.IdMenu = d.IdMenu " +
                "WHERE p.Estado <> N'Cancelado' AND p.Fecha >= '" + d + "' AND p.Fecha <= '" + h + "' " +
                "GROUP BY m.Nombre ORDER BY SUM(d.Cantidad) DESC, m.Nombre";
            return new clsEjecutarComando().Ejecutar(sSql);
        }

        public DataTable PedidosDelDia()
        {
            string sSql = "SELECT IdPedido, NombreCliente, Cantidad, Estado, PrecioTotal, Fecha " +
                "FROM Pedido WHERE Fecha >= CAST(GETDATE() AS DATE) AND Fecha < DATEADD(DAY, 1, CAST(GETDATE() AS DATE)) ORDER BY IdPedido DESC";
            clsEjecutarComando ejecutar = new clsEjecutarComando();
            return ejecutar.Ejecutar(sSql);
        }
    }
}
