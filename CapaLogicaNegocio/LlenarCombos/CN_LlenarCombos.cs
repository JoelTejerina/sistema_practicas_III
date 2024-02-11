using System.Data;
using CapaAccesoDatos;
using System.Windows.Forms;



namespace CapaLogicaNegocio

{
    public class CN_LlenarCombos
    {
        private CD_LlenarCombos llenar = new CD_LlenarCombos();

        public CN_LlenarCombos(ComboBox CMB, string NombreTabla, string CampoID, string CampoDescrip, string campo1, string campo2, string Condicion = "")
        {
            llenar.Tabla = NombreTabla;
            llenar.CampoId = CampoID;
            llenar.CampoDescrip = CampoDescrip;
            llenar.CampoAdicional1 = campo1;
            llenar.CampoAdicional2 = campo2;
            llenar.Condicion = Condicion;

            CMB.DataSource = llenar.CargarCMB();

            CMB.DisplayMember = CampoDescrip;
            CMB.ValueMember = CampoID;
            CMB.SelectedIndex = -1;
        }

        /*public CN_LlenarCombosPersonal(ComboBox CMB)
        {
            CMB.DataSource = llenar.CargarCMB();

            CMB.DisplayMember = "Apellido"+" "+"Nombre";
            CMB.ValueMember = "IdPersona";
            CMB.SelectedIndex = -1;
        }
        */
    }
}
