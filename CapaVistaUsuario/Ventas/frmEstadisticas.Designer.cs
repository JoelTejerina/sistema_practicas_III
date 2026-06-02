namespace CapaVistaUsuario.Ventas
{
    partial class frmEstadisticas
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.lblResumen = new System.Windows.Forms.Label();
            this.dgvResumen = new System.Windows.Forms.DataGridView();
            this.lblHoy = new System.Windows.Forms.Label();
            this.dgvHoy = new System.Windows.Forms.DataGridView();
            this.btnSalir = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvResumen)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvHoy)).BeginInit();
            this.SuspendLayout();
            this.lblResumen.AutoSize = true;
            this.lblResumen.Location = new System.Drawing.Point(12, 12);
            this.lblResumen.Name = "lblResumen";
            this.lblResumen.Text = "Pedidos por estado";
            this.dgvResumen.Location = new System.Drawing.Point(12, 32);
            this.dgvResumen.Name = "dgvResumen";
            this.dgvResumen.Size = new System.Drawing.Size(760, 120);
            this.dgvResumen.ReadOnly = true;
            this.lblHoy.AutoSize = true;
            this.lblHoy.Location = new System.Drawing.Point(12, 165);
            this.lblHoy.Name = "lblHoy";
            this.lblHoy.Text = "Pedidos del día";
            this.dgvHoy.Location = new System.Drawing.Point(12, 185);
            this.dgvHoy.Name = "dgvHoy";
            this.dgvHoy.Size = new System.Drawing.Size(760, 180);
            this.dgvHoy.ReadOnly = true;
            this.btnSalir.Location = new System.Drawing.Point(682, 380);
            this.btnSalir.Name = "btnSalir";
            this.btnSalir.Size = new System.Drawing.Size(90, 30);
            this.btnSalir.Text = "Salir";
            this.btnSalir.Click += new System.EventHandler(this.btnSalir_Click);
            this.ClientSize = new System.Drawing.Size(784, 421);
            this.Controls.Add(this.btnSalir);
            this.Controls.Add(this.dgvHoy);
            this.Controls.Add(this.lblHoy);
            this.Controls.Add(this.dgvResumen);
            this.Controls.Add(this.lblResumen);
            this.Name = "frmEstadisticas";
            this.Text = "Estadísticas";
            this.Load += new System.EventHandler(this.frmEstadisticas_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvResumen)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvHoy)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.Label lblResumen;
        private System.Windows.Forms.DataGridView dgvResumen;
        private System.Windows.Forms.Label lblHoy;
        private System.Windows.Forms.DataGridView dgvHoy;
        private System.Windows.Forms.Button btnSalir;
    }
}
