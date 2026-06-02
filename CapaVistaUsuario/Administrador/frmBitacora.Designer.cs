namespace CapaVistaUsuario.Administrador
{
    partial class frmBitacora
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
            this.panelFiltros = new System.Windows.Forms.Panel();
            this.btnLimpiarFiltros = new System.Windows.Forms.Button();
            this.btnFiltrar = new System.Windows.Forms.Button();
            this.dtpFechaHasta = new System.Windows.Forms.DateTimePicker();
            this.lblFechaHasta = new System.Windows.Forms.Label();
            this.dtpFechaDesde = new System.Windows.Forms.DateTimePicker();
            this.lblFechaDesde = new System.Windows.Forms.Label();
            this.txtFiltroUsuario = new System.Windows.Forms.TextBox();
            this.lblFiltroUsuario = new System.Windows.Forms.Label();
            this.txtFiltroEvento = new System.Windows.Forms.TextBox();
            this.lblFiltroEvento = new System.Windows.Forms.Label();
            this.dgvBitacora = new System.Windows.Forms.DataGridView();
            this.btnVerDetalle = new System.Windows.Forms.Button();
            this.panelDetalle = new System.Windows.Forms.Panel();
            this.txtDetalleCompleto = new System.Windows.Forms.TextBox();
            this.lblDetalleTitulo = new System.Windows.Forms.Label();
            this.btnSalir = new System.Windows.Forms.Button();
            this.panelFiltros.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvBitacora)).BeginInit();
            this.panelDetalle.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelFiltros
            // 
            this.panelFiltros.Controls.Add(this.btnLimpiarFiltros);
            this.panelFiltros.Controls.Add(this.btnFiltrar);
            this.panelFiltros.Controls.Add(this.dtpFechaHasta);
            this.panelFiltros.Controls.Add(this.lblFechaHasta);
            this.panelFiltros.Controls.Add(this.dtpFechaDesde);
            this.panelFiltros.Controls.Add(this.lblFechaDesde);
            this.panelFiltros.Controls.Add(this.txtFiltroUsuario);
            this.panelFiltros.Controls.Add(this.lblFiltroUsuario);
            this.panelFiltros.Controls.Add(this.txtFiltroEvento);
            this.panelFiltros.Controls.Add(this.lblFiltroEvento);
            this.panelFiltros.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelFiltros.Location = new System.Drawing.Point(0, 0);
            this.panelFiltros.Name = "panelFiltros";
            this.panelFiltros.Padding = new System.Windows.Forms.Padding(6);
            this.panelFiltros.Size = new System.Drawing.Size(884, 72);
            this.panelFiltros.TabIndex = 0;
            // 
            // btnLimpiarFiltros
            // 
            this.btnLimpiarFiltros.Location = new System.Drawing.Point(790, 38);
            this.btnLimpiarFiltros.Name = "btnLimpiarFiltros";
            this.btnLimpiarFiltros.Size = new System.Drawing.Size(82, 26);
            this.btnLimpiarFiltros.TabIndex = 9;
            this.btnLimpiarFiltros.Text = "Limpiar";
            this.btnLimpiarFiltros.UseVisualStyleBackColor = true;
            this.btnLimpiarFiltros.Click += new System.EventHandler(this.btnLimpiarFiltros_Click);
            // 
            // btnFiltrar
            // 
            this.btnFiltrar.Location = new System.Drawing.Point(698, 38);
            this.btnFiltrar.Name = "btnFiltrar";
            this.btnFiltrar.Size = new System.Drawing.Size(82, 26);
            this.btnFiltrar.TabIndex = 8;
            this.btnFiltrar.Text = "Filtrar";
            this.btnFiltrar.UseVisualStyleBackColor = true;
            this.btnFiltrar.Click += new System.EventHandler(this.btnFiltrar_Click);
            // 
            // dtpFechaHasta
            // 
            this.dtpFechaHasta.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpFechaHasta.Location = new System.Drawing.Point(548, 40);
            this.dtpFechaHasta.Name = "dtpFechaHasta";
            this.dtpFechaHasta.Size = new System.Drawing.Size(110, 20);
            this.dtpFechaHasta.TabIndex = 7;
            this.dtpFechaHasta.ShowCheckBox = true;
            this.dtpFechaHasta.Checked = false;
            // 
            // lblFechaHasta
            // 
            this.lblFechaHasta.AutoSize = true;
            this.lblFechaHasta.Location = new System.Drawing.Point(478, 43);
            this.lblFechaHasta.Name = "lblFechaHasta";
            this.lblFechaHasta.Size = new System.Drawing.Size(64, 13);
            this.lblFechaHasta.TabIndex = 6;
            this.lblFechaHasta.Text = "Fecha hasta";
            // 
            // dtpFechaDesde
            // 
            this.dtpFechaDesde.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpFechaDesde.Location = new System.Drawing.Point(348, 40);
            this.dtpFechaDesde.Name = "dtpFechaDesde";
            this.dtpFechaDesde.Size = new System.Drawing.Size(110, 20);
            this.dtpFechaDesde.TabIndex = 5;
            this.dtpFechaDesde.ShowCheckBox = true;
            this.dtpFechaDesde.Checked = false;
            // 
            // lblFechaDesde
            // 
            this.lblFechaDesde.AutoSize = true;
            this.lblFechaDesde.Location = new System.Drawing.Point(278, 43);
            this.lblFechaDesde.Name = "lblFechaDesde";
            this.lblFechaDesde.Size = new System.Drawing.Size(67, 13);
            this.lblFechaDesde.TabIndex = 4;
            this.lblFechaDesde.Text = "Fecha desde";
            // 
            // txtFiltroUsuario
            // 
            this.txtFiltroUsuario.Location = new System.Drawing.Point(348, 12);
            this.txtFiltroUsuario.Name = "txtFiltroUsuario";
            this.txtFiltroUsuario.Size = new System.Drawing.Size(310, 20);
            this.txtFiltroUsuario.TabIndex = 3;
            this.txtFiltroUsuario.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Filtros_KeyDown);
            // 
            // lblFiltroUsuario
            // 
            this.lblFiltroUsuario.AutoSize = true;
            this.lblFiltroUsuario.Location = new System.Drawing.Point(278, 15);
            this.lblFiltroUsuario.Name = "lblFiltroUsuario";
            this.lblFiltroUsuario.Size = new System.Drawing.Size(43, 13);
            this.lblFiltroUsuario.TabIndex = 2;
            this.lblFiltroUsuario.Text = "Usuario";
            // 
            // txtFiltroEvento
            // 
            this.txtFiltroEvento.Location = new System.Drawing.Point(62, 12);
            this.txtFiltroEvento.Name = "txtFiltroEvento";
            this.txtFiltroEvento.Size = new System.Drawing.Size(200, 20);
            this.txtFiltroEvento.TabIndex = 1;
            this.txtFiltroEvento.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Filtros_KeyDown);
            // 
            // lblFiltroEvento
            // 
            this.lblFiltroEvento.AutoSize = true;
            this.lblFiltroEvento.Location = new System.Drawing.Point(12, 15);
            this.lblFiltroEvento.Name = "lblFiltroEvento";
            this.lblFiltroEvento.Size = new System.Drawing.Size(41, 13);
            this.lblFiltroEvento.TabIndex = 0;
            this.lblFiltroEvento.Text = "Evento";
            // 
            // dgvBitacora
            // 
            this.dgvBitacora.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvBitacora.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvBitacora.Location = new System.Drawing.Point(0, 72);
            this.dgvBitacora.Name = "dgvBitacora";
            this.dgvBitacora.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvBitacora.Size = new System.Drawing.Size(884, 298);
            this.dgvBitacora.TabIndex = 1;
            this.dgvBitacora.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvBitacora_CellClick);
            this.dgvBitacora.SelectionChanged += new System.EventHandler(this.dgvBitacora_SelectionChanged);
            // 
            // btnVerDetalle
            // 
            this.btnVerDetalle.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.btnVerDetalle.Location = new System.Drawing.Point(0, 370);
            this.btnVerDetalle.Name = "btnVerDetalle";
            this.btnVerDetalle.Size = new System.Drawing.Size(884, 28);
            this.btnVerDetalle.TabIndex = 2;
            this.btnVerDetalle.Text = "Ver detalle completo";
            this.btnVerDetalle.UseVisualStyleBackColor = true;
            this.btnVerDetalle.Visible = false;
            this.btnVerDetalle.Click += new System.EventHandler(this.btnVerDetalle_Click);
            // 
            // panelDetalle
            // 
            this.panelDetalle.Controls.Add(this.txtDetalleCompleto);
            this.panelDetalle.Controls.Add(this.lblDetalleTitulo);
            this.panelDetalle.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelDetalle.Location = new System.Drawing.Point(0, 398);
            this.panelDetalle.Name = "panelDetalle";
            this.panelDetalle.Size = new System.Drawing.Size(884, 0);
            this.panelDetalle.TabIndex = 3;
            this.panelDetalle.Visible = false;
            // 
            // txtDetalleCompleto
            // 
            this.txtDetalleCompleto.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtDetalleCompleto.Font = new System.Drawing.Font("Consolas", 9F);
            this.txtDetalleCompleto.Location = new System.Drawing.Point(0, 22);
            this.txtDetalleCompleto.Multiline = true;
            this.txtDetalleCompleto.Name = "txtDetalleCompleto";
            this.txtDetalleCompleto.ReadOnly = true;
            this.txtDetalleCompleto.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtDetalleCompleto.Size = new System.Drawing.Size(884, 0);
            this.txtDetalleCompleto.TabIndex = 1;
            this.txtDetalleCompleto.WordWrap = true;
            // 
            // lblDetalleTitulo
            // 
            this.lblDetalleTitulo.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblDetalleTitulo.Location = new System.Drawing.Point(0, 0);
            this.lblDetalleTitulo.Name = "lblDetalleTitulo";
            this.lblDetalleTitulo.Padding = new System.Windows.Forms.Padding(6, 4, 0, 0);
            this.lblDetalleTitulo.Size = new System.Drawing.Size(884, 22);
            this.lblDetalleTitulo.TabIndex = 0;
            this.lblDetalleTitulo.Text = "Detalle del registro";
            // 
            // btnSalir
            // 
            this.btnSalir.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.btnSalir.Location = new System.Drawing.Point(0, 398);
            this.btnSalir.Name = "btnSalir";
            this.btnSalir.Size = new System.Drawing.Size(884, 40);
            this.btnSalir.TabIndex = 4;
            this.btnSalir.Text = "Salir";
            this.btnSalir.UseVisualStyleBackColor = true;
            this.btnSalir.Click += new System.EventHandler(this.btnSalir_Click);
            // 
            // frmBitacora
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(884, 438);
            this.Controls.Add(this.dgvBitacora);
            this.Controls.Add(this.btnVerDetalle);
            this.Controls.Add(this.panelDetalle);
            this.Controls.Add(this.btnSalir);
            this.Controls.Add(this.panelFiltros);
            this.MinimumSize = new System.Drawing.Size(700, 450);
            this.Name = "frmBitacora";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Bitácora";
            this.Load += new System.EventHandler(this.frmBitacora_Load);
            this.panelFiltros.ResumeLayout(false);
            this.panelFiltros.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvBitacora)).EndInit();
            this.panelDetalle.ResumeLayout(false);
            this.panelDetalle.PerformLayout();
            this.ResumeLayout(false);
        }

        private System.Windows.Forms.Panel panelFiltros;
        private System.Windows.Forms.Label lblFiltroEvento;
        private System.Windows.Forms.TextBox txtFiltroEvento;
        private System.Windows.Forms.Label lblFiltroUsuario;
        private System.Windows.Forms.TextBox txtFiltroUsuario;
        private System.Windows.Forms.Label lblFechaDesde;
        private System.Windows.Forms.DateTimePicker dtpFechaDesde;
        private System.Windows.Forms.Label lblFechaHasta;
        private System.Windows.Forms.DateTimePicker dtpFechaHasta;
        private System.Windows.Forms.Button btnFiltrar;
        private System.Windows.Forms.Button btnLimpiarFiltros;
        private System.Windows.Forms.DataGridView dgvBitacora;
        private System.Windows.Forms.Button btnVerDetalle;
        private System.Windows.Forms.Panel panelDetalle;
        private System.Windows.Forms.Label lblDetalleTitulo;
        private System.Windows.Forms.TextBox txtDetalleCompleto;
        private System.Windows.Forms.Button btnSalir;
    }
}
