using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using CapaComun;
using CapaLogicaNegocio;
using CapaLogicaNegocio.Ventas;

namespace CapaVistaUsuario.Ventas
{
    public partial class frmEstadisticas : Form
    {
        CN_Estadisticas Est = new CN_Estadisticas();

        private Label lblPeriodo;
        private Label lblDesde;
        private DateTimePicker dtpDesde;
        private Label lblHasta;
        private DateTimePicker dtpHasta;
        private Label lblAgrupar;
        private ComboBox cmbAgrupar;
        private Button btnConsultar;
        private Label lblVentas;
        private DataGridView dgvVentas;
        private Label lblPlatos;
        private DataGridView dgvPlatos;

        public frmEstadisticas()
        {
            InitializeComponent();
            AgregarConsultaPorPeriodo();
        }

        // Ventas por dia, semana o mes y platos mas vendidos en un rango de fechas.
        private void AgregarConsultaPorPeriodo()
        {
            const int extra = 230;
            ClientSize = new Size(ClientSize.Width, ClientSize.Height + extra);
            btnSalir.Top += extra;

            lblPeriodo = new Label { AutoSize = true, Location = new Point(12, 378), Font = new Font(Font, FontStyle.Bold) };
            lblDesde = new Label { AutoSize = true, Location = new Point(12, 404) };
            dtpDesde = new DateTimePicker { Name = "dtpDesde", Format = DateTimePickerFormat.Short, Location = new Point(70, 400), Width = 110 };
            lblHasta = new Label { AutoSize = true, Location = new Point(200, 404) };
            dtpHasta = new DateTimePicker { Name = "dtpHasta", Format = DateTimePickerFormat.Short, Location = new Point(255, 400), Width = 110 };
            lblAgrupar = new Label { AutoSize = true, Location = new Point(385, 404) };
            cmbAgrupar = new ComboBox { Name = "cmbAgrupar", DropDownStyle = ComboBoxStyle.DropDownList, Location = new Point(465, 400), Width = 110 };
            btnConsultar = new Button { Name = "btnConsultar", Location = new Point(595, 397), Size = new Size(110, 28) };
            btnConsultar.Click += btnConsultar_Click;

            lblVentas = new Label { AutoSize = true, Location = new Point(12, 436) };
            dgvVentas = CrearGrilla(new Point(12, 456), new Size(370, 190));
            lblPlatos = new Label { AutoSize = true, Location = new Point(402, 436) };
            dgvPlatos = CrearGrilla(new Point(402, 456), new Size(370, 190));

            Controls.AddRange(new Control[]
            {
                lblPeriodo, lblDesde, dtpDesde, lblHasta, dtpHasta, lblAgrupar, cmbAgrupar, btnConsultar,
                lblVentas, dgvVentas, lblPlatos, dgvPlatos
            });
        }

        private static DataGridView CrearGrilla(Point ubicacion, Size tamano)
        {
            return new DataGridView
            {
                Location = ubicacion,
                Size = tamano,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                MultiSelect = false,
                RowHeadersVisible = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };
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

            lblPeriodo.Text = Idioma.Texto("Estadisticas_Periodo");
            lblDesde.Text = Idioma.Texto("Estadisticas_Desde");
            lblHasta.Text = Idioma.Texto("Estadisticas_Hasta");
            lblAgrupar.Text = Idioma.Texto("Estadisticas_Agrupar");
            btnConsultar.Text = Idioma.Texto("Estadisticas_Consultar");
            lblVentas.Text = Idioma.Texto("Estadisticas_VentasPeriodo");
            lblPlatos.Text = Idioma.Texto("Estadisticas_PlatosVendidos");
            cmbAgrupar.DataSource = new[]
            {
                new KeyValuePair<string, string>("Dia", Idioma.Texto("Estadisticas_Dia")),
                new KeyValuePair<string, string>("Semana", Idioma.Texto("Estadisticas_Semana")),
                new KeyValuePair<string, string>("Mes", Idioma.Texto("Estadisticas_Mes"))
            };
            cmbAgrupar.DisplayMember = "Value";
            cmbAgrupar.ValueMember = "Key";
            dtpDesde.Value = DateTime.Today.AddDays(-30);
            dtpHasta.Value = DateTime.Today;
            Consultar();
        }

        private void btnConsultar_Click(object sender, EventArgs e)
        {
            Consultar();
        }

        private void Consultar()
        {
            try
            {
                string agrupar = Convert.ToString(cmbAgrupar.SelectedValue);
                dgvVentas.DataSource = Est.VentasPorPeriodo(dtpDesde.Value.Date, dtpHasta.Value.Date, agrupar);
                dgvPlatos.DataSource = Est.PlatosMasVendidos(dtpDesde.Value.Date, dtpHasta.Value.Date, 10);
                new CL_clsBitacora("Consulta de estadísticas", dtpDesde.Value.ToString("dd/MM/yyyy") + " - " + dtpHasta.Value.ToString("dd/MM/yyyy") + " por " + agrupar, Name);
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show(ex.Message);
            }
            catch (Exception ex)
            {
                CV_ExcepcionBitacora.RegistrarYMostrar(ex, Name);
            }
        }

        private void btnSalir_Click(object sender, System.EventArgs e)
        {
            Close();
        }
    }
}
