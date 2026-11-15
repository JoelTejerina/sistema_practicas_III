namespace CapaVistaUsuario
{
    /// <summary>Un tema de la ayuda en linea.</summary>
    public class TemaAyuda
    {
        public string Titulo { get; set; }
        public string Texto { get; set; }

        public override string ToString()
        {
            return Titulo;
        }
    }

    /// <summary>
    /// Contenido de la ayuda en linea (menu Ayuda o tecla F1). Es el mismo contenido del manual de usuario del
    /// documento del proyecto; esta en español porque es el idioma de trabajo del restaurante.
    /// </summary>
    public static class CV_Ayuda
    {
        public static readonly TemaAyuda[] Temas =
        {
            new TemaAyuda
            {
                Titulo = "Primeros pasos",
                Texto =
                    "Al abrir el sistema aparece la pantalla de ingreso: escriba su usuario y su contraseña y presione Ingresar.\r\n\r\n" +
                    "El menú principal muestra solo lo que su rol puede usar. Un módulo que aparece en gris o con los botones " +
                    "deshabilitados es de consulta: se puede ver la información pero no modificarla.\r\n\r\n" +
                    "Roles: Gerente (todo el sistema), Propietario (usuarios, bitácora, estadísticas), Cajero (pedidos y caja), " +
                    "Jefe de Cocina (carta, recetas y elaboración) y Jefe de Salón (pedidos y consulta de la carta).\r\n\r\n" +
                    "Los campos obligatorios que se dejan vacíos se marcan en rojo al guardar. Cada pantalla tiene un buscador " +
                    "que filtra la grilla a medida que usted escribe."
            },
            new TemaAyuda
            {
                Titulo = "Iniciar y cerrar sesión",
                Texto =
                    "Ingreso: escriba usuario y contraseña. Con tres intentos fallidos el usuario se bloquea y debe pedir al " +
                    "administrador que lo desbloquee.\r\n\r\n" +
                    "Solo se puede tener una sesión abierta por usuario. Si ya hay otra, el sistema informa en qué terminal está.\r\n\r\n" +
                    "¿Olvidó su contraseña? En la pantalla de ingreso elija “¿Olvidó su contraseña?”, escriba su usuario y el " +
                    "sistema le envía una contraseña temporal a su correo. Al ingresar con ella el sistema le pide elegir una nueva.\r\n\r\n" +
                    "Para salir: Mantenimiento > Usuarios > Cerrar sesión (vuelve a la pantalla de ingreso) o Mantenimiento > Salir."
            },
            new TemaAyuda
            {
                Titulo = "Cambiar la contraseña y el idioma",
                Texto =
                    "Contraseña: Mantenimiento > Usuarios > Cambio de Password. Ingrese la contraseña actual, la nueva y repita la nueva.\r\n\r\n" +
                    "Idioma: en el menú Idioma elija español, inglés o portugués. El sistema recuerda su elección para el próximo ingreso."
            },
            new TemaAyuda
            {
                Titulo = "Usuarios, roles y permisos",
                Texto =
                    "Lo administran el Propietario y el Gerente.\r\n\r\n" +
                    "Alta: Mantenimiento > Usuarios > Registrar usuario (datos de la persona, usuario, contraseña y rol).\r\n" +
                    "Baja y reactivación, modificación y restablecer contraseña: Mantenimiento de usuarios. Nadie puede darse de " +
                    "baja a sí mismo ni dar de baja al último Gerente.\r\n" +
                    "Roles y permisos: elija un rol, tilde los permisos y presione Guardar permisos (los del Gerente no se modifican).\r\n" +
                    "Asignar rol: elija el usuario y el rol y presione Guardar rol."
            },
            new TemaAyuda
            {
                Titulo = "Compras: proveedores, productos y stock",
                Texto =
                    "Solo el Gerente modifica Compras.\r\n\r\n" +
                    "Para agregar un registro presione Agregar, complete los campos y presione Guardar. Para modificar, elija el " +
                    "registro en la grilla, presione Modificar, cambie los datos y presione Guardar cambios. Eliminar da de baja " +
                    "el registro (no se pierde el historial). Cancelar descarta lo que está editando.\r\n\r\n" +
                    "Proveedores: el CUIT debe ser válido y no repetirse, y el correo debe llevar @.\r\n" +
                    "Productos: no se puede repetir el mismo producto. Si indica cantidad inicial se crea el primer lote.\r\n" +
                    "Stock: cada ingreso es un lote con vencimiento, stock mínimo y máximo. Al abrir Stock se avisan los lotes bajo el " +
                    "mínimo. “Ver historial” muestra todos los movimientos (ingresos, egresos por pedido y ajustes)."
            },
            new TemaAyuda
            {
                Titulo = "Pedidos y cobro",
                Texto =
                    "Alta: presione Agregar, escriba el cliente, elija el canal (Salón, Barra o Delivery; en Salón también la mesa) " +
                    "y agregue los platillos con su cantidad. Al guardar se muestra un resumen para confirmar. El sistema descuenta " +
                    "los insumos de la receta; si falta stock informa qué falta y no guarda el pedido.\r\n\r\n" +
                    "Modificar: cambia los datos y el detalle (los insumos se reajustan). Cancelar pedido: solo si está Pendiente; " +
                    "devuelve los insumos al stock. Eliminar: baja del pedido.\r\n\r\n" +
                    "Cobrar: requiere una caja abierta (ver Caja). Un pedido cobrado o cancelado no se vuelve a cobrar.\r\n" +
                    "Ticket: imprime el comprobante del pedido. No es una factura: el sistema no emite facturas."
            },
            new TemaAyuda
            {
                Titulo = "Carta y recetas",
                Texto =
                    "Carta (Menú): alta, modificación y baja de platillos. “Disponible / No disponible” oculta un platillo sin borrarlo " +
                    "(no se puede pedir mientras no esté disponible). “Actualizar precios” aplica un porcentaje (entre -90 y 300) a toda " +
                    "la carta o a una categoría.\r\n\r\n" +
                    "Receta: desde Menú elija el platillo y presione Receta para cargar los ingredientes y cantidades; marque el " +
                    "ingrediente principal. “Finalizar receta” pide confirmar el resumen.\r\n\r\n" +
                    "Buscar receta (Ventas > Buscar receta): filtre por ingrediente, región, categoría, popularidad, temporada, " +
                    "tipo de evento, precio y tiempo, y vea cuántos platos se pueden preparar con el stock actual."
            },
            new TemaAyuda
            {
                Titulo = "Elaboración",
                Texto =
                    "Es la pantalla de cocina: lista los pedidos Pendientes y En curso con su detalle e instrucciones.\r\n\r\n" +
                    "Elija un pedido pendiente y presione Iniciar elaboración; cuando esté preparado, presione Marcar como listo. " +
                    "Cancelar elaboración devuelve los insumos al stock."
            },
            new TemaAyuda
            {
                Titulo = "Caja",
                Texto =
                    "Abrir caja: ingrese el monto inicial y presione Abrir caja. Solo puede haber una caja abierta y no se puede cobrar " +
                    "sin ella.\r\n\r\n" +
                    "Con la caja abierta se ve el resumen por forma de pago y las cuentas por mesa.\r\n\r\n" +
                    "Cerrar caja: cuente el efectivo, ingréselo (y observaciones si hace falta) y presione Cerrar caja. El sistema " +
                    "muestra el efectivo esperado, el contado y la diferencia, y pide confirmar. El historial de cajas queda en la misma pantalla."
            },
            new TemaAyuda
            {
                Titulo = "Estadísticas y bitácora",
                Texto =
                    "Estadísticas: elija desde y hasta, cómo agrupar (día, semana o mes) y presione Consultar. Muestra las ventas por " +
                    "período y los platos más vendidos.\r\n\r\n" +
                    "Bitácora (Administración > Bitácora): registro de todo lo que hace cada usuario (fecha, hora, usuario, actividad " +
                    "y detalle). Se puede filtrar con el buscador."
            },
            new TemaAyuda
            {
                Titulo = "Copia de respaldo",
                Texto =
                    "Administración > Copia de respaldo (Gerente). El sistema propone un nombre con fecha y hora y una carpeta; elija la " +
                    "ubicación y confirme. Se genera un archivo .bak de la base de datos y se informa la ruta.\r\n\r\n" +
                    "La restauración no se hace desde la aplicación: se realiza con las herramientas de SQL Server."
            },
            new TemaAyuda
            {
                Titulo = "Mensajes frecuentes",
                Texto =
                    "“Usuario inexistente” / “Ingreso erróneo”: revise usuario y contraseña.\r\n" +
                    "“Terminal bloqueada”: la terminal se bloquea 10 minutos cuando se detecta una alteración de los usuarios; espere o avise al Gerente.\r\n" +
                    "“Sesión ya abierta”: el usuario está en otra terminal; ciérrela o espere 15 minutos sin actividad.\r\n" +
                    "“Debe abrir la caja”: abra la caja (Ventas > Caja) antes de cobrar.\r\n" +
                    "“Stock insuficiente”: el mensaje indica qué insumo falta; cargue stock o modifique el pedido.\r\n" +
                    "“No está autorizado”: su rol no tiene ese permiso; consulte al Gerente."
            }
        };
    }
}
