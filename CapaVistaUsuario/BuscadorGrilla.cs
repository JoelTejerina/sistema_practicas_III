using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CapaComun;

namespace CapaVistaUsuario
{
    /// <summary>
    /// Cuadro de busqueda para una grilla: filtra las filas a medida que se escribe,
    /// buscando el texto en las columnas indicadas (cualquier coincidencia parcial).
    ///
    /// Se coloca justo encima de la grilla, sin mover ningun otro control:
    ///     BuscadorGrilla.Agregar(dgvStock, "NumeroLote");
    ///
    /// Es un UserControl a proposito: CV_Utiles.BloquearControles/LimpiarControles
    /// deshabilitan y vacian los TextBox que cuelgan del formulario o de un Panel,
    /// y el buscador tiene que seguir usable mientras el formulario esta bloqueado.
    /// El filtro se vuelve a aplicar cada vez que la grilla recibe datos nuevos.
    /// </summary>
    public class BuscadorGrilla : UserControl
    {
        private const int AltoBuscador = 26;
        private const int Separacion = 2;

        private readonly DataGridView grilla;
        private readonly string[] columnas;
        private readonly Label lblBuscar = new Label();
        private readonly TextBox txtBuscar = new TextBox();

        private BuscadorGrilla(DataGridView grilla, string[] columnas)
        {
            this.grilla = grilla;
            this.columnas = columnas;

            lblBuscar.Text = Idioma.Texto("Buscar");
            lblBuscar.AutoSize = true;
            lblBuscar.Location = new Point(0, 5);

            txtBuscar.Location = new Point(lblBuscar.PreferredWidth + 6, 2);
            txtBuscar.Width = Math.Max(80, grilla.Width - txtBuscar.Left);
            txtBuscar.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtBuscar.TextChanged += (s, e) => Aplicar();
            txtBuscar.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Escape)
                {
                    txtBuscar.Clear();
                    e.SuppressKeyPress = true;
                }
            };

            Controls.Add(lblBuscar);
            Controls.Add(txtBuscar);

            grilla.DataSourceChanged += (s, e) => Aplicar();
        }

        /// <summary>
        /// Agrega el buscador encima de la grilla (la grilla pierde su franja superior,
        /// su borde inferior no se mueve). Las columnas que no existan se ignoran.
        /// </summary>
        public static BuscadorGrilla Agregar(DataGridView grilla, params string[] columnas)
        {
            BuscadorGrilla buscador = new BuscadorGrilla(grilla, columnas);
            buscador.Location = grilla.Location;
            buscador.Size = new Size(grilla.Width, AltoBuscador);
            buscador.Anchor = (grilla.Anchor & (AnchorStyles.Left | AnchorStyles.Right)) | AnchorStyles.Top;

            grilla.Top += AltoBuscador + Separacion;
            grilla.Height -= AltoBuscador + Separacion;

            grilla.Parent.Controls.Add(buscador);
            return buscador;
        }

        private void Aplicar()
        {
            DataView vista = (grilla.DataSource as DataTable)?.DefaultView ?? grilla.DataSource as DataView;
            if (vista == null)
            {
                return;
            }

            string texto = txtBuscar.Text.Trim();
            string[] existentes = columnas.Where(c => vista.Table.Columns.Contains(c)).ToArray();
            if (texto.Length == 0 || existentes.Length == 0)
            {
                vista.RowFilter = string.Empty;
                return;
            }

            // Convert(..., String) permite buscar tambien en columnas numericas (NroDoc, Cantidad, etc.).
            string patron = "'%" + EscaparLike(texto) + "%'";
            vista.RowFilter = string.Join(" OR ",
                existentes.Select(c => "Convert([" + c + "], 'System.String') LIKE " + patron));
        }

        private static string EscaparLike(string texto)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in texto)
            {
                switch (c)
                {
                    case '\'': sb.Append("''"); break;
                    case '[': sb.Append("[[]"); break;
                    case '%': sb.Append("[%]"); break;
                    case '*': sb.Append("[*]"); break;
                    default: sb.Append(c); break;
                }
            }
            return sb.ToString();
        }
    }
}
