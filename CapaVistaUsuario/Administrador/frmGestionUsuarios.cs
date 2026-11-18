using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using CapaComun;
using CapaLogicaNegocio;
using CapaLogicaNegocio.Administrador;

namespace CapaVistaUsuario.Administrador
{
    /// <summary>
    /// Mantenimiento de usuarios (CU-USU-002 y CU-USU-003): consulta, modificacion, baja, reactivacion
    /// (tambien desbloquea a quien fue bloqueado por intentos fallidos) y restablecimiento de contraseña.
    /// </summary>
    public class frmGestionUsuarios : Form
    {
        private readonly CN_GestionUsuarios usuarios = new CN_GestionUsuarios();
        private readonly CV_Obligatorios obligatorios = new CV_Obligatorios();

        private readonly DataGridView dgvUsuarios = new DataGridView();
        private readonly Label lblUsuario = new Label();
        private readonly TextBox txtUsuario = new TextBox();
        private readonly Label lblCambiaCada = new Label();
        private readonly TextBox txtCambiaCada = new TextBox();
        private readonly Label lblAyuda = new Label();
        private readonly Button btnGuardar = new Button();
        private readonly Button btnBaja = new Button();
        private readonly Button btnReactivar = new Button();
        private readonly Button btnClave = new Button();
        private readonly Button btnCerrar = new Button();
        private int idSeleccionado;

        public frmGestionUsuarios()
        {
            Name = "frmGestionUsuarios";
            Text = Idioma.Texto("GestionUsuarios_Titulo");
            ClientSize = new Size(840, 540);
            StartPosition = FormStartPosition.CenterParent;

            dgvUsuarios.Location = new Point(12, 12);
            dgvUsuarios.Size = new Size(816, 320);
            dgvUsuarios.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            dgvUsuarios.ReadOnly = true;
            dgvUsuarios.AllowUserToAddRows = false;
            dgvUsuarios.AllowUserToDeleteRows = false;
            dgvUsuarios.MultiSelect = false;
            dgvUsuarios.RowHeadersVisible = false;
            dgvUsuarios.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvUsuarios.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvUsuarios.CellClick += (s, e) => { if (e.RowIndex >= 0) CargarSeleccion(); };
            dgvUsuarios.CellFormatting += dgvUsuarios_CellFormatting;

            lblUsuario.AutoSize = true;
            lblUsuario.Location = new Point(12, 380);
            txtUsuario.Location = new Point(130, 376);
            txtUsuario.Size = new Size(220, 20);
            lblCambiaCada.AutoSize = true;
            lblCambiaCada.Location = new Point(380, 380);
            txtCambiaCada.Location = new Point(560, 376);
            txtCambiaCada.Size = new Size(70, 20);
            lblAyuda.Location = new Point(12, 412);
            lblAyuda.Size = new Size(816, 34);
            lblAyuda.ForeColor = SystemColors.GrayText;

            btnGuardar.Location = new Point(12, 462);
            btnGuardar.Size = new Size(150, 34);
            btnGuardar.Click += btnGuardar_Click;
            btnBaja.Location = new Point(170, 462);
            btnBaja.Size = new Size(130, 34);
            btnBaja.Click += btnBaja_Click;
            btnReactivar.Location = new Point(308, 462);
            btnReactivar.Size = new Size(170, 34);
            btnReactivar.Click += btnReactivar_Click;
            btnClave.Location = new Point(486, 462);
            btnClave.Size = new Size(190, 34);
            btnClave.Click += btnClave_Click;
            btnCerrar.Location = new Point(738, 462);
            btnCerrar.Size = new Size(90, 34);
            btnCerrar.Click += (s, e) => Close();

            Controls.AddRange(new Control[]
            {
                dgvUsuarios, lblUsuario, txtUsuario, lblCambiaCada, txtCambiaCada, lblAyuda,
                btnGuardar, btnBaja, btnReactivar, btnClave, btnCerrar
            });

            lblUsuario.Text = Idioma.Texto("UsuarioLogin");
            lblCambiaCada.Text = Idioma.Texto("CambiaCada");
            lblAyuda.Text = Idioma.Texto("GestionUsuarios_Ayuda");
            btnGuardar.Text = Idioma.Texto("Btn_GuardarCambios");
            btnBaja.Text = Idioma.Texto("Btn_DarDeBaja");
            btnReactivar.Text = Idioma.Texto("Btn_Reactivar");
            btnClave.Text = Idioma.Texto("Btn_RestablecerClave");
            btnCerrar.Text = Idioma.Texto("Btn_Cerrar");

            BuscadorGrilla.Agregar(dgvUsuarios, "Usuario", "Apellido", "Nombres", "Rol", "Estado");
            Load += (s, e) => Cargar();
        }

        private void Cargar()
        {
            try
            {
                int anterior = idSeleccionado;
                dgvUsuarios.DataSource = usuarios.Listar();
                dgvUsuarios.Columns["IdUsuario"].Visible = false;
                dgvUsuarios.Columns["DebeCambiarPassword"].HeaderText = Idioma.Texto("ClaveTemporal");
                dgvUsuarios.Columns["CambiaCada"].HeaderText = Idioma.Texto("CambiaCada");
                dgvUsuarios.Columns["FechaAlta"].DefaultCellStyle.Format = "dd/MM/yyyy";
                dgvUsuarios.Columns["FechaBaja"].DefaultCellStyle.Format = "dd/MM/yyyy";
                dgvUsuarios.ClearSelection();
                foreach (DataGridViewRow fila in dgvUsuarios.Rows)
                {
                    if (Convert.ToInt32(fila.Cells["IdUsuario"].Value) == anterior)
                    {
                        fila.Selected = true;
                        dgvUsuarios.CurrentCell = fila.Cells["Usuario"];
                        CargarSeleccion();
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                CV_ExcepcionBitacora.RegistrarYMostrar(ex, Name);
            }
        }

        // Los usuarios dados de baja se ven en gris y los bloqueados con fondo rosado.
        private void dgvUsuarios_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0 || !dgvUsuarios.Columns.Contains("Estado")) return;
            string estado = Convert.ToString(dgvUsuarios.Rows[e.RowIndex].Cells["Estado"].Value);
            if (estado == "Baja")
            {
                e.CellStyle.ForeColor = Color.Gray;
            }
            else if (estado == "Bloqueado")
            {
                e.CellStyle.BackColor = Color.FromArgb(255, 220, 220);
                e.CellStyle.ForeColor = Color.Black;
            }
        }

        private void CargarSeleccion()
        {
            if (dgvUsuarios.CurrentRow == null) return;
            DataGridViewRow f = dgvUsuarios.CurrentRow;
            idSeleccionado = Convert.ToInt32(f.Cells["IdUsuario"].Value);
            txtUsuario.Text = Convert.ToString(f.Cells["Usuario"].Value);
            txtCambiaCada.Text = Convert.ToString(f.Cells["CambiaCada"].Value);
            obligatorios.LimpiarMarcas();
        }

        private bool HaySeleccion()
        {
            if (idSeleccionado == 0 || dgvUsuarios.CurrentRow == null)
            {
                MessageBox.Show(Idioma.Texto("Msg_SeleccioneRegistro"));
                return false;
            }
            return true;
        }

        private string NombreSeleccionado()
        {
            return Convert.ToString(dgvUsuarios.CurrentRow.Cells["Usuario"].Value);
        }

        // Ejecuta una accion sobre el usuario y se ocupa de lo que se repite en todas: bitacora, aviso y recarga de la grilla.
        private void Ejecutar(Action accion, string evento, string detalle, string exito)
        {
            try
            {
                accion();
                new CL_clsBitacora(evento, detalle, Name);
                MessageBox.Show(exito, Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                Cargar();
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show(ex.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                CV_ExcepcionBitacora.RegistrarYMostrar(ex, Name);
            }
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (!HaySeleccion() || !obligatorios.Validar(txtUsuario, txtCambiaCada)) return;
            int dias;
            if (!int.TryParse(txtCambiaCada.Text.Trim(), out dias))
            {
                obligatorios.Marcar(txtCambiaCada);
                MessageBox.Show(Idioma.Texto("Msg_CambiaCadaInvalido"));
                return;
            }
            string anterior = NombreSeleccionado();
            Ejecutar(() => usuarios.Modificar(idSeleccionado, txtUsuario.Text, dias), "Modificación de usuario",
                "Usuario " + anterior + (anterior != txtUsuario.Text.Trim() ? " → " + txtUsuario.Text.Trim() : ""),
                Idioma.Texto("Msg_DatosActualizados"));
        }

        // La baja es logica (queda la fecha de baja); no se borra el usuario ni su historial.
        private void btnBaja_Click(object sender, EventArgs e)
        {
            if (!HaySeleccion()) return;
            string nombre = NombreSeleccionado();
            if (MessageBox.Show(string.Format(Idioma.Texto("Msg_ConfirmBajaUsuario"), nombre), Text,
                MessageBoxButtons.OKCancel, MessageBoxIcon.Question) != DialogResult.OK) return;
            Ejecutar(() => usuarios.DarDeBaja(idSeleccionado), "Baja de usuario", "Usuario " + nombre,
                string.Format(Idioma.Texto("Msg_UsuarioBaja"), nombre));
        }

        private void btnReactivar_Click(object sender, EventArgs e)
        {
            if (!HaySeleccion()) return;
            string nombre = NombreSeleccionado();
            Ejecutar(() => usuarios.Reactivar(idSeleccionado), "Reactivación de usuario", "Usuario " + nombre,
                string.Format(Idioma.Texto("Msg_UsuarioReactivado"), nombre));
        }

        // Genera una clave temporal: el usuario tiene que cambiarla en su proximo ingreso.
        private void btnClave_Click(object sender, EventArgs e)
        {
            if (!HaySeleccion()) return;
            string nombre = NombreSeleccionado();
            if (MessageBox.Show(string.Format(Idioma.Texto("Msg_ConfirmRestablecer"), nombre), Text,
                MessageBoxButtons.OKCancel, MessageBoxIcon.Question) != DialogResult.OK) return;
            try
            {
                string temporal = usuarios.RestablecerPassword(idSeleccionado);
                new CL_clsBitacora("Restablecimiento de contraseña", "Usuario " + nombre, Name);
                MessageBox.Show(string.Format(Idioma.Texto("Msg_ClaveTemporal"), nombre, temporal), Text,
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                Cargar();
            }
            catch (Exception ex)
            {
                CV_ExcepcionBitacora.RegistrarYMostrar(ex, Name);
            }
        }
    }
}
