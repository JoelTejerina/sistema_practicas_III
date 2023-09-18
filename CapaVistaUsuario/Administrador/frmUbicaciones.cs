using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CapaVistaUsuario.Administrador.ABMsUbicaciones;
using CapaLogicaNegocio;
using CapaLogicaNegocio.Administrador.ABMsUbicaciones;

namespace CapaVistaUsuario.Administrador
{
    public partial class frmUbicaciones : Form
    {
        Cls_CProvincias CPR = new Cls_CProvincias();

        public frmUbicaciones()
        {
            InitializeComponent();
        }

        private void Ubicaciones_Load(object sender, EventArgs e)
        {

            CmbLocalidad.DataSource = Cls_CLocalidades.BaseLocalidades;
            CmbLocalidad.DisplayMember = "NombreLocalidad";
            CmbPartido.DataSource = Cls_CLocalidades.BaseLocalidades;
            CmbPartido.DisplayMember = "NombrePartido";
            CmbProvincia.DataSource = Cls_CProvincias.BaseProvincias;
            CmbProvincia.DisplayMember = "NombreProvincia";
        }

        private void btmlocalidadplus_Click(object sender, EventArgs e)
        {
            Administrador.ABMsUbicaciones.frmABMLocalidad fAux = new Administrador.ABMsUbicaciones.frmABMLocalidad();
            fAux.Show();
        }

        private void btmpartidoplus_Click(object sender, EventArgs e)
        {

            Administrador.ABMsUbicaciones.frmABMPartido fAux = new Administrador.ABMsUbicaciones.frmABMPartido();
            fAux.Show();
        }

        private void btmprovinciaplus_Click(object sender, EventArgs e)
        {
            Administrador.ABMsUbicaciones.frmABMProvincia fAux = new Administrador.ABMsUbicaciones.frmABMProvincia();
            fAux.Show();
        }

        private void BtmActualizar_Click(object sender, EventArgs e)
        {
            CmbProvincia.DataSource = null;
            CmbPartido.DataSource = null;
            CmbLocalidad.DataSource = null;
            CmbProvincia.Items.Clear();
            CmbLocalidad.Items.Clear();
            CmbPartido.Items.Clear();
            CmbLocalidad.DataSource = Cls_CLocalidades.BaseLocalidades;
            CmbLocalidad.DisplayMember = "NombreLocalidad";
            CmbPartido.DataSource = Cls_CLocalidades.BaseLocalidades;
            CmbPartido.DisplayMember = "NombrePartido";
            CmbProvincia.DataSource = Cls_CProvincias.BaseProvincias;
            CmbProvincia.DisplayMember = "NombreProvincia";
        }
    }
}
