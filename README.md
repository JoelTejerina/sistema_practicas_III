# sistema_practicas_III

## Base de datos: SQL Server (reemplaza a Access)

La app ahora usa **SQL Server**. Cada uno trabaja con su instancia local y la estructura/datos
base se versionan en git (carpeta `Database/`).

1. Instalar SQL Server Express (links mas abajo).
2. Ejecutar `Database\Instalar-BaseDeDatos.bat` (agregar `-Demo` para datos de ejemplo).
3. Abrir `SistemaLoguin.sln` y ejecutar. Ingresar con alguno de los usuarios de prueba (ver abajo).
4. Despues de cada `git pull`, volver a correr el `.bat`: aplica las migraciones nuevas de otros.

## Usuarios de prueba (uno por rol)

Son datos de **demostracion** para probar el sistema, no cuentas reales. Todos tienen la misma
contraseña: `123`. Cambiala en cualquier entorno real. Excepto `Edu` y `titor`, que vienen con la
instalacion base, los demas se cargan al instalar con `-Demo`
(`Database\Instalar-BaseDeDatos.bat -Demo`).

| Rol | Usuario | Contraseña | Que puede hacer |
|---|---|---|---|
| Gerente | `Edu` | `123` | Todo el sistema: Compras (Proveedores, Productos, Stock), Elaboracion, usuarios y copia de respaldo |
| Cajero | `titor` (tambien `lgomez`) | `123` | Pedidos / Facturacion |
| Jefe de Cocina | `mfernandez` | `123` | Menu (recetas) y Elaboracion |
| Propietario | `propietario` | `123` | Estadisticas, Bitacora y administracion de usuarios (Registrar usuario, Asignar rol, Gestion de Personal) |
| Jefe de Salon | `jefesalon` | `123` | Menu (consulta) y Pedidos |

Todos pueden cambiar su contraseña desde **Usuarios > Cambio de Password**.

Los roles **Subgerente** y **Recursos Humanos** ya no existen (migracion `003`): las compras las hace el
Gerente y la administracion de usuarios, roles y personal la hacen el Gerente y el Propietario. Los
usuarios que tenian esos roles quedan **dados de baja** (desactivados, sin borrarse).

Detalle completo (como cambiar la estructura sin pisarnos, conexion a otro servidor, etc.) en
[`Database/README.md`](Database/README.md).

Instalar SQL Server: https://www.youtube.com/watch?v=mA1qoWdNCOE&ab_channel=SergioAlejandroCampos-EXCELeINFO

SQL Server Express: https://www.microsoft.com/es-ar/sql-server/sql-server-downloads
SQL Server Management Studio: https://learn.microsoft.com/es-es/sql/ssms/download-sql-server-management-studio-ssms?view=sql-server-ver16

/**********************************************************************************************\

Github: https://github.com/
Crear cuenta: https://github.com/signup?ref_cta=Sign+up&ref_loc=header+logged+out&ref_page=%2F&source=header-home

/**********************************************************************************************\

Controlador de versionado: https://git-scm.com/downloads
Controlar que funcione correctamente: En simbolo del sistema (terminal) git version

/**********************************************************************************************\
Configurar usuario en git: TODO ESTO DESDE UNA CONSOLA (simbolo del sistema | ejemplo; C:\Users\Joel>git config --global user.name "John Doe") 

git config --global user.name "John Doe" (Nombre en github)
git config --global user.email johndoe@example.com (Email que utilizamos para crear cuenta en github)

/**********************************************************************************************\

1) Crearse una nueva rama en github:
https://docs.github.com/es/pull-requests/collaborating-with-pull-requests/proposing-changes-to-your-work-with-pull-requests/creating-and-deleting-branches-within-your-repository

ejemplo nombre de rama: joel/pedidos

/**********************************************************************************************\

2) Clonar/descargar el repositorio: ESTO DESDE UNA CONSOLA (simbolo del sistema | ejemplo: C:\Users\Joel>git clone <URLrepositorio>) 

git clone <URLrepositorio>

/**********************************************************************************************\
3) Subir cambios al repositorio: TODO ESTO DESDE UNA CONSOLA (simbolo del sistema |ejemplo: C:\Users\Joel\nuestroProyectoClonado>git add .) 

git add .
git commit -m "Nombre del commit";
git push origin <nombreDeLaRama>


/**********************************************************************************************\
Obtener los cambios del repositorio: ESTO DESDE UNA CONSOLA (simbolo del sistema | ejemplo: C:\Users\Joel\nuestroProyectoClonado>git pull origin <nombreDeLaRama>)  

git pull origin <nombreDeLaRama>
