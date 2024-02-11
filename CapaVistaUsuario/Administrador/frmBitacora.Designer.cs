namespace CapaVistaUsuario.Administrador
{
    partial class frmBitacora
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.panelBitacora = new System.Windows.Forms.Panel();
            this.btnFacturacion = new System.Windows.Forms.Button();
            this.btnMenu = new System.Windows.Forms.Button();
            this.btnPersonal = new System.Windows.Forms.Button();
            this.btnProducto = new System.Windows.Forms.Button();
            this.btnStock = new System.Windows.Forms.Button();
            this.btnLogin = new System.Windows.Forms.Button();
            this.btnProveedores = new System.Windows.Forms.Button();
            this.dgvBitacora = new System.Windows.Forms.DataGridView();
            this.panel1 = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.dgvBitacora)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelBitacora
            // 
            this.panelBitacora.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(182)))), ((int)(((byte)(231)))), ((int)(((byte)(253)))));
            this.panelBitacora.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelBitacora.Location = new System.Drawing.Point(0, 0);
            this.panelBitacora.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.panelBitacora.Name = "panelBitacora";
            this.panelBitacora.Size = new System.Drawing.Size(1547, 63);
            this.panelBitacora.TabIndex = 9;
            // 
            // btnFacturacion
            // 
            this.btnFacturacion.Location = new System.Drawing.Point(12, 71);
            this.btnFacturacion.Name = "btnFacturacion";
            this.btnFacturacion.Size = new System.Drawing.Size(250, 67);
            this.btnFacturacion.TabIndex = 10;
            this.btnFacturacion.Text = "Facturacion";
            this.btnFacturacion.UseVisualStyleBackColor = true;
            this.btnFacturacion.Click += new System.EventHandler(this.btnFacturacion_Click);
            // 
            // btnMenu
            // 
            this.btnMenu.Location = new System.Drawing.Point(12, 96);
            this.btnMenu.Name = "btnMenu";
            this.btnMenu.Size = new System.Drawing.Size(250, 67);
            this.btnMenu.TabIndex = 11;
            this.btnMenu.Text = "Menu";
            this.btnMenu.UseVisualStyleBackColor = true;
            this.btnMenu.Click += new System.EventHandler(this.btnMenu_Click);
            // 
            // btnPersonal
            // 
            this.btnPersonal.Location = new System.Drawing.Point(12, 178);
            this.btnPersonal.Name = "btnPersonal";
            this.btnPersonal.Size = new System.Drawing.Size(250, 67);
            this.btnPersonal.TabIndex = 12;
            this.btnPersonal.Text = "Personal";
            this.btnPersonal.UseVisualStyleBackColor = true;
            this.btnPersonal.Click += new System.EventHandler(this.btnPersonal_Click);
            // 
            // btnProducto
            // 
            this.btnProducto.Location = new System.Drawing.Point(12, 265);
            this.btnProducto.Name = "btnProducto";
            this.btnProducto.Size = new System.Drawing.Size(250, 67);
            this.btnProducto.TabIndex = 13;
            this.btnProducto.Text = "Producto";
            this.btnProducto.UseVisualStyleBackColor = true;
            this.btnProducto.Click += new System.EventHandler(this.btnProducto_Click);
            // 
            // btnStock
            // 
            this.btnStock.Location = new System.Drawing.Point(12, 441);
            this.btnStock.Name = "btnStock";
            this.btnStock.Size = new System.Drawing.Size(250, 67);
            this.btnStock.TabIndex = 14;
            this.btnStock.Text = "Stock";
            this.btnStock.UseVisualStyleBackColor = true;
            this.btnStock.Click += new System.EventHandler(this.btnStock_Click);
            // 
            // btnLogin
            // 
            this.btnLogin.Location = new System.Drawing.Point(12, 526);
            this.btnLogin.Name = "btnLogin";
            this.btnLogin.Size = new System.Drawing.Size(250, 67);
            this.btnLogin.TabIndex = 15;
            this.btnLogin.Text = "Login";
            this.btnLogin.UseVisualStyleBackColor = true;
            this.btnLogin.Click += new System.EventHandler(this.btnLogin_Click);
            // 
            // btnProveedores
            // 
            this.btnProveedores.Location = new System.Drawing.Point(12, 354);
            this.btnProveedores.Name = "btnProveedores";
            this.btnProveedores.Size = new System.Drawing.Size(250, 67);
            this.btnProveedores.TabIndex = 16;
            this.btnProveedores.Text = "Proveedores";
            this.btnProveedores.UseVisualStyleBackColor = true;
            this.btnProveedores.Click += new System.EventHandler(this.btnProveedores_Click);
            // 
            // dgvBitacora
            // 
            this.dgvBitacora.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvBitacora.Location = new System.Drawing.Point(278, 62);
            this.dgvBitacora.Name = "dgvBitacora";
            this.dgvBitacora.RowHeadersWidth = 62;
            this.dgvBitacora.RowTemplate.Height = 28;
            this.dgvBitacora.Size = new System.Drawing.Size(1269, 607);
            this.dgvBitacora.TabIndex = 17;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(182)))), ((int)(((byte)(231)))), ((int)(((byte)(253)))));
            this.panel1.Controls.Add(this.btnLogin);
            this.panel1.Controls.Add(this.btnMenu);
            this.panel1.Controls.Add(this.btnPersonal);
            this.panel1.Controls.Add(this.btnProducto);
            this.panel1.Controls.Add(this.btnProveedores);
            this.panel1.Controls.Add(this.btnStock);
            this.panel1.Location = new System.Drawing.Point(0, 62);
            this.panel1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(279, 607);
            this.panel1.TabIndex = 18;
            // 
            // frmBitacora
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1547, 667);
            this.Controls.Add(this.dgvBitacora);
            this.Controls.Add(this.btnFacturacion);
            this.Controls.Add(this.panelBitacora);
            this.Controls.Add(this.panel1);
            this.Name = "frmBitacora";
            this.Text = "Bitacora";
            this.Load += new System.EventHandler(this.frmBitacora_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvBitacora)).EndInit();
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelBitacora;
        private System.Windows.Forms.Button btnFacturacion;
        private System.Windows.Forms.Button btnMenu;
        private System.Windows.Forms.Button btnPersonal;
        private System.Windows.Forms.Button btnProducto;
        private System.Windows.Forms.Button btnStock;
        private System.Windows.Forms.Button btnLogin;
        private System.Windows.Forms.Button btnProveedores;
        private System.Windows.Forms.DataGridView dgvBitacora;
        private System.Windows.Forms.Panel panel1;
    }
}