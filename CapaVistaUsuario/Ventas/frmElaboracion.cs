using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using CapaComun;
using CapaLogicaNegocio;
using CapaLogicaNegocio.Ventas;

namespace CapaVistaUsuario.Ventas
{
    /// <summary>
    /// Gestion de Elaboracion (cocina): muestra los pedidos que estan en elaboracion para poder
    /// iniciarlos, darlos por realizados o cancelarlos (cancelar devuelve los insumos al stock).
    /// Tambien muestra cuantas unidades de cada platillo se pueden preparar con el stock actual.
    /// </summary>
    public class frmElaboracion : Form
    {
        private readonly CN_Pedido pedidos = new CN_Pedido();
        private readonly CN_Receta recetas = new CN_Receta();
        private readonly bool soloLectura;

        private readonly Label lblPedidos = new Label();
        private readonly DataGridView dgvPedidos = new DataGridView();
        private readonly Label lblCapacidad = new Label();
        private readonly DataGridView dgvCapacidad = new DataGridView();
        private readonly Button btnIniciar = new Button();
        private readonly Button btnListo = new Button();
        private readonly Button btnCancelarElab = new Button();
        private readonly Button btnActualizar = new Button();
        private readonly Button btnSalir = new Button();

        public frmElaboracion()
        {
            Name = "frmElaboracion";
            soloLectura = PermisosSistema.SoloLectura(PermisosSistema.Elaboracion);
            ConstruirPantalla();
            Load += (s, e) => Cargar();
        }

        private void ConstruirPantalla()
        {
            Text = Idioma.Texto("Elab_Titulo");
            ClientSize = new Size(784, 560);
            StartPosition = FormStartPosition.CenterParent;

            lblPedidos.AutoSize = true;
            lblPedidos.Font = new Font(Font, FontStyle.Bold);
            lblPedidos.Location = new Point(12, 12);

            ConfigurarGrilla(dgvPedidos, new Point(12, 34), new Size(760, 270));
            ConfigurarGrilla(dgvCapacidad, new Point(12, 348), new Size(760, 150));

            lblCapacidad.AutoSize = true;
            lblCapacidad.Font = new Font(Font, FontStyle.Bold);
            lblCapacidad.Location = new Point(12, 326);

            btnIniciar.Location = new Point(12, 512);
            btnIniciar.Size = new Size(150, 34);
            btnIniciar.Click += (s, e) => CambiarEstado("En curso", "Elaboración iniciada");
            btnListo.Location = new Point(170, 512);
            btnListo.Size = new Size(150, 34);
            btnListo.Click += (s, e) => CambiarEstado("Listo", "Elaboración finalizada");
            btnCancelarElab.Location = new Point(328, 512);
            btnCancelarElab.Size = new Size(170, 34);
            btnCancelarElab.Click += btnCancelarElab_Click;
            btnActualizar.Location = new Point(560, 512);
            btnActualizar.Size = new Size(100, 34);
            btnActualizar.Click += (s, e) => Cargar();
            btnSalir.Location = new Point(668, 512);
            btnSalir.Size = new Size(104, 34);
            btnSalir.Click += (s, e) => Close();

            Controls.AddRange(new Control[]
            {
                lblPedidos, dgvPedidos, lblCapacidad, dgvCapacidad,
                btnIniciar, btnListo, btnCancelarElab, btnActualizar, btnSalir
            });

            lblPedidos.Text = Idioma.Texto("Elab_Pedidos");
            lblCapacidad.Text = Idioma.Texto("Elab_Capacidad");
            btnIniciar.Text = Idioma.Texto("Elab_Iniciar");
            btnListo.Text = Idioma.Texto("Elab_Listo");
            btnCancelarElab.Text = Idioma.Texto("Elab_Cancelar");
            btnActualizar.Text = Idioma.Texto("Btn_Actualizar");
            btnSalir.Text = Idioma.Texto("Btn_Salir");

            if (soloLectura)
            {
                btnIniciar.Visible = btnListo.Visible = btnCancelarElab.Visible = false;
            }
        }

        private static void ConfigurarGrilla(DataGridView g, Point ubicacion, Size tamano)
        {
            g.Location = ubicacion;
            g.Size = tamano;
            g.ReadOnly = true;
            g.AllowUserToAddRows = false;
            g.AllowUserToDeleteRows = false;
            g.MultiSelect = false;
            g.RowHeadersVisible = false;
            g.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            g.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        private void Cargar()
        {
            try
            {
                dgvPedidos.DataSource = pedidos.MostrarEnElaboracion();
                if (dgvPedidos.Columns.Contains("Fecha"))
                {
                    dgvPedidos.Columns["Fecha"].DefaultCellStyle.Format = "dd/MM/yyyy";
                    dgvPedidos.Columns["IdPedido"].HeaderText = "Nº";
                    dgvPedidos.Columns["IdPedido"].FillWeight = 35;
                    dgvPedidos.Columns["Detalle"].FillWeight = 200;
                }

                DataTable capacidad = recetas.Capacidad();
                dgvCapacidad.DataSource = capacidad;
                if (dgvCapacidad.Columns.Contains("IdMenu"))
                {
                    dgvCapacidad.Columns["IdMenu"].Visible = false;
                    dgvCapacidad.Columns["Capacidad"].HeaderText = Idioma.Texto("Elab_CapacidadCol");
                    dgvCapacidad.Columns["Capacidad"].DefaultCellStyle.NullValue = Idioma.Texto("Elab_SinReceta");
                }
            }
            catch (Exception ex)
            {
                CV_ExcepcionBitacora.RegistrarYMostrar(ex, Name);
            }
        }

        private bool PedidoElegido(out int idPedido, out string estado)
        {
            idPedido = 0;
            estado = "";
            if (dgvPedidos.CurrentRow == null)
            {
                MessageBox.Show(Idioma.Texto("Msg_SeleccioneRegistro"));
                return false;
            }
            idPedido = Convert.ToInt32(dgvPedidos.CurrentRow.Cells["IdPedido"].Value);
            estado = Convert.ToString(dgvPedidos.CurrentRow.Cells["Estado"].Value);
            return true;
        }

        // Solo se puede iniciar un pedido Pendiente y terminar uno En curso; el resto de los cambios se rechaza.
        private void CambiarEstado(string nuevoEstado, string evento)
        {
            int id;
            string estado;
            if (!PedidoElegido(out id, out estado)) return;

            // Iniciar solo si esta pendiente; finalizar solo si esta en curso.
            if ((nuevoEstado == "En curso" && estado != "Pendiente") || (nuevoEstado == "Listo" && estado != "En curso"))
            {
                MessageBox.Show(Idioma.Texto("Msg_EstadoNoPermitido"));
                return;
            }

            try
            {
                pedidos.CambiarEstado(id, nuevoEstado);
                new CL_clsBitacora(evento, "Pedido " + id, "frmElaboracion");
                Cargar();
            }
            catch (StockInsuficienteException ex)
            {
                MessageBox.Show(ex.Message, Idioma.Texto("Stock_AlertaTitulo"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                CV_ExcepcionBitacora.RegistrarYMostrar(ex, Name);
            }
        }

        private void btnCancelarElab_Click(object sender, EventArgs e)
        {
            int id;
            string estado;
            if (!PedidoElegido(out id, out estado)) return;

            DialogResult r = MessageBox.Show(Idioma.Texto("Elab_ConfirmCancelar"), Idioma.Texto("Elab_Cancelar"),
                MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            if (r != DialogResult.OK) return;

            try
            {
                // Cancelar la elaboracion devuelve al stock los insumos que se habian descontado.
                pedidos.CambiarEstado(id, "Cancelado");
                new CL_clsBitacora("Cancelación de elaboración", "Pedido " + id, "frmElaboracion");
                Cargar();
            }
            catch (Exception ex)
            {
                CV_ExcepcionBitacora.RegistrarYMostrar(ex, Name);
            }
        }
    }
}
