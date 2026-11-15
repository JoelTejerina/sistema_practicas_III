using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CapaComun;
using CapaLogicaNegocio;
using CapaLogicaNegocio.Administrador;
using CapaLogicaNegocio.Loguin;


namespace CapaVistaUsuario
{
    public partial class frmPrincipal : Form
    {
        // Entradas de menu del alcance: Elaboracion (cocina) y Copia de respaldo (backup).
        private readonly ToolStripMenuItem elaboracionToolStripMenuItem = new ToolStripMenuItem { Name = "elaboracionToolStripMenuItem" };
        private readonly ToolStripMenuItem backupToolStripMenuItem = new ToolStripMenuItem { Name = "backupToolStripMenuItem" };
        private readonly ToolStripMenuItem cajaToolStripMenuItem = new ToolStripMenuItem { Name = "cajaToolStripMenuItem" };
        private readonly ToolStripMenuItem buscarRecetaToolStripMenuItem = new ToolStripMenuItem { Name = "buscarRecetaToolStripMenuItem" };

        private readonly ToolStripMenuItem ayudaToolStripMenuItem = new ToolStripMenuItem { Name = "ayudaToolStripMenuItem", ShortcutKeys = Keys.F1 };

        private readonly ToolStripMenuItem cerrarSesionToolStripMenuItem = new ToolStripMenuItem { Name = "cerrarSesionToolStripMenuItem" };
        private readonly ToolStripMenuItem rolesPermisosToolStripMenuItem = new ToolStripMenuItem { Name = "rolesPermisosToolStripMenuItem" };
        private readonly Timer tmrSesion = new Timer { Interval = 60000 };

        /// <summary>true si el usuario eligio "Cerrar sesion": al cerrarse vuelve a la pantalla de ingreso.</summary>
        public bool CerrarSesionSolicitada { get; private set; }

        public frmPrincipal()
        {
            InitializeComponent();

            // Cierre de sesion, mantenimiento de usuarios y roles/permisos (menu Mantenimiento > Usuarios).
            cerrarSesionToolStripMenuItem.Click += cerrarSesionToolStripMenuItem_Click;
            rolesPermisosToolStripMenuItem.Click += rolesPermisosToolStripMenuItem_Click;
            mantenimientoDeUsuariosToolStripMenuItem.Click += mantenimientoDeUsuariosToolStripMenuItem_Click;
            usuariosToolStripMenuItem.DropDownItems.Add(rolesPermisosToolStripMenuItem);
            usuariosToolStripMenuItem.DropDownItems.Add(new ToolStripSeparator());
            usuariosToolStripMenuItem.DropDownItems.Add(cerrarSesionToolStripMenuItem);

            // Cada minuto se marca actividad: asi la sesion de otra PC no se considera abandonada.
            tmrSesion.Tick += (s, e) =>
            {
                try { new CN_Seguridad().Latido(); } catch (Exception) { }
            };
            tmrSesion.Start();
            FormClosed += frmPrincipal_FormClosed;

            elaboracionToolStripMenuItem.Click += elaboracionToolStripMenuItem_Click;
            backupToolStripMenuItem.Click += backupToolStripMenuItem_Click;
            facturacionToolStripMenuItem.DropDownItems.Add(elaboracionToolStripMenuItem);
            cajaToolStripMenuItem.Click += cajaToolStripMenuItem_Click;
            buscarRecetaToolStripMenuItem.Click += buscarRecetaToolStripMenuItem_Click;
            facturacionToolStripMenuItem.DropDownItems.Add(cajaToolStripMenuItem);
            facturacionToolStripMenuItem.DropDownItems.Add(buscarRecetaToolStripMenuItem);
            administracionToolStripMenuItem.DropDownItems.Add(backupToolStripMenuItem);

            // Ayuda en linea (menu Ayuda o tecla F1).
            ayudaToolStripMenuItem.Click += ayudaToolStripMenuItem_Click;
            menuStrip1.Items.Add(ayudaToolStripMenuItem);
        }

        private void ayudaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (frmAyuda f = new frmAyuda())
            {
                f.ShowDialog(this);
            }
        }

        private void frmPrincipal_Load(object sender, EventArgs e)
        {
            // Cada usuario entra con el idioma que eligio la ultima vez.
            try
            {
                new CN_IdiomaUsuario().AplicarIdiomaDelUsuario(UserCache.IdUsuario);
            }
            catch (Exception)
            {
                // Si no se puede leer el idioma guardado se sigue con el activo.
            }
            AplicarIdioma();
            AplicarPermisos();
        }

        private void cerrarSesionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(Idioma.Texto("Msg_ConfirmCerrarSesion"), Idioma.Texto("CerrarSesion"),
                MessageBoxButtons.OKCancel, MessageBoxIcon.Question) != DialogResult.OK)
            {
                return;
            }
            CerrarSesionSolicitada = true;
            Close();
        }

        // Al cerrarse (por "Cerrar sesion", "Salir" o la X) la sesion se libera y queda en la bitacora.
        private void frmPrincipal_FormClosed(object sender, FormClosedEventArgs e)
        {
            tmrSesion.Stop();
            try
            {
                new CL_clsBitacora("Cierre de sesión",
                    CerrarSesionSolicitada ? "El usuario cerró su sesión" : "El usuario salió del sistema", "frmPrincipal");
                new CN_Seguridad().CerrarSesion();
            }
            catch (Exception)
            {
                // Si no se pudo avisar a la base, la sesion vence sola por inactividad.
            }
        }

        private void mantenimientoDeUsuariosToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!PermisosSistema.PuedeGestionarUsuarios())
            {
                MessageBox.Show(Idioma.Texto("Msg_SinPermiso"));
                return;
            }
            Administrador.frmGestionUsuarios fAux = new Administrador.frmGestionUsuarios();
            fAux.MdiParent = this;
            fAux.Show();
        }

        private void rolesPermisosToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!PermisosSistema.PuedeAsignarRoles())
            {
                MessageBox.Show(Idioma.Texto("Msg_SinPermiso"));
                return;
            }
            Administrador.frmRolesPermisos fAux = new Administrador.frmRolesPermisos();
            fAux.MdiParent = this;
            fAux.Show();
        }

        private void GuardarIdiomaElegido(string idioma)
        {
            try
            {
                new CN_IdiomaUsuario().GuardarIdioma(UserCache.IdUsuario, idioma);
            }
            catch (Exception)
            {
                // No poder guardar la preferencia no debe impedir cambiar el idioma.
            }
        }

        private void cajaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!VerificarPermiso(PermisosSistema.Caja)) return;
            Ventas.frmCaja fAux = new Ventas.frmCaja();
            fAux.MdiParent = this;
            fAux.Show();
        }

        private void buscarRecetaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!VerificarPermiso(PermisosSistema.Menu)) return;
            Ventas.frmBuscarReceta fAux = new Ventas.frmBuscarReceta();
            fAux.MdiParent = this;
            fAux.Show();
        }

        private void elaboracionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!VerificarPermiso(PermisosSistema.Elaboracion)) return;
            Ventas.frmElaboracion fAux = new Ventas.frmElaboracion();
            fAux.MdiParent = this;
            fAux.Show();
        }

        // Copia de respaldo de la base de datos (solo quien administra el sistema).
        private void backupToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!VerificarPermiso(PermisosSistema.AdministracionSistema)) return;
            try
            {
                CN_Backup backup = new CN_Backup();
                string carpeta = backup.RutaPredeterminada();
                string ruta;
                using (SaveFileDialog dialogo = new SaveFileDialog())
                {
                    dialogo.Title = Idioma.Texto("Backup");
                    dialogo.Filter = "SQL Server (*.bak)|*.bak";
                    dialogo.FileName = backup.NombreBase() + "_" + DateTime.Now.ToString("yyyyMMdd_HHmm") + ".bak";
                    if (!string.IsNullOrEmpty(carpeta) && System.IO.Directory.Exists(carpeta))
                    {
                        dialogo.InitialDirectory = carpeta;
                    }
                    if (dialogo.ShowDialog(this) != DialogResult.OK) return;
                    ruta = dialogo.FileName;
                }

                Cursor = Cursors.WaitCursor;
                try
                {
                    backup.Respaldar(ruta);
                }
                finally
                {
                    Cursor = Cursors.Default;
                }
                new CL_clsBitacora("Copia de respaldo", ruta, "frmPrincipal");
                MessageBox.Show(Idioma.Texto("Msg_BackupOk") + "\n" + ruta, Idioma.Texto("Backup"));
            }
            catch (Exception ex)
            {
                CV_ExcepcionBitacora.RegistrarYMostrar(ex, Name);
            }
        }

        // Muestra u oculta menús según permisos cargados al loguearse
        private void AplicarPermisos()
        {
            bool admin = PermisosSistema.EsAdministrador();

            comprasToolStripMenuItem.Visible = admin || PermisosSistema.TieneAlguno(
                PermisosSistema.Proveedores, PermisosSistema.Productos, PermisosSistema.Stock);
            proveedoresToolStripMenuItem.Visible = admin || PermisosSistema.Tiene(PermisosSistema.Proveedores);
            productosToolStripMenuItem.Visible = admin || PermisosSistema.Tiene(PermisosSistema.Productos);
            stockComprasToolStripMenuItem.Visible = admin || PermisosSistema.Tiene(PermisosSistema.Stock);

            facturacionToolStripMenuItem.Visible = admin || PermisosSistema.TieneAlguno(
                PermisosSistema.Menu, PermisosSistema.Pedidos, PermisosSistema.Elaboracion, PermisosSistema.Caja);
            menuToolStripMenuItem.Visible = admin || PermisosSistema.Tiene(PermisosSistema.Menu);
            pedidosToolStripMenuItem.Visible = admin || PermisosSistema.Tiene(PermisosSistema.Pedidos);
            elaboracionToolStripMenuItem.Visible = admin || PermisosSistema.Tiene(PermisosSistema.Elaboracion);
            cajaToolStripMenuItem.Visible = admin || PermisosSistema.Tiene(PermisosSistema.Caja);
            buscarRecetaToolStripMenuItem.Visible = admin || PermisosSistema.Tiene(PermisosSistema.Menu);

            administracionToolStripMenuItem.Visible = admin || PermisosSistema.TieneAlguno(
                PermisosSistema.AltaUsuarios, PermisosSistema.BajaUsuarios,
                PermisosSistema.ModificacionUsuarios, PermisosSistema.Personal,
                PermisosSistema.Bitacora, PermisosSistema.Ubicaciones);
            backupToolStripMenuItem.Visible = admin;
            gestiónDePersonalToolStripMenuItem.Visible = admin || PermisosSistema.Tiene(PermisosSistema.Personal);
            registroToolStripMenuItem.Visible = admin || PermisosSistema.Tiene(PermisosSistema.Bitacora);
            bitacoraToolStripMenuItem.Visible = admin || PermisosSistema.Tiene(PermisosSistema.Bitacora);

            // El menu Mantenimiento tambien contiene "Usuarios" (cambio de password, cierre de sesion): lo ve todo el mundo.
            mantenimientoToolStripMenuItem.Visible = true;
            mantenimientoDeUsuariosToolStripMenuItem.Visible = PermisosSistema.PuedeGestionarUsuarios();
            rolesPermisosToolStripMenuItem.Visible = PermisosSistema.PuedeAsignarRoles();
            ubicacionesToolStripMenuItem.Visible = admin || PermisosSistema.Tiene(PermisosSistema.Ubicaciones);

            usuariosToolStripMenuItem.Visible = true;
            cambioDePasswordToolStripMenuItem.Visible = true;

            gestionDeUsuariosToolStripMenuItem.Visible = PermisosSistema.PuedeGestionarUsuarios();
            asignarRolToolStripMenuItem.Visible = PermisosSistema.PuedeAsignarRoles();

            estadisticasToolStripMenuItem.Visible = PermisosSistema.PuedeVerEstadisticas();

            if (!string.IsNullOrEmpty(UserCache.NombreGrupo))
            {
                lblSesion.Text += " | " + Idioma.Texto("Rol") + ": " + UserCache.NombreGrupo;
            }
        }

        private bool VerificarPermiso(string permiso)
        {
            if (PermisosSistema.Tiene(permiso) || PermisosSistema.EsAdministrador())
            {
                return true;
            }
            MessageBox.Show(Idioma.Texto("Msg_SinPermiso"));
            return false;
        }

        // Aplica el idioma activo a los textos del formulario principal
        private void AplicarIdioma()
        {
            lblSesion.Text = Idioma.Texto("Sesion") + " " + UserCache.Apellido + " " + UserCache.Nombres;

            mantenimientoToolStripMenuItem.Text = Idioma.Texto("Mantenimiento");
            usuariosToolStripMenuItem.Text = Idioma.Texto("Usuarios");
            cambioDePasswordToolStripMenuItem.Text = Idioma.Texto("CambioPassword");
            mantenimientoDeUsuariosToolStripMenuItem.Text = Idioma.Texto("MantenimientoUsuarios");
            ubicacionesToolStripMenuItem.Text = Idioma.Texto("Ubicaciones");
            salirToolStripMenuItem.Text = Idioma.Texto("Salir");
            facturacionToolStripMenuItem.Text = Idioma.Texto("Facturacion");
            ventasToolStripMenuItem.Text = Idioma.Texto("Ventas");
            stockToolStripMenuItem.Text = Idioma.Texto("Stock");
            administracionToolStripMenuItem.Text = Idioma.Texto("Administracion");
            registroToolStripMenuItem.Text = Idioma.Texto("Registros");
            bitacoraToolStripMenuItem.Text = Idioma.Texto("Bitacora");
            ventasToolStripMenuItem1.Text = Idioma.Texto("Ventas");
            gestionDeUsuariosToolStripMenuItem.Text = Idioma.Texto("RegistrarUsuario");
            gestiónDePersonalToolStripMenuItem.Text = Idioma.Texto("GestionPersonal");
            idiomaToolStripMenuItem.Text = Idioma.Texto("Idioma");
            comprasToolStripMenuItem.Text = Idioma.Texto("Compras");
            proveedoresToolStripMenuItem.Text = Idioma.Texto("Proveedores");
            productosToolStripMenuItem.Text = Idioma.Texto("Productos");
            stockComprasToolStripMenuItem.Text = Idioma.Texto("Stock");
            facturacionToolStripMenuItem.Text = Idioma.Texto("Ventas");
            menuToolStripMenuItem.Text = Idioma.Texto("Menu");
            pedidosToolStripMenuItem.Text = Idioma.Texto("Pedidos");
            elaboracionToolStripMenuItem.Text = Idioma.Texto("Elaboracion");
            cajaToolStripMenuItem.Text = Idioma.Texto("Caja_Titulo");
            ayudaToolStripMenuItem.Text = Idioma.Texto("Ayuda_Titulo");
            buscarRecetaToolStripMenuItem.Text = Idioma.Texto("BuscarReceta_Titulo");
            cerrarSesionToolStripMenuItem.Text = Idioma.Texto("CerrarSesion");
            rolesPermisosToolStripMenuItem.Text = Idioma.Texto("RolesPermisos_Titulo");
            backupToolStripMenuItem.Text = Idioma.Texto("Backup");
            estadisticasToolStripMenuItem.Text = Idioma.Texto("Estadisticas");
            asignarRolToolStripMenuItem.Text = Idioma.Texto("AsignarRol");
        }

        private void estadisticasToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!PermisosSistema.PuedeVerEstadisticas())
            {
                MessageBox.Show(Idioma.Texto("Msg_SinPermiso"));
                return;
            }
            Ventas.frmEstadisticas fAux = new Ventas.frmEstadisticas();
            fAux.MdiParent = this;
            fAux.Show();
        }

        private void asignarRolToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!PermisosSistema.PuedeAsignarRoles())
            {
                MessageBox.Show(Idioma.Texto("Msg_SinPermiso"));
                return;
            }
            Administrador.frmAsignarRol fAux = new Administrador.frmAsignarRol();
            fAux.MdiParent = this;
            fAux.Show();
        }

        private void bitacoraToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!PermisosSistema.Tiene(PermisosSistema.Bitacora) && !PermisosSistema.EsAdministrador())
            {
                MessageBox.Show(Idioma.Texto("Msg_SinPermiso"));
                return;
            }
            Administrador.frmBitacora fAux = new Administrador.frmBitacora();
            fAux.MdiParent = this;
            fAux.Show();
        }

        private void proveedoresToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!VerificarPermiso(PermisosSistema.Proveedores)) return;
            Compras.frmProveedores fAux = new Compras.frmProveedores();
            fAux.MdiParent = this;
            fAux.Show();
        }

        private void productosToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!VerificarPermiso(PermisosSistema.Productos)) return;
            Compras.frmProducto fAux = new Compras.frmProducto();
            fAux.MdiParent = this;
            fAux.Show();
        }

        private void stockComprasToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!VerificarPermiso(PermisosSistema.Stock)) return;
            Compras.frmStock fAux = new Compras.frmStock();
            fAux.MdiParent = this;
            fAux.Show();
        }

        private void menuToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!VerificarPermiso(PermisosSistema.Menu)) return;
            Ventas.frmMenu fAux = new Ventas.frmMenu();
            fAux.MdiParent = this;
            fAux.Show();
        }

        private void pedidosToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!VerificarPermiso(PermisosSistema.Pedidos)) return;
            Ventas.frmFacturacion fAux = new Ventas.frmFacturacion();
            fAux.MdiParent = this;
            fAux.Show();
        }

        private void espanolToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Idioma.Establecer(Idioma.Espanol);
            GuardarIdiomaElegido(Idioma.Espanol);
            AplicarIdioma();
        }

        private void englishToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Idioma.Establecer(Idioma.Ingles);
            GuardarIdiomaElegido(Idioma.Ingles);
            AplicarIdioma();
        }

        private void portuguesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Idioma.Establecer(Idioma.Portugues);
            GuardarIdiomaElegido(Idioma.Portugues);
            AplicarIdioma();
        }

        private void cambioDePasswordToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            Loguin.frmCambioPassword fAux = new Loguin.frmCambioPassword();
            fAux.MdiParent = this;
            fAux.Show();
        }

        private void salirToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void frmPrincipal_FormClosing(object sender, FormClosingEventArgs e)
        /*
         * El evento FormClosing se produce al momento de cerrar el formulario, en este evento
         * que se dispara a través de una instruccion o bien al clickear sobre la X de los ControlBox
         * del formulario, es donde colocamos lo que queremos que suceda al cerrar
         * */
        {
            if (CerrarSesionSolicitada) return;   // ya se confirmo en "Cerrar sesion"

            //pregunto si desea cerrar, dando la opción de cancelar la operación
            DialogResult resultado = MessageBox.Show("Esta seguro de CERRAR el sistema?",
                                "FINALIZAR SISTEMA",
                                MessageBoxButtons.OKCancel,
                                MessageBoxIcon.Asterisk,
                                MessageBoxDefaultButton.Button2);

            if (resultado == DialogResult.Cancel)
            {
                e.Cancel = true; //Cancela el cierre del formulario
            }
        }

        private void gestiónDePersonalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!VerificarPermiso(PermisosSistema.Personal)) return;
            frmPersonas fAux = new frmPersonas();
            //Loguin.frmCambioPassword fAux = new Loguin.frmCambioPassword();
            fAux.MdiParent = this;
            fAux.Show();
        }

        private void ubicacionesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!VerificarPermiso(PermisosSistema.Ubicaciones)) return;
            Administrador.frmUbicaciones fAux = new Administrador.frmUbicaciones();
            fAux.MdiParent = this;
            fAux.Show();
            //Hmmm que delicia
        }

        private void gestionDeUsuariosToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!PermisosSistema.PuedeGestionarUsuarios())
            {
                MessageBox.Show(Idioma.Texto("Msg_SinPermiso"));
                return;
            }
            Administrador.frmRegistrarUsuario fAux = new Administrador.frmRegistrarUsuario();
            fAux.MdiParent = this;
            fAux.Show();
        }
    }
}
