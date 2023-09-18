using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CapaLogicaNegocio.Administrador.ABMsUbicaciones;

namespace CapaVistaUsuario.Administrador.ABMsUbicaciones
{
    public partial class frmABMProvincia : Form
    {

        CLN_Provincia Provi = new CLN_Provincia();
        CV_Validar_Mail ValidaCorreo = new CV_Validar_Mail();
        Cls_CProvincias CPR = new Cls_CProvincias();
        public frmABMProvincia()
        {
            InitializeComponent();
        }

        private void ABMProvincia_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {

            try
            {
                PasarDatos(false);

                Provi.InsertarProvincia();

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

        private void txtprovincia_TextChanged(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {

            //Cls_CProvincias.BaseProvincias.Remove(listBox1.SelectedValue.ToString());
        }

        private void PasarDatos(bool origen)
        {
            if (origen == true)
            {
                Provi.IdProvincia = Convert.ToInt32(lblID.Text);
            }
            else
            {
                Provi.IdProvincia = Convert.ToInt32("0");
            }
            Provi.Provincia = txtprovincia.Text;
        }
    }
}
