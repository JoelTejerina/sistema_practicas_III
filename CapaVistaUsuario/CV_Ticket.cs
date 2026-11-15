using System;
using System.Drawing;
using System.Drawing.Printing;
using System.Windows.Forms;

namespace CapaVistaUsuario
{
    /// <summary>Vista previa e impresion del ticket (comprobante de venta no fiscal) en papel angosto de 80 mm.</summary>
    static class CV_Ticket
    {
        public static void MostrarVistaPrevia(IWin32Window dueno, string texto, string titulo)
        {
            string[] lineas = (texto ?? "").Replace("\r", "").TrimEnd('\n').Split('\n');
            int siguiente = 0;

            using (PrintDocument documento = new PrintDocument())
            {
                documento.DocumentName = titulo;
                documento.DefaultPageSettings.PaperSize = new PaperSize("Ticket 80mm", 315, 900);
                documento.DefaultPageSettings.Margins = new Margins(10, 10, 10, 10);
                documento.PrintPage += (s, e) =>
                {
                    using (Font fuente = new Font("Consolas", 8.5f))
                    {
                        float alto = fuente.GetHeight(e.Graphics);
                        float y = e.MarginBounds.Top;
                        while (siguiente < lineas.Length && y + alto <= e.MarginBounds.Bottom)
                        {
                            e.Graphics.DrawString(lineas[siguiente], fuente, Brushes.Black, e.MarginBounds.Left, y);
                            y += alto;
                            siguiente++;
                        }
                        e.HasMorePages = siguiente < lineas.Length;
                    }
                };

                using (PrintPreviewDialog vista = new PrintPreviewDialog())
                {
                    vista.Document = documento;
                    vista.Text = titulo;
                    vista.Width = 520;
                    vista.Height = 760;
                    vista.StartPosition = FormStartPosition.CenterParent;
                    vista.ShowDialog(dueno);
                }
            }
        }
    }
}
