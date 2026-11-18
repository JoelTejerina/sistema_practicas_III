using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using CapaAccesoDatos;
using CapaComun;

namespace CapaAccesoDatos.Ventas
{
    /// <summary>
    /// Pedidos. Un pedido tiene un detalle de uno o mas platillos. Al confirmarlo se verifica que
    /// el stock alcance para preparar todo y se descuentan los insumos segun las recetas
    /// (primero los lotes que vencen antes); al cancelarlo se devuelven. Todo en una transaccion.
    /// </summary>
    public class CD_Pedido : clsConexion
    {
        public const string EstadoCancelado = "Cancelado";

        #region PROPERTIES
        public int IdPedido { get; set; }
        public string NombreCliente { get; set; }
        public string FormaPago { get; set; }
        public string InstruccionesEspeciales { get; set; }
        public string Estado { get; set; }
        public DateTime Fecha { get; set; }

        /// <summary>Salon, Barra o Delivery.</summary>
        public string Canal { get; set; }

        /// <summary>Mesa (solo para el salon).</summary>
        public string Mesa { get; set; }
        #endregion

        #region CONSULTAS
        public DataTable Mostrar()
        {
            string sSql = "SELECT p.IdPedido, p.NombreCliente, p.IdMenu, p.Cantidad, p.FormaPago, " +
                "p.InstruccionesEspeciales, p.Estado, p.PrecioTotal, p.Fecha, ISNULL(u.Usuario, '') AS Empleado, " +
                "p.Canal, ISNULL(p.Mesa, '') AS Mesa " +
                "FROM Pedido p LEFT JOIN Usuarios u ON u.IdUsuario = p.IdUsuario ORDER BY p.IdPedido DESC";
            clsEjecutarComando Ejecutar = new clsEjecutarComando();
            return Ejecutar.Ejecutar(sSql);
        }

        public DataTable Detalle(int idPedido)
        {
            string sSql = "SELECT d.IdMenu, m.Nombre AS Platillo, d.Cantidad, d.PrecioUnitario " +
                "FROM PedidoDetalle d INNER JOIN Menu m ON m.IdMenu = d.IdMenu " +
                "WHERE d.IdPedido = " + idPedido + " ORDER BY d.IdDetalle";
            clsEjecutarComando Ejecutar = new clsEjecutarComando();
            return Ejecutar.Ejecutar(sSql);
        }

        /// <summary>Pedidos que estan en etapa de elaboracion (pendientes o en curso).</summary>
        public DataTable MostrarEnElaboracion()
        {
            string sSql = "SELECT p.IdPedido, p.NombreCliente AS Cliente, " +
                "ISNULL((SELECT STRING_AGG(CONCAT(d.Cantidad, N' x ', m.Nombre), N', ') " +
                "FROM PedidoDetalle d INNER JOIN Menu m ON m.IdMenu = d.IdMenu WHERE d.IdPedido = p.IdPedido), N'') AS Detalle, " +
                "p.Estado, p.Fecha, ISNULL(p.InstruccionesEspeciales, N'') AS Instrucciones " +
                "FROM Pedido p WHERE p.Estado IN (N'Pendiente', N'En curso') " +
                "ORDER BY CASE p.Estado WHEN N'En curso' THEN 0 ELSE 1 END, p.Fecha, p.IdPedido";
            clsEjecutarComando Ejecutar = new clsEjecutarComando();
            return Ejecutar.Ejecutar(sSql);
        }
        #endregion

        #region ALTA / MODIFICACION / BAJA
        /// <summary>Da de alta el pedido con su detalle. Devuelve el numero de pedido.</summary>
        public int InsertarPedido(List<ItemPedido> items)
        {
            ValidarItems(items);
            using (SqlConnection cn = GetConexion())
            {
                cn.Open();
                using (SqlTransaction tx = cn.BeginTransaction())
                {
                    try
                    {
                        // Se valida antes de insertar nada: si algun platillo no se puede pedir, ni se toca la tabla.
                        VerificarDisponibles(cn, tx, items, new List<ItemPedido>());
                        int id;
                        using (SqlCommand cmd = new SqlCommand(
                            "INSERT INTO Pedido (NombreCliente, IdMenu, Cantidad, FormaPago, InstruccionesEspeciales, Estado, PrecioTotal, Fecha, IdUsuario, Canal, Mesa) " +
                            "VALUES (@cli, @menu, @cant, @pago, @instr, @estado, @total, @fecha, @usr, @canal, @mesa); SELECT CAST(SCOPE_IDENTITY() AS INT);", cn, tx))
                        {
                            ParametrosPedido(cmd, items);
                            cmd.Parameters.AddWithValue("@usr", UserCache.IdUsuario > 0 ? (object)UserCache.IdUsuario : DBNull.Value);
                            id = (int)cmd.ExecuteScalar();
                        }

                        GuardarDetalle(cn, tx, id, items);
                        if (!EsCancelado(Estado))
                        {
                            Descontar(cn, tx, id, items);
                        }
                        tx.Commit();
                        return id;
                    }
                    catch
                    {
                        tx.Rollback();
                        throw;
                    }
                }
            }
        }

        /// <summary>
        /// Modifica el pedido. El stock solo se toca si cambia el detalle o si el pedido pasa
        /// a/desde Cancelado: cambiar la forma de pago de un pedido ya servido no mueve stock.
        /// </summary>
        public void ModificarPedido(List<ItemPedido> items)
        {
            ValidarItems(items);
            using (SqlConnection cn = GetConexion())
            {
                cn.Open();
                using (SqlTransaction tx = cn.BeginTransaction())
                {
                    try
                    {
                        string estadoAnterior = LeerEstado(cn, tx, IdPedido);
                        List<ItemPedido> anteriores = LeerDetalle(cn, tx, IdPedido);
                        VerificarDisponibles(cn, tx, items, anteriores);

                        using (SqlCommand cmd = new SqlCommand(
                            "UPDATE Pedido SET NombreCliente=@cli, IdMenu=@menu, Cantidad=@cant, FormaPago=@pago, " +
                            "InstruccionesEspeciales=@instr, Estado=@estado, PrecioTotal=@total, Fecha=@fecha, Canal=@canal, Mesa=@mesa WHERE IdPedido=@id", cn, tx))
                        {
                            ParametrosPedido(cmd, items);
                            cmd.Parameters.AddWithValue("@id", IdPedido);
                            cmd.ExecuteNonQuery();
                        }

                        GuardarDetalle(cn, tx, IdPedido, items);

                        // Que pasa con el stock segun como cambio el pedido:
                        // - pasa a Cancelado: se devuelve todo lo descontado
                        // - estaba Cancelado y vuelve a estar activo: se descuenta de nuevo
                        // - sigue activo pero cambio el detalle: se devuelve todo y se descuenta con el detalle nuevo
                        // - sigue activo con el mismo detalle: no se toca el stock (por ejemplo, solo cambio la forma de pago)
                        bool eraCancelado = EsCancelado(estadoAnterior);
                        bool esCancelado = EsCancelado(Estado);
                        if (!eraCancelado && esCancelado)
                        {
                            Reponer(cn, tx, IdPedido);
                        }
                        else if (eraCancelado && !esCancelado)
                        {
                            Descontar(cn, tx, IdPedido, items);
                        }
                        else if (!esCancelado && !MismoDetalle(anteriores, items))
                        {
                            Reponer(cn, tx, IdPedido);
                            Descontar(cn, tx, IdPedido, items);
                        }
                        tx.Commit();
                    }
                    catch
                    {
                        tx.Rollback();
                        throw;
                    }
                }
            }
        }

        /// <summary>Cambia solo el estado (lo usa Elaboracion). Cancelar devuelve el stock.</summary>
        public void CambiarEstado(int idPedido, string nuevoEstado)
        {
            using (SqlConnection cn = GetConexion())
            {
                cn.Open();
                using (SqlTransaction tx = cn.BeginTransaction())
                {
                    try
                    {
                        string anterior = LeerEstado(cn, tx, idPedido);
                        using (SqlCommand cmd = new SqlCommand("UPDATE Pedido SET Estado=@e WHERE IdPedido=@id", cn, tx))
                        {
                            cmd.Parameters.AddWithValue("@e", nuevoEstado);
                            cmd.Parameters.AddWithValue("@id", idPedido);
                            cmd.ExecuteNonQuery();
                        }

                        if (!EsCancelado(anterior) && EsCancelado(nuevoEstado))
                        {
                            Reponer(cn, tx, idPedido);
                        }
                        else if (EsCancelado(anterior) && !EsCancelado(nuevoEstado))
                        {
                            Descontar(cn, tx, idPedido, LeerDetalle(cn, tx, idPedido));
                        }
                        tx.Commit();
                    }
                    catch
                    {
                        tx.Rollback();
                        throw;
                    }
                }
            }
        }

        public void EliminarPedido()
        {
            using (SqlConnection cn = GetConexion())
            {
                cn.Open();
                using (SqlTransaction tx = cn.BeginTransaction())
                {
                    try
                    {
                        // Solo un pedido que todavia no se empezo a preparar devuelve sus insumos.
                        if (LeerEstado(cn, tx, IdPedido) == "Pendiente")
                        {
                            Reponer(cn, tx, IdPedido);
                        }
                        using (SqlCommand cmd = new SqlCommand("DELETE FROM Pedido WHERE IdPedido=@id", cn, tx))
                        {
                            cmd.Parameters.AddWithValue("@id", IdPedido);
                            cmd.ExecuteNonQuery();
                        }
                        tx.Commit();
                    }
                    catch
                    {
                        tx.Rollback();
                        throw;
                    }
                }
            }
        }

        /// <summary>Cobra el pedido en la caja indicada. Un pedido cancelado o ya cobrado no se puede cobrar.</summary>
        public void CobrarPedido(int idCaja)
        {
            using (SqlConnection cn = GetConexion())
            {
                cn.Open();
                using (SqlCommand cmd = new SqlCommand(
                    "UPDATE Pedido SET Estado = N'Cobrado', IdCaja = @caja, FechaCobro = SYSDATETIME() " +
                    "WHERE IdPedido = @id AND Estado NOT IN (N'Cancelado', N'Cobrado')", cn))
                {
                    cmd.Parameters.AddWithValue("@caja", idCaja);
                    cmd.Parameters.AddWithValue("@id", IdPedido);
                    if (cmd.ExecuteNonQuery() == 0)
                    {
                        throw new InvalidOperationException(Idioma.Texto("Msg_PedidoNoCobrable"));
                    }
                }
            }
        }

        /// <summary>Estado actual del pedido (vacio si no existe).</summary>
        public string EstadoDe(int idPedido)
        {
            DataTable dt = new clsEjecutarComando().Ejecutar("SELECT Estado FROM Pedido WHERE IdPedido = " + idPedido);
            return dt.Rows.Count == 0 ? "" : Convert.ToString(dt.Rows[0][0]);
        }

        /// <summary>Datos de cabecera del pedido para el ticket.</summary>
        public DataRow Cabecera(int idPedido)
        {
            DataTable dt = new clsEjecutarComando().Ejecutar(
                "SELECT p.IdPedido, p.NombreCliente, p.Fecha, p.FormaPago, p.Estado, p.PrecioTotal, p.Canal, ISNULL(p.Mesa, N'') AS Mesa, " +
                "ISNULL(p.InstruccionesEspeciales, N'') AS Instrucciones, ISNULL(u.Usuario, N'') AS Empleado " +
                "FROM Pedido p LEFT JOIN Usuarios u ON u.IdUsuario = p.IdUsuario WHERE p.IdPedido = " + idPedido);
            return dt.Rows.Count == 0 ? null : dt.Rows[0];
        }
        #endregion

        #region STOCK
        private class Lote
        {
            public int IdStock;
            public string NumeroLote;
            public int Cantidad;
        }

        private class Ingrediente
        {
            public int IdProducto;
            public decimal Cantidad;
        }

        // Verifica que el stock alcance para todo el pedido y descuenta los insumos.
        private void Descontar(SqlConnection cn, SqlTransaction tx, int idPedido, List<ItemPedido> items)
        {
            Dictionary<int, decimal> necesidad = new Dictionary<int, decimal>();
            Dictionary<int, List<Ingrediente>> recetas = new Dictionary<int, List<Ingrediente>>();
            foreach (ItemPedido item in items)
            {
                if (!recetas.ContainsKey(item.IdMenu))
                {
                    recetas[item.IdMenu] = LeerReceta(cn, tx, item.IdMenu);
                }
                foreach (Ingrediente ing in recetas[item.IdMenu])
                {
                    decimal actual;
                    necesidad.TryGetValue(ing.IdProducto, out actual);
                    necesidad[ing.IdProducto] = actual + ing.Cantidad * item.Cantidad;
                }
            }

            // Los platillos sin receta no tienen ingredientes que descontar.
            if (necesidad.Count == 0)
            {
                return;
            }

            // Primero se junta todo lo que falta y recien despues se descuenta: asi un pedido nunca queda descontado a medias.
            Dictionary<int, List<Lote>> lotes = new Dictionary<int, List<Lote>>();
            Dictionary<int, int> disponible = new Dictionary<int, int>();
            List<string> faltantes = new List<string>();
            foreach (KeyValuePair<int, decimal> n in necesidad)
            {
                List<Lote> lista = LeerLotes(cn, tx, n.Key);
                lotes[n.Key] = lista;
                int hay = lista.Sum(l => l.Cantidad);
                disponible[n.Key] = hay;
                // El stock se maneja en enteros: si la receta pide fracciones (0,5 de lechuga) la necesidad se redondea hacia arriba.
                int necesita = (int)Math.Ceiling(n.Value);
                if (hay < necesita)
                {
                    faltantes.Add(string.Format(Idioma.Texto("Stock_Falta"), NombreProducto(cn, tx, n.Key), necesita, hay));
                }
            }

            if (faltantes.Count > 0)
            {
                throw new StockInsuficienteException(ArmarMensaje(items, recetas, disponible, faltantes));
            }

            foreach (KeyValuePair<int, decimal> n in necesidad)
            {
                int restante = (int)Math.Ceiling(n.Value);
                // LeerLotes ya los trae ordenados por vencimiento, o sea que se gasta primero lo que vence antes (FIFO).
                foreach (Lote lote in lotes[n.Key])
                {
                    if (restante <= 0)
                    {
                        break;
                    }
                    int toma = Math.Min(lote.Cantidad, restante);
                    using (SqlCommand cmd = new SqlCommand(
                        "UPDATE Stock SET Cantidad = Cantidad - @t WHERE IdStock = @s; " +
                        "INSERT INTO MovimientoStock (Tipo, IdStock, IdProducto, NumeroLote, Cantidad, CantidadResultante, IdUsuario, IdPedido, Detalle) " +
                        "VALUES (N'Egreso', @s, @p, @lote, -@t, @res, @u, @ped, @det)", cn, tx))
                    {
                        cmd.Parameters.AddWithValue("@t", toma);
                        cmd.Parameters.AddWithValue("@s", lote.IdStock);
                        cmd.Parameters.AddWithValue("@p", n.Key);
                        cmd.Parameters.AddWithValue("@lote", lote.NumeroLote ?? "");
                        cmd.Parameters.AddWithValue("@res", lote.Cantidad - toma);
                        cmd.Parameters.AddWithValue("@u", UserCache.IdUsuario);
                        cmd.Parameters.AddWithValue("@ped", idPedido);
                        cmd.Parameters.AddWithValue("@det", "Pedido #" + idPedido);
                        cmd.ExecuteNonQuery();
                    }
                    restante -= toma;
                }
            }
        }

        // Devuelve al stock lo que el pedido habia descontado (segun los movimientos registrados).
        private void Reponer(SqlConnection cn, SqlTransaction tx, int idPedido)
        {
            List<int[]> aDevolver = new List<int[]>();   // {IdStock, IdProducto, cantidad}
            List<string> nombres = new List<string>();
            using (SqlCommand cmd = new SqlCommand(
                // Se devuelve exactamente lo que dejaron registrado los movimientos del pedido, lote por lote.
                "SELECT IdStock, IdProducto, MAX(NumeroLote), -SUM(Cantidad) FROM MovimientoStock " +
                // Egresos (negativos) mas devoluciones anteriores (positivas): si ya se devolvio, la suma da 0 y no se repone dos veces.
                "WHERE IdPedido = @p AND IdStock IS NOT NULL GROUP BY IdStock, IdProducto HAVING SUM(Cantidad) < 0", cn, tx))
            {
                cmd.Parameters.AddWithValue("@p", idPedido);
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        aDevolver.Add(new[] { dr.GetInt32(0), dr.GetInt32(1), dr.GetInt32(3) });
                        nombres.Add(dr.IsDBNull(2) ? "" : dr.GetString(2));
                    }
                }
            }

            for (int i = 0; i < aDevolver.Count; i++)
            {
                int[] d = aDevolver[i];
                int resultante;
                using (SqlCommand cmd = new SqlCommand(
                    "UPDATE Stock SET Cantidad = Cantidad + @c OUTPUT INSERTED.Cantidad WHERE IdStock = @s", cn, tx))
                {
                    cmd.Parameters.AddWithValue("@c", d[2]);
                    cmd.Parameters.AddWithValue("@s", d[0]);
                    object r = cmd.ExecuteScalar();
                    if (r == null)
                    {
                        continue;   // el lote ya no existe
                    }
                    resultante = Convert.ToInt32(r);
                }

                using (SqlCommand cmd = new SqlCommand(
                    "INSERT INTO MovimientoStock (Tipo, IdStock, IdProducto, NumeroLote, Cantidad, CantidadResultante, IdUsuario, IdPedido, Detalle) " +
                    "VALUES (N'Devolucion', @s, @p, @lote, @c, @res, @u, @ped, @det)", cn, tx))
                {
                    cmd.Parameters.AddWithValue("@s", d[0]);
                    cmd.Parameters.AddWithValue("@p", d[1]);
                    cmd.Parameters.AddWithValue("@lote", nombres[i]);
                    cmd.Parameters.AddWithValue("@c", d[2]);
                    cmd.Parameters.AddWithValue("@res", resultante);
                    cmd.Parameters.AddWithValue("@u", UserCache.IdUsuario);
                    cmd.Parameters.AddWithValue("@ped", idPedido);
                    cmd.Parameters.AddWithValue("@det", "Devolucion del pedido #" + idPedido);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private string ArmarMensaje(List<ItemPedido> items, Dictionary<int, List<Ingrediente>> recetas,
            Dictionary<int, int> disponible, List<string> faltantes)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(Idioma.Texto("Stock_NoAlcanza"));
            foreach (string f in faltantes)
            {
                sb.AppendLine("- " + f);
            }

            sb.AppendLine();
            sb.AppendLine(Idioma.Texto("Stock_Puede"));
            HashSet<int> vistos = new HashSet<int>();
            foreach (ItemPedido item in items)
            {
                if (!vistos.Add(item.IdMenu) || recetas[item.IdMenu].Count == 0)
                {
                    continue;
                }
                // Cuantos platos se pueden hacer con lo que hay: manda el insumo mas escaso.
                int capacidad = recetas[item.IdMenu]
                    .Min(ing => (int)Math.Floor(disponible[ing.IdProducto] / ing.Cantidad));
                sb.AppendLine("- " + string.Format(Idioma.Texto("Stock_PuedeLinea"), item.Platillo, capacidad));
            }
            return sb.ToString().TrimEnd();
        }
        #endregion

        #region AUXILIARES
        // No se pueden pedir platillos dados de baja o marcados como no disponibles
        // (los que el pedido ya tenia antes de esta modificacion se respetan).
        private void VerificarDisponibles(SqlConnection cn, SqlTransaction tx, List<ItemPedido> items, List<ItemPedido> anteriores)
        {
            HashSet<int> previos = new HashSet<int>(anteriores.Select(i => i.IdMenu));
            // Solo se verifican los platillos nuevos: si el pedido ya tenia uno, no se lo rechaza aunque despues lo hayan marcado como no disponible.
            List<int> aVerificar = items.Select(i => i.IdMenu).Where(id => !previos.Contains(id)).Distinct().ToList();
            if (aVerificar.Count == 0)
            {
                return;
            }

            List<string> noDisponibles = new List<string>();
            using (SqlCommand cmd = new SqlCommand(
                "SELECT Nombre FROM Menu WHERE IdMenu IN (" + string.Join(",", aVerificar) + ") AND (Activo = 0 OR Disponible = 0)", cn, tx))
            using (SqlDataReader dr = cmd.ExecuteReader())
            {
                while (dr.Read())
                {
                    noDisponibles.Add(dr.GetString(0));
                }
            }
            if (noDisponibles.Count > 0)
            {
                throw new InvalidOperationException(string.Format(Idioma.Texto("Msg_PlatilloNoDisponible"), string.Join(", ", noDisponibles)));
            }
        }

        private static bool EsCancelado(string estado)
        {
            return string.Equals((estado ?? "").Trim(), EstadoCancelado, StringComparison.OrdinalIgnoreCase);
        }

        private static void ValidarItems(List<ItemPedido> items)
        {
            if (items == null || items.Count == 0)
            {
                throw new ArgumentException("El pedido debe tener al menos un platillo.");
            }
            if (items.Any(i => i.Cantidad <= 0))
            {
                throw new ArgumentException("La cantidad de cada platillo debe ser mayor a 0.");
            }
        }

        private void ParametrosPedido(SqlCommand cmd, List<ItemPedido> items)
        {
            cmd.Parameters.AddWithValue("@cli", NombreCliente ?? "");
            cmd.Parameters.AddWithValue("@menu", items[0].IdMenu);
            cmd.Parameters.AddWithValue("@cant", items.Sum(i => i.Cantidad));
            cmd.Parameters.AddWithValue("@pago", FormaPago ?? "");
            cmd.Parameters.AddWithValue("@instr", InstruccionesEspeciales ?? "");
            cmd.Parameters.AddWithValue("@estado", Estado ?? "Pendiente");
            cmd.Parameters.AddWithValue("@canal", string.IsNullOrWhiteSpace(Canal) ? "Salon" : Canal);
            cmd.Parameters.AddWithValue("@mesa", string.IsNullOrWhiteSpace(Mesa) ? (object)DBNull.Value : Mesa.Trim());
            SqlParameter total = cmd.Parameters.Add("@total", SqlDbType.Decimal);
            total.Precision = 18;
            total.Scale = 2;
            total.Value = items.Sum(i => i.Subtotal);
            cmd.Parameters.Add("@fecha", SqlDbType.Date).Value = Fecha.Date;
        }

        private void GuardarDetalle(SqlConnection cn, SqlTransaction tx, int idPedido, List<ItemPedido> items)
        {
            using (SqlCommand cmd = new SqlCommand("DELETE FROM PedidoDetalle WHERE IdPedido = @p", cn, tx))
            {
                cmd.Parameters.AddWithValue("@p", idPedido);
                cmd.ExecuteNonQuery();
            }

            foreach (ItemPedido item in items)
            {
                using (SqlCommand cmd = new SqlCommand(
                    "INSERT INTO PedidoDetalle (IdPedido, IdMenu, Cantidad, PrecioUnitario) VALUES (@p, @m, @c, @pu)", cn, tx))
                {
                    cmd.Parameters.AddWithValue("@p", idPedido);
                    cmd.Parameters.AddWithValue("@m", item.IdMenu);
                    cmd.Parameters.AddWithValue("@c", item.Cantidad);
                    SqlParameter pu = cmd.Parameters.Add("@pu", SqlDbType.Decimal);
                    pu.Precision = 18;
                    pu.Scale = 2;
                    pu.Value = item.PrecioUnitario;
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private string LeerEstado(SqlConnection cn, SqlTransaction tx, int idPedido)
        {
            using (SqlCommand cmd = new SqlCommand("SELECT Estado FROM Pedido WITH (UPDLOCK) WHERE IdPedido = @p", cn, tx))
            {
                cmd.Parameters.AddWithValue("@p", idPedido);
                object r = cmd.ExecuteScalar();
                if (r == null)
                {
                    throw new InvalidOperationException("El pedido " + idPedido + " no existe.");
                }
                return Convert.ToString(r);
            }
        }

        private List<ItemPedido> LeerDetalle(SqlConnection cn, SqlTransaction tx, int idPedido)
        {
            List<ItemPedido> lista = new List<ItemPedido>();
            using (SqlCommand cmd = new SqlCommand(
                "SELECT d.IdMenu, m.Nombre, d.Cantidad, d.PrecioUnitario FROM PedidoDetalle d " +
                "INNER JOIN Menu m ON m.IdMenu = d.IdMenu WHERE d.IdPedido = @p ORDER BY d.IdDetalle", cn, tx))
            {
                cmd.Parameters.AddWithValue("@p", idPedido);
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        lista.Add(new ItemPedido
                        {
                            IdMenu = dr.GetInt32(0),
                            Platillo = dr.GetString(1),
                            Cantidad = dr.GetInt32(2),
                            PrecioUnitario = dr.GetDecimal(3)
                        });
                    }
                }
            }
            return lista;
        }

        private static bool MismoDetalle(List<ItemPedido> a, List<ItemPedido> b)
        {
            Func<List<ItemPedido>, Dictionary<int, int>> agrupar = l => l
                .GroupBy(i => i.IdMenu).ToDictionary(g => g.Key, g => g.Sum(i => i.Cantidad));
            Dictionary<int, int> x = agrupar(a);
            Dictionary<int, int> y = agrupar(b);
            return x.Count == y.Count && x.All(kv => y.ContainsKey(kv.Key) && y[kv.Key] == kv.Value);
        }

        private List<Ingrediente> LeerReceta(SqlConnection cn, SqlTransaction tx, int idMenu)
        {
            List<Ingrediente> lista = new List<Ingrediente>();
            using (SqlCommand cmd = new SqlCommand("SELECT IdProducto, Cantidad FROM RecetaIngrediente WHERE IdMenu = @m", cn, tx))
            {
                cmd.Parameters.AddWithValue("@m", idMenu);
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        lista.Add(new Ingrediente { IdProducto = dr.GetInt32(0), Cantidad = dr.GetDecimal(1) });
                    }
                }
            }
            return lista;
        }

        // Lotes utilizables (no vencidos) de un producto, los que vencen antes primero.
        private List<Lote> LeerLotes(SqlConnection cn, SqlTransaction tx, int idProducto)
        {
            List<Lote> lista = new List<Lote>();
            using (SqlCommand cmd = new SqlCommand(
                // UPDLOCK + ROWLOCK: dentro de la transaccion reservan los lotes que se leen, asi un pedido simultaneo espera
                // en lugar de gastar el mismo stock.
                "SELECT IdStock, NumeroLote, Cantidad FROM Stock WITH (UPDLOCK, ROWLOCK) " +
                "WHERE IdProducto = @p AND Cantidad > 0 AND (FechaVencimiento IS NULL OR FechaVencimiento >= CAST(GETDATE() AS DATE)) " +
                // Orden FIFO: primero los que vencen antes; los que no tienen vencimiento van al final.
                "ORDER BY CASE WHEN FechaVencimiento IS NULL THEN 1 ELSE 0 END, FechaVencimiento, IdStock", cn, tx))
            {
                cmd.Parameters.AddWithValue("@p", idProducto);
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        lista.Add(new Lote { IdStock = dr.GetInt32(0), NumeroLote = dr.IsDBNull(1) ? "" : dr.GetString(1), Cantidad = dr.GetInt32(2) });
                    }
                }
            }
            return lista;
        }

        private string NombreProducto(SqlConnection cn, SqlTransaction tx, int idProducto)
        {
            using (SqlCommand cmd = new SqlCommand("SELECT Nombre + CASE WHEN ISNULL(Medida, '') = '' THEN '' ELSE ' (' + Medida + ')' END FROM Producto WHERE IdProducto = @p", cn, tx))
            {
                cmd.Parameters.AddWithValue("@p", idProducto);
                object r = cmd.ExecuteScalar();
                return r == null ? "#" + idProducto : Convert.ToString(r);
            }
        }
        #endregion
    }
}
