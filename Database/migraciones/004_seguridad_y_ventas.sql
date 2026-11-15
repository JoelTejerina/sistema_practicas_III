/*==============================================================================
  004_seguridad_y_ventas.sql
  Lo que faltaba segun los casos de uso del documento de alcance:

   Seguridad
   - Digito verificador de Usuarios (DVH por fila + DVV de la tabla) y su verificacion.
   - Bloqueo de la terminal por 10 minutos cuando el digito verificador no coincide.
   - Sesion unica por usuario.
   - Usuarios.DebeCambiarPassword: contraseña temporal (recuperacion por mail) que obliga a cambiarla.

   Compras
   - Producto.CantidadInicial (al dar de alta se crea el primer lote de stock).
   - Stock.IdProveedor: el proveedor de cada lote.

   Ventas
   - Menu.Disponible: ocultar / mostrar un platillo con un clic.
   - Pedido.Canal y Pedido.Mesa (salon, barra o delivery).
   - Caja: apertura y cierre de caja; los cobros quedan asociados a la caja abierta.
==============================================================================*/
SET NOCOUNT ON;
GO

/* ---------------------------------------------------------------- Seguridad */
ALTER TABLE dbo.Usuarios ADD
    DVH                 CHAR(64) NULL,
    DebeCambiarPassword BIT      NOT NULL CONSTRAINT DF_Usuarios_DebeCambiar DEFAULT (0);
GO

CREATE TABLE dbo.DigitoVerificador (
    Tabla NVARCHAR(50) NOT NULL,
    DVV   CHAR(64)     NOT NULL,
    CONSTRAINT PK_DigitoVerificador PRIMARY KEY (Tabla)
);
GO

/* DVH de cada usuario = SHA-256 de sus datos criticos. DVV = SHA-256 de todos los DVH juntos. */
CREATE PROCEDURE dbo.usp_RecalcularDigitosUsuarios
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.Usuarios
    SET DVH = CONVERT(CHAR(64), HASHBYTES('SHA2_256',
        CONCAT(IdUsuario, '|', Usuario, '|', [Password], '|', IdPersona, '|', UsuarioDesactivado)), 2);

    DECLARE @dvv CHAR(64) = CONVERT(CHAR(64), HASHBYTES('SHA2_256',
        ISNULL((SELECT STRING_AGG(CAST(DVH AS VARCHAR(MAX)), '') WITHIN GROUP (ORDER BY IdUsuario) FROM dbo.Usuarios), '')), 2);

    MERGE dbo.DigitoVerificador AS t
    USING (SELECT N'Usuarios' AS Tabla, @dvv AS DVV) AS s ON t.Tabla = s.Tabla
    WHEN MATCHED THEN UPDATE SET DVV = s.DVV
    WHEN NOT MATCHED THEN INSERT (Tabla, DVV) VALUES (s.Tabla, s.DVV);
END
GO

/* Devuelve Valido = 1 si ninguna fila fue alterada y el DVV guardado coincide. */
CREATE PROCEDURE dbo.usp_VerificarDigitosUsuarios
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @filas INT = (
        SELECT COUNT(*) FROM dbo.Usuarios
        WHERE DVH IS NULL OR DVH <> CONVERT(CHAR(64), HASHBYTES('SHA2_256',
            CONCAT(IdUsuario, '|', Usuario, '|', [Password], '|', IdPersona, '|', UsuarioDesactivado)), 2));

    DECLARE @dvv CHAR(64) = CONVERT(CHAR(64), HASHBYTES('SHA2_256',
        ISNULL((SELECT STRING_AGG(CAST(DVH AS VARCHAR(MAX)), '') WITHIN GROUP (ORDER BY IdUsuario) FROM dbo.Usuarios), '')), 2);
    DECLARE @guardado CHAR(64) = (SELECT DVV FROM dbo.DigitoVerificador WHERE Tabla = N'Usuarios');

    SELECT CAST(CASE WHEN @filas = 0 AND @guardado = @dvv THEN 1 ELSE 0 END AS BIT) AS Valido,
           @filas AS FilasAlteradas;
END
GO

CREATE TABLE dbo.BloqueoTerminal (
    Terminal        NVARCHAR(100) NOT NULL,
    BloqueadaHasta  DATETIME2(0)  NOT NULL,
    Motivo          NVARCHAR(200) NULL,
    CONSTRAINT PK_BloqueoTerminal PRIMARY KEY (Terminal)
);
GO

CREATE TABLE dbo.SesionActiva (
    IdUsuario       INT           NOT NULL,
    Terminal        NVARCHAR(100) NOT NULL,
    IdProceso       INT           NOT NULL,
    Inicio          DATETIME2(0)  NOT NULL CONSTRAINT DF_Sesion_Inicio DEFAULT (SYSDATETIME()),
    UltimaActividad DATETIME2(0)  NOT NULL CONSTRAINT DF_Sesion_Actividad DEFAULT (SYSDATETIME()),
    CONSTRAINT PK_SesionActiva PRIMARY KEY (IdUsuario),
    CONSTRAINT FK_Sesion_Usuarios FOREIGN KEY (IdUsuario) REFERENCES dbo.Usuarios (IdUsuario) ON DELETE CASCADE
);
GO

/* ------------------------------------------------------------------ Compras */
ALTER TABLE dbo.Producto ADD CantidadInicial INT NOT NULL CONSTRAINT DF_Producto_CantInicial DEFAULT (0);
ALTER TABLE dbo.Stock    ADD IdProveedor     INT NULL;
GO
ALTER TABLE dbo.Stock ADD CONSTRAINT FK_Stock_Proveedor FOREIGN KEY (IdProveedor) REFERENCES dbo.Proveedor (IdProveedor);
GO
/* Los lotes que ya existian toman el proveedor de su producto. */
UPDATE s SET IdProveedor = p.IdProveedor
FROM dbo.Stock s INNER JOIN dbo.Producto p ON p.IdProducto = s.IdProducto
WHERE s.IdProveedor IS NULL;
GO

/* ------------------------------------------------------------------- Ventas */
ALTER TABLE dbo.Menu ADD Disponible BIT NOT NULL CONSTRAINT DF_Menu_Disponible DEFAULT (1);
GO

CREATE TABLE dbo.Caja (
    IdCaja            INT IDENTITY(1,1) NOT NULL,
    Apertura          DATETIME2(0)      NOT NULL CONSTRAINT DF_Caja_Apertura DEFAULT (SYSDATETIME()),
    IdUsuarioApertura INT               NOT NULL,
    MontoInicial      DECIMAL(18,2)     NOT NULL CONSTRAINT DF_Caja_Inicial DEFAULT (0),
    Cierre            DATETIME2(0)      NULL,
    IdUsuarioCierre   INT               NULL,
    TotalCobrado      DECIMAL(18,2)     NULL,
    MontoContado      DECIMAL(18,2)     NULL,
    Observaciones     NVARCHAR(255)     NULL,
    CONSTRAINT PK_Caja PRIMARY KEY (IdCaja)
);
/* Solo puede haber una caja abierta a la vez (todas las filas abiertas tienen Cierre NULL). */
CREATE UNIQUE INDEX UX_Caja_Abierta ON dbo.Caja (Cierre) WHERE Cierre IS NULL;
GO

ALTER TABLE dbo.Pedido ADD
    Canal      NVARCHAR(20)  NOT NULL CONSTRAINT DF_Pedido_Canal DEFAULT (N'Salon'),   -- Salon | Barra | Delivery
    Mesa       NVARCHAR(20)  NULL,
    IdCaja     INT           NULL,
    FechaCobro DATETIME2(0)  NULL;
GO
ALTER TABLE dbo.Pedido ADD CONSTRAINT FK_Pedido_Caja FOREIGN KEY (IdCaja) REFERENCES dbo.Caja (IdCaja);
GO

/* Los digitos verificadores quedan calculados con los usuarios que ya existian. */
EXEC dbo.usp_RecalcularDigitosUsuarios;
GO
