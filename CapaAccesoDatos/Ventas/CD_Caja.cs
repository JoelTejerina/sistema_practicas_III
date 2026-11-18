using System;
using System.Data;
using System.Globalization;

namespace CapaAccesoDatos.Ventas
{
    /// <summary>Caja del local: apertura, cierre, cobros del turno y cuentas por mesa.</summary>
    public class CD_Caja
    {
        private static string Num(decimal valor)
        {
            return valor.ToString(CultureInfo.InvariantCulture);
        }

        private static string T(string valor)
        {
            return (valor ?? "").Replace("'", "''");
        }

        /// <summary>La caja abierta (null si no hay): IdCaja, Apertura, MontoInicial y quien la abrio.</summary>
        public DataRow Abierta()
        {
            DataTable dt = new clsEjecutarComando().Ejecutar(
                "SELECT c.IdCaja, c.Apertura, c.MontoInicial, ISNULL(u.Usuario, N'') AS Usuario " +
                "FROM Caja c LEFT JOIN Usuarios u ON u.IdUsuario = c.IdUsuarioApertura WHERE c.Cierre IS NULL");
            return dt.Rows.Count == 0 ? null : dt.Rows[0];
        }

        /// <summary>Abre la caja. Solo puede haber una abierta a la vez (lo garantiza un indice unico en la base).</summary>
        public int Abrir(decimal montoInicial, int idUsuario)
        {
            object id = new clsEjecutarComando().EjecutarEscalar(
                "INSERT INTO Caja (IdUsuarioApertura, MontoInicial) VALUES (" + idUsuario + ", " + Num(montoInicial) + "); " +
                "SELECT CAST(SCOPE_IDENTITY() AS INT)");
            return Convert.ToInt32(id);
        }

        /// <summary>Cobros de la caja agrupados por forma de pago.</summary>
        public DataTable CobrosPorFormaPago(int idCaja)
        {
            return new clsEjecutarComando().Ejecutar(
                "SELECT FormaPago, COUNT(*) AS Pedidos, SUM(PrecioTotal) AS Total FROM Pedido " +
                "WHERE IdCaja = " + idCaja + " AND Estado = N'Cobrado' GROUP BY FormaPago ORDER BY FormaPago");
        }

        public decimal TotalCobrado(int idCaja)
        {
            object r = new clsEjecutarComando().EjecutarEscalar(
                "SELECT ISNULL(SUM(PrecioTotal), 0) FROM Pedido WHERE IdCaja = " + idCaja + " AND Estado = N'Cobrado'");
            return r == null ? 0 : Convert.ToDecimal(r);
        }

        public decimal CobradoEnEfectivo(int idCaja)
        {
            object r = new clsEjecutarComando().EjecutarEscalar(
                "SELECT ISNULL(SUM(PrecioTotal), 0) FROM Pedido WHERE IdCaja = " + idCaja +
                " AND Estado = N'Cobrado' AND FormaPago = N'Efectivo'");
            return r == null ? 0 : Convert.ToDecimal(r);
        }

        // El total cobrado se calcula en la base con los pedidos Cobrados de esa caja, no con lo que diga la pantalla.
        public void Cerrar(int idCaja, decimal montoContado, string observaciones, int idUsuario)
        {
            new clsEjecutarComando().EjecucionDirecta(
                "UPDATE Caja SET Cierre = SYSDATETIME(), IdUsuarioCierre = " + idUsuario +
                ", TotalCobrado = (SELECT ISNULL(SUM(PrecioTotal), 0) FROM Pedido WHERE IdCaja = " + idCaja + " AND Estado = N'Cobrado')" +
                ", MontoContado = " + Num(montoContado) + ", Observaciones = N'" + T(observaciones) + "' WHERE IdCaja = " + idCaja + " AND Cierre IS NULL");
        }

        /// <summary>Pedidos del salon que todavia no se cobraron, agrupados por mesa ("cuentas por mesa").</summary>
        public DataTable CuentasPorMesa()
        {
            return new clsEjecutarComando().Ejecutar(
                "SELECT ISNULL(NULLIF(Mesa, N''), N'(sin mesa)') AS Mesa, COUNT(*) AS Pedidos, SUM(PrecioTotal) AS Total " +
                "FROM Pedido WHERE Canal = N'Salon' AND Estado IN (N'Pendiente', N'En curso', N'Listo') " +
                "GROUP BY ISNULL(NULLIF(Mesa, N''), N'(sin mesa)') ORDER BY Mesa");
        }

        /// <summary>Pedidos sin cobrar ni cancelar (para avisar al cerrar la caja).</summary>
        public int PedidosSinCobrar()
        {
            return Convert.ToInt32(new clsEjecutarComando().EjecutarEscalar(
                "SELECT COUNT(*) FROM Pedido WHERE Estado IN (N'Pendiente', N'En curso', N'Listo')"));
        }

        public DataTable Historial()
        {
            return new clsEjecutarComando().Ejecutar(
                "SELECT c.IdCaja, c.Apertura, c.Cierre, ua.Usuario AS Abrio, uc.Usuario AS Cerro, c.MontoInicial, " +
                "c.TotalCobrado, c.MontoContado, " +
                "CASE WHEN c.Cierre IS NULL THEN NULL ELSE c.MontoContado - (c.MontoInicial + " +
                "ISNULL((SELECT SUM(p.PrecioTotal) FROM Pedido p WHERE p.IdCaja = c.IdCaja AND p.Estado = N'Cobrado' AND p.FormaPago = N'Efectivo'), 0)) END AS Diferencia, " +
                "ISNULL(c.Observaciones, N'') AS Observaciones " +
                "FROM Caja c LEFT JOIN Usuarios ua ON ua.IdUsuario = c.IdUsuarioApertura " +
                "LEFT JOIN Usuarios uc ON uc.IdUsuario = c.IdUsuarioCierre ORDER BY c.IdCaja DESC");
        }
    }
}
