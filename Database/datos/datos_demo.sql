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
    (1, N'Distribuidora Sur', N'Distribuidora Sur SA', N'Bebidas',  N'30-11111111-8', N'CAI-1001', N'Av. Italia 123', N'ventas@dsur.com',     N'1144440001'),
    (2, N'Carnes Premium',    N'Carnes Premium SRL',    N'Carnicos', N'30-22222222-9', N'CAI-1002', N'Espana 456',     N'compras@cpremium.com', N'1144440002'),
    (3, N'Verduleria Central', N'Verduleria Central SA', N'Verduras', N'30-44444444-0', N'CAI-1003', N'Mercado 789',    N'pedidos@vcentral.com', N'1144440003')
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

/* CUIT de los proveedores de ejemplo: el sistema valida el digito verificador del CUIT.
   (Esto tambien corrige bases que ya tenian los CUIT de ejemplo anteriores.) */
UPDATE dbo.Proveedor SET CUIT = N'30-11111111-8' WHERE CUIT = N'20-11111111-1';
UPDATE dbo.Proveedor SET CUIT = N'30-22222222-9' WHERE CUIT = N'20-22222222-2';
UPDATE dbo.Proveedor SET CUIT = N'30-44444444-0' WHERE CUIT = N'30-33333333-3';
GO

/* Historial de una caja ya cerrada (ayer), para ver el resumen de cierre. */
IF NOT EXISTS (SELECT 1 FROM dbo.Caja)
    INSERT INTO dbo.Caja (Apertura, IdUsuarioApertura, MontoInicial, Cierre, IdUsuarioCierre, TotalCobrado, MontoContado, Observaciones)
    VALUES (DATEADD(DAY, -1, DATEADD(HOUR, 9, CAST(CAST(GETDATE() AS DATE) AS DATETIME2(0)))), 3, 5000,
            DATEADD(DAY, -1, DATEADD(HOUR, 23, CAST(CAST(GETDATE() AS DATE) AS DATETIME2(0)))), 3, 3000, 8000, N'Caja de ejemplo');
GO

/*==============================================================================
  DATOS DEMO AMPLIADOS: mas proveedores, productos, stock (con casos de alerta, vencimiento y
  varios lotes), carta, recetas, pedidos historicos, cajas cerradas y movimientos.
  Idempotente: cada bloque inserta solo lo que falta (por clave).
==============================================================================*/

/* ------------------------------------------------------ Proveedores (CUIT validos) */
SET IDENTITY_INSERT dbo.Proveedor ON;
MERGE dbo.Proveedor AS t USING (VALUES
    (4, N'Lacteos del Valle', N'Lacteos del Valle SA', N'Lacteos', N'30-51234567-7', N'CAI-1004', N'Ruta 3 km 45', N'ventas@lacteosvalle.com', N'1144440004'),
    (5, N'Panificados Don Julio', N'Panificados Don Julio SRL', N'Panaderia', N'30-52345671-3', N'CAI-1005', N'Calle 12 N 840', N'pedidos@donjulio.com', N'1144440005'),
    (6, N'Pescados del Puerto', N'Pescados del Puerto SA', N'Pescados', N'30-53456712-6', N'CAI-1006', N'Dock Sud 15', N'compras@pescadospuerto.com', N'1144440006'),
    (7, N'Almacen Mayorista Oeste', N'Mayorista Oeste SA', N'Almacen', N'30-54567123-5', N'CAI-1007', N'Av. Eva Peron 2300', N'ventas@mayoristaoeste.com', N'1144440007'),
    (8, N'Bebidas del Plata', N'Bebidas del Plata SA', N'Bebidas', N'30-55671234-0', N'CAI-1008', N'Camino Negro 700', N'pedidos@bebidasplata.com', N'1144440008'),
    (9, N'Frutas Frescas Norte', N'Frutas Frescas Norte SRL', N'Frutas', N'30-56712345-2', N'CAI-1009', N'Mercado Central Nave 4', N'ventas@frutasnorte.com', N'1144440009'),
    (10, N'Fiambres La Tradicion', N'La Tradicion Fiambres SA', N'Fiambres', N'30-57123456-0', N'CAI-1010', N'Av. Hipolito Yrigoyen 3100', N'compras@latradicion.com', N'1144440010')
) AS s (IdProveedor, Nombre, RazonSocial, Categoria, CUIT, CertificadoAfip, Direccion, Email, Telefono)
   ON t.IdProveedor = s.IdProveedor
WHEN NOT MATCHED BY TARGET THEN INSERT (IdProveedor, Nombre, RazonSocial, Categoria, CUIT, CertificadoAfip, Direccion, Email, Telefono)
    VALUES (s.IdProveedor, s.Nombre, s.RazonSocial, s.Categoria, s.CUIT, s.CertificadoAfip, s.Direccion, s.Email, s.Telefono);
SET IDENTITY_INSERT dbo.Proveedor OFF;
GO

/* ------------------------------------------------------------------ Productos */
SET IDENTITY_INSERT dbo.Producto ON;
MERGE dbo.Producto AS t USING (VALUES
    (4, N'Harina 000', N'Harina de trigo', N'Pureza', N'Almacen', N'gr', 7),
    (5, N'Aceite de girasol', N'Aceite comestible', N'Natura', N'Almacen', N'ml', 7),
    (6, N'Sal fina', N'Sal de mesa', N'Celusal', N'Almacen', N'gr', 7),
    (7, N'Azucar', N'Azucar comun', N'Ledesma', N'Almacen', N'gr', 7),
    (8, N'Arroz largo fino', N'Arroz', N'Gallo', N'Almacen', N'gr', 7),
    (9, N'Fideos tirabuzon', N'Fideos secos', N'Matarazzo', N'Almacen', N'gr', 7),
    (10, N'Salsa de tomate', N'Pure de tomate', N'Arcor', N'Almacen', N'ml', 7),
    (11, N'Huevos', N'Huevos de campo', N'Granja Sol', N'Huevos', N'unidad', 7),
    (12, N'Leche entera', N'Leche fluida', N'La Serenisima', N'Lacteos', N'ml', 4),
    (13, N'Queso mozzarella', N'Queso para pizza', N'Tregar', N'Lacteos', N'gr', 4),
    (14, N'Manteca', N'Manteca', N'Sancor', N'Lacteos', N'gr', 4),
    (15, N'Crema de leche', N'Crema', N'Sancor', N'Lacteos', N'ml', 4),
    (16, N'Queso rallado', N'Queso duro rallado', N'Tregar', N'Lacteos', N'gr', 4),
    (17, N'Pan de hamburguesa', N'Pan para hamburguesa', N'Don Julio', N'Panaderia', N'unidad', 5),
    (18, N'Pan rallado', N'Pan rallado', N'Don Julio', N'Panaderia', N'gr', 5),
    (19, N'Masa de pizza', N'Prepizza', N'Don Julio', N'Panaderia', N'unidad', 5),
    (20, N'Salmon rosado', N'Filet de salmon', N'Puerto', N'Pescados', N'gr', 6),
    (21, N'Merluza', N'Filet de merluza', N'Puerto', N'Pescados', N'gr', 6),
    (22, N'Pechuga de pollo', N'Pechuga sin hueso', N'AviCola', N'Carnicos', N'gr', 2),
    (23, N'Carne picada', N'Picada especial', N'CP', N'Carnicos', N'gr', 2),
    (24, N'Costilla de cerdo', N'Costilla', N'CP', N'Carnicos', N'gr', 2),
    (25, N'Jamon cocido', N'Fiambre', N'La Tradicion', N'Fiambres', N'gr', 10),
    (26, N'Panceta ahumada', N'Panceta', N'La Tradicion', N'Fiambres', N'gr', 10),
    (27, N'Tomate perita', N'Tomate fresco', N'Campo', N'Verduras', N'gr', 3),
    (28, N'Cebolla', N'Cebolla', N'Campo', N'Verduras', N'gr', 3),
    (29, N'Lechuga', N'Lechuga criolla', N'Campo', N'Verduras', N'unidad', 3),
    (30, N'Zanahoria', N'Zanahoria', N'Campo', N'Verduras', N'gr', 3),
    (31, N'Limon', N'Limon', N'Campo', N'Frutas', N'gr', 9),
    (32, N'Manzana roja', N'Manzana', N'Campo', N'Frutas', N'gr', 9),
    (33, N'Agua mineral 500ml', N'Agua sin gas', N'Villavicencio', N'Bebidas', N'unidad', 8),
    (34, N'Cerveza rubia 1L', N'Cerveza', N'Quilmes', N'Bebidas', N'unidad', 8),
    (35, N'Vino Malbec', N'Vino tinto', N'Trapiche', N'Bebidas', N'unidad', 8)
) AS s (IdProducto, Nombre, Descripcion, Marca, Categoria, Medida, IdProveedor)
   ON t.IdProducto = s.IdProducto
WHEN NOT MATCHED BY TARGET THEN INSERT (IdProducto, Nombre, Descripcion, Marca, Categoria, Medida, IdProveedor)
    VALUES (s.IdProducto, s.Nombre, s.Descripcion, s.Marca, s.Categoria, s.Medida, s.IdProveedor);
SET IDENTITY_INSERT dbo.Producto OFF;
GO

/* --------------------------------------------------------------------- Stock
   Casos para probar: mozzarella y lechuga BAJO el minimo (alerta), huevos SOBRE el maximo,
   leche con dos lotes (FIFO: se gasta primero el que vence antes) y un lote de salmon VENCIDO
   (no se usa para los pedidos). */
SET IDENTITY_INSERT dbo.Stock ON;
MERGE dbo.Stock AS t USING (VALUES
    (4, 4, N'L-0004', 25000, NULL, CAST(1400 AS DECIMAL(18,2)), 5000, 50000, 7),
    (5, 5, N'L-0005', 10000, NULL, CAST(2600 AS DECIMAL(18,2)), 2000, 20000, 7),
    (6, 6, N'L-0006', 5000, NULL, CAST(800 AS DECIMAL(18,2)), 500, 10000, 7),
    (7, 7, N'L-0007', 8000, NULL, CAST(1200 AS DECIMAL(18,2)), 1000, 20000, 7),
    (8, 8, N'L-0008', 15000, NULL, CAST(1900 AS DECIMAL(18,2)), 3000, 30000, 7),
    (9, 9, N'L-0009', 12000, NULL, CAST(1500 AS DECIMAL(18,2)), 3000, 30000, 7),
    (10, 10, N'L-0010', 9000, DATEADD(DAY, 300, CAST(GETDATE() AS DATE)), CAST(1100 AS DECIMAL(18,2)), 2000, 20000, 7),
    (11, 11, N'L-0011', 300, DATEADD(DAY, 18, CAST(GETDATE() AS DATE)), CAST(220 AS DECIMAL(18,2)), 60, 200, 7),
    (12, 12, N'L-0012A', 8000, DATEADD(DAY, 8, CAST(GETDATE() AS DATE)), CAST(1300 AS DECIMAL(18,2)), 5000, 40000, 4),
    (13, 12, N'L-0012B', 20000, DATEADD(DAY, 40, CAST(GETDATE() AS DATE)), CAST(1300 AS DECIMAL(18,2)), 5000, 40000, 4),
    (14, 13, N'L-0013', 800, DATEADD(DAY, 12, CAST(GETDATE() AS DATE)), CAST(7800 AS DECIMAL(18,2)), 2000, 20000, 4),
    (15, 14, N'L-0014', 3000, DATEADD(DAY, 25, CAST(GETDATE() AS DATE)), CAST(6200 AS DECIMAL(18,2)), 500, 6000, 4),
    (16, 15, N'L-0015', 4000, DATEADD(DAY, 14, CAST(GETDATE() AS DATE)), CAST(3400 AS DECIMAL(18,2)), 1000, 10000, 4),
    (17, 16, N'L-0016', 2500, DATEADD(DAY, 60, CAST(GETDATE() AS DATE)), CAST(8900 AS DECIMAL(18,2)), 500, 5000, 4),
    (18, 17, N'L-0017', 60, DATEADD(DAY, 4, CAST(GETDATE() AS DATE)), CAST(450 AS DECIMAL(18,2)), 20, 150, 5),
    (19, 18, N'L-0018', 4000, DATEADD(DAY, 90, CAST(GETDATE() AS DATE)), CAST(900 AS DECIMAL(18,2)), 500, 8000, 5),
    (20, 19, N'L-0019', 30, DATEADD(DAY, 6, CAST(GETDATE() AS DATE)), CAST(1200 AS DECIMAL(18,2)), 10, 60, 5),
    (21, 20, N'L-0020V', 5000, DATEADD(DAY, -1, CAST(GETDATE() AS DATE)), CAST(21000 AS DECIMAL(18,2)), 3000, 20000, 6),
    (22, 20, N'L-0020', 9000, DATEADD(DAY, 5, CAST(GETDATE() AS DATE)), CAST(21000 AS DECIMAL(18,2)), 3000, 20000, 6),
    (23, 21, N'L-0021', 7000, DATEADD(DAY, 4, CAST(GETDATE() AS DATE)), CAST(9800 AS DECIMAL(18,2)), 2000, 15000, 6),
    (24, 22, N'L-0022', 18000, DATEADD(DAY, 6, CAST(GETDATE() AS DATE)), CAST(5200 AS DECIMAL(18,2)), 5000, 40000, 2),
    (25, 23, N'L-0023', 15000, DATEADD(DAY, 5, CAST(GETDATE() AS DATE)), CAST(6800 AS DECIMAL(18,2)), 4000, 30000, 2),
    (26, 24, N'L-0024', 10000, DATEADD(DAY, 7, CAST(GETDATE() AS DATE)), CAST(7400 AS DECIMAL(18,2)), 3000, 20000, 2),
    (27, 25, N'L-0025', 3000, DATEADD(DAY, 15, CAST(GETDATE() AS DATE)), CAST(9500 AS DECIMAL(18,2)), 1000, 8000, 10),
    (28, 26, N'L-0026', 2000, DATEADD(DAY, 20, CAST(GETDATE() AS DATE)), CAST(8800 AS DECIMAL(18,2)), 500, 6000, 10),
    (29, 27, N'L-0027', 12000, DATEADD(DAY, 6, CAST(GETDATE() AS DATE)), CAST(1800 AS DECIMAL(18,2)), 3000, 25000, 3),
    (30, 28, N'L-0028', 15000, DATEADD(DAY, 30, CAST(GETDATE() AS DATE)), CAST(900 AS DECIMAL(18,2)), 3000, 30000, 3),
    (31, 29, N'L-0029', 4, DATEADD(DAY, 3, CAST(GETDATE() AS DATE)), CAST(700 AS DECIMAL(18,2)), 10, 40, 3),
    (32, 30, N'L-0030', 9000, DATEADD(DAY, 14, CAST(GETDATE() AS DATE)), CAST(800 AS DECIMAL(18,2)), 2000, 20000, 3),
    (33, 31, N'L-0031', 5000, DATEADD(DAY, 15, CAST(GETDATE() AS DATE)), CAST(1600 AS DECIMAL(18,2)), 1000, 10000, 9),
    (34, 32, N'L-0032', 6000, DATEADD(DAY, 12, CAST(GETDATE() AS DATE)), CAST(2100 AS DECIMAL(18,2)), 1000, 12000, 9),
    (35, 33, N'L-0033', 120, NULL, CAST(650 AS DECIMAL(18,2)), 36, 300, 8),
    (36, 34, N'L-0034', 96, DATEADD(DAY, 120, CAST(GETDATE() AS DATE)), CAST(2400 AS DECIMAL(18,2)), 24, 240, 8),
    (37, 35, N'L-0035', 24, NULL, CAST(6500 AS DECIMAL(18,2)), 6, 60, 8)
) AS s (IdStock, IdProducto, NumeroLote, Cantidad, FechaVencimiento, Precio, StockMinimo, StockMaximo, IdProveedor)
   ON t.IdStock = s.IdStock
WHEN NOT MATCHED BY TARGET THEN INSERT (IdStock, IdProducto, NumeroLote, Cantidad, FechaVencimiento, Precio, StockMinimo, StockMaximo, IdProveedor)
    VALUES (s.IdStock, s.IdProducto, s.NumeroLote, s.Cantidad, s.FechaVencimiento, s.Precio, s.StockMinimo, s.StockMaximo, s.IdProveedor);
SET IDENTITY_INSERT dbo.Stock OFF;
GO

/* El proveedor de los lotes de ejemplo anteriores y el historial de ingreso de los nuevos. */
UPDATE s SET s.IdProveedor = p.IdProveedor FROM dbo.Stock s INNER JOIN dbo.Producto p ON p.IdProducto = s.IdProducto WHERE s.IdProveedor IS NULL;
GO
INSERT INTO dbo.MovimientoStock (Tipo, IdStock, IdProducto, NumeroLote, Cantidad, CantidadResultante, IdUsuario, Detalle)
SELECT N'Ingreso', s.IdStock, s.IdProducto, s.NumeroLote, s.Cantidad, s.Cantidad, 1, N'Carga inicial (demo)'
FROM dbo.Stock s WHERE NOT EXISTS (SELECT 1 FROM dbo.MovimientoStock m WHERE m.IdStock = s.IdStock);
GO
/* Algunos movimientos de ejemplo para la trazabilidad (ajustes y una baja). */
INSERT INTO dbo.MovimientoStock (Tipo, IdStock, IdProducto, NumeroLote, Cantidad, CantidadResultante, IdUsuario, Detalle)
SELECT v.Tipo, s.IdStock, s.IdProducto, s.NumeroLote, v.Cant, s.Cantidad, 1, v.Detalle
FROM (VALUES (N'L-0011', N'Ajuste', -6, N'Demo: huevos rotos en el traslado'),
             (N'L-0020V', N'Ajuste', 0, N'Demo: lote vencido, pendiente de baja'),
             (N'L-0013', N'Ajuste', -400, N'Demo: merma por vencimiento parcial')) AS v (Lote, Tipo, Cant, Detalle)
INNER JOIN dbo.Stock s ON s.NumeroLote = v.Lote
WHERE NOT EXISTS (SELECT 1 FROM dbo.MovimientoStock m WHERE m.IdStock = s.IdStock AND m.Detalle = v.Detalle);
GO

/* ----------------------------------------------------------------- Carta */
SET IDENTITY_INSERT dbo.Menu ON;
MERGE dbo.Menu AS t USING (VALUES
    (4, N'Hamburguesa completa', N'Carne, queso, tomate y lechuga en pan', N'Plato principal', N'Pan de hamburguesa, Carne picada, Queso mozzarella, Tomate perita', CAST(9500 AS DECIMAL(18,2)), N'Argentina', N'Todo el ano', N'Alta', 15, N'General', 0, 1),
    (5, N'Milanesa napolitana', N'Milanesa de pollo con salsa, jamon y queso', N'Plato principal', N'Pechuga de pollo, Pan rallado, Huevos, Queso mozzarella', CAST(11000 AS DECIMAL(18,2)), N'Argentina', N'Todo el ano', N'Alta', 20, N'General', 0, 1),
    (6, N'Pizza muzzarella', N'Pizza grande de muzzarella', N'Plato principal', N'Masa de pizza, Salsa de tomate, Queso mozzarella, Aceite de girasol', CAST(8500 AS DECIMAL(18,2)), N'Argentina', N'Todo el ano', N'Alta', 18, N'Cumpleanos', 0, 1),
    (7, N'Tallarines con salsa', N'Pasta con salsa de tomate y queso rallado', N'Pastas', N'Fideos tirabuzon, Salsa de tomate, Queso rallado', CAST(7800 AS DECIMAL(18,2)), N'Italia', N'Todo el ano', N'Media', 15, N'General', 0, 1),
    (8, N'Risotto de pollo', N'Arroz cremoso con pollo', N'Pastas', N'Arroz largo fino, Pechuga de pollo, Crema de leche, Manteca', CAST(9800 AS DECIMAL(18,2)), N'Italia', N'Invierno', N'Media', 25, N'Aniversario', 0, 1),
    (9, N'Salmon a la plancha', N'Salmon con limon y zanahoria', N'Pescados', N'Salmon rosado, Limon, Aceite de girasol, Zanahoria', CAST(15500 AS DECIMAL(18,2)), N'Patagonia', N'Todo el ano', N'Media', 20, N'Aniversario', 0, 1),
    (10, N'Merluza al horno', N'Merluza con limon y zanahoria', N'Pescados', N'Merluza, Limon, Zanahoria, Aceite de girasol', CAST(11800 AS DECIMAL(18,2)), N'Argentina', N'Todo el ano', N'Media', 25, N'General', 0, 1),
    (11, N'Costillas BBQ', N'Costillas de cerdo con salsa barbacoa', N'Plato principal', N'Costilla de cerdo, Salsa de tomate, Cebolla', CAST(14200 AS DECIMAL(18,2)), N'Argentina', N'Invierno', N'Alta', 40, N'Fin de semana', 0, 0),
    (12, N'Ensalada mixta', N'Lechuga, tomate, zanahoria y cebolla', N'Ensaladas', N'Lechuga, Tomate perita, Zanahoria, Cebolla', CAST(6200 AS DECIMAL(18,2)), N'Argentina', N'Verano', N'Media', 8, N'General', 0, 1),
    (13, N'Ensalada Caesar', N'Lechuga, pollo, queso y crutones', N'Ensaladas', N'Lechuga, Pechuga de pollo, Queso rallado, Pan rallado', CAST(8200 AS DECIMAL(18,2)), N'Estados Unidos', N'Todo el ano', N'Alta', 10, N'General', 0, 1),
    (14, N'Omelette de jamon y queso', N'Tres huevos con jamon y queso', N'Entradas', N'Huevos, Jamon cocido, Queso mozzarella, Manteca', CAST(5800 AS DECIMAL(18,2)), N'Argentina', N'Todo el ano', N'Baja', 10, N'Desayuno', 0, 1),
    (15, N'Flan casero', N'Flan con dulce de leche', N'Postres', N'Leche entera, Huevos, Azucar', CAST(4800 AS DECIMAL(18,2)), N'Argentina', N'Todo el ano', N'Media', 5, N'Cumpleanos', 0, 1),
    (16, N'Tarta de manzana', N'Tarta tibia de manzana', N'Postres', N'Manzana roja, Harina 000, Manteca, Azucar', CAST(5200 AS DECIMAL(18,2)), N'Europa', N'Otono', N'Baja', 8, N'General', 0, 1),
    (17, N'Limonada', N'Limonada con agua mineral', N'Bebida', N'Limon, Azucar, Agua mineral 500ml', CAST(2800 AS DECIMAL(18,2)), N'Argentina', N'Verano', N'Alta', 3, N'General', 0, 1),
    (18, N'Cerveza rubia', N'Botella de 1 litro', N'Bebida', N'Cerveza rubia 1L', CAST(4200 AS DECIMAL(18,2)), N'Argentina', N'Verano', N'Alta', 1, N'Fin de semana', 0, 1),
    (19, N'Vino Malbec', N'Botella de vino tinto', N'Bebida', N'Vino Malbec', CAST(12000 AS DECIMAL(18,2)), N'Mendoza', N'Todo el ano', N'Media', 2, N'Aniversario', 0, 1),
    (20, N'Agua mineral', N'Botella de 500 ml', N'Bebida', N'Agua mineral 500ml', CAST(1800 AS DECIMAL(18,2)), N'Argentina', N'Todo el ano', N'Alta', 1, N'General', 0, 1),
    (21, N'Tabla de fiambres', N'Jamon, panceta y queso', N'Entradas', N'Jamon cocido, Panceta ahumada, Queso mozzarella', CAST(9200 AS DECIMAL(18,2)), N'Argentina', N'Todo el ano', N'Media', 10, N'Cumpleanos', 0, 1)
) AS s (IdMenu, Nombre, Descripcion, Categoria, Ingredientes, Precio, Region, Temporada, Popularidad, TiempoPreparacion, TipoEvento, IdStock, Disponible)
   ON t.IdMenu = s.IdMenu
WHEN NOT MATCHED BY TARGET THEN INSERT (IdMenu, Nombre, Descripcion, Categoria, Ingredientes, Precio, Region, Temporada, Popularidad, TiempoPreparacion, TipoEvento, IdStock, Disponible)
    VALUES (s.IdMenu, s.Nombre, s.Descripcion, s.Categoria, s.Ingredientes, s.Precio, s.Region, s.Temporada, s.Popularidad, s.TiempoPreparacion, s.TipoEvento, s.IdStock, s.Disponible);
SET IDENTITY_INSERT dbo.Menu OFF;
GO

/* ---------------------------------------------------------------- Recetas
   Cantidad en la medida de cada producto (gr, ml o unidad). Costillas BBQ queda NO disponible a proposito. */
MERGE dbo.RecetaIngrediente AS t USING (VALUES
    (4, 17, CAST(1 AS DECIMAL(18,3)), 0),
    (4, 23, CAST(180 AS DECIMAL(18,3)), 1),
    (4, 13, CAST(40 AS DECIMAL(18,3)), 0),
    (4, 27, CAST(40 AS DECIMAL(18,3)), 0),
    (4, 29, CAST(0.25 AS DECIMAL(18,3)), 0),
    (5, 22, CAST(250 AS DECIMAL(18,3)), 1),
    (5, 18, CAST(60 AS DECIMAL(18,3)), 0),
    (5, 11, CAST(1 AS DECIMAL(18,3)), 0),
    (5, 13, CAST(80 AS DECIMAL(18,3)), 0),
    (5, 10, CAST(60 AS DECIMAL(18,3)), 0),
    (5, 25, CAST(40 AS DECIMAL(18,3)), 0),
    (6, 19, CAST(1 AS DECIMAL(18,3)), 1),
    (6, 10, CAST(80 AS DECIMAL(18,3)), 0),
    (6, 13, CAST(150 AS DECIMAL(18,3)), 0),
    (6, 5, CAST(5 AS DECIMAL(18,3)), 0),
    (7, 9, CAST(150 AS DECIMAL(18,3)), 1),
    (7, 10, CAST(120 AS DECIMAL(18,3)), 0),
    (7, 16, CAST(15 AS DECIMAL(18,3)), 0),
    (8, 8, CAST(120 AS DECIMAL(18,3)), 1),
    (8, 22, CAST(120 AS DECIMAL(18,3)), 0),
    (8, 15, CAST(50 AS DECIMAL(18,3)), 0),
    (8, 14, CAST(15 AS DECIMAL(18,3)), 0),
    (8, 16, CAST(15 AS DECIMAL(18,3)), 0),
    (9, 20, CAST(220 AS DECIMAL(18,3)), 1),
    (9, 31, CAST(30 AS DECIMAL(18,3)), 0),
    (9, 5, CAST(10 AS DECIMAL(18,3)), 0),
    (9, 30, CAST(80 AS DECIMAL(18,3)), 0),
    (10, 21, CAST(200 AS DECIMAL(18,3)), 1),
    (10, 31, CAST(20 AS DECIMAL(18,3)), 0),
    (10, 30, CAST(60 AS DECIMAL(18,3)), 0),
    (10, 5, CAST(10 AS DECIMAL(18,3)), 0),
    (11, 24, CAST(400 AS DECIMAL(18,3)), 1),
    (11, 10, CAST(60 AS DECIMAL(18,3)), 0),
    (11, 28, CAST(40 AS DECIMAL(18,3)), 0),
    (12, 29, CAST(0.5 AS DECIMAL(18,3)), 1),
    (12, 27, CAST(80 AS DECIMAL(18,3)), 0),
    (12, 30, CAST(40 AS DECIMAL(18,3)), 0),
    (12, 28, CAST(20 AS DECIMAL(18,3)), 0),
    (12, 5, CAST(8 AS DECIMAL(18,3)), 0),
    (12, 31, CAST(10 AS DECIMAL(18,3)), 0),
    (13, 29, CAST(0.5 AS DECIMAL(18,3)), 1),
    (13, 22, CAST(100 AS DECIMAL(18,3)), 0),
    (13, 16, CAST(20 AS DECIMAL(18,3)), 0),
    (13, 18, CAST(15 AS DECIMAL(18,3)), 0),
    (13, 11, CAST(0.5 AS DECIMAL(18,3)), 0),
    (14, 11, CAST(3 AS DECIMAL(18,3)), 1),
    (14, 25, CAST(40 AS DECIMAL(18,3)), 0),
    (14, 13, CAST(40 AS DECIMAL(18,3)), 0),
    (14, 14, CAST(10 AS DECIMAL(18,3)), 0),
    (15, 12, CAST(150 AS DECIMAL(18,3)), 1),
    (15, 11, CAST(2 AS DECIMAL(18,3)), 0),
    (15, 7, CAST(40 AS DECIMAL(18,3)), 0),
    (16, 32, CAST(150 AS DECIMAL(18,3)), 1),
    (16, 4, CAST(80 AS DECIMAL(18,3)), 0),
    (16, 14, CAST(40 AS DECIMAL(18,3)), 0),
    (16, 7, CAST(40 AS DECIMAL(18,3)), 0),
    (17, 31, CAST(80 AS DECIMAL(18,3)), 1),
    (17, 7, CAST(20 AS DECIMAL(18,3)), 0),
    (17, 33, CAST(0.6 AS DECIMAL(18,3)), 0),
    (18, 34, CAST(1 AS DECIMAL(18,3)), 1),
    (19, 35, CAST(1 AS DECIMAL(18,3)), 1),
    (20, 33, CAST(1 AS DECIMAL(18,3)), 1),
    (21, 25, CAST(80 AS DECIMAL(18,3)), 1),
    (21, 26, CAST(50 AS DECIMAL(18,3)), 0),
    (21, 13, CAST(80 AS DECIMAL(18,3)), 0)
) AS s (IdMenu, IdProducto, Cantidad, EsPrincipal)
   ON t.IdMenu = s.IdMenu AND t.IdProducto = s.IdProducto
WHEN NOT MATCHED BY TARGET THEN INSERT (IdMenu, IdProducto, Cantidad, EsPrincipal) VALUES (s.IdMenu, s.IdProducto, s.Cantidad, s.EsPrincipal);
GO

/* ------------------------------------------------ Cajas cerradas de dias anteriores
   Tres cajas (hace 2, 3 y 4 dias) con sus pedidos cobrados; los totales se calculan desde los pedidos. */
IF (SELECT COUNT(*) FROM dbo.Caja) < 2
BEGIN
    INSERT INTO dbo.Caja (Apertura, IdUsuarioApertura, MontoInicial, Cierre, IdUsuarioCierre, TotalCobrado, MontoContado, Observaciones)
    VALUES (DATEADD(DAY, -4, DATEADD(HOUR, 9, CAST(CAST(GETDATE() AS DATE) AS DATETIME2(0)))), 3, 5000, DATEADD(DAY, -4, DATEADD(HOUR, 23, CAST(CAST(GETDATE() AS DATE) AS DATETIME2(0)))), 3, 0, 0, N'Caja demo');
    INSERT INTO dbo.Caja (Apertura, IdUsuarioApertura, MontoInicial, Cierre, IdUsuarioCierre, TotalCobrado, MontoContado, Observaciones)
    VALUES (DATEADD(DAY, -3, DATEADD(HOUR, 9, CAST(CAST(GETDATE() AS DATE) AS DATETIME2(0)))), 4, 4000, DATEADD(DAY, -3, DATEADD(HOUR, 23, CAST(CAST(GETDATE() AS DATE) AS DATETIME2(0)))), 4, 0, 0, N'Caja demo');
    INSERT INTO dbo.Caja (Apertura, IdUsuarioApertura, MontoInicial, Cierre, IdUsuarioCierre, TotalCobrado, MontoContado, Observaciones)
    VALUES (DATEADD(DAY, -2, DATEADD(HOUR, 9, CAST(CAST(GETDATE() AS DATE) AS DATETIME2(0)))), 3, 6000, DATEADD(DAY, -2, DATEADD(HOUR, 23, CAST(CAST(GETDATE() AS DATE) AS DATETIME2(0)))), 3, 0, 0, N'Caja demo');
END
GO

/* --------------------------------------------------- Pedidos historicos y de hoy
   Los de hoy en Pendiente / En curso sirven para probar Elaboracion y las cuentas por mesa de Caja.
   (Son datos cargados directamente: no descuentan stock.) */
SET IDENTITY_INSERT dbo.Pedido ON;
MERGE dbo.Pedido AS t USING (VALUES
    (101, N'Camila Acosta', 18, 3, N'Transferencia', N'', N'Cobrado', CAST(22100 AS DECIMAL(18,2)), DATEADD(DAY, -4, CAST(GETDATE() AS DATE)), 4, N'Delivery', NULL),
    (102, N'Ana Lopez', 19, 3, N'Efectivo', N'', N'Cobrado', CAST(49600 AS DECIMAL(18,2)), DATEADD(DAY, -4, CAST(GETDATE() AS DATE)), 4, N'Salon', N'10'),
    (103, N'Federico Rojas', 16, 1, N'Tarjeta', N'', N'Cobrado', CAST(5200 AS DECIMAL(18,2)), DATEADD(DAY, -4, CAST(GETDATE() AS DATE)), 3, N'Barra', NULL),
    (104, N'Federico Rojas', 9, 2, N'Efectivo', N'', N'Cobrado', CAST(47400 AS DECIMAL(18,2)), DATEADD(DAY, -4, CAST(GETDATE() AS DATE)), 4, N'Delivery', NULL),
    (105, N'Valeria Sosa', 19, 3, N'Efectivo', N'', N'Cobrado', CAST(36000 AS DECIMAL(18,2)), DATEADD(DAY, -4, CAST(GETDATE() AS DATE)), 4, N'Salon', N'5'),
    (106, N'Lucas Morales', 13, 2, N'Efectivo', N'', N'Cobrado', CAST(16400 AS DECIMAL(18,2)), DATEADD(DAY, -4, CAST(GETDATE() AS DATE)), 4, N'Barra', NULL),
    (107, N'Tomas Aguirre', 17, 1, N'Transferencia', N'', N'Cobrado', CAST(35600 AS DECIMAL(18,2)), DATEADD(DAY, -3, CAST(GETDATE() AS DATE)), 4, N'Barra', NULL),
    (108, N'Julieta Ramos', 20, 2, N'Efectivo', N'', N'Cobrado', CAST(16200 AS DECIMAL(18,2)), DATEADD(DAY, -3, CAST(GETDATE() AS DATE)), 3, N'Salon', N'10'),
    (109, N'Mesa 5', 18, 3, N'Tarjeta', N'', N'Cobrado', CAST(59500 AS DECIMAL(18,2)), DATEADD(DAY, -3, CAST(GETDATE() AS DATE)), 3, N'Barra', NULL),
    (110, N'Paula Medina', 16, 3, N'Transferencia', N'', N'Cobrado', CAST(15600 AS DECIMAL(18,2)), DATEADD(DAY, -3, CAST(GETDATE() AS DATE)), 4, N'Barra', NULL),
    (111, N'Nicolas Herrera', 14, 3, N'Tarjeta', N'', N'Cobrado', CAST(65400 AS DECIMAL(18,2)), DATEADD(DAY, -3, CAST(GETDATE() AS DATE)), 4, N'Barra', NULL),
    (112, N'Mesa 5', 10, 2, N'Transferencia', N'', N'Cobrado', CAST(23600 AS DECIMAL(18,2)), DATEADD(DAY, -3, CAST(GETDATE() AS DATE)), 4, N'Salon', N'1'),
    (113, N'Nicolas Herrera', 17, 4, N'Tarjeta', N'', N'Cobrado', CAST(26800 AS DECIMAL(18,2)), DATEADD(DAY, -3, CAST(GETDATE() AS DATE)), 4, N'Delivery', NULL),
    (114, N'Pedro Gimenez', 2, 2, N'Tarjeta', N'', N'Cobrado', CAST(40400 AS DECIMAL(18,2)), DATEADD(DAY, -3, CAST(GETDATE() AS DATE)), 3, N'Delivery', NULL),
    (115, N'Sol Benitez', 17, 1, N'Tarjeta', N'', N'Cobrado', CAST(42000 AS DECIMAL(18,2)), DATEADD(DAY, -2, CAST(GETDATE() AS DATE)), 3, N'Salon', N'12'),
    (116, N'Martin Ortiz', 1, 3, N'Efectivo', N'', N'Cobrado', CAST(36000 AS DECIMAL(18,2)), DATEADD(DAY, -2, CAST(GETDATE() AS DATE)), 3, N'Delivery', NULL),
    (117, N'Martin Ortiz', 4, 3, N'Tarjeta', N'', N'Cobrado', CAST(28500 AS DECIMAL(18,2)), DATEADD(DAY, -2, CAST(GETDATE() AS DATE)), 4, N'Salon', N'13'),
    (118, N'Camila Acosta', 1, 2, N'Tarjeta', N'', N'Cobrado', CAST(24000 AS DECIMAL(18,2)), DATEADD(DAY, -2, CAST(GETDATE() AS DATE)), 3, N'Delivery', NULL),
    (119, N'Pedro Gimenez', 8, 2, N'Efectivo', N'', N'Cobrado', CAST(49000 AS DECIMAL(18,2)), DATEADD(DAY, -2, CAST(GETDATE() AS DATE)), 4, N'Delivery', NULL),
    (120, N'Nicolas Herrera', 6, 1, N'Efectivo', N'', N'Cobrado', CAST(20500 AS DECIMAL(18,2)), DATEADD(DAY, -2, CAST(GETDATE() AS DATE)), 3, N'Barra', NULL),
    (121, N'Familia Diaz', 1, 1, N'Tarjeta', N'', N'Cobrado', CAST(17800 AS DECIMAL(18,2)), DATEADD(DAY, -2, CAST(GETDATE() AS DATE)), 3, N'Salon', N'14'),
    (122, N'Valeria Sosa', 17, 2, N'Transferencia', N'', N'Cobrado', CAST(35400 AS DECIMAL(18,2)), DATEADD(DAY, -1, CAST(GETDATE() AS DATE)), 3, N'Barra', NULL),
    (123, N'Camila Acosta', 20, 3, N'Tarjeta', N'', N'Cobrado', CAST(5400 AS DECIMAL(18,2)), DATEADD(DAY, -1, CAST(GETDATE() AS DATE)), 4, N'Salon', N'7'),
    (124, N'Tomas Aguirre', 8, 3, N'Efectivo', N'', N'Cobrado', CAST(44200 AS DECIMAL(18,2)), DATEADD(DAY, -1, CAST(GETDATE() AS DATE)), 3, N'Salon', N'11'),
    (125, N'Federico Rojas', 19, 1, N'Transferencia', N'', N'Cobrado', CAST(43200 AS DECIMAL(18,2)), DATEADD(DAY, -1, CAST(GETDATE() AS DATE)), 3, N'Salon', N'3'),
    (126, N'Lucas Morales', 5, 1, N'Efectivo', N'', N'Cobrado', CAST(59000 AS DECIMAL(18,2)), DATEADD(DAY, -1, CAST(GETDATE() AS DATE)), 4, N'Delivery', NULL),
    (127, N'Valeria Sosa', 6, 4, N'Tarjeta', N'', N'Cobrado', CAST(34000 AS DECIMAL(18,2)), DATEADD(DAY, -1, CAST(GETDATE() AS DATE)), 4, N'Barra', NULL),
    (128, N'Mesa 3', 4, 2, N'Efectivo', N'', N'Pendiente', CAST(24600 AS DECIMAL(18,2)), CAST(GETDATE() AS DATE), 3, N'Salon', N'3'),
    (129, N'Mesa 7', 6, 1, N'Tarjeta', N'', N'En curso', CAST(16900 AS DECIMAL(18,2)), CAST(GETDATE() AS DATE), 3, N'Salon', N'7'),
    (130, N'Mesa 7', 15, 2, N'Tarjeta', N'', N'Pendiente', CAST(9600 AS DECIMAL(18,2)), CAST(GETDATE() AS DATE), 3, N'Salon', N'7'),
    (131, N'Mesa 12', 9, 1, N'Transferencia', N'', N'En curso', CAST(27500 AS DECIMAL(18,2)), CAST(GETDATE() AS DATE), 3, N'Salon', N'12'),
    (132, N'Delivery Sofia', 5, 2, N'Efectivo', N'', N'Pendiente', CAST(25600 AS DECIMAL(18,2)), CAST(GETDATE() AS DATE), 3, N'Delivery', NULL),
    (133, N'Barra Ezequiel', 13, 1, N'Efectivo', N'', N'Listo', CAST(11000 AS DECIMAL(18,2)), CAST(GETDATE() AS DATE), 3, N'Barra', NULL)
) AS s (IdPedido, NombreCliente, IdMenu, Cantidad, FormaPago, InstruccionesEspeciales, Estado, PrecioTotal, Fecha, IdUsuario, Canal, Mesa)
   ON t.IdPedido = s.IdPedido
WHEN NOT MATCHED BY TARGET THEN INSERT (IdPedido, NombreCliente, IdMenu, Cantidad, FormaPago, InstruccionesEspeciales, Estado, PrecioTotal, Fecha, IdUsuario, Canal, Mesa)
    VALUES (s.IdPedido, s.NombreCliente, s.IdMenu, s.Cantidad, s.FormaPago, s.InstruccionesEspeciales, s.Estado, s.PrecioTotal, s.Fecha, s.IdUsuario, s.Canal, s.Mesa);
SET IDENTITY_INSERT dbo.Pedido OFF;
GO

INSERT INTO dbo.PedidoDetalle (IdPedido, IdMenu, Cantidad, PrecioUnitario)
SELECT v.IdPedido, v.IdMenu, v.Cantidad, m.Precio
FROM (VALUES
    (101, 18, 3),
    (101, 4, 1),
    (102, 19, 3),
    (102, 17, 3),
    (102, 16, 1),
    (103, 16, 1),
    (104, 9, 2),
    (104, 13, 2),
    (105, 19, 3),
    (106, 13, 2),
    (107, 17, 1),
    (107, 15, 3),
    (107, 21, 2),
    (108, 20, 2),
    (108, 18, 3),
    (109, 18, 3),
    (109, 4, 3),
    (109, 21, 2),
    (110, 16, 3),
    (111, 14, 3),
    (111, 10, 3),
    (111, 18, 3),
    (112, 10, 2),
    (113, 17, 4),
    (113, 7, 2),
    (114, 2, 2),
    (114, 12, 2),
    (114, 4, 2),
    (115, 17, 1),
    (115, 9, 2),
    (115, 13, 1),
    (116, 1, 3),
    (117, 4, 3),
    (118, 1, 2),
    (119, 8, 2),
    (119, 20, 1),
    (119, 21, 3),
    (120, 6, 1),
    (120, 20, 2),
    (120, 18, 2),
    (121, 1, 1),
    (121, 14, 1),
    (122, 17, 2),
    (122, 10, 2),
    (122, 12, 1),
    (123, 20, 3),
    (124, 8, 3),
    (124, 3, 3),
    (124, 14, 1),
    (125, 19, 1),
    (125, 16, 3),
    (125, 7, 2),
    (126, 5, 1),
    (126, 6, 2),
    (126, 9, 2),
    (127, 6, 4),
    (128, 4, 2),
    (128, 17, 2),
    (129, 6, 1),
    (129, 18, 2),
    (130, 15, 2),
    (131, 9, 1),
    (131, 19, 1),
    (132, 5, 2),
    (132, 20, 2),
    (133, 13, 1),
    (133, 17, 1)
) AS v (IdPedido, IdMenu, Cantidad)
INNER JOIN dbo.Menu m ON m.IdMenu = v.IdMenu
WHERE NOT EXISTS (SELECT 1 FROM dbo.PedidoDetalle d WHERE d.IdPedido = v.IdPedido);
GO

/* Los cobrados de hace 2 a 4 dias se asocian a la caja de ese dia; el cobro de hoy queda sin caja (la abre el Cajero). */
UPDATE p SET p.IdCaja = c.IdCaja, p.FechaCobro = DATEADD(HOUR, 14, CAST(p.Fecha AS DATETIME2(0)))
FROM dbo.Pedido p INNER JOIN dbo.Caja c ON CAST(c.Apertura AS DATE) = p.Fecha AND c.Observaciones IN (N'Caja demo', N'Caja de ejemplo')
WHERE p.IdPedido > 100 AND p.Estado = N'Cobrado' AND p.IdCaja IS NULL;
GO
/* Totales de cada caja de ejemplo, calculados desde sus pedidos cobrados (contado = esperado +/- una diferencia chica). */
UPDATE c SET c.TotalCobrado = ISNULL((SELECT SUM(p.PrecioTotal) FROM dbo.Pedido p WHERE p.IdCaja = c.IdCaja AND p.Estado = N'Cobrado'), 0)
FROM dbo.Caja c WHERE c.Observaciones IN (N'Caja demo', N'Caja de ejemplo');
GO
UPDATE c SET c.MontoContado = c.MontoInicial + ISNULL((SELECT SUM(p.PrecioTotal) FROM dbo.Pedido p WHERE p.IdCaja = c.IdCaja AND p.Estado = N'Cobrado' AND p.FormaPago = N'Efectivo'), 0)
                                + CASE c.IdCaja % 3 WHEN 0 THEN 0 WHEN 1 THEN -150 ELSE 200 END
FROM dbo.Caja c WHERE c.Observaciones IN (N'Caja demo', N'Caja de ejemplo');
GO


EXEC dbo.usp_RecalcularDigitosUsuarios;
GO
