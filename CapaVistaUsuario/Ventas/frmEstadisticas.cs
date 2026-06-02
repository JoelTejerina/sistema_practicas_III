using System.Windows.Forms;
using CapaComun;
using CapaLogicaNegocio.Ventas;

namespace CapaVistaUsuario.Ventas
{
    public partial class frmEstadisticas : Form
    {
        CN_Estadisticas Est = new CN_Estadisticas();

        public frmEstadisticas()
        {
            InitializeComponent();
        }

        private void frmEstadisticas_Load(object sender, System.EventArgs e)
        {
            dgvResumen.AllowUserToAddRows = false;
            dgvHoy.AllowUserToAddRows = false;
            this.Text = Idioma.Texto("Estadisticas_Titulo");
            lblResumen.Text = Idioma.Texto("Estadisticas_Resumen");
            lblHoy.Text = Idioma.Texto("Estadisticas_Hoy");
            btnSalir.Text = Idioma.Texto("Btn_Salir");
            dgvResumen.DataSource = Est.ResumenPedidosPorEstado();
            dgvHoy.DataSource = Est.PedidosDelDia();
        }

        private void btnSalir_Click(object sender, System.EventArgs e)
        {
            Close();
        }
    }
}
