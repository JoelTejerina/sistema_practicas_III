/*==============================================================================
  datos_demo.sql
  Datos de ejemplo para poder probar todos los formularios sin cargar todo a mano:
  personal y usuarios, proveedores, productos, stock, carta, pedidos y bitacora.
  (Provincias, partidos y localidades ya vienen en datos_iniciales.sql.)
  - OPCIONAL: solo se ejecuta con  Instalar-BaseDeDatos.bat -Demo
  - Idempotente: solo inserta lo que falta (por clave).
  - Los usuarios de ejemplo tienen contrasena "123" (hash SHA-256).
==============================================================================*/
SET NOCOUNT ON;
GO

/* ---------------------------------------------------------------- Personal */
SET IDENTITY_INSERT dbo.Personal ON;
MERGE dbo.Personal AS t
USING (VALUES
    (3, N'Gomez',   N'Laura',   1, 30111222, N'27-30111222-4', N'1155550003', N'lgomez@demo.com',   N'Av. Mitre',   1250, N'',  N'',  1, 3),
    (4, N'Fernandez', N'Marcos', 1, 28333444, N'20-28333444-7', N'1155550004', N'mfernandez@demo.com', N'Belgrano',  480,  N'2', N'B', 2, 4),
    (5, N'Rios',    N'Sofia',   1, 33555666, N'27-33555666-1', N'1155550005', N'srios@demo.com',    N'San Martin',  75,   N'',  N'',  3, 4),
    (6, N'Suarez',  N'Diego',   1, 29777888, N'20-29777888-5', N'1155550006', N'dsuarez@demo.com',  N'Alsina',      910,  N'',  N'',  5, 2),
    (7, N'Alvarez', N'Marta',   1, 25999000, N'27-25999000-9', N'1155550007', N'malvarez@demo.com', N'Italia',      320,  N'1', N'A', 6, 5),
    (8, N'Torres',  N'Lucia',   1, 31222333, N'27-31222333-6', N'1155550008', N'ltorres@demo.com',  N'Rivadavia',   640,  N'',  N'',  2, 3)
) AS s (IdPersona, Apellido, Nombres, IdTDoc, NroDoc, CuitCuil, Telefono, Correo, Calle, Nro, Piso, Dto, IdLocalidad, IdCargo)
   ON t.IdPersona = s.IdPersona
WHEN NOT MATCHED BY TARGET THEN
    INSERT (IdPersona, Apellido, Nombres, IdTDoc, NroDoc, CuitCuil, Telefono, Correo, Calle, Nro, Piso, Dto, IdLocalidad, IdCargo)
    VALUES (s.IdPersona, s.Apellido, s.Nombres, s.IdTDoc, s.NroDoc, s.CuitCuil, s.Telefono, s.Correo, s.Calle, s.Nro, s.Piso, s.Dto, s.IdLocalidad, s.IdCargo);
SET IDENTITY_INSERT dbo.Personal OFF;
GO

/* ------------------------------------------------- Usuarios y sus roles */
SET IDENTITY_INSERT dbo.Usuarios ON;
MERGE dbo.Usuarios AS t
USING (VALUES
    (4, N'lgomez',     N'a665a45920422f9d417e4867efdc4fb8a04a1f3fff1fa07e998e86f7f7a27ae3', 3, CAST('2024-03-01' AS DATE), 0),
    (5, N'mfernandez', N'a665a45920422f9d417e4867efdc4fb8a04a1f3fff1fa07e998e86f7f7a27ae3', 4, CAST('2024-03-01' AS DATE), 0),
    (6, N'srios',      N'a665a45920422f9d417e4867efdc4fb8a04a1f3fff1fa07e998e86f7f7a27ae3', 5, CAST('2024-03-01' AS DATE), 0),
    (7, N'subgerente', N'a665a45920422f9d417e4867efdc4fb8a04a1f3fff1fa07e998e86f7f7a27ae3', 6, CAST('2024-03-01' AS DATE), 0),
    (8, N'propietario', N'a665a45920422f9d417e4867efdc4fb8a04a1f3fff1fa07e998e86f7f7a27ae3', 7, CAST('2024-03-01' AS DATE), 0),
    (9, N'jefesalon',  N'a665a45920422f9d417e4867efdc4fb8a04a1f3fff1fa07e998e86f7f7a27ae3', 8, CAST('2024-03-01' AS DATE), 0)
) AS s (IdUsuario, Usuario, [Password], IdPersona, FechaAlta, CambiaCada)
   ON t.IdUsuario = s.IdUsuario
WHEN NOT MATCHED BY TARGET THEN
    INSERT (IdUsuario, Usuario, [Password], IdPersona, FechaAlta, CambiaCada)
    VALUES (s.IdUsuario, s.Usuario, s.[Password], s.IdPersona, s.FechaAlta, s.CambiaCada);
SET IDENTITY_INSERT dbo.Usuarios OFF;
GO

MERGE dbo.UsuariosGrupos AS t
USING (VALUES
    (4, 3),   -- lgomez     -> Cajero
    (5, 4),   -- mfernandez -> Jefe de Cocina
    (6, 6),   -- srios      -> Recursos Humanos
    (7, 2),   -- subgerente  -> Subgerente
    (8, 5),   -- propietario -> Propietario
    (9, 7)    -- jefesalon   -> Jefe de Salon
) AS s (IdUsuario, IdGrupo)
   ON t.IdUsuario = s.IdUsuario
WHEN NOT MATCHED BY TARGET THEN
    INSERT (IdUsuario, IdGrupo) VALUES (s.IdUsuario, s.IdGrupo);
GO

/* ------------------------------------------------------------- Proveedores */
SET IDENTITY_INSERT dbo.Proveedor ON;
MERGE dbo.Proveedor AS t
USING (VALUES
    (1, N'Distribuidora Sur', N'Distribuidora Sur SA', N'Bebidas',  N'20-11111111-1', N'CAI-1001', N'Av. Italia 123', N'ventas@dsur.com',     N'1144440001'),
    (2, N'Carnes Premium',    N'Carnes Premium SRL',    N'Carnicos', N'20-22222222-2', N'CAI-1002', N'Espana 456',     N'compras@cpremium.com', N'1144440002'),
    (3, N'Verduleria Central', N'Verduleria Central SA', N'Verduras', N'30-33333333-3', N'CAI-1003', N'Mercado 789',    N'pedidos@vcentral.com', N'1144440003')
) AS s (IdProveedor, Nombre, RazonSocial, Categoria, CUIT, CertificadoAfip, Direccion, Email, Telefono)
   ON t.IdProveedor = s.IdProveedor
WHEN NOT MATCHED BY TARGET THEN
    INSERT (IdProveedor, Nombre, RazonSocial, Categoria, CUIT, CertificadoAfip, Direccion, Email, Telefono)
    VALUES (s.IdProveedor, s.Nombre, s.RazonSocial, s.Categoria, s.CUIT, s.CertificadoAfip, s.Direccion, s.Email, s.Telefono);
SET IDENTITY_INSERT dbo.Proveedor OFF;
GO

/* ------------------------------------------------------------- Productos */
SET IDENTITY_INSERT dbo.Producto ON;
MERGE dbo.Producto AS t
USING (VALUES
    (1, N'Gaseosa Cola 2.25L', N'Bebida gaseosa',     N'ColaCo', N'Bebidas',  N'ml', 1),
    (2, N'Lomo de res',        N'Corte premium',      N'CP',     N'Carnicos', N'gr', 2),
    (3, N'Papa blanca',        N'Papa para guarnicion', N'Campo',  N'Verduras', N'kg', 3)
) AS s (IdProducto, Nombre, Descripcion, Marca, Categoria, Medida, IdProveedor)
   ON t.IdProducto = s.IdProducto
WHEN NOT MATCHED BY TARGET THEN
    INSERT (IdProducto, Nombre, Descripcion, Marca, Categoria, Medida, IdProveedor)
    VALUES (s.IdProducto, s.Nombre, s.Descripcion, s.Marca, s.Categoria, s.Medida, s.IdProveedor);
SET IDENTITY_INSERT dbo.Producto OFF;
GO

/* ----------------------------------------------------------------- Stock */
SET IDENTITY_INSERT dbo.Stock ON;
MERGE dbo.Stock AS t
USING (VALUES
    (1, 1, N'L-0001', 120, DATEADD(MONTH, 6, CAST(GETDATE() AS DATE)), CAST(1500 AS DECIMAL(18,2))),
    (2, 2, N'L-0002',  40, DATEADD(MONTH, 1, CAST(GETDATE() AS DATE)), CAST(9000 AS DECIMAL(18,2))),
    (3, 3, N'L-0003',  80, DATEADD(DAY, 20,  CAST(GETDATE() AS DATE)), CAST(800 AS DECIMAL(18,2)))
) AS s (IdStock, IdProducto, NumeroLote, Cantidad, FechaVencimiento, Precio)
   ON t.IdStock = s.IdStock
WHEN NOT MATCHED BY TARGET THEN
    INSERT (IdStock, IdProducto, NumeroLote, Cantidad, FechaVencimiento, Precio)
    VALUES (s.IdStock, s.IdProducto, s.NumeroLote, s.Cantidad, s.FechaVencimiento, s.Precio);
SET IDENTITY_INSERT dbo.Stock OFF;
GO

/* ------------------------------------------------------------------ Menu */
SET IDENTITY_INSERT dbo.Menu ON;
MERGE dbo.Menu AS t
USING (VALUES
    (1, N'Bife con guarnicion', N'Lomo a la plancha',     N'Plato principal', N'Lomo, papas',      CAST(12000 AS DECIMAL(18,2)), N'Argentina', N'Todo el ano', N'Alta',  25, N'General',    2),
    (2, N'Papas fritas',        N'Porcion de papas fritas', N'Guarnicion',    N'Papa, aceite',     CAST(4500 AS DECIMAL(18,2)),  N'Argentina', N'Todo el ano', N'Media', 15, N'General',    3),
    (3, N'Gaseosa cola',        N'Botella 2.25L para compartir', N'Bebida',   N'Gaseosa',          CAST(3000 AS DECIMAL(18,2)),  N'Argentina', N'Verano',      N'Alta',   2, N'Cumpleanos', 1)
) AS s (IdMenu, Nombre, Descripcion, Categoria, Ingredientes, Precio, Region, Temporada, Popularidad, TiempoPreparacion, TipoEvento, IdStock)
   ON t.IdMenu = s.IdMenu
WHEN NOT MATCHED BY TARGET THEN
    INSERT (IdMenu, Nombre, Descripcion, Categoria, Ingredientes, Precio, Region, Temporada, Popularidad, TiempoPreparacion, TipoEvento, IdStock)
    VALUES (s.IdMenu, s.Nombre, s.Descripcion, s.Categoria, s.Ingredientes, s.Precio, s.Region, s.Temporada, s.Popularidad, s.TiempoPreparacion, s.TipoEvento, s.IdStock);
SET IDENTITY_INSERT dbo.Menu OFF;
GO

/* ---------------------------------------------------------------- Pedidos
   Fecha relativa a hoy, para que "Pedidos del dia" de Estadisticas tenga datos. */
SET IDENTITY_INSERT dbo.Pedido ON;
MERGE dbo.Pedido AS t
USING (VALUES
    (1, N'Carlos Perez',  1, 2, N'Efectivo',      N'Carne bien cocida', N'Pendiente', CAST(24000 AS DECIMAL(18,2)), CAST(GETDATE() AS DATE)),
    (2, N'Ana Lopez',     2, 3, N'Tarjeta',       N'',                  N'En curso',  CAST(13500 AS DECIMAL(18,2)), CAST(GETDATE() AS DATE)),
    (3, N'Mesa 5',        3, 1, N'Transferencia', N'Sin hielo',         N'Cobrado',   CAST(3000 AS DECIMAL(18,2)),  DATEADD(DAY, -1, CAST(GETDATE() AS DATE)))
) AS s (IdPedido, NombreCliente, IdMenu, Cantidad, FormaPago, InstruccionesEspeciales, Estado, PrecioTotal, Fecha)
   ON t.IdPedido = s.IdPedido
WHEN NOT MATCHED BY TARGET THEN
    INSERT (IdPedido, NombreCliente, IdMenu, Cantidad, FormaPago, InstruccionesEspeciales, Estado, PrecioTotal, Fecha)
    VALUES (s.IdPedido, s.NombreCliente, s.IdMenu, s.Cantidad, s.FormaPago, s.InstruccionesEspeciales, s.Estado, s.PrecioTotal, s.Fecha);
SET IDENTITY_INSERT dbo.Pedido OFF;
GO

/* --------------------------------------------------------------- Bitacora */
INSERT INTO dbo.Bitacora (Fecha, Hora, IdUsuario, Usuario, Evento, Detalle, Origen)
SELECT v.Fecha, v.Hora, v.IdUsuario, v.Usuario, v.Evento, v.Detalle, N'Demo'
FROM (VALUES
    (DATEADD(DAY, -2, CAST(GETDATE() AS DATE)), CAST('09:15:00' AS TIME(0)), 1, N'Pinos Eduardo',  N'Inicio de sesion', N'Ingreso correcto al sistema'),
    (DATEADD(DAY, -1, CAST(GETDATE() AS DATE)), CAST('13:40:00' AS TIME(0)), 4, N'Gomez Laura',    N'Alta de pedido',   N'Pedido registrado para Mesa 5'),
    (CAST(GETDATE() AS DATE),                   CAST('10:05:00' AS TIME(0)), 3, N'Montecino Hector', N'Cambio de password', N'El usuario modifico su contrasena')
) AS v (Fecha, Hora, IdUsuario, Usuario, Evento, Detalle)
WHERE NOT EXISTS (SELECT 1 FROM dbo.Bitacora b WHERE b.Origen = N'Demo');
GO

/* ------------------------------------------------- Fecha de nacimiento (edad) */
UPDATE dbo.Personal
SET FechaNacimiento = CASE IdPersona
        WHEN 1 THEN CAST('19850312' AS DATE) WHEN 2 THEN CAST('19880720' AS DATE)
        WHEN 3 THEN CAST('19920505' AS DATE) WHEN 4 THEN CAST('19901117' AS DATE)
        WHEN 5 THEN CAST('19950228' AS DATE) WHEN 6 THEN CAST('19830915' AS DATE)
        WHEN 7 THEN CAST('19790401' AS DATE) WHEN 8 THEN CAST('19940123' AS DATE) END
WHERE FechaNacimiento IS NULL AND IdPersona BETWEEN 1 AND 8;
GO

/* ----------------------------------------------- Stock minimo/maximo y alertas */
UPDATE dbo.Stock SET StockMinimo = 20, StockMaximo = 200 WHERE IdStock = 1 AND StockMinimo = 0 AND StockMaximo = 0;
UPDATE dbo.Stock SET StockMinimo = 10, StockMaximo = 100 WHERE IdStock = 2 AND StockMinimo = 0 AND StockMaximo = 0;
UPDATE dbo.Stock SET StockMinimo = 15, StockMaximo = 150 WHERE IdStock = 3 AND StockMinimo = 0 AND StockMaximo = 0;
GO

/* ------------------------------------------------------------------ Recetas
   Cantidad = unidades del ingrediente por cada platillo (en la medida del producto). */
MERGE dbo.RecetaIngrediente AS t
USING (VALUES
    (1, 2, CAST(1 AS DECIMAL(18,3)), 1),   -- Bife con guarnicion: 1 Lomo (principal)
    (1, 3, CAST(2 AS DECIMAL(18,3)), 0),   --                      2 Papa
    (2, 3, CAST(3 AS DECIMAL(18,3)), 1),   -- Papas fritas:        3 Papa (principal)
    (3, 1, CAST(1 AS DECIMAL(18,3)), 1)    -- Gaseosa cola:        1 Gaseosa (principal)
) AS s (IdMenu, IdProducto, Cantidad, EsPrincipal)
   ON t.IdMenu = s.IdMenu AND t.IdProducto = s.IdProducto
WHEN NOT MATCHED BY TARGET THEN
    INSERT (IdMenu, IdProducto, Cantidad, EsPrincipal) VALUES (s.IdMenu, s.IdProducto, s.Cantidad, s.EsPrincipal);
GO

/* ------------------------------------------------ Detalle de los pedidos demo */
UPDATE dbo.Pedido SET IdUsuario = 3 WHERE IdUsuario IS NULL;
GO
INSERT INTO dbo.PedidoDetalle (IdPedido, IdMenu, Cantidad, PrecioUnitario)
SELECT p.IdPedido, p.IdMenu, p.Cantidad,
       CASE WHEN p.Cantidad > 0 THEN CAST(p.PrecioTotal / p.Cantidad AS DECIMAL(18,2)) ELSE p.PrecioTotal END
FROM dbo.Pedido p
WHERE p.Cantidad > 0
  AND NOT EXISTS (SELECT 1 FROM dbo.PedidoDetalle d WHERE d.IdPedido = p.IdPedido);
GO

/* ------------------------------------------- Historial inicial de movimientos */
INSERT INTO dbo.MovimientoStock (Tipo, IdStock, IdProducto, NumeroLote, Cantidad, CantidadResultante, IdUsuario, Detalle)
SELECT N'Ingreso', s.IdStock, s.IdProducto, s.NumeroLote, s.Cantidad, s.Cantidad, 1, N'Carga inicial (demo)'
FROM dbo.Stock s
WHERE NOT EXISTS (SELECT 1 FROM dbo.MovimientoStock m WHERE m.IdStock = s.IdStock);
GO
