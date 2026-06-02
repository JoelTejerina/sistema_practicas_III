using System;
using System.Windows.Forms;
using CapaComun;
using CapaLogicaNegocio;
using CapaVistaUsuario;
using CapaLogicaNegocio.Administrador;

namespace CapaVistaUsuario.Administrador
{
    public partial class frmAsignarRol : Form
    {
        CN_UsuariosGrupos Roles = new CN_UsuariosGrupos();

        public frmAsignarRol()
        {
            InitializeComponent();
        }

        private void frmAsignarRol_Load(object sender, EventArgs e)
        {
            dgvUsuarios.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvUsuarios.ReadOnly = true;
            dgvUsuarios.MultiSelect = false;
            dgvUsuarios.AllowUserToAddRows = false;

            this.Text = Idioma.Texto("AsignarRol_Titulo");
            lblRol.Text = Idioma.Texto("RolUsuario");
            btnGuardar.Text = Idioma.Texto("Btn_GuardarRol");
            btnSalir.Text = Idioma.Texto("Btn_Salir");

            new CN_LlenarCombos(cmbGrupo, "Grupos", "IdGrupo", "Grupo");
            MostrarUsuarios();
        }

        private void MostrarUsuarios()
        {
            dgvUsuarios.DataSource = Roles.MostrarUsuariosConRol();
        }

        private void dgvUsuarios_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || dgvUsuarios.SelectedRows.Count == 0) return;
            var fila = dgvUsuarios.SelectedRows[0];
            lblIdUsuario.Text = fila.Cells["IdUsuario"].Value.ToString();
            if (fila.Cells["IdGrupo"].Value != DBNull.Value && fila.Cells["IdGrupo"].Value != null)
            {
                cmbGrupo.SelectedValue = Convert.ToInt32(fila.Cells["IdGrupo"].Value);
            }
            else
            {
                cmbGrupo.SelectedIndex = -1;
            }
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (Convert.ToInt32(lblIdUsuario.Text) == 0)
            {
                MessageBox.Show(Idioma.Texto("Msg_SeleccioneUsuario"));
                return;
            }
            if (cmbGrupo.SelectedValue == null)
            {
                MessageBox.Show(Idioma.Texto("Msg_SeleccioneRol"));
                return;
            }
            try
            {
                Roles.IdUsuario = Convert.ToInt32(lblIdUsuario.Text);
                Roles.IdGrupo = Convert.ToInt32(cmbGrupo.SelectedValue);
                Roles.AsignarGrupo();
                new CL_clsBitacora("Asignación de rol",
                    "Usuario " + lblIdUsuario.Text + " -> " + cmbGrupo.Text, "frmAsignarRol");
                MessageBox.Show(Idioma.Texto("Msg_RolGuardado"));
                MostrarUsuarios();
            }
            catch (Exception ex)
            {
                CV_ExcepcionBitacora.RegistrarYMostrar(ex, Name);
            }
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
