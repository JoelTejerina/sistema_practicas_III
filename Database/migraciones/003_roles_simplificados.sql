/*==============================================================================
  003_roles_simplificados.sql
  Se eliminan los roles Subgerente y Recursos Humanos:
   - Compras (proveedores, productos y stock) pasa a ser del Gerente.
   - La administracion de usuarios, roles y personal la hacen el Gerente y el Propietario
     (los permisos del Propietario se cargan en datos_iniciales.sql).
  Siguen existiendo Gerente, Propietario, Cajero, Jefe de Cocina y Jefe de Salon.

  Los usuarios que tenian alguno de los roles eliminados se DAN DE BAJA (no se borran, para
  conservar su historial en la bitacora): quedan desactivados y ya no pueden ingresar.
==============================================================================*/
SET NOCOUNT ON;
GO

/* 1. Baja de los usuarios de los roles que se eliminan */
UPDATE u
SET UsuarioDesactivado = 1,
    FechaBaja          = ISNULL(u.FechaBaja, CAST(GETDATE() AS DATE)),
    FechaDesactivacion = CAST(GETDATE() AS DATE)
FROM dbo.Usuarios u
INNER JOIN dbo.UsuariosGrupos ug ON ug.IdUsuario = u.IdUsuario
INNER JOIN dbo.Grupos g          ON g.IdGrupo    = ug.IdGrupo
WHERE g.Grupo IN (N'Subgerente', N'Recursos Humanos');
GO

/* 2. Se quitan las asignaciones, los permisos y los roles */
DELETE ug
FROM dbo.UsuariosGrupos ug
INNER JOIN dbo.Grupos g ON g.IdGrupo = ug.IdGrupo
WHERE g.Grupo IN (N'Subgerente', N'Recursos Humanos');

DELETE pg
FROM dbo.PermisosGrupos pg
INNER JOIN dbo.Grupos g ON g.IdGrupo = pg.IdGrupo
WHERE g.Grupo IN (N'Subgerente', N'Recursos Humanos');

DELETE FROM dbo.Grupos WHERE Grupo IN (N'Subgerente', N'Recursos Humanos');
GO
