/*==============================================================================
  001_esquema_inicial.sql
  Esquema inicial (migrado desde Loguin.accdb de Access a SQL Server).
  - Se ejecuta UNA sola vez por base (lo registra dbo.SchemaVersion).
  - NO editar este archivo una vez que alguien lo aplico: para cambios de
    estructura crear una migracion nueva (002_..., 003_..., ver Database/README.md)
==============================================================================*/

/* ---------- Ubicaciones ---------------------------------------------------- */
CREATE TABLE dbo.Provincias (
    IdProvincia   INT IDENTITY(1,1) NOT NULL,
    Provincia     NVARCHAR(100)     NOT NULL,
    CONSTRAINT PK_Provincias PRIMARY KEY (IdProvincia),
    CONSTRAINT UQ_Provincias_Provincia UNIQUE (Provincia)
);
GO

CREATE TABLE dbo.Partidos (
    IdPartido     INT IDENTITY(1,1) NOT NULL,
    Partido       NVARCHAR(100)     NOT NULL,
    IdProvincia   INT               NOT NULL,
    CONSTRAINT PK_Partidos PRIMARY KEY (IdPartido),
    CONSTRAINT FK_Partidos_Provincias FOREIGN KEY (IdProvincia) REFERENCES dbo.Provincias (IdProvincia)
);
CREATE INDEX IX_Partidos_IdProvincia ON dbo.Partidos (IdProvincia);
GO

CREATE TABLE dbo.Localidades (
    idLocalidad   INT IDENTITY(1,1) NOT NULL,
    Localidades   NVARCHAR(100)     NOT NULL,
    CP            NVARCHAR(10)      NULL,
    idPartido     INT               NOT NULL,
    CONSTRAINT PK_Localidades PRIMARY KEY (idLocalidad),
    CONSTRAINT FK_Localidades_Partidos FOREIGN KEY (idPartido) REFERENCES dbo.Partidos (IdPartido)
);
CREATE INDEX IX_Localidades_idPartido ON dbo.Localidades (idPartido);
GO

/* ---------- Personal ------------------------------------------------------- */
CREATE TABLE dbo.TipoDoc (
    Id    INT IDENTITY(1,1) NOT NULL,
    Tipo  NVARCHAR(50)      NOT NULL,
    CONSTRAINT PK_TipoDoc PRIMARY KEY (Id),
    CONSTRAINT UQ_TipoDoc_Tipo UNIQUE (Tipo)
);
GO

CREATE TABLE dbo.Cargos (
    IdCargo     INT IDENTITY(1,1) NOT NULL,
    Cargo       NVARCHAR(100)     NOT NULL,
    IdGerencia  INT               NOT NULL CONSTRAINT DF_Cargos_IdGerencia DEFAULT (0),
    CONSTRAINT PK_Cargos PRIMARY KEY (IdCargo),
    CONSTRAINT UQ_Cargos_Cargo UNIQUE (Cargo)
);
GO

CREATE TABLE dbo.Personal (
    IdPersona    INT IDENTITY(1,1) NOT NULL,
    Apellido     NVARCHAR(100)     NOT NULL,
    Nombres      NVARCHAR(100)     NOT NULL,
    IdTDoc       INT               NOT NULL CONSTRAINT DF_Personal_IdTDoc DEFAULT (1),
    NroDoc       INT               NOT NULL CONSTRAINT DF_Personal_NroDoc DEFAULT (0),
    CuitCuil     NVARCHAR(20)      NULL,
    Telefono     NVARCHAR(30)      NULL,
    Correo       NVARCHAR(150)     NULL,
    Calle        NVARCHAR(150)     NULL,
    Nro          INT               NOT NULL CONSTRAINT DF_Personal_Nro DEFAULT (0),
    Piso         NVARCHAR(10)      NULL,
    Dto          NVARCHAR(10)      NULL,
    IdLocalidad  INT               NOT NULL,
    IdCargo      INT               NOT NULL CONSTRAINT DF_Personal_IdCargo DEFAULT (6),
    CONSTRAINT PK_Personal PRIMARY KEY (IdPersona),
    CONSTRAINT FK_Personal_TipoDoc     FOREIGN KEY (IdTDoc)      REFERENCES dbo.TipoDoc (Id),
    CONSTRAINT FK_Personal_Localidades FOREIGN KEY (IdLocalidad) REFERENCES dbo.Localidades (idLocalidad),
    CONSTRAINT FK_Personal_Cargos      FOREIGN KEY (IdCargo)     REFERENCES dbo.Cargos (IdCargo)
);
/* Un mismo documento no puede repetirse (NroDoc = 0 significa "sin dato") */
CREATE UNIQUE INDEX UX_Personal_Documento ON dbo.Personal (IdTDoc, NroDoc) WHERE NroDoc > 0;
CREATE INDEX IX_Personal_IdLocalidad ON dbo.Personal (IdLocalidad);
CREATE INDEX IX_Personal_IdCargo     ON dbo.Personal (IdCargo);
GO

/* ---------- Seguridad: usuarios, grupos y permisos ------------------------- */
CREATE TABLE dbo.Usuarios (
    IdUsuario           INT IDENTITY(1,1) NOT NULL,
    Usuario             NVARCHAR(50)      NOT NULL,
    [Password]          NVARCHAR(256)     NOT NULL,   -- hash SHA-256 en hexadecimal
    IdPersona           INT               NOT NULL,
    FechaAlta           DATE              NULL,
    FechaBaja           DATE              NULL,
    CambiaCada          INT               NOT NULL CONSTRAINT DF_Usuarios_CambiaCada DEFAULT (0),
    FechaUltimoCambio   DATE              NULL,
    UsuarioDesactivado  BIT               NOT NULL CONSTRAINT DF_Usuarios_Desactivado DEFAULT (0),
    FechaDesactivacion  DATE              NULL,
    CONSTRAINT PK_Usuarios PRIMARY KEY (IdUsuario),
    CONSTRAINT UQ_Usuarios_Usuario UNIQUE (Usuario),
    CONSTRAINT UQ_Usuarios_IdPersona UNIQUE (IdPersona),   -- una persona = un usuario
    CONSTRAINT FK_Usuarios_Personal FOREIGN KEY (IdPersona) REFERENCES dbo.Personal (IdPersona)
);
GO

CREATE TABLE dbo.Grupos (
    IdGrupo  INT IDENTITY(1,1) NOT NULL,
    Grupo    NVARCHAR(100)     NOT NULL,
    CONSTRAINT PK_Grupos PRIMARY KEY (IdGrupo),
    CONSTRAINT UQ_Grupos_Grupo UNIQUE (Grupo)
);
GO

CREATE TABLE dbo.UsuariosGrupos (
    IdUsuario  INT NOT NULL,
    IdGrupo    INT NOT NULL,
    CONSTRAINT PK_UsuariosGrupos PRIMARY KEY (IdUsuario),   -- un usuario tiene un solo rol
    CONSTRAINT FK_UsuariosGrupos_Usuarios FOREIGN KEY (IdUsuario) REFERENCES dbo.Usuarios (IdUsuario) ON DELETE CASCADE,
    CONSTRAINT FK_UsuariosGrupos_Grupos   FOREIGN KEY (IdGrupo)   REFERENCES dbo.Grupos (IdGrupo)
);
GO

CREATE TABLE dbo.Permisos (
    IdPermiso      INT IDENTITY(1,1) NOT NULL,
    Funcionalidad  NVARCHAR(100)     NOT NULL,
    CONSTRAINT PK_Permisos PRIMARY KEY (IdPermiso),
    CONSTRAINT UQ_Permisos_Funcionalidad UNIQUE (Funcionalidad)
);
GO

CREATE TABLE dbo.PermisosGrupos (
    IdPermiso  INT NOT NULL,
    IdGrupo    INT NOT NULL,
    CONSTRAINT PK_PermisosGrupos PRIMARY KEY (IdPermiso, IdGrupo),
    CONSTRAINT FK_PermisosGrupos_Permisos FOREIGN KEY (IdPermiso) REFERENCES dbo.Permisos (IdPermiso),
    CONSTRAINT FK_PermisosGrupos_Grupos   FOREIGN KEY (IdGrupo)   REFERENCES dbo.Grupos (IdGrupo)
);
GO

CREATE TABLE dbo.PermisosUsuarios (
    IdPermisoUsuario  INT IDENTITY(1,1) NOT NULL,
    IdUsuario         INT  NOT NULL,
    IdPermiso         INT  NOT NULL,
    FechaAlta         DATE NULL,
    FechaBaja         DATE NULL,
    AltaProvisoria    DATE NULL,
    CONSTRAINT PK_PermisosUsuarios PRIMARY KEY (IdPermisoUsuario),
    CONSTRAINT FK_PermisosUsuarios_Usuarios FOREIGN KEY (IdUsuario) REFERENCES dbo.Usuarios (IdUsuario) ON DELETE CASCADE,
    CONSTRAINT FK_PermisosUsuarios_Permisos FOREIGN KEY (IdPermiso) REFERENCES dbo.Permisos (IdPermiso)
);
CREATE INDEX IX_PermisosUsuarios_IdUsuario ON dbo.PermisosUsuarios (IdUsuario);
GO

/* ---------- Bitacora (auditoria) ------------------------------------------- */
CREATE TABLE dbo.Bitacora (
    IdEvento   INT IDENTITY(1,1) NOT NULL,
    Fecha      DATE           NOT NULL,
    Hora       TIME(0)        NOT NULL,
    IdUsuario  INT            NOT NULL CONSTRAINT DF_Bitacora_IdUsuario DEFAULT (0),  -- sin FK a proposito: se loguean intentos de usuarios inexistentes
    Usuario    NVARCHAR(200)  NULL,
    Evento     NVARCHAR(200)  NULL,
    Detalle    NVARCHAR(MAX)  NULL,
    Origen     NVARCHAR(200)  NULL,
    CONSTRAINT PK_Bitacora PRIMARY KEY (IdEvento)
);
CREATE INDEX IX_Bitacora_Fecha ON dbo.Bitacora (Fecha DESC, Hora DESC);
GO

/* ---------- Compras -------------------------------------------------------- */
CREATE TABLE dbo.Proveedor (
    IdProveedor      INT IDENTITY(1,1) NOT NULL,
    Nombre           NVARCHAR(100)     NOT NULL,
    RazonSocial      NVARCHAR(100)     NULL,
    Categoria        NVARCHAR(50)      NULL,
    CUIT             NVARCHAR(20)      NULL,
    CertificadoAfip  NVARCHAR(50)      NULL,
    Direccion        NVARCHAR(150)     NULL,
    Email            NVARCHAR(100)     NULL,
    Telefono         NVARCHAR(30)      NULL,
    CONSTRAINT PK_Proveedor PRIMARY KEY (IdProveedor)
);
GO

CREATE TABLE dbo.Producto (
    IdProducto   INT IDENTITY(1,1) NOT NULL,
    Nombre       NVARCHAR(100)     NOT NULL,
    Descripcion  NVARCHAR(255)     NULL,
    Marca        NVARCHAR(100)     NULL,
    Categoria    NVARCHAR(50)      NULL,
    Medida       NVARCHAR(50)      NULL,
    IdProveedor  INT               NOT NULL,
    CONSTRAINT PK_Producto PRIMARY KEY (IdProducto),
    CONSTRAINT FK_Producto_Proveedor FOREIGN KEY (IdProveedor) REFERENCES dbo.Proveedor (IdProveedor)
);
CREATE INDEX IX_Producto_IdProveedor ON dbo.Producto (IdProveedor);
GO

CREATE TABLE dbo.Stock (
    IdStock           INT IDENTITY(1,1) NOT NULL,
    IdProducto        INT               NOT NULL,
    NumeroLote        NVARCHAR(50)      NOT NULL,
    Cantidad          INT               NOT NULL,
    FechaVencimiento  DATE              NULL,
    Precio            DECIMAL(18,2)     NOT NULL CONSTRAINT DF_Stock_Precio DEFAULT (0),
    CONSTRAINT PK_Stock PRIMARY KEY (IdStock),
    CONSTRAINT FK_Stock_Producto FOREIGN KEY (IdProducto) REFERENCES dbo.Producto (IdProducto)
);
CREATE INDEX IX_Stock_IdProducto ON dbo.Stock (IdProducto);
GO

/* ---------- Ventas --------------------------------------------------------- */
CREATE TABLE dbo.Menu (
    IdMenu             INT IDENTITY(1,1) NOT NULL,
    Nombre             NVARCHAR(100)     NOT NULL,
    Descripcion        NVARCHAR(255)     NULL,
    Categoria          NVARCHAR(50)      NULL,
    Ingredientes       NVARCHAR(255)     NULL,
    Precio             DECIMAL(18,2)     NOT NULL CONSTRAINT DF_Menu_Precio DEFAULT (0),
    Region             NVARCHAR(50)      NULL,
    Temporada          NVARCHAR(50)      NULL,
    Popularidad        NVARCHAR(50)      NULL,
    TiempoPreparacion  INT               NOT NULL CONSTRAINT DF_Menu_Tiempo DEFAULT (0),
    TipoEvento         NVARCHAR(50)      NULL,
    IdStock            INT               NOT NULL CONSTRAINT DF_Menu_IdStock DEFAULT (0),  -- 0 = sin stock asociado (por eso sin FK)
    CONSTRAINT PK_Menu PRIMARY KEY (IdMenu)
);
GO

CREATE TABLE dbo.Pedido (
    IdPedido                 INT IDENTITY(1,1) NOT NULL,
    NombreCliente            NVARCHAR(100)     NOT NULL,
    IdMenu                   INT               NOT NULL,
    Cantidad                 INT               NOT NULL,
    FormaPago                NVARCHAR(50)      NULL,
    InstruccionesEspeciales  NVARCHAR(255)     NULL,
    Estado                   NVARCHAR(30)      NOT NULL,
    PrecioTotal              DECIMAL(18,2)     NOT NULL CONSTRAINT DF_Pedido_PrecioTotal DEFAULT (0),
    Fecha                    DATE              NOT NULL,
    CONSTRAINT PK_Pedido PRIMARY KEY (IdPedido),
    CONSTRAINT FK_Pedido_Menu FOREIGN KEY (IdMenu) REFERENCES dbo.Menu (IdMenu)
);
CREATE INDEX IX_Pedido_Fecha ON dbo.Pedido (Fecha);
CREATE INDEX IX_Pedido_IdMenu ON dbo.Pedido (IdMenu);
GO
