using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using CapaComun;
using CapaLogicaNegocio.Compras;

namespace CapaVistaUsuario.Compras
{
    /// <summary>
    /// Trazabilidad de inventarios: historial de movimientos de stock (ingresos, egresos por pedidos,
    /// ajustes, bajas y devoluciones) de un producto o de todos.
    /// </summary>
    public class frmHistorialStock : Form
    {
        private readonly DataGridView dgvHistorial = new DataGridView();
        private readonly Label lblTitulo = new Label();
        private readonly Button btnCerrar = new Button();

        public frmHistorialStock(int idProducto, string producto)
        {
            Name = "frmHistorialStock";
            Text = Idioma.Texto("Stock_Historial_Titulo");
            ClientSize = new Size(860, 450);
            StartPosition = FormStartPosition.CenterParent;
            MinimizeBox = false;

            lblTitulo.AutoSize = true;
            lblTitulo.Font = new Font(Font.FontFamily, 11f, FontStyle.Bold);
            lblTitulo.Location = new Point(12, 12);
            lblTitulo.Text = idProducto > 0
                ? Idioma.Texto("Stock_Historial_Titulo") + ": " + producto
                : Idioma.Texto("Stock_Historial_Titulo") + " (" + Idioma.Texto("Todos") + ")";

            dgvHistorial.Location = new Point(12, 40);
            dgvHistorial.Size = new Size(836, 360);
            dgvHistorial.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            dgvHistorial.ReadOnly = true;
            dgvHistorial.AllowUserToAddRows = false;
            dgvHistorial.AllowUserToDeleteRows = false;
            dgvHistorial.MultiSelect = false;
            dgvHistorial.RowHeadersVisible = false;
            dgvHistorial.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvHistorial.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            btnCerrar.Text = Idioma.Texto("Btn_Cerrar");
            btnCerrar.Size = new Size(110, 30);
            btnCerrar.Location = new Point(738, 410);
            btnCerrar.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnCerrar.Click += (s, e) => Close();

            Controls.AddRange(new Control[] { lblTitulo, dgvHistorial, btnCerrar });
            BuscadorGrilla.Agregar(dgvHistorial, "Producto", "Lote", "Tipo", "Usuario", "Detalle");

            Load += (s, e) =>
            {
                try
                {
                    dgvHistorial.DataSource = new CN_Stock().Historial(idProducto);
                    if (dgvHistorial.Columns.Contains("Fecha"))
                    {
                        dgvHistorial.Columns["Fecha"].DefaultCellStyle.Format = "dd/MM/yyyy HH:mm";
                    }
                }
                catch (Exception ex)
                {
                    CV_ExcepcionBitacora.RegistrarYMostrar(ex, Name);
                }
            };
        }
    }
}
