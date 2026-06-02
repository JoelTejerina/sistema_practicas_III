using System;
using System.Data;
using System.Text;
using System.Windows.Forms;
using CapaComun;
using CapaLogicaNegocio.Administrador;
using CapaVistaUsuario;

namespace CapaVistaUsuario.Administrador
{
    public partial class frmBitacora : Form
    {
        private readonly CN_Bitacora bit = new CN_Bitacora();
        private bool detalleExpandido;
        private const int AlturaPanelDetalle = 180;

        public frmBitacora()
        {
            InitializeComponent();
        }

        private void frmBitacora_Load(object sender, EventArgs e)
        {
            dgvBitacora.ReadOnly = true;
            dgvBitacora.AllowUserToAddRows = false;
            dgvBitacora.MultiSelect = false;
            dgvBitacora.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvBitacora.DefaultCellStyle.WrapMode = DataGridViewTriState.False;

            dtpFechaDesde.Value = DateTime.Today;
            dtpFechaHasta.Value = DateTime.Today;
            dtpFechaDesde.Checked = false;
            dtpFechaHasta.Checked = false;

            AplicarIdioma();
            CargarGrilla();
        }

        private void AplicarIdioma()
        {
            Text = Idioma.Texto("Bitacora_Titulo");
            lblFiltroEvento.Text = Idioma.Texto("Bitacora_FiltroEvento");
            lblFiltroUsuario.Text = Idioma.Texto("Bitacora_FiltroUsuario");
            lblFechaDesde.Text = Idioma.Texto("Bitacora_FechaDesde");
            lblFechaHasta.Text = Idioma.Texto("Bitacora_FechaHasta");
            btnFiltrar.Text = Idioma.Texto("Bitacora_Filtrar");
            btnLimpiarFiltros.Text = Idioma.Texto("Btn_Limpiar");
            btnVerDetalle.Text = Idioma.Texto("Bitacora_VerDetalle");
            lblDetalleTitulo.Text = Idioma.Texto("Bitacora_DetalleTitulo");
            btnSalir.Text = Idioma.Texto("Btn_Salir");
        }

        private void CargarGrilla()
        {
            DateTime? desde = dtpFechaDesde.Checked ? dtpFechaDesde.Value.Date : (DateTime?)null;
            DateTime? hasta = dtpFechaHasta.Checked ? dtpFechaHasta.Value.Date : (DateTime?)null;

            if (desde.HasValue && hasta.HasValue && desde > hasta)
            {
                MessageBox.Show(Idioma.Texto("Bitacora_FechaInvalida"));
                return;
            }

            try
            {
                dgvBitacora.DataSource = bit.MostrarBitacoraFiltrada(
                    txtFiltroEvento.Text,
                    txtFiltroUsuario.Text,
                    desde,
                    hasta);

                AjustarColumnasGrilla();
                OcultarPanelDetalle();
            }
            catch (Exception ex)
            {
                CV_ExcepcionBitacora.RegistrarYMostrar(ex, Name);
            }
        }

        private void AjustarColumnasGrilla()
        {
            if (dgvBitacora.Columns.Count == 0)
            {
                return;
            }

            if (dgvBitacora.Columns.Contains("Detalle"))
            {
                dgvBitacora.Columns["Detalle"].FillWeight = 200;
            }
            if (dgvBitacora.Columns.Contains("Evento"))
            {
                dgvBitacora.Columns["Evento"].FillWeight = 80;
            }
            if (dgvBitacora.Columns.Contains("Usuario"))
            {
                dgvBitacora.Columns["Usuario"].FillWeight = 90;
            }
            if (dgvBitacora.Columns.Contains("Fecha"))
            {
                dgvBitacora.Columns["Fecha"].DefaultCellStyle.Format = "dd/MM/yyyy";
            }
        }

        private void btnFiltrar_Click(object sender, EventArgs e)
        {
            CargarGrilla();
        }

        private void btnLimpiarFiltros_Click(object sender, EventArgs e)
        {
            txtFiltroEvento.Clear();
            txtFiltroUsuario.Clear();
            dtpFechaDesde.Checked = false;
            dtpFechaHasta.Checked = false;
            CargarGrilla();
        }

        private void Filtros_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                CargarGrilla();
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }

        private void dgvBitacora_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                MostrarBotonDetalle();
            }
        }

        private void dgvBitacora_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvBitacora.SelectedRows.Count > 0)
            {
                MostrarBotonDetalle();
                if (detalleExpandido)
                {
                    CargarTextoDetalle();
                }
            }
            else
            {
                btnVerDetalle.Visible = false;
                OcultarPanelDetalle();
            }
        }

        private void MostrarBotonDetalle()
        {
            btnVerDetalle.Visible = dgvBitacora.SelectedRows.Count > 0;
            if (!detalleExpandido)
            {
                btnVerDetalle.Text = Idioma.Texto("Bitacora_VerDetalle");
            }
        }

        private void btnVerDetalle_Click(object sender, EventArgs e)
        {
            if (dgvBitacora.SelectedRows.Count == 0)
            {
                return;
            }

            if (detalleExpandido)
            {
                OcultarPanelDetalle();
            }
            else
            {
                ExpandirPanelDetalle();
            }
        }

        private void ExpandirPanelDetalle()
        {
            CargarTextoDetalle();
            panelDetalle.Visible = true;
            panelDetalle.Height = AlturaPanelDetalle;
            detalleExpandido = true;
            btnVerDetalle.Text = Idioma.Texto("Bitacora_OcultarDetalle");
        }

        private void OcultarPanelDetalle()
        {
            panelDetalle.Visible = false;
            panelDetalle.Height = 0;
            detalleExpandido = false;
            txtDetalleCompleto.Clear();
            if (btnVerDetalle.Visible)
            {
                btnVerDetalle.Text = Idioma.Texto("Bitacora_VerDetalle");
            }
        }

        private void CargarTextoDetalle()
        {
            if (dgvBitacora.SelectedRows.Count == 0)
            {
                return;
            }

            DataGridViewRow fila = dgvBitacora.SelectedRows[0];
            var sb = new StringBuilder();

            AgregarLinea(sb, "Fecha", fila, "Fecha");
            AgregarLinea(sb, "Hora", fila, "Hora");
            AgregarLinea(sb, "Usuario", fila, "Usuario");
            AgregarLinea(sb, "Evento", fila, "Evento");
            AgregarLinea(sb, "Origen", fila, "Origen");
            sb.AppendLine();
            sb.AppendLine("--- Detalle ---");
            sb.AppendLine(ObtenerValorCelda(fila, "Detalle"));

            txtDetalleCompleto.Text = sb.ToString();
        }

        private static void AgregarLinea(StringBuilder sb, string etiqueta, DataGridViewRow fila, string columna)
        {
            string valor = ObtenerValorCelda(fila, columna);
            if (!string.IsNullOrEmpty(valor))
            {
                sb.AppendLine(etiqueta + ": " + valor);
            }
        }

        private static string ObtenerValorCelda(DataGridViewRow fila, string nombreColumna)
        {
            if (!fila.DataGridView.Columns.Contains(nombreColumna))
            {
                return "";
            }
            object valor = fila.Cells[nombreColumna].Value;
            if (valor == null || valor == DBNull.Value)
            {
                return "";
            }
            if (valor is DateTime fecha)
            {
                return fecha.ToString("dd/MM/yyyy");
            }
            return valor.ToString();
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
