/*==============================================================================
  datos_iniciales.sql
  Datos base del sistema (catalogos, roles, permisos y usuarios de acceso).
  - Es IDEMPOTENTE: se puede correr cuantas veces se quiera. Solo inserta lo que
    falta (por clave), nunca pisa ni borra lo que cada uno ya cargo.
  - Origen: datos reales de Loguin.accdb (se descartaron filas de prueba como
    "Palta", "Carpincho", etc.).
  - Usuarios por defecto (contrasena "123", cambiarla en cualquier entorno real):
        Edu    -> grupo Gerente (todos los permisos de gestion)
        titor  -> grupo Cajero
==============================================================================*/
SET NOCOUNT ON;
GO

SET IDENTITY_INSERT dbo.TipoDoc ON;
MERGE dbo.TipoDoc AS t
USING (VALUES
    (1, N'DNI'),
    (2, N'Pasaporte')
) AS s (Id, Tipo)
   ON t.Id = s.Id
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Id, Tipo) VALUES (s.Id, s.Tipo);
SET IDENTITY_INSERT dbo.TipoDoc OFF;
GO
SET IDENTITY_INSERT dbo.Cargos ON;
MERGE dbo.Cargos AS t
USING (VALUES
    (1, N'Gerente', 0),
    (2, N'Jefe de Seccion', 0),
    (3, N'Empleado Calificado', 0),
    (4, N'Empleado', 0),
    (5, N'Administrador del Sistema', 0),
    (6, N'Sin cargo', 0)
) AS s (IdCargo, Cargo, IdGerencia)
   ON t.IdCargo = s.IdCargo
WHEN NOT MATCHED BY TARGET THEN
    INSERT (IdCargo, Cargo, IdGerencia) VALUES (s.IdCargo, s.Cargo, s.IdGerencia);
SET IDENTITY_INSERT dbo.Cargos OFF;
GO
SET IDENTITY_INSERT dbo.Grupos ON;
MERGE dbo.Grupos AS t
USING (VALUES
    (1, N'Gerente'),
    (2, N'Subgerente'),
    (3, N'Cajero'),
    (4, N'Jefe de Cocina'),
    (5, N'Propietario'),
    (6, N'Recursos Humanos')
) AS s (IdGrupo, Grupo)
   ON t.IdGrupo = s.IdGrupo
WHEN NOT MATCHED BY TARGET THEN
    INSERT (IdGrupo, Grupo) VALUES (s.IdGrupo, s.Grupo);
SET IDENTITY_INSERT dbo.Grupos OFF;
GO
SET IDENTITY_INSERT dbo.Permisos ON;
MERGE dbo.Permisos AS t
USING (VALUES
    (1, N'Alta de Usuarios'),
    (2, N'Baja de Usuarios'),
    (3, N'Modificacion de Usuarios'),
    (4, N'Administracion del Sistema'),
    (5, N'Cambios de Password'),
    (6, N'Sin cargo'),
    (7, N'Proveedores'),
    (8, N'Productos'),
    (9, N'Stock'),
    (10, N'Menu'),
    (11, N'Pedidos'),
    (12, N'Bitacora'),
    (13, N'Personal'),
    (14, N'Ubicaciones'),
    (15, N'Estadisticas')
) AS s (IdPermiso, Funcionalidad)
   ON t.IdPermiso = s.IdPermiso
WHEN NOT MATCHED BY TARGET THEN
    INSERT (IdPermiso, Funcionalidad) VALUES (s.IdPermiso, s.Funcionalidad);
SET IDENTITY_INSERT dbo.Permisos OFF;
GO
MERGE dbo.PermisosGrupos AS t
USING (VALUES
    (1, 1),
    (2, 1),
    (3, 1),
    (4, 1),
    (5, 1),
    (7, 1),
    (8, 1),
    (9, 1),
    (10, 1),
    (11, 1),
    (12, 1),
    (13, 1),
    (14, 1),
    (15, 1),
    (5, 2),
    (7, 2),
    (8, 2),
    (9, 2),
    (5, 3),
    (11, 3),
    (5, 4),
    (10, 4),
    (5, 5),
    (12, 5),
    (15, 5),
    (1, 6),
    (2, 6),
    (3, 6),
    (5, 6),
    (13, 6)
) AS s (IdPermiso, IdGrupo)
   ON t.IdPermiso = s.IdPermiso AND t.IdGrupo = s.IdGrupo
WHEN NOT MATCHED BY TARGET THEN
    INSERT (IdPermiso, IdGrupo) VALUES (s.IdPermiso, s.IdGrupo);
GO
SET IDENTITY_INSERT dbo.Provincias ON;
MERGE dbo.Provincias AS t
USING (VALUES
    (1, N'Buenos Aires'),
    (2, N'CABA'),
    (3, N'Catamarca'),
    (4, N'Chaco'),
    (5, N'Chubut'),
    (6, N'Cordoba'),
    (7, N'Corrientes'),
    (8, N'Entre Rios'),
    (9, N'Formosa'),
    (10, N'Jujuy'),
    (11, N'La Pampa'),
    (12, N'La Rioja'),
    (13, N'Mendoza'),
    (14, N'Misiones'),
    (15, N'Neuquen'),
    (16, N'Rio Negro'),
    (17, N'Salta'),
    (18, N'San Juan'),
    (19, N'San Luis'),
    (20, N'Santa Cruz'),
    (21, N'Santa Fe'),
    (22, N'Santiago del Estero'),
    (23, N'Tierra Del Fuego'),
    (24, N'Tucuman')
) AS s (IdProvincia, Provincia)
   ON t.IdProvincia = s.IdProvincia
WHEN NOT MATCHED BY TARGET THEN
    INSERT (IdProvincia, Provincia) VALUES (s.IdProvincia, s.Provincia);
SET IDENTITY_INSERT dbo.Provincias OFF;
GO
SET IDENTITY_INSERT dbo.Partidos ON;
MERGE dbo.Partidos AS t
USING (VALUES
    (1, N'Avellaneda', 1),
    (2, N'Almirante Brown', 1),
    (3, N'Berazategui', 1),
    (4, N'Esteban Echeverria', 1),
    (5, N'Florencio Varela', 1),
    (6, N'General San Martin', 1),
    (7, N'La Matanza', 1),
    (8, N'Lanus', 1),
    (9, N'Lomas de Zamora', 1),
    (10, N'Merlo', 1),
    (11, N'Moreno', 1),
    (12, N'Moron', 1),
    (13, N'Quilmes', 1),
    (14, N'San Fernando', 1),
    (15, N'San Isidro', 1),
    (16, N'Tres de Febrero', 1),
    (17, N'Tigre', 1),
    (18, N'Vicente Lopez', 1)
) AS s (IdPartido, Partido, IdProvincia)
   ON t.IdPartido = s.IdPartido
WHEN NOT MATCHED BY TARGET THEN
    INSERT (IdPartido, Partido, IdProvincia) VALUES (s.IdPartido, s.Partido, s.IdProvincia);
SET IDENTITY_INSERT dbo.Partidos OFF;
GO
SET IDENTITY_INSERT dbo.Localidades ON;
MERGE dbo.Localidades AS t
USING (VALUES
    (1, N'Remedios de Escalada', N'1826', 8),
    (2, N'Lanus', NULL, 8),
    (3, N'Gerli', NULL, 8),
    (4, N'Banfield', NULL, 9),
    (5, N'Lomas de Zamora', NULL, 9),
    (6, N'Temperley', NULL, 9),
    (7, N'Adrogue', NULL, 2),
    (8, N'Burzaco', NULL, 2),
    (9, N'Monte Grande', NULL, 4),
    (10, N'El Jaguel', NULL, 4)
) AS s (idLocalidad, Localidades, CP, idPartido)
   ON t.idLocalidad = s.idLocalidad
WHEN NOT MATCHED BY TARGET THEN
    INSERT (idLocalidad, Localidades, CP, idPartido) VALUES (s.idLocalidad, s.Localidades, s.CP, s.idPartido);
SET IDENTITY_INSERT dbo.Localidades OFF;
GO
-- Personal y usuarios de acceso por defecto
SET IDENTITY_INSERT dbo.Personal ON;
MERGE dbo.Personal AS t
USING (VALUES
    (1, N'Pinos', N'Eduardo', 1, 0, N'', 0, N'', N'', 4, 5),
    (2, N'Montecino', N'Hector', 1, 0, N'', 0, N'', N'', 8, 5)
) AS s (IdPersona, Apellido, Nombres, IdTDoc, NroDoc, Telefono, Nro, Piso, Dto, IdLocalidad, IdCargo)
   ON t.IdPersona = s.IdPersona
WHEN NOT MATCHED BY TARGET THEN
    INSERT (IdPersona, Apellido, Nombres, IdTDoc, NroDoc, Telefono, Nro, Piso, Dto, IdLocalidad, IdCargo) VALUES (s.IdPersona, s.Apellido, s.Nombres, s.IdTDoc, s.NroDoc, s.Telefono, s.Nro, s.Piso, s.Dto, s.IdLocalidad, s.IdCargo);
SET IDENTITY_INSERT dbo.Personal OFF;
GO
SET IDENTITY_INSERT dbo.Usuarios ON;
MERGE dbo.Usuarios AS t
USING (VALUES
    (1, N'Edu', N'a665a45920422f9d417e4867efdc4fb8a04a1f3fff1fa07e998e86f7f7a27ae3', 1, N'2023-01-01', 0),
    (3, N'titor', N'a665a45920422f9d417e4867efdc4fb8a04a1f3fff1fa07e998e86f7f7a27ae3', 2, N'2023-01-01', 0)
) AS s (IdUsuario, Usuario, [Password], IdPersona, FechaAlta, CambiaCada)
   ON t.IdUsuario = s.IdUsuario
WHEN NOT MATCHED BY TARGET THEN
    INSERT (IdUsuario, Usuario, [Password], IdPersona, FechaAlta, CambiaCada) VALUES (s.IdUsuario, s.Usuario, s.[Password], s.IdPersona, s.FechaAlta, s.CambiaCada);
SET IDENTITY_INSERT dbo.Usuarios OFF;
GO
MERGE dbo.UsuariosGrupos AS t
USING (VALUES
    (1, 1),
    (3, 3)
) AS s (IdUsuario, IdGrupo)
   ON t.IdUsuario = s.IdUsuario
WHEN NOT MATCHED BY TARGET THEN
    INSERT (IdUsuario, IdGrupo) VALUES (s.IdUsuario, s.IdGrupo);
GO
