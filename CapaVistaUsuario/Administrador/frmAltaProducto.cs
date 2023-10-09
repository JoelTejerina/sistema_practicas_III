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
using CapaLogicaNegocio.Administrador;
using CapaComun;
using CapaLogicaNegocio.Administrador.AltaProductos;
using System.Reflection.Emit;

namespace CapaVistaUsuario.Administrador
{
    public partial class frmAltaProducto : Form
    {

        clsAltaProductoLN Pers = new clsAltaProductoLN();

        public frmAltaProducto()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //Este es un boton que no sirve.
            //Debo eliminarlo cuando aprenda como eliminar controles sin que rompa el programa.
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //Boton de guardar. Se utilizara para dar de alta el producto.

            try
            {
                PasarDatos(false);

                Pers.InsertarPersona();
                MostrarPersonas();

                CV_Botonera.btnFormularios(this, btnGuardaCambios);
                CV_Utiles.BloquearControles(this);
                dgvPersonas.Select();
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se guardaron los datos por: \n" + ex);
            }

        }

        private void button3_Click(object sender, EventArgs e)
        {
            //Boton de cerrar. Se utilizara para cerrar la pestaña.
            this.Hide();
        }


        #region METODOS
        private void PasarDatos()
        {

        }

        #endregion
    }
}
