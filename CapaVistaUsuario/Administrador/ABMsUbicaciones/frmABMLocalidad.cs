using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CapaLogicaNegocio;
using CapaLogicaNegocio.Administrador.ABMsUbicaciones;

namespace CapaVistaUsuario.Administrador.ABMsUbicaciones
{
    public partial class frmABMLocalidad : Form
    {
        Cls_CLocalidades CL = new Cls_CLocalidades();


        public frmABMLocalidad()
        {
            InitializeComponent();
            BuscadorGrilla.Agregar(dataGridView1, "Localidades", "CP");
        }

        private void ABMLocalidad_Load(object sender, EventArgs e)
        {
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.ReadOnly = true;
            dataGridView1.MultiSelect = false;
            dataGridView1.AllowUserToAddRows = false;
            try
            {
                dataGridView1.DataSource = new CLN_Localidad().MostrarLocalidad();
            }
            catch (Exception ex)
            {
                CV_ExcepcionBitacora.RegistrarYMostrar(ex, Name);
            }

            cmbpartidos.DataSource = Cls_CPartidos.BasePartidos;
            cmbpartidos.DisplayMember = "NombrePartido";
        }

        private void btnagregar_Click(object sender, EventArgs e)
        {
            Cls_CLocalidades.BaseLocalidades.Add(new Cls_LLocalidades()
            {
                NombreLocalidad = txtnombre.Text,

                Partido = cmbpartidos.Text,

            });
        }

        private void btnatras_Click(object sender, EventArgs e)
        {

            this.Hide();
        }

        private void btneliminar_Click(object sender, EventArgs e)
        {
            //Cls_CLocalidades.BaseLocalidades.Remove(listBox1.SelectedValue.ToString());
        }
    }
}
