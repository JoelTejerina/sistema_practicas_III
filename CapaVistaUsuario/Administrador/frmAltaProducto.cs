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
        // Arreglar instancia despues del testeo
        private clsAltaProductoTesteo productotesteo = new clsAltaProductoTesteo();

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
            PasarDatos();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            //Boton de cerrar. Se utilizara para cerrar la pestaña.
            this.Hide();
        }


        #region METODOS
        private void PasarDatos()
        {
            productotesteo.CLNNombreProducto = textBox1.Text;
            productotesteo.CLNMarca = textBox2.Text;
            productotesteo.CLNCategoria = comboBox1.Text;
            productotesteo.CLNTipoCantidadA = comboBox2.Text;
            productotesteo.CLNTipoCantidadB = comboBox3.Text;
            productotesteo.CLNCantidadA = Convert.ToInt32(textBox3.Text);
            productotesteo.CLNCantidadB = Convert.ToInt32(textBox4.Text);
            productotesteo.PasarDatosLN();
        }

        #endregion
    }
}
