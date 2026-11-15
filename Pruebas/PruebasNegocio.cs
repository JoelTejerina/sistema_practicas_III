using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using CapaComun;
using CapaLogicaNegocio;
using CapaLogicaNegocio.Administrador;
using CapaLogicaNegocio.Compras;
using CapaLogicaNegocio.Loguin;
using CapaLogicaNegocio.Ventas;

namespace Pruebas
{
    /// <summary>
    /// Pruebas de la capa de negocio contra una base descartable (la crea Ejecutar-Pruebas.ps1 con los datos demo).
    /// Cada prueba arma sus propios datos (nombres unicos) y no depende del orden de ejecucion.
    /// </summary>
    public static class PruebasNegocio
    {
        // ------------------------------------------------------------ utilitarios
        private static string Cadena()
        {
            return Environment.GetEnvironmentVariable("SISTEMA_DB_CONNECTION");
        }

        private static object Escalar(string sql)
        {
            using (SqlConnection cn = new SqlConnection(Cadena()))
            using (SqlCommand cmd = new SqlCommand(sql, cn))
            {
                cn.Open();
                return cmd.ExecuteScalar();
            }
        }

        private static void Ejecutar(string sql)
        {
            using (SqlConnection cn = new SqlConnection(Cadena()))
            using (SqlCommand cmd = new SqlCommand(sql, cn))
            {
                cn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        private static decimal Num(string sql)
        {
            object o = Escalar(sql);
            return o == null || o == DBNull.Value ? 0m : Convert.ToDecimal(o);
        }

        private static bool Entrar(string usuario, string clave = "123")
        {
            UserCache.Limpiar();
            return new clsLogicaLoguin().LoginUser(usuario, clave).Item1;
        }

        private static void CerrarCajas()
        {
            Ejecutar("UPDATE Caja SET Cierre = SYSDATETIME(), IdUsuarioCierre = IdUsuarioApertura, TotalCobrado = 0, MontoContado = 0 WHERE Cierre IS NULL");
        }

        /// <summary>Un platillo con una receta de un solo insumo (3 unidades por plato) y 100 unidades en stock.</summary>
        private class Fixture
        {
            public int IdMenu, IdProducto;
            public string Sufijo = Guid.NewGuid().ToString("N").Substring(0, 8);

            public Fixture()
            {
                int prov = Convert.ToInt32(Escalar("SELECT TOP 1 IdProveedor FROM Proveedor"));
                IdProducto = Convert.ToInt32(Escalar(
                    "INSERT INTO Producto (Nombre, Descripcion, Marca, Categoria, Medida, IdProveedor, CantidadInicial) " +
                    "VALUES (N'Insumo " + Sufijo + "', N'prueba', N'M', N'Prueba', N'u', " + prov + ", 0); SELECT SCOPE_IDENTITY()"));
                Ejecutar("INSERT INTO Stock (IdProducto, NumeroLote, Cantidad, FechaVencimiento, Precio, StockMinimo, StockMaximo, IdProveedor) " +
                         "VALUES (" + IdProducto + ", N'L-" + Sufijo + "', 100, NULL, 0, 0, 0, " + prov + ")");
                IdMenu = Convert.ToInt32(Escalar(
                    "INSERT INTO Menu (Nombre, Descripcion, Categoria, Precio, TiempoPreparacion) " +
                    "VALUES (N'Plato " + Sufijo + "', N'prueba', N'Prueba', 100, 5); SELECT SCOPE_IDENTITY()"));
                Ejecutar("INSERT INTO RecetaIngrediente (IdMenu, IdProducto, Cantidad, EsPrincipal) VALUES (" + IdMenu + ", " + IdProducto + ", 3, 1)");
            }

            public decimal Stock()
            {
                return Num("SELECT ISNULL(SUM(Cantidad), 0) FROM Stock WHERE IdProducto = " + IdProducto);
            }

            public CN_Pedido Pedido(int cantidad)
            {
                CN_Pedido p = new CN_Pedido
                {
                    NombreCliente = "Cliente " + Sufijo,
                    FormaPago = "Efectivo",
                    Canal = "Barra",
                    Estado = "Pendiente",
                    Fecha = DateTime.Now
                };
                p.Items.Add(new ItemPedido { IdMenu = IdMenu, Platillo = "Plato " + Sufijo, Cantidad = cantidad, PrecioUnitario = 100 });
                return p;
            }
        }

        /// <summary>CUIT con digito verificador correcto y numero aleatorio (para no chocar con datos existentes).</summary>
        private static string CuitNuevo()
        {
            int[] pesos = { 5, 4, 3, 2, 7, 6, 5, 4, 3, 2 };
            string baseCuit = "20" + new Random(Guid.NewGuid().GetHashCode()).Next(10000000, 99999999).ToString();
            int suma = 0;
            for (int i = 0; i < 10; i++) suma += (baseCuit[i] - '0') * pesos[i];
            int d = 11 - suma % 11;
            if (d == 11) d = 0;
            if (d == 10) return CuitNuevo();
            return baseCuit + d;
        }

        private static string EstadoPedido(int id)
        {
            return Convert.ToString(Escalar("SELECT Estado FROM Pedido WHERE IdPedido = " + id));
        }

        // ------------------------------------------------------------ casos
        public static List<Caso> Casos()
        {
            List<Caso> c = new List<Caso>();
            Action<string, string, Action> add = (id, nombre, accion) => c.Add(new Caso { Id = id, Nombre = nombre, Accion = accion });

            // ---- Inicio de sesion y seguridad
            add("PU-NEG-001", "Login: usuario y contraseña correctos ingresan y cargan permisos", () =>
            {
                Afirmar.Verdadero(Entrar("Edu"), "Edu/123 deberia ingresar");
                Afirmar.Verdadero(UserCache.IdUsuario > 0, "IdUsuario");
                Afirmar.Verdadero(UserCache.PermisosUsuario.Count > 0, "permisos cargados");
                Afirmar.Igual("Gerente", UserCache.NombreGrupo, "rol del usuario");
            });
            add("PU-NEG-002", "Login: contraseña incorrecta no ingresa", () =>
                Afirmar.Verdadero(!Entrar("Edu", "clave-equivocada"), "no deberia ingresar"));
            add("PU-NEG-003", "Login: usuario inexistente no ingresa", () =>
            {
                Afirmar.Verdadero(!Entrar("no_existe_" + Guid.NewGuid().ToString("N").Substring(0, 6)), "no deberia ingresar");
                UserCache.Limpiar();
            });
            add("PU-NEG-004", "Cada rol de prueba ingresa con su perfil (5 roles)", () =>
            {
                string[,] roles = { { "Edu", "Gerente" }, { "titor", "Cajero" }, { "mfernandez", "Jefe de Cocina" },
                                    { "propietario", "Propietario" }, { "jefesalon", "Jefe de Salon" } };
                for (int i = 0; i < roles.GetLength(0); i++)
                {
                    Afirmar.Verdadero(Entrar(roles[i, 0]), "login " + roles[i, 0]);
                    Afirmar.Igual(roles[i, 1], UserCache.NombreGrupo, "rol de " + roles[i, 0]);
                }
            });
            add("PU-NEG-005", "Dígito verificador: la tabla de usuarios es íntegra y se detecta una alteración externa", () =>
            {
                CN_Seguridad s = new CN_Seguridad();
                Ejecutar("EXEC usp_RecalcularDigitosUsuarios");
                Afirmar.Verdadero(s.IntegridadValida(), "deberia ser integra");
                string original = Convert.ToString(Escalar("SELECT [Password] FROM Usuarios WHERE Usuario = N'titor'"));
                try
                {
                    Ejecutar("UPDATE Usuarios SET [Password] = N'alterada-por-fuera' WHERE Usuario = N'titor'");   // alteracion "por fuera del sistema"
                    Afirmar.Verdadero(!s.IntegridadValida(), "deberia detectar la alteracion");
                }
                finally
                {
                    Ejecutar("UPDATE Usuarios SET [Password] = N'" + original + "' WHERE Usuario = N'titor'");
                    Ejecutar("EXEC usp_RecalcularDigitosUsuarios");
                }
                Afirmar.Verdadero(s.IntegridadValida(), "deberia volver a ser integra");
            });
            add("PU-NEG-006", "Bloqueo de terminal: se bloquea y se libera", () =>
            {
                CN_Seguridad s = new CN_Seguridad();
                try
                {
                    s.BloquearTerminalPorIntegridad();
                    Afirmar.Verdadero(s.TerminalBloqueadaHasta().HasValue, "deberia estar bloqueada");
                }
                finally { s.LiberarTerminal(); }
                Afirmar.Verdadero(!s.TerminalBloqueadaHasta().HasValue, "deberia estar libre");
            });
            add("PU-NEG-007", "Sesión única: un usuario con la sesión abierta en otra terminal no puede abrir otra", () =>
            {
                Afirmar.Verdadero(Entrar("titor"), "login");
                int id = UserCache.IdUsuario;
                Ejecutar("DELETE FROM SesionActiva WHERE IdUsuario = " + id);
                Ejecutar("INSERT INTO SesionActiva (IdUsuario, Terminal, IdProceso, Inicio, UltimaActividad) VALUES (" + id +
                         ", N'OTRA-PC', 1, SYSDATETIME(), SYSDATETIME())");
                try
                {
                    string ajena;
                    Afirmar.Verdadero(!new CN_Seguridad().AbrirSesion(id, out ajena), "no deberia abrir");
                    Afirmar.Igual("OTRA-PC", ajena, "terminal de la otra sesion");
                }
                finally { Ejecutar("DELETE FROM SesionActiva WHERE IdUsuario = " + id); }
            });

            // ---- Usuarios
            add("PU-NEG-008", "Usuarios: nadie puede darse de baja a sí mismo", () =>
            {
                Afirmar.Verdadero(Entrar("Edu"), "login");
                Afirmar.Lanza<InvalidOperationException>(() => new CN_GestionUsuarios().DarDeBaja(UserCache.IdUsuario), "baja propia");
            });
            add("PU-NEG-009", "Usuarios: restablecer la contraseña devuelve una temporal que permite ingresar y obliga a cambiarla", () =>
            {
                Afirmar.Verdadero(Entrar("Edu"), "login gerente");
                int idCajero = Convert.ToInt32(Escalar("SELECT IdUsuario FROM Usuarios WHERE Usuario = N'lgomez'"));
                string temporal = new CN_GestionUsuarios().RestablecerPassword(idCajero);
                try
                {
                    Afirmar.Igual(10, temporal.Length, "largo de la temporal");
                    Afirmar.Verdadero(Entrar("lgomez", temporal), "ingreso con la temporal");
                    Afirmar.Verdadero(UserCache.DebeCambiarPassword, "debe cambiarla");
                    Afirmar.Verdadero(!Entrar("lgomez", "123"), "la clave anterior ya no sirve");
                }
                finally
                {
                    Ejecutar("UPDATE Usuarios SET Password = N'" + clsSeguridad.SHA256("123") + "', DebeCambiarPassword = 0 WHERE IdUsuario = " + idCajero);
                    Ejecutar("EXEC usp_RecalcularDigitosUsuarios");
                }
            });
            add("PU-NEG-010", "Usuarios: dar de baja y reactivar un usuario", () =>
            {
                Afirmar.Verdadero(Entrar("Edu"), "login gerente");
                int id = Convert.ToInt32(Escalar("SELECT IdUsuario FROM Usuarios WHERE Usuario = N'jefesalon'"));
                CN_GestionUsuarios g = new CN_GestionUsuarios();
                try
                {
                    g.DarDeBaja(id);
                    Afirmar.Verdadero(Convert.ToString(Escalar("SELECT CAST(FechaBaja AS NVARCHAR(30)) FROM Usuarios WHERE IdUsuario = " + id)) != "", "FechaBaja cargada");
                    Afirmar.Verdadero(new CN_Seguridad().IntegridadValida(), "el dígito verificador sigue válido tras la baja");
                }
                finally { g.Reactivar(id); }
                Afirmar.Verdadero(Escalar("SELECT FechaBaja FROM Usuarios WHERE IdUsuario = " + id) == DBNull.Value, "reactivado");
            });

            // ---- Compras
            add("PU-NEG-011", "Proveedores: el CUIT inválido se rechaza", () =>
            {
                CN_Proveedor p = new CN_Proveedor
                {
                    Nombre = "Prov " + Guid.NewGuid().ToString("N").Substring(0, 6), RazonSocial = "SA", Categoria = "Prueba",
                    CUIT = "20-12345678-5", Direccion = "Calle 1", Email = "a@b.com", Telefono = "1122334455"
                };
                Afirmar.Lanza<InvalidOperationException>(() => p.InsertarProveedor(), "CUIT con digito incorrecto");
            });
            add("PU-NEG-012", "Proveedores: alta válida y rechazo del CUIT duplicado", () =>
            {
                string n = Guid.NewGuid().ToString("N").Substring(0, 6);
                string cuit = CuitNuevo();
                Func<string, CN_Proveedor> nuevo = nombre => new CN_Proveedor
                {
                    Nombre = nombre, RazonSocial = "SA", Categoria = "Prueba", CUIT = cuit,
                    Direccion = "Calle 1", Email = "a@b.com", Telefono = "1122334455"
                };
                nuevo("Prov A " + n).InsertarProveedor();
                Afirmar.Lanza<InvalidOperationException>(() => nuevo("Prov B " + n).InsertarProveedor(), "CUIT repetido");
            });
            add("PU-NEG-013", "Productos: no se repite el mismo producto", () =>
            {
                int prov = Convert.ToInt32(Escalar("SELECT TOP 1 IdProveedor FROM Proveedor"));
                string n = Guid.NewGuid().ToString("N").Substring(0, 6);
                Func<CN_Producto> nuevo = () => new CN_Producto
                {
                    Nombre = "Harina " + n, Descripcion = "prueba", Marca = "Marca", Categoria = "Prueba", Medida = "kg",
                    IdProveedor = prov, CantidadInicial = 5
                };
                nuevo().InsertarProducto();
                Afirmar.Lanza<InvalidOperationException>(() => nuevo().InsertarProducto(), "producto duplicado");
            });
            add("PU-NEG-014", "Productos: con cantidad inicial se crea el lote y el movimiento de ingreso", () =>
            {
                int prov = Convert.ToInt32(Escalar("SELECT TOP 1 IdProveedor FROM Proveedor"));
                string n = "Azucar " + Guid.NewGuid().ToString("N").Substring(0, 6);
                new CN_Producto { Nombre = n, Descripcion = "prueba", Marca = "Marca", Categoria = "Prueba", Medida = "kg", IdProveedor = prov, CantidadInicial = 8 }.InsertarProducto();
                int id = Convert.ToInt32(Escalar("SELECT IdProducto FROM Producto WHERE Nombre = N'" + n + "'"));
                Afirmar.Igual(8m, Num("SELECT SUM(Cantidad) FROM Stock WHERE IdProducto = " + id), "stock del lote inicial");
                Afirmar.Igual(1, Convert.ToInt32(Escalar("SELECT COUNT(*) FROM MovimientoStock WHERE IdProducto = " + id + " AND Tipo = N'Ingreso'")), "movimiento de ingreso");
            });

            // ---- Pedidos y stock
            add("PU-NEG-015", "Pedido: al guardar descuenta los insumos de la receta", () =>
            {
                Afirmar.Verdadero(Entrar("titor"), "login");
                Fixture f = new Fixture();
                int id = f.Pedido(2).InsertarPedido();
                Afirmar.Verdadero(id > 0, "idPedido");
                Afirmar.Igual(94m, f.Stock(), "100 - 2 platos x 3");
                Afirmar.Igual("Pendiente", EstadoPedido(id), "estado");
            });
            add("PU-NEG-016", "Pedido: sin stock suficiente lanza StockInsuficienteException y no guarda nada", () =>
            {
                Afirmar.Verdadero(Entrar("titor"), "login");
                Fixture f = new Fixture();
                int antes = Convert.ToInt32(Escalar("SELECT COUNT(*) FROM Pedido"));
                Afirmar.Lanza<StockInsuficienteException>(() => f.Pedido(40).InsertarPedido(), "40 platos x 3 = 120 > 100");
                Afirmar.Igual(100m, f.Stock(), "el stock no cambia");
                Afirmar.Igual(antes, Convert.ToInt32(Escalar("SELECT COUNT(*) FROM Pedido")), "no se crea el pedido");
            });
            add("PU-NEG-017", "Pedido: cancelar un pendiente devuelve los insumos", () =>
            {
                Afirmar.Verdadero(Entrar("titor"), "login");
                Fixture f = new Fixture();
                int id = f.Pedido(5).InsertarPedido();
                Afirmar.Igual(85m, f.Stock(), "tras el pedido");
                new CN_Pedido().CancelarPedido(id);
                Afirmar.Igual("Cancelado", EstadoPedido(id), "estado");
                Afirmar.Igual(100m, f.Stock(), "tras cancelar");
            });
            add("PU-NEG-018", "Pedido: solo se cancela desde la pantalla un pedido Pendiente", () =>
            {
                Afirmar.Verdadero(Entrar("titor"), "login");
                Fixture f = new Fixture();
                int id = f.Pedido(1).InsertarPedido();
                new CN_Pedido().CambiarEstado(id, "En curso");
                Afirmar.Lanza<InvalidOperationException>(() => new CN_Pedido().CancelarPedido(id), "cancelar En curso");
            });
            add("PU-NEG-019", "Pedido: en Salón la mesa es obligatoria", () =>
            {
                Afirmar.Verdadero(Entrar("titor"), "login");
                Fixture f = new Fixture();
                CN_Pedido p = f.Pedido(1);
                p.Canal = "Salon";
                p.Mesa = "";
                Afirmar.Lanza<InvalidOperationException>(() => p.InsertarPedido(), "salon sin mesa");
                p.Mesa = "12";
                Afirmar.Verdadero(p.InsertarPedido() > 0, "salon con mesa");
            });
            add("PU-NEG-020", "Pedido: un platillo no disponible no se puede pedir", () =>
            {
                Afirmar.Verdadero(Entrar("titor"), "login");
                Fixture f = new Fixture();
                Ejecutar("UPDATE Menu SET Disponible = 0 WHERE IdMenu = " + f.IdMenu);
                Afirmar.Lanza<InvalidOperationException>(() => f.Pedido(1).InsertarPedido(), "platillo no disponible");
                Afirmar.Igual(100m, f.Stock(), "el stock no cambia");
            });

            // ---- Caja y cobro
            add("PU-NEG-021", "Caja: abrir con monto negativo se rechaza y no se abre dos veces", () =>
            {
                Afirmar.Verdadero(Entrar("titor"), "login");
                CerrarCajas();
                CN_Caja caja = new CN_Caja();
                Afirmar.Lanza<InvalidOperationException>(() => caja.Abrir(-1), "monto negativo");
                caja.Abrir(1000);
                Afirmar.Lanza<InvalidOperationException>(() => caja.Abrir(500), "segunda apertura");
                CerrarCajas();
            });
            add("PU-NEG-022", "Caja: no se puede cobrar un pedido sin caja abierta", () =>
            {
                Afirmar.Verdadero(Entrar("titor"), "login");
                Fixture f = new Fixture();
                CN_Pedido p = f.Pedido(1);
                p.IdPedido = p.InsertarPedido();
                CerrarCajas();
                Afirmar.Lanza<InvalidOperationException>(() => p.CobrarPedido(), "cobro sin caja");
                Afirmar.Igual("Pendiente", EstadoPedido(p.IdPedido), "sigue pendiente");
            });
            add("PU-NEG-023", "Caja: cobrar con caja abierta deja el pedido Cobrado y asociado a la caja; no se cobra dos veces", () =>
            {
                Afirmar.Verdadero(Entrar("titor"), "login");
                Fixture f = new Fixture();
                CN_Pedido p = f.Pedido(1);
                p.IdPedido = p.InsertarPedido();
                CerrarCajas();
                int idCaja = new CN_Caja().Abrir(1000);
                p.CobrarPedido();
                Afirmar.Igual("Cobrado", EstadoPedido(p.IdPedido), "estado");
                Afirmar.Igual(idCaja, Convert.ToInt32(Escalar("SELECT IdCaja FROM Pedido WHERE IdPedido = " + p.IdPedido)), "IdCaja del cobro");
                Afirmar.Lanza<InvalidOperationException>(() => p.CobrarPedido(), "segundo cobro");
                CerrarCajas();
            });
            add("PU-NEG-024", "Caja: el efectivo esperado es el monto inicial más lo cobrado en efectivo", () =>
            {
                Afirmar.Verdadero(Entrar("titor"), "login");
                Fixture f = new Fixture();
                CerrarCajas();
                CN_Caja caja = new CN_Caja();
                int idCaja = caja.Abrir(1000);
                CN_Pedido p = f.Pedido(2);          // 2 x 100 = 200, forma de pago Efectivo
                p.IdPedido = p.InsertarPedido();
                p.CobrarPedido();
                Afirmar.Igual(1200m, caja.EfectivoEsperado(idCaja, 1000m), "esperado");
                caja.Cerrar(idCaja, 1150m, "faltan 50");
                Afirmar.Verdadero(caja.Abierta() == null, "la caja quedo cerrada");
                Afirmar.Igual(1150m, Num("SELECT MontoContado FROM Caja WHERE IdCaja = " + idCaja), "monto contado");
            });

            // ---- Idioma por usuario
            add("PU-NEG-025", "Idioma por usuario: la preferencia queda guardada y se aplica en el próximo ingreso", () =>
            {
                Afirmar.Verdadero(Entrar("titor"), "login");
                int id = UserCache.IdUsuario;
                CN_IdiomaUsuario cn = new CN_IdiomaUsuario();
                try
                {
                    cn.GuardarIdioma(id, Idioma.Ingles);
                    Afirmar.Igual("en", Convert.ToString(Escalar("SELECT Idioma FROM Usuarios WHERE IdUsuario = " + id)), "idioma guardado");
                    Idioma.Establecer(Idioma.Espanol);
                    cn.AplicarIdiomaDelUsuario(id);
                    Afirmar.Igual(Idioma.Ingles, Idioma.Actual, "idioma aplicado");
                }
                finally { cn.GuardarIdioma(id, Idioma.Espanol); Idioma.Establecer(Idioma.Espanol); }
            });
            return c;
        }
    }
}
