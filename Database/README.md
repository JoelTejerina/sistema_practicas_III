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
- `datos/datos_demo.sql`: datos de ejemplo (solo con `-Demo`, idempotente): 10 proveedores con CUIT validos, 35 productos,
  37 lotes de stock, 21 platillos con sus recetas (69 ingredientes), 36 pedidos (historicos y de hoy), 4 cajas cerradas y
  movimientos de stock. Incluye casos para probar: lotes bajo el minimo (mozzarella, lechuga), sobre el maximo (huevos),
  leche con dos lotes (FIFO), un lote de salmon vencido y un platillo no disponible (Costillas BBQ).
- Si se agrega un permiso/rol nuevo que todos necesitan, va en una migracion o en `datos_iniciales.sql`.

## Empezar de cero

`Instalar-BaseDeDatos.bat -Recrear` borra **tu** base local y la arma de nuevo (pide confirmacion).

## Notas

- Intercalacion (collation) `Latin1_General_100_CI_AI`: las busquedas no distinguen mayusculas ni acentos.
- Contraseñas: se guardan como hash SHA-256; las de los usuarios por defecto son `123`.
- Usuarios de prueba, uno por rol (todos con contraseña `123`; los que no son Edu/titor vienen con `-Demo`):

  | Rol | Usuario |
  |---|---|
  | Gerente | `Edu` |
  | Cajero | `titor` (también `lgomez`) |
  | Jefe de Cocina | `mfernandez` |
  | Propietario | `propietario` (tambien administra usuarios) |
  | Jefe de Salon | `jefesalon` |
- `migraciones/002_ajustes_alcance.sql` agrega lo que pide el documento de alcance: recetas por
  platillo, detalle de pedidos (varios platillos), movimientos de stock (trazabilidad), stock
  minimo/maximo, baja logica de Producto/Proveedor/Menu, fecha de nacimiento e idioma por usuario.
  Con eso el pedido verifica el stock, descuenta los insumos y los devuelve al cancelar.
- `migraciones/003_roles_simplificados.sql` elimina los roles Subgerente y Recursos Humanos: Compras pasa al
  Gerente y la administracion de usuarios, roles y personal al Gerente y al Propietario. Los usuarios que
  tenian esos roles **se dan de baja** (desactivados con fecha de baja; no se borran para conservar su historial).
- `migraciones/004_seguridad_y_ventas.sql` agrega la seguridad y las ventas del alcance: digito verificador de
  `Usuarios` (`DVH` por fila + `DigitoVerificador`, con `usp_RecalcularDigitosUsuarios` / `usp_VerificarDigitosUsuarios`),
  bloqueo de terminal (`BloqueoTerminal`), sesion unica (`SesionActiva`), cambio de clave obligatorio
  (`Usuarios.DebeCambiarPassword`), cantidad inicial del producto, proveedor y lote en `Stock`, disponibilidad del
  platillo (`Menu.Disponible`), `Caja` (una sola abierta a la vez) y canal, mesa, caja y fecha de cobro en `Pedido`.
  **Todo codigo o script que escriba en `Usuarios` debe terminar con `EXEC usp_RecalcularDigitosUsuarios`**
  (la aplicacion lo hace sola; los seeds tambien).
- La demo trae una caja cerrada de ejemplo y ninguna abierta: para cobrar hay que abrirla desde la aplicacion.
- Las claves foraneas ahora se **hacen cumplir**: no se puede borrar, por ejemplo, un proveedor
  con productos. Antes Access lo permitia en las tablas sin relacion.
- `Loguin.accdb` y `Loguin - Copia.accdb` quedan solo como respaldo historico; la app ya no los usa.
