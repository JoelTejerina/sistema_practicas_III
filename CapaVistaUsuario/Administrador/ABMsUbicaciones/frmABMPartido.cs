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
    public partial class frmABMPartido : Form
    {
        //Cls_CPartidos CP = new Cls_CPartidos();
        CLN_Partido Parti = new CLN_Partido();
        public frmABMPartido()
        {
            InitializeComponent();
        }

        private void ABMPartido_Load(object sender, EventArgs e)
        {
            //Revisar lo de la carga. Utilizar una lista para que tenga lo mismo que la base
            //cmbprovincia.DataSource = Cls_CProvincias.BaseProvincias;
            //cmbprovincia.DisplayMember = "NombreProvincia";
        }

        private void btnagregar_Click(object sender, EventArgs e)
        {

            try
            {
                PasarDatos(false);

                Parti.InsertarPartido();

                CV_Utiles.BloquearControles(this);
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se guardaron los datos por: \n" + ex);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {

            this.Hide();
        }

        private void button3_Click(object sender, EventArgs e)
        {

            //Cls_CPartidos.BasePartidos.Remove(listBox1.SelectedValue.ToString());
        }
        private void PasarDatos(bool origen)
        {
            if (origen == true)
            {
                Parti.IdPartido = Convert.ToInt32(lblID.Text);
            }
            else
            {
                Parti.IdPartido = Convert.ToInt32("0");
            }
            Parti.Partido = txtpartido.Text;
        }
    }
}
