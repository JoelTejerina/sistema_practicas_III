# Base de datos (SQL Server)

La aplicacion usa **SQL Server** (antes Access). Cada desarrollador trabaja contra su propia
instancia local y **la estructura y los datos base viajan por git** como scripts, asi los tres
tenemos siempre la misma base sin pasarnos archivos `.accdb`.

## Puesta en marcha (una sola vez por maquina)

1. Instalar **SQL Server Express** (y SSMS si queres ver las tablas): ver links en el README de la raiz.
2. Ejecutar `Database\Instalar-BaseDeDatos.bat` (doble clic).
   - Con datos de ejemplo de Compras/Ventas: `Instalar-BaseDeDatos.bat -Demo`
   - Otra instancia o nombre de base: `Instalar-BaseDeDatos.bat -Servidor "localhost\SQLEXPRESS" -BaseDeDatos MiBase`
   - Autenticacion SQL en vez de Windows: agregar `-Usuario sa -Clave ****`
3. Abrir `SistemaLoguin.sln` en Visual Studio y ejecutar. Usuario de prueba: **Edu / 123**.

El script crea la base `SistemaPracticas`, aplica el esquema, carga los datos base y genera
`conexion.local.txt` en la raiz del repo (**no se sube a git**) con tu cadena de conexion.

## Como encuentra la app la conexion

Orden de prioridad (ver `CapaAccesoDatos/clsConexion.cs`):

1. Variable de entorno `SISTEMA_DB_CONNECTION`
2. Archivo `conexion.local.txt` (junto al .exe o en cualquier carpeta padre, p. ej. la raiz del repo)
3. `App.config` -> `connectionStrings` -> `SistemaDB` (por defecto `.\SQLEXPRESS`, base `SistemaPracticas`)

Si el equipo prefiere **un unico servidor compartido**, alcanza con que cada uno ponga en su
`conexion.local.txt` la cadena de ese servidor; el codigo no cambia.

## Trabajo diario: cambios en la estructura

**Regla de oro: nunca se edita una migracion que ya esta en git.** Para cambiar la base:

1. Crear un archivo nuevo en `Database/migraciones/` con el siguiente numero:
   `002_agregar_columna_x.sql`, `003_...` (si dos personas toman el mismo numero, el segundo en
   hacer merge lo renumera).
2. Escribir el cambio (`ALTER TABLE`, `CREATE TABLE`, etc.). Usar `GO` para separar lotes.
3. Probarlo corriendo `Instalar-BaseDeDatos.bat` (solo aplica lo que falta).
4. Commitear el `.sql` junto con el codigo C# que lo usa.
5. Los otros, despues de `git pull`, corren `Instalar-BaseDeDatos.bat` y quedan al dia.

El script registra cada migracion aplicada en la tabla `dbo.SchemaVersion`
(quien y cuando). Cada migracion corre dentro de una transaccion: si falla, no queda a medias.

Ejemplo `002_proveedor_activo.sql`:

```sql
ALTER TABLE dbo.Proveedor ADD Activo BIT NOT NULL CONSTRAINT DF_Proveedor_Activo DEFAULT (1);
GO
```

## Datos

- `datos/datos_iniciales.sql`: catalogos (provincias, partidos, localidades, tipos de documento,
  cargos), grupos, permisos y usuarios por defecto. Es **idempotente**: se corre siempre y solo
  inserta lo que falta, nunca pisa lo que cargaste.
- `datos/datos_demo.sql`: ejemplos de proveedores, productos, stock y carta (solo con `-Demo`).
- Si se agrega un permiso/rol nuevo que todos necesitan, va en una migracion o en `datos_iniciales.sql`.

## Empezar de cero

`Instalar-BaseDeDatos.bat -Recrear` borra **tu** base local y la arma de nuevo (pide confirmacion).

## Notas

- Intercalacion (collation) `Latin1_General_100_CI_AI`: las busquedas no distinguen mayusculas ni acentos.
- Contraseñas: se guardan como hash SHA-256; las de los usuarios por defecto son `123`.
- Las claves foraneas ahora se **hacen cumplir**: no se puede borrar, por ejemplo, un proveedor
  con productos. Antes Access lo permitia en las tablas sin relacion.
- `Loguin.accdb` y `Loguin - Copia.accdb` quedan solo como respaldo historico; la app ya no los usa.
