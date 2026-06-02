using System;
using System.Windows.Forms;
using CapaComun;
using CapaLogicaNegocio.Administrador.ABMsUbicaciones;
using CapaVistaUsuario;

namespace CapaVistaUsuario.Administrador.ABMsUbicaciones
{
    public partial class frmABMProvincia : Form
    {
        CLN_Provincia Provi = new CLN_Provincia();

        public frmABMProvincia()
        {
            InitializeComponent();
        }

        private void ABMProvincia_Load(object sender, EventArgs e)
        {
            // Controles heredados del diseño anterior que este ABM no utiliza
            listBox1.Visible = false;
            checkBox1.Visible = false;
            comboBox1.Visible = false;

            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.ReadOnly = true;
            dataGridView1.MultiSelect = false;
            dataGridView1.AllowUserToAddRows = false;

            AplicarIdioma();
            MostrarProvincias();
        }

        #region METODOS

        private void AplicarIdioma()
        {
            this.Text = Idioma.Texto("ABMProvincia_Titulo");
            label1.Text = Idioma.Texto("Provincia_Nombre");
            label2.Text = Idioma.Texto("Provincia_Id");
            button1.Text = Idioma.Texto("Btn_Agregar");
            button2.Text = Idioma.Texto("Btn_Modificar");
            button3.Text = Idioma.Texto("Btn_Eliminar");
            button4.Text = Idioma.Texto("Btn_Atras");
        }

        private void MostrarProvincias()
        {
            dataGridView1.DataSource = Provi.MostrarProvincia();
        }

        private void LimpiarCampos()
        {
            lblID.Text = "0";
            txtprovincia.Text = "";
            txtprovincia.Select();
        }

        private bool DatosValidos()
        {
            if (txtprovincia.Text.Trim().Length == 0)
            {
                MessageBox.Show(Idioma.Texto("Msg_NombreVacio"));
                txtprovincia.Select();
                return false;
            }
            return true;
        }

        private void PasarDatos(bool modificar)
        {
            Provi.IdProvincia = modificar ? Convert.ToInt32(lblID.Text) : 0;
            Provi.Provincia = txtprovincia.Text.Trim();
        }

        #endregion

        #region EVENTOS

        // Agregar
        private void button1_Click(object sender, EventArgs e)
        {
            if (!DatosValidos()) return;

            try
            {
                PasarDatos(false);
                Provi.InsertarProvincia();
                MostrarProvincias();
                LimpiarCampos();
            }
            catch (Exception ex)
            {
                CV_ExcepcionBitacora.RegistrarYMostrar(ex, Name);
            }
        }

        // Modificar
        private void button2_Click(object sender, EventArgs e)
        {
            if (Convert.ToInt32(lblID.Text) == 0)
            {
                MessageBox.Show(Idioma.Texto("Msg_SeleccioneRegistro"));
                return;
            }
            if (!DatosValidos()) return;

            try
            {
                PasarDatos(true);
                Provi.ModificarProvincia();
                MostrarProvincias();
                LimpiarCampos();
            }
            catch (Exception ex)
            {
                CV_ExcepcionBitacora.RegistrarYMostrar(ex, Name);
            }
        }

        // Eliminar
        private void button3_Click(object sender, EventArgs e)
        {
            if (Convert.ToInt32(lblID.Text) == 0)
            {
                MessageBox.Show(Idioma.Texto("Msg_SeleccioneRegistro"));
                return;
            }

            DialogResult resultado = MessageBox.Show(
                Idioma.Texto("Msg_ConfirmEliminar"),
                Idioma.Texto("Msg_ConfirmEliminarTitulo"),
                MessageBoxButtons.OKCancel,
                MessageBoxIcon.Question);

            if (resultado == DialogResult.OK)
            {
                Provi.IdProvincia = Convert.ToInt32(lblID.Text);
                Provi.EliminarProvincia();
                MostrarProvincias();
                LimpiarCampos();
            }
        }

        // Atras
        private void button4_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        // Carga el registro seleccionado de la grilla en los campos
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || dataGridView1.SelectedRows.Count == 0) return;

            var fila = dataGridView1.SelectedRows[0];
            lblID.Text = fila.Cells["IdProvincia"].Value.ToString();
            txtprovincia.Text = fila.Cells["Provincia"].Value.ToString();
        }

        private void txtprovincia_TextChanged(object sender, EventArgs e)
        {
        }

        #endregion
    }
}
