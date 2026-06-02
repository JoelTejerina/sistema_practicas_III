namespace CapaVistaUsuario.Ventas
{
    partial class frmMenu
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

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.dgvMenu = new System.Windows.Forms.DataGridView();
            this.lblNombre = new System.Windows.Forms.Label();
            this.txtNombre = new System.Windows.Forms.TextBox();
            this.lblDescripcion = new System.Windows.Forms.Label();
            this.txtDescripcion = new System.Windows.Forms.TextBox();
            this.lblIngredientes = new System.Windows.Forms.Label();
            this.txtIngredientes = new System.Windows.Forms.TextBox();
            this.lblPopularidad = new System.Windows.Forms.Label();
            this.txtPopularidad = new System.Windows.Forms.TextBox();
            this.lblCategoria = new System.Windows.Forms.Label();
            this.txtCategoria = new System.Windows.Forms.TextBox();
            this.lblRegion = new System.Windows.Forms.Label();
            this.txtRegion = new System.Windows.Forms.TextBox();
            this.lblTemporada = new System.Windows.Forms.Label();
            this.txtTemporada = new System.Windows.Forms.TextBox();
            this.lblTipoEvento = new System.Windows.Forms.Label();
            this.txtTipoEvento = new System.Windows.Forms.TextBox();
            this.lblPrecio = new System.Windows.Forms.Label();
            this.txtPrecio = new System.Windows.Forms.TextBox();
            this.lblTiempo = new System.Windows.Forms.Label();
            this.txtTiempo = new System.Windows.Forms.TextBox();
            this.lblStock = new System.Windows.Forms.Label();
            this.cmbStock = new System.Windows.Forms.ComboBox();
            this.lblID = new System.Windows.Forms.Label();
            this.pnlBotones = new System.Windows.Forms.Panel();
            this.btnAgregar = new System.Windows.Forms.Button();
            this.btnModificar = new System.Windows.Forms.Button();
            this.btnGuardar = new System.Windows.Forms.Button();
            this.btnGuardaCambios = new System.Windows.Forms.Button();
            this.btnEliminar = new System.Windows.Forms.Button();
            this.btnCancelar = new System.Windows.Forms.Button();
            this.btnSalir = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvMenu)).BeginInit();
            this.pnlBotones.SuspendLayout();
            this.SuspendLayout();
            // 
            // dgvMenu
            // 
            this.dgvMenu.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvMenu.Location = new System.Drawing.Point(12, 12);
            this.dgvMenu.Name = "dgvMenu";
            this.dgvMenu.Size = new System.Drawing.Size(760, 190);
            this.dgvMenu.TabIndex = 0;
            this.dgvMenu.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvMenu_CellClick);
            // 
            // lblNombre
            // 
            this.lblNombre.AutoSize = true;
            this.lblNombre.Location = new System.Drawing.Point(12, 218);
            this.lblNombre.Name = "lblNombre";
            this.lblNombre.Text = "Nombre";
            // 
            // txtNombre
            // 
            this.txtNombre.Location = new System.Drawing.Point(120, 215);
            this.txtNombre.Name = "txtNombre";
            this.txtNombre.Size = new System.Drawing.Size(160, 20);
            this.txtNombre.TabIndex = 1;
            // 
            // lblDescripcion
            // 
            this.lblDescripcion.AutoSize = true;
            this.lblDescripcion.Location = new System.Drawing.Point(12, 248);
            this.lblDescripcion.Name = "lblDescripcion";
            this.lblDescripcion.Text = "Descripción";
            // 
            // txtDescripcion
            // 
            this.txtDescripcion.Location = new System.Drawing.Point(120, 245);
            this.txtDescripcion.Name = "txtDescripcion";
            this.txtDescripcion.Size = new System.Drawing.Size(160, 20);
            this.txtDescripcion.TabIndex = 2;
            // 
            // lblIngredientes
            // 
            this.lblIngredientes.AutoSize = true;
            this.lblIngredientes.Location = new System.Drawing.Point(12, 278);
            this.lblIngredientes.Name = "lblIngredientes";
            this.lblIngredientes.Text = "Ingredientes";
            // 
            // txtIngredientes
            // 
            this.txtIngredientes.Location = new System.Drawing.Point(120, 275);
            this.txtIngredientes.Name = "txtIngredientes";
            this.txtIngredientes.Size = new System.Drawing.Size(160, 20);
            this.txtIngredientes.TabIndex = 3;
            // 
            // lblPopularidad
            // 
            this.lblPopularidad.AutoSize = true;
            this.lblPopularidad.Location = new System.Drawing.Point(12, 308);
            this.lblPopularidad.Name = "lblPopularidad";
            this.lblPopularidad.Text = "Popularidad";
            // 
            // txtPopularidad
            // 
            this.txtPopularidad.Location = new System.Drawing.Point(120, 305);
            this.txtPopularidad.Name = "txtPopularidad";
            this.txtPopularidad.Size = new System.Drawing.Size(160, 20);
            this.txtPopularidad.TabIndex = 4;
            // 
            // lblCategoria
            // 
            this.lblCategoria.AutoSize = true;
            this.lblCategoria.Location = new System.Drawing.Point(300, 218);
            this.lblCategoria.Name = "lblCategoria";
            this.lblCategoria.Text = "Categoría";
            // 
            // txtCategoria
            // 
            this.txtCategoria.Location = new System.Drawing.Point(390, 215);
            this.txtCategoria.Name = "txtCategoria";
            this.txtCategoria.Size = new System.Drawing.Size(160, 20);
            this.txtCategoria.TabIndex = 5;
            // 
            // lblRegion
            // 
            this.lblRegion.AutoSize = true;
            this.lblRegion.Location = new System.Drawing.Point(300, 248);
            this.lblRegion.Name = "lblRegion";
            this.lblRegion.Text = "Región";
            // 
            // txtRegion
            // 
            this.txtRegion.Location = new System.Drawing.Point(390, 245);
            this.txtRegion.Name = "txtRegion";
            this.txtRegion.Size = new System.Drawing.Size(160, 20);
            this.txtRegion.TabIndex = 6;
            // 
            // lblTemporada
            // 
            this.lblTemporada.AutoSize = true;
            this.lblTemporada.Location = new System.Drawing.Point(300, 278);
            this.lblTemporada.Name = "lblTemporada";
            this.lblTemporada.Text = "Temporada";
            // 
            // txtTemporada
            // 
            this.txtTemporada.Location = new System.Drawing.Point(390, 275);
            this.txtTemporada.Name = "txtTemporada";
            this.txtTemporada.Size = new System.Drawing.Size(160, 20);
            this.txtTemporada.TabIndex = 7;
            // 
            // lblTipoEvento
            // 
            this.lblTipoEvento.AutoSize = true;
            this.lblTipoEvento.Location = new System.Drawing.Point(300, 308);
            this.lblTipoEvento.Name = "lblTipoEvento";
            this.lblTipoEvento.Text = "Tipo de evento";
            // 
            // txtTipoEvento
            // 
            this.txtTipoEvento.Location = new System.Drawing.Point(390, 305);
            this.txtTipoEvento.Name = "txtTipoEvento";
            this.txtTipoEvento.Size = new System.Drawing.Size(160, 20);
            this.txtTipoEvento.TabIndex = 8;
            // 
            // lblPrecio
            // 
            this.lblPrecio.AutoSize = true;
            this.lblPrecio.Location = new System.Drawing.Point(570, 218);
            this.lblPrecio.Name = "lblPrecio";
            this.lblPrecio.Text = "Precio";
            // 
            // txtPrecio
            // 
            this.txtPrecio.Location = new System.Drawing.Point(650, 215);
            this.txtPrecio.Name = "txtPrecio";
            this.txtPrecio.Size = new System.Drawing.Size(120, 20);
            this.txtPrecio.TabIndex = 9;
            this.txtPrecio.Text = "0";
            // 
            // lblTiempo
            // 
            this.lblTiempo.AutoSize = true;
            this.lblTiempo.Location = new System.Drawing.Point(570, 248);
            this.lblTiempo.Name = "lblTiempo";
            this.lblTiempo.Text = "Tiempo (min)";
            // 
            // txtTiempo
            // 
            this.txtTiempo.Location = new System.Drawing.Point(650, 245);
            this.txtTiempo.Name = "txtTiempo";
            this.txtTiempo.Size = new System.Drawing.Size(120, 20);
            this.txtTiempo.TabIndex = 10;
            this.txtTiempo.Text = "0";
            // 
            // lblStock
            // 
            this.lblStock.AutoSize = true;
            this.lblStock.Location = new System.Drawing.Point(570, 278);
            this.lblStock.Name = "lblStock";
            this.lblStock.Text = "Stock (lote)";
            // 
            // cmbStock
            // 
            this.cmbStock.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbStock.FormattingEnabled = true;
            this.cmbStock.Location = new System.Drawing.Point(650, 275);
            this.cmbStock.Name = "cmbStock";
            this.cmbStock.Size = new System.Drawing.Size(120, 21);
            this.cmbStock.TabIndex = 11;
            // 
            // lblID
            // 
            this.lblID.AutoSize = true;
            this.lblID.Location = new System.Drawing.Point(650, 308);
            this.lblID.Name = "lblID";
            this.lblID.Text = "0";
            this.lblID.Visible = false;
            // 
            // pnlBotones
            // 
            this.pnlBotones.Controls.Add(this.btnAgregar);
            this.pnlBotones.Controls.Add(this.btnModificar);
            this.pnlBotones.Controls.Add(this.btnGuardar);
            this.pnlBotones.Controls.Add(this.btnGuardaCambios);
            this.pnlBotones.Controls.Add(this.btnEliminar);
            this.pnlBotones.Controls.Add(this.btnCancelar);
            this.pnlBotones.Controls.Add(this.btnSalir);
            this.pnlBotones.Location = new System.Drawing.Point(12, 350);
            this.pnlBotones.Name = "pnlBotones";
            this.pnlBotones.Size = new System.Drawing.Size(760, 60);
            this.pnlBotones.TabIndex = 12;
            // 
            // btnAgregar
            // 
            this.btnAgregar.Location = new System.Drawing.Point(10, 15);
            this.btnAgregar.Name = "btnAgregar";
            this.btnAgregar.Size = new System.Drawing.Size(90, 30);
            this.btnAgregar.TabIndex = 0;
            this.btnAgregar.Text = "Agregar";
            this.btnAgregar.UseVisualStyleBackColor = true;
            this.btnAgregar.Click += new System.EventHandler(this.btnAgregar_Click);
            // 
            // btnModificar
            // 
            this.btnModificar.Location = new System.Drawing.Point(110, 15);
            this.btnModificar.Name = "btnModificar";
            this.btnModificar.Size = new System.Drawing.Size(90, 30);
            this.btnModificar.TabIndex = 1;
            this.btnModificar.Text = "Modificar";
            this.btnModificar.UseVisualStyleBackColor = true;
            this.btnModificar.Click += new System.EventHandler(this.btnModificar_Click);
            // 
            // btnGuardar
            // 
            this.btnGuardar.Location = new System.Drawing.Point(210, 15);
            this.btnGuardar.Name = "btnGuardar";
            this.btnGuardar.Size = new System.Drawing.Size(90, 30);
            this.btnGuardar.TabIndex = 2;
            this.btnGuardar.Text = "Guardar";
            this.btnGuardar.UseVisualStyleBackColor = true;
            this.btnGuardar.Click += new System.EventHandler(this.btnGuardar_Click);
            // 
            // btnGuardaCambios
            // 
            this.btnGuardaCambios.Location = new System.Drawing.Point(310, 15);
            this.btnGuardaCambios.Name = "btnGuardaCambios";
            this.btnGuardaCambios.Size = new System.Drawing.Size(110, 30);
            this.btnGuardaCambios.TabIndex = 3;
            this.btnGuardaCambios.Text = "Guardar Cambios";
            this.btnGuardaCambios.UseVisualStyleBackColor = true;
            this.btnGuardaCambios.Click += new System.EventHandler(this.btnGuardaCambios_Click);
            // 
            // btnEliminar
            // 
            this.btnEliminar.Location = new System.Drawing.Point(440, 15);
            this.btnEliminar.Name = "btnEliminar";
            this.btnEliminar.Size = new System.Drawing.Size(90, 30);
            this.btnEliminar.TabIndex = 4;
            this.btnEliminar.Text = "Eliminar";
            this.btnEliminar.UseVisualStyleBackColor = true;
            this.btnEliminar.Click += new System.EventHandler(this.btnEliminar_Click);
            // 
            // btnCancelar
            // 
            this.btnCancelar.Location = new System.Drawing.Point(550, 15);
            this.btnCancelar.Name = "btnCancelar";
            this.btnCancelar.Size = new System.Drawing.Size(90, 30);
            this.btnCancelar.TabIndex = 5;
            this.btnCancelar.Text = "Cancelar";
            this.btnCancelar.UseVisualStyleBackColor = true;
            this.btnCancelar.Click += new System.EventHandler(this.btnCancelar_Click);
            // 
            // btnSalir
            // 
            this.btnSalir.Location = new System.Drawing.Point(660, 15);
            this.btnSalir.Name = "btnSalir";
            this.btnSalir.Size = new System.Drawing.Size(90, 30);
            this.btnSalir.TabIndex = 6;
            this.btnSalir.Text = "Salir";
            this.btnSalir.UseVisualStyleBackColor = true;
            this.btnSalir.Click += new System.EventHandler(this.btnSalir_Click);
            // 
            // frmMenu
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 421);
            this.Controls.Add(this.pnlBotones);
            this.Controls.Add(this.lblID);
            this.Controls.Add(this.cmbStock);
            this.Controls.Add(this.lblStock);
            this.Controls.Add(this.txtTiempo);
            this.Controls.Add(this.lblTiempo);
            this.Controls.Add(this.txtPrecio);
            this.Controls.Add(this.lblPrecio);
            this.Controls.Add(this.txtTipoEvento);
            this.Controls.Add(this.lblTipoEvento);
            this.Controls.Add(this.txtTemporada);
            this.Controls.Add(this.lblTemporada);
            this.Controls.Add(this.txtRegion);
            this.Controls.Add(this.lblRegion);
            this.Controls.Add(this.txtCategoria);
            this.Controls.Add(this.lblCategoria);
            this.Controls.Add(this.txtPopularidad);
            this.Controls.Add(this.lblPopularidad);
            this.Controls.Add(this.txtIngredientes);
            this.Controls.Add(this.lblIngredientes);
            this.Controls.Add(this.txtDescripcion);
            this.Controls.Add(this.lblDescripcion);
            this.Controls.Add(this.txtNombre);
            this.Controls.Add(this.lblNombre);
            this.Controls.Add(this.dgvMenu);
            this.Name = "frmMenu";
            this.Text = "Gestión de Menú";
            this.Load += new System.EventHandler(this.frmMenu_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvMenu)).EndInit();
            this.pnlBotones.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.DataGridView dgvMenu;
        private System.Windows.Forms.Label lblNombre;
        private System.Windows.Forms.TextBox txtNombre;
        private System.Windows.Forms.Label lblDescripcion;
        private System.Windows.Forms.TextBox txtDescripcion;
        private System.Windows.Forms.Label lblIngredientes;
        private System.Windows.Forms.TextBox txtIngredientes;
        private System.Windows.Forms.Label lblPopularidad;
        private System.Windows.Forms.TextBox txtPopularidad;
        private System.Windows.Forms.Label lblCategoria;
        private System.Windows.Forms.TextBox txtCategoria;
        private System.Windows.Forms.Label lblRegion;
        private System.Windows.Forms.TextBox txtRegion;
        private System.Windows.Forms.Label lblTemporada;
        private System.Windows.Forms.TextBox txtTemporada;
        private System.Windows.Forms.Label lblTipoEvento;
        private System.Windows.Forms.TextBox txtTipoEvento;
        private System.Windows.Forms.Label lblPrecio;
        private System.Windows.Forms.TextBox txtPrecio;
        private System.Windows.Forms.Label lblTiempo;
        private System.Windows.Forms.TextBox txtTiempo;
        private System.Windows.Forms.Label lblStock;
        private System.Windows.Forms.ComboBox cmbStock;
        private System.Windows.Forms.Label lblID;
        private System.Windows.Forms.Panel pnlBotones;
        private System.Windows.Forms.Button btnAgregar;
        private System.Windows.Forms.Button btnModificar;
        private System.Windows.Forms.Button btnGuardar;
        private System.Windows.Forms.Button btnGuardaCambios;
        private System.Windows.Forms.Button btnEliminar;
        private System.Windows.Forms.Button btnCancelar;
        private System.Windows.Forms.Button btnSalir;
    }
}
