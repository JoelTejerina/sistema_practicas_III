using System;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using CapaComun;
using CapaLogicaNegocio;
using CapaLogicaNegocio.Ventas;

namespace CapaVistaUsuario.Ventas
{
    /// <summary>
    /// Caja: apertura con monto inicial, resumen de lo cobrado por forma de pago, cuentas por mesa,
    /// cierre con el monto contado (arqueo) e historial de cierres.
    /// </summary>
    public class frmCaja : Form
    {
        private readonly CN_Caja caja = new CN_Caja();
        private readonly CV_Obligatorios obligatorios = new CV_Obligatorios();
        private int idCaja;
        private decimal montoInicial;

        private readonly Label lblEstado = new Label();
        private readonly Label lblMontoInicial = new Label();
        private readonly TextBox txtMontoInicial = new TextBox();
        private readonly Button btnAbrir = new Button();

        private readonly GroupBox grpResumen = new GroupBox();
        private readonly DataGridView dgvCobros = new DataGridView();
        private readonly Label lblTotalCobrado = new Label();
        private readonly Label lblEsperado = new Label();
        private readonly Label lblContado = new Label();
        private readonly TextBox txtContado = new TextBox();
        private readonly Label lblObservaciones = new Label();
        private readonly TextBox txtObservaciones = new TextBox();
        private readonly Label lblSinCobrar = new Label();
        private readonly Button btnCerrarCaja = new Button();

        private readonly GroupBox grpMesas = new GroupBox();
        private readonly DataGridView dgvMesas = new DataGridView();
        private readonly GroupBox grpHistorial = new GroupBox();
        private readonly DataGridView dgvHistorial = new DataGridView();
        private readonly Button btnSalir = new Button();

        public frmCaja()
        {
            Name = "frmCaja";
            Text = Idioma.Texto("Caja_Titulo");
            ClientSize = new Size(900, 640);
            StartPosition = FormStartPosition.CenterParent;
            MinimizeBox = false;

            lblEstado.AutoSize = true;
            lblEstado.Font = new Font(Font.FontFamily, 11f, FontStyle.Bold);
            lblEstado.Location = new Point(12, 12);

            lblMontoInicial.AutoSize = true;
            lblMontoInicial.Location = new Point(12, 46);
            lblMontoInicial.Text = Idioma.Texto("Caja_MontoInicial");
            txtMontoInicial.Name = "txtMontoInicial";
            txtMontoInicial.Location = new Point(150, 43);
            txtMontoInicial.Size = new Size(120, 20);
            btnAbrir.Name = "btnAbrirCaja";
            btnAbrir.Text = Idioma.Texto("Caja_Abrir");
            btnAbrir.Location = new Point(285, 40);
            btnAbrir.Size = new Size(130, 26);
            btnAbrir.Click += btnAbrir_Click;

            ConfigurarGrilla(dgvCobros);
            ConfigurarGrilla(dgvMesas);
            ConfigurarGrilla(dgvHistorial);

            grpResumen.Text = Idioma.Texto("Caja_Resumen");
            grpResumen.Location = new Point(12, 78);
            grpResumen.Size = new Size(440, 270);
            dgvCobros.Location = new Point(10, 20);
            dgvCobros.Size = new Size(420, 100);
            lblTotalCobrado.AutoSize = true;
            lblTotalCobrado.Location = new Point(10, 128);
            lblEsperado.AutoSize = true;
            lblEsperado.Location = new Point(10, 150);
            lblContado.AutoSize = true;
            lblContado.Location = new Point(10, 178);
            lblContado.Text = Idioma.Texto("Caja_MontoContado");
            txtContado.Name = "txtMontoContado";
            txtContado.Location = new Point(150, 175);
            txtContado.Size = new Size(120, 20);
            lblObservaciones.AutoSize = true;
            lblObservaciones.Location = new Point(10, 206);
            lblObservaciones.Text = Idioma.Texto("Caja_Observaciones");
            txtObservaciones.Name = "txtObservaciones";
            txtObservaciones.Location = new Point(150, 203);
            txtObservaciones.Size = new Size(280, 20);
            txtObservaciones.MaxLength = 200;
            lblSinCobrar.AutoSize = true;
            lblSinCobrar.ForeColor = Color.Firebrick;
            lblSinCobrar.Location = new Point(10, 232);
            btnCerrarCaja.Name = "btnCerrarCaja";
            btnCerrarCaja.Text = Idioma.Texto("Caja_Cerrar");
            btnCerrarCaja.Location = new Point(300, 232);
            btnCerrarCaja.Size = new Size(130, 28);
            btnCerrarCaja.Click += btnCerrarCaja_Click;
            grpResumen.Controls.AddRange(new Control[]
            {
                dgvCobros, lblTotalCobrado, lblEsperado, lblContado, txtContado,
                lblObservaciones, txtObservaciones, lblSinCobrar, btnCerrarCaja
            });

            grpMesas.Text = Idioma.Texto("Caja_CuentasMesa");
            grpMesas.Location = new Point(462, 78);
            grpMesas.Size = new Size(426, 270);
            dgvMesas.Dock = DockStyle.Fill;
            grpMesas.Controls.Add(dgvMesas);

            grpHistorial.Text = Idioma.Texto("Caja_Historial");
            grpHistorial.Location = new Point(12, 356);
            grpHistorial.Size = new Size(876, 232);
            grpHistorial.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            dgvHistorial.Dock = DockStyle.Fill;
            grpHistorial.Controls.Add(dgvHistorial);

            btnSalir.Text = Idioma.Texto("Btn_Cerrar");
            btnSalir.Size = new Size(110, 30);
            btnSalir.Location = new Point(778, 598);
            btnSalir.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnSalir.Click += (s, e) => Close();

            Controls.AddRange(new Control[]
            {
                lblEstado, lblMontoInicial, txtMontoInicial, btnAbrir, grpResumen, grpMesas, grpHistorial, btnSalir
            });

            // Solo el cajero y el gerente operan la caja; el resto la consulta.
            bool soloLectura = PermisosSistema.SoloLectura(PermisosSistema.Caja);
            btnAbrir.Enabled = !soloLectura;
            btnCerrarCaja.Enabled = !soloLectura;
            txtMontoInicial.Enabled = !soloLectura;
            txtContado.Enabled = !soloLectura;
            txtObservaciones.Enabled = !soloLectura;

            Load += (s, e) => Refrescar();
        }

        private static void ConfigurarGrilla(DataGridView g)
        {
            g.ReadOnly = true;
            g.AllowUserToAddRows = false;
            g.AllowUserToDeleteRows = false;
            g.MultiSelect = false;
            g.RowHeadersVisible = false;
            g.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            g.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        // Acepta coma o punto decimal; un monto negativo no es valido.
        private static bool LeerMonto(TextBox caja, out decimal monto)
        {
            return decimal.TryParse(caja.Text.Trim().Replace(',', '.'), NumberStyles.Number, CultureInfo.InvariantCulture, out monto) && monto >= 0;
        }

        private static string Dinero(decimal v)
        {
            return v.ToString("N2", CultureInfo.CurrentCulture);
        }

        // Muestra la pantalla segun haya o no una caja abierta.
        private void Refrescar()
        {
            try
            {
                DataRow abierta = caja.Abierta();
                bool hayAbierta = abierta != null;
                if (hayAbierta)
                {
                    idCaja = Convert.ToInt32(abierta["IdCaja"]);
                    montoInicial = Convert.ToDecimal(abierta["MontoInicial"]);
                    lblEstado.Text = string.Format(Idioma.Texto("Caja_EstadoAbierta"),
                        Convert.ToDateTime(abierta["Apertura"]).ToString("dd/MM/yyyy HH:mm"), abierta["Usuario"]);
                    lblEstado.ForeColor = Color.SeaGreen;
                    CargarResumen();
                }
                else
                {
                    idCaja = 0;
                    lblEstado.Text = Idioma.Texto("Caja_EstadoCerrada");
                    lblEstado.ForeColor = Color.Firebrick;
                    dgvCobros.DataSource = null;
                    lblTotalCobrado.Text = "";
                    lblEsperado.Text = "";
                    lblSinCobrar.Text = "";
                    txtContado.Clear();
                    txtObservaciones.Clear();
                }

                bool puedeOperar = !PermisosSistema.SoloLectura(PermisosSistema.Caja);
                lblMontoInicial.Visible = !hayAbierta;
                txtMontoInicial.Visible = !hayAbierta;
                btnAbrir.Visible = !hayAbierta;
                grpResumen.Visible = hayAbierta;
                btnCerrarCaja.Enabled = hayAbierta && puedeOperar;

                dgvMesas.DataSource = caja.CuentasPorMesa();
                dgvHistorial.DataSource = caja.Historial();
                FormatearHistorial();
            }
            catch (Exception ex)
            {
                CV_ExcepcionBitacora.RegistrarYMostrar(ex, Name);
            }
        }

        // Resumen de lo cobrado en la caja abierta, agrupado por forma de pago.
        private void CargarResumen()
        {
            dgvCobros.DataSource = caja.CobrosPorFormaPago(idCaja);
            decimal total = caja.TotalCobrado(idCaja);
            lblTotalCobrado.Text = Idioma.Texto("Caja_TotalCobrado") + ": " + Dinero(total);
            lblEsperado.Text = Idioma.Texto("Caja_EfectivoEsperado") + ": " + Dinero(caja.EfectivoEsperado(idCaja, montoInicial))
                + "  (" + Idioma.Texto("Caja_MontoInicial") + " " + Dinero(montoInicial) + ")";
            int pendientes = caja.PedidosSinCobrar();
            lblSinCobrar.Text = pendientes > 0 ? string.Format(Idioma.Texto("Caja_SinCobrar"), pendientes) : "";
        }

        private void FormatearHistorial()
        {
            foreach (string col in new[] { "Apertura", "Cierre" })
            {
                if (dgvHistorial.Columns.Contains(col))
                {
                    dgvHistorial.Columns[col].DefaultCellStyle.Format = "dd/MM/yyyy HH:mm";
                }
            }
        }

        private void btnAbrir_Click(object sender, EventArgs e)
        {
            if (!obligatorios.Validar(txtMontoInicial))
            {
                return;
            }
            decimal monto;
            if (!LeerMonto(txtMontoInicial, out monto))
            {
                obligatorios.Marcar(txtMontoInicial);
                MessageBox.Show(Idioma.Texto("Msg_MontoInvalido"));
                return;
            }
            try
            {
                caja.Abrir(monto);
                new CL_clsBitacora("Apertura de caja", "Monto inicial " + Dinero(monto), Name);
                MessageBox.Show(Idioma.Texto("Caja_AbiertaOk"), Idioma.Texto("Caja_Titulo"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtMontoInicial.Clear();
                Refrescar();
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show(ex.Message, Idioma.Texto("Caja_Titulo"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Refrescar();
            }
            catch (Exception ex)
            {
                CV_ExcepcionBitacora.RegistrarYMostrar(ex, Name);
            }
        }

        // Arqueo: se compara lo contado con lo que deberia haber (inicial + cobros en efectivo) y se avisa la diferencia antes de cerrar.
        private void btnCerrarCaja_Click(object sender, EventArgs e)
        {
            if (!obligatorios.Validar(txtContado))
            {
                return;
            }
            decimal contado;
            if (!LeerMonto(txtContado, out contado))
            {
                obligatorios.Marcar(txtContado);
                MessageBox.Show(Idioma.Texto("Msg_MontoInvalido"));
                return;
            }
            try
            {
                decimal esperado = caja.EfectivoEsperado(idCaja, montoInicial);
                decimal diferencia = contado - esperado;
                string aviso = string.Format(Idioma.Texto("Caja_ConfirmCierre"), Dinero(esperado), Dinero(contado), Dinero(diferencia));
                int pendientes = caja.PedidosSinCobrar();
                if (pendientes > 0)
                {
                    aviso += Environment.NewLine + Environment.NewLine + string.Format(Idioma.Texto("Caja_SinCobrar"), pendientes);
                }
                if (MessageBox.Show(aviso, Idioma.Texto("Caja_Cerrar"), MessageBoxButtons.OKCancel, MessageBoxIcon.Question) != DialogResult.OK)
                {
                    return;
                }
                caja.Cerrar(idCaja, contado, txtObservaciones.Text.Trim());
                new CL_clsBitacora("Cierre de caja", "Caja " + idCaja + ", diferencia " + Dinero(diferencia), Name);
                MessageBox.Show(Idioma.Texto("Caja_CerradaOk"), Idioma.Texto("Caja_Titulo"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                Refrescar();
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show(ex.Message, Idioma.Texto("Caja_Titulo"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                CV_ExcepcionBitacora.RegistrarYMostrar(ex, Name);
            }
        }
    }
}
