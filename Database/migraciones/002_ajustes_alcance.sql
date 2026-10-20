/*==============================================================================
  002_ajustes_alcance.sql
  Ajustes de esquema pedidos por el documento de alcance ("Entrega del 25/9/23"):

   1. Eliminacion logica de Producto, Proveedor y Menu (columna Activo).
   2. Stock minimo/maximo para las alertas de stock (CU-STO-004).
   3. MovimientoStock: trazabilidad de inventarios (ingresos, egresos, ajustes).
   4. RecetaIngrediente: ingredientes (con cantidad) de cada platillo. Es lo que permite
      saber si el local puede tomar un pedido segun el stock y descontar los insumos.
   5. PedidoDetalle: un pedido puede llevar varios platillos ("Detalle (Menus y Cantidades)").
      Pedido.IdUsuario guarda quien tomo el pedido ("Factura por empleado").
   6. Personal.FechaNacimiento (el alta de usuario pide la edad).
   7. Usuarios.Idioma: el idioma elegido por cada usuario se carga al iniciar sesion.
   8. Pedido.FormaPago pasa a ser obligatoria (NOT NULL), como indica el documento.
==============================================================================*/
SET NOCOUNT ON;
GO

/* ---- 1. Eliminacion logica ------------------------------------------------ */
ALTER TABLE dbo.Producto  ADD Activo BIT NOT NULL CONSTRAINT DF_Producto_Activo  DEFAULT (1);
ALTER TABLE dbo.Proveedor ADD Activo BIT NOT NULL CONSTRAINT DF_Proveedor_Activo DEFAULT (1);
ALTER TABLE dbo.Menu      ADD Activo BIT NOT NULL CONSTRAINT DF_Menu_Activo      DEFAULT (1);
GO

/* ---- 2. Minimos y maximos de stock ---------------------------------------- */
ALTER TABLE dbo.Stock ADD
    StockMinimo INT NOT NULL CONSTRAINT DF_Stock_Minimo DEFAULT (0),
    StockMaximo INT NOT NULL CONSTRAINT DF_Stock_Maximo DEFAULT (0);
GO

/* ---- 3. Trazabilidad de stock --------------------------------------------- */
CREATE TABLE dbo.MovimientoStock (
    IdMovimiento       INT IDENTITY(1,1) NOT NULL,
    Fecha              DATETIME2(0)      NOT NULL CONSTRAINT DF_MovStock_Fecha DEFAULT (SYSDATETIME()),
    Tipo               NVARCHAR(20)      NOT NULL,   -- Ingreso | Egreso | Ajuste | Baja | Devolucion
    IdStock            INT               NULL,       -- sin FK: el historial se conserva aunque se borre el lote
    IdProducto         INT               NOT NULL,
    NumeroLote         NVARCHAR(50)      NULL,
    Cantidad           INT               NOT NULL,   -- variacion con signo (+ entra, - sale)
    CantidadResultante INT               NULL,       -- existencia del lote despues del movimiento
    IdUsuario          INT               NOT NULL CONSTRAINT DF_MovStock_Usuario DEFAULT (0),
    IdPedido           INT               NULL,
    Detalle            NVARCHAR(255)     NULL,
    CONSTRAINT PK_MovimientoStock PRIMARY KEY (IdMovimiento),
    CONSTRAINT FK_MovStock_Producto FOREIGN KEY (IdProducto) REFERENCES dbo.Producto (IdProducto)
);
CREATE INDEX IX_MovStock_Producto ON dbo.MovimientoStock (IdProducto, Fecha DESC);
CREATE INDEX IX_MovStock_Pedido   ON dbo.MovimientoStock (IdPedido);
GO

/* ---- 4. Recetas ------------------------------------------------------------ */
CREATE TABLE dbo.RecetaIngrediente (
    IdMenu      INT           NOT NULL,
    IdProducto  INT           NOT NULL,
    Cantidad    DECIMAL(18,3) NOT NULL,   -- por cada unidad del platillo, en la medida del producto
    EsPrincipal BIT           NOT NULL CONSTRAINT DF_Receta_Principal DEFAULT (0),
    CONSTRAINT PK_RecetaIngrediente PRIMARY KEY (IdMenu, IdProducto),
    CONSTRAINT CK_Receta_Cantidad CHECK (Cantidad > 0),
    CONSTRAINT FK_Receta_Menu     FOREIGN KEY (IdMenu)     REFERENCES dbo.Menu (IdMenu) ON DELETE CASCADE,
    CONSTRAINT FK_Receta_Producto FOREIGN KEY (IdProducto) REFERENCES dbo.Producto (IdProducto)
);
GO

/* ---- 5. Pedidos con detalle ------------------------------------------------ */
UPDATE dbo.Pedido SET FormaPago = N'Efectivo' WHERE FormaPago IS NULL OR LTRIM(RTRIM(FormaPago)) = N'';
GO
ALTER TABLE dbo.Pedido ALTER COLUMN FormaPago NVARCHAR(50) NOT NULL;
ALTER TABLE dbo.Pedido ADD IdUsuario INT NULL;
GO

CREATE TABLE dbo.PedidoDetalle (
    IdDetalle      INT IDENTITY(1,1) NOT NULL,
    IdPedido       INT               NOT NULL,
    IdMenu         INT               NOT NULL,
    Cantidad       INT               NOT NULL,
    PrecioUnitario DECIMAL(18,2)     NOT NULL,
    CONSTRAINT PK_PedidoDetalle PRIMARY KEY (IdDetalle),
    CONSTRAINT CK_PedidoDetalle_Cantidad CHECK (Cantidad > 0),
    CONSTRAINT FK_PedidoDetalle_Pedido FOREIGN KEY (IdPedido) REFERENCES dbo.Pedido (IdPedido) ON DELETE CASCADE,
    CONSTRAINT FK_PedidoDetalle_Menu   FOREIGN KEY (IdMenu)   REFERENCES dbo.Menu (IdMenu)
);
CREATE INDEX IX_PedidoDetalle_Pedido ON dbo.PedidoDetalle (IdPedido);
GO

/* Los pedidos que ya existian tenian un solo platillo: pasan a ser un detalle de una linea. */
INSERT INTO dbo.PedidoDetalle (IdPedido, IdMenu, Cantidad, PrecioUnitario)
SELECT p.IdPedido, p.IdMenu, p.Cantidad,
       CASE WHEN p.Cantidad > 0 THEN CAST(p.PrecioTotal / p.Cantidad AS DECIMAL(18,2)) ELSE p.PrecioTotal END
FROM dbo.Pedido p
WHERE p.Cantidad > 0;
GO

/* ---- 6 y 7. Personal y Usuarios -------------------------------------------- */
ALTER TABLE dbo.Personal ADD FechaNacimiento DATE NULL;
ALTER TABLE dbo.Usuarios ADD Idioma NVARCHAR(5) NULL;   -- es | en | pt (NULL = el que este activo)
GO
