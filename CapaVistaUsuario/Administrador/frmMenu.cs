using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CapaVistaUsuario
{
    public partial class frmMenu : Form
    {
        CN_Menu menu = new CN_Menu();

        public frmMenu()
        {
            InitializeComponent();
        }

        private void frmMenu_Load(object sender, EventArgs e)
        {
            CV_Utiles.InicializarLabelPanel("Menu", panelMenu);
            dgvMenu.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvMenu.ReadOnly = true;
            dgvMenu.MultiSelect = false;
            dgvMenu.AllowUserToAddRows = false;

            MostrarMenus();
            dgvMenu.Select();
            CV_Utiles.BloquearControles(this);
            CV_Botonera.btnFormularios(this, btnCancelar);
        }

        private void dgvMenu_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvMenu.SelectedRows.Count > 0)
            {
                txtNombre.Text = dgvMenu.Rows[dgvMenu.SelectedRows[0].Index].Cells["nombre"].Value.ToString();
                txtDescripcion.Text = dgvMenu.Rows[dgvMenu.SelectedRows[0].Index].Cells["descripcion"].Value.ToString();
                txtPrecio.Text = dgvMenu.Rows[dgvMenu.SelectedRows[0].Index].Cells["precio"].Value.ToString();
                cbxRegion.Text = dgvMenu.Rows[dgvMenu.SelectedRows[0].Index].Cells["region"].Value.ToString();
                cbxCategoria.Text = dgvMenu.Rows[dgvMenu.SelectedRows[0].Index].Cells["categoria"].Value.ToString();
                txtPopularidad.Text = dgvMenu.Rows[dgvMenu.SelectedRows[0].Index].Cells["popularidad"].Value.ToString();
                cbxTemporada.Text = dgvMenu.Rows[dgvMenu.SelectedRows[0].Index].Cells["temporada"].Value.ToString();
                txtTipoDeEvento.Text = dgvMenu.Rows[dgvMenu.SelectedRows[0].Index].Cells["tipoDeEvento"].Value.ToString();
                txtTiempoPreparacionEstimado.Text = dgvMenu.Rows[dgvMenu.SelectedRows[0].Index].Cells["tiempoPreparacionEstimado"].Value.ToString();
            }
        }


        #region BOTONES

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                PasarDatos(false);

                menu.InsertarMenu();
                MostrarMenus();

                CV_Botonera.btnFormularios(this, btnGuardaCambios);
                CV_Utiles.BloquearControles(this);
                dgvMenu.Select();
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se guardaron los datos por: \n" + ex);
            }
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            CV_Utiles.DesbloquearControles(this);
            CV_Utiles.LimpiarControles(this);
            CV_Botonera.btnFormularios(this, btnAgregar);
            cbxRegion.Select();
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            CV_Botonera.btnFormularios(this, btnCancelar);
            CV_Utiles.BloquearControles(this);
            CV_Utiles.LimpiarControles(this);
            dgvMenu.Select();
        }

        private void btnModificar_Click(object sender, EventArgs e)
        {
            CV_Botonera.btnFormularios(this, btnModificar);
            CV_Utiles.DesbloquearControles(this);
            cbxRegion.Select();
        }

        private void btnGuardaCambios_Click(object sender, EventArgs e)
        {
            try
            {
                PasarDatos(true);

                menu.ModificarMenu();

                MostrarMenus();
                CV_Botonera.btnFormularios(this, btnGuardaCambios);
                CV_Utiles.BloquearControles(this);
                dgvMenu.Select();
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se guardaron los datos por: \n" + ex);
            }
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            var resultado = MessageBox.Show("¿Está seguro de ELIMINAR definitivamente el menú?",
                                            "ELIMINAR MENÚ",
                                            MessageBoxButtons.OKCancel,
                                            MessageBoxIcon.Question);
            if (resultado == DialogResult.OK)
            {
                menu.IdMenu = Convert.ToInt32(dgvMenu.Rows[dgvMenu.SelectedRows[0].Index].Cells["idMenu"].Value.ToString());
                menu.EliminarMenu();
                MostrarMenus();
                dgvMenu.Select();
            }
        }

        #endregion

        private void MostrarMenus()
        {
            dgvMenu.DataSource = menu.MostrarMenu();
        }
        private void PasarDatos(bool origen)
        {
            if (origen == true)
            {
                menu.IdMenu = Convert.ToInt32(dgvMenu.Rows[dgvMenu.SelectedRows[0].Index].Cells["idMenu"].Value.ToString());
            }
            else
            {
                menu.IdMenu = 0;
            }

            menu.Nombre = txtNombre.Text;
            menu.Descripcion = txtDescripcion.Text;
            menu.Precio = Convert.ToSingle(txtPrecio.Text);
            menu.Region = cbxRegion.Text;
            menu.Categoria = cbxCategoria.Text;
            menu.Popularidad = Convert.ToInt32(txtPopularidad.Text);
            menu.Temporada = cbxTemporada.Text;
            menu.TipoDeEvento = txtTipoDeEvento.Text;
            menu.TiempoPreparacionEstimado = Convert.ToInt32(txtTiempoPreparacionEstimado.Text);
        }
    }
}
