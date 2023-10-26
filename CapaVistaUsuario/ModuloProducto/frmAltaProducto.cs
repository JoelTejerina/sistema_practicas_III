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
using CapaLogicaNegocio.ModuloProducto;
using CapaLogicaNegocio.Administrador.AltaProductos;
using System.Reflection.Emit;

namespace CapaVistaUsuario.Administrador
{
    public partial class frmAltaProducto : Form
    {
        //Instancio los objetos de las clses que utilizo en el form
        clsProductosLN ProductosLN = new clsProductosLN();
        CV_Validar_Mail ValidaCorreo = new CV_Validar_Mail();


        public frmAltaProducto()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //Boton de modificar

            try
            {
                PasarDatos(true);

                ProductosLN.ModificarProducto();

                MostrarProductos();
                dataGridView2.Select();
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se guardaron los datos por: \n" + ex);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //Boton de guardar. Se utilizara para dar de alta el producto.

            try
            {
                PasarDatos(false);

                ProductosLN.InsertarProducto();
                MostrarProductos();


                dataGridView2.Select();

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

        private void MostrarProductos()
        {
            clsProductosLN ProductosLN = new clsProductosLN();
            dataGridView2.DataSource = ProductosLN.MostrarProducto();
        }
        private void PasarDatos(bool origen)
        {
            if (origen == true)
            {
                ProductosLN.CLNIdProducto = Convert.ToInt32(dataGridView2.Rows[dataGridView2.SelectedRows[0].Index].Cells["IdProducto"].Value.ToString());
            }
            else
            {
                ProductosLN.CLNIdProducto = 0;
            }
            ProductosLN.CLNNombreProducto = textBox1.Text;
            ProductosLN.CLNCategoria = comboBox1.Text;
            ProductosLN.CLNMarca = textBox2.Text;
            ProductosLN.CLNTipoCantidad = comboBox2.Text;
            ProductosLN.CLNCantidad = Convert.ToInt32(textBox3.Text);

        }

        #endregion

        private void button4_Click(object sender, EventArgs e)
        {

            ProductosLN.CLNIdProducto = Convert.ToInt32(dataGridView2.Rows[dataGridView2.SelectedRows[0].Index].Cells["IdPersona"].Value.ToString());
            ProductosLN.EliminarProducto();
            MostrarProductos();
            dataGridView2.Select();
        }

        private void frmAltaProducto_Load(object sender, EventArgs e)
        {

            MostrarProductos();
        }
    }
}
