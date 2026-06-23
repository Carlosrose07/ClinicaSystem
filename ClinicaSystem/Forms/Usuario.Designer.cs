namespace ClinicaSystem.Forms
{
    partial class FrmUsuarios
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

        private System.Windows.Forms.Panel panelTitulo;
        private System.Windows.Forms.Label lblTitulo;
        private System.Windows.Forms.Label lblBuscar;
        private System.Windows.Forms.TextBox txtBuscar;
        private System.Windows.Forms.DataGridView dgvUsuarios;
        private System.Windows.Forms.Panel panelEdicion;
        private System.Windows.Forms.Label lblNombreUsuario;
        private System.Windows.Forms.TextBox txtNombreUsuario;
        private System.Windows.Forms.Label lblRol;
        private System.Windows.Forms.ComboBox cmbRol;
        private System.Windows.Forms.CheckBox chkActivo;
        private System.Windows.Forms.CheckBox chkCambiarClave;
        private System.Windows.Forms.Label lblClave;
        private System.Windows.Forms.TextBox txtClave;
        private System.Windows.Forms.Label lblConfirmarClave;
        private System.Windows.Forms.TextBox txtConfirmarClave;
        private System.Windows.Forms.Button btnNuevo;
        private System.Windows.Forms.Button btnGuardar;
        private System.Windows.Forms.Button btnEliminar;
        private System.Windows.Forms.Button btnCancelar;

        private void InitializeComponent()
        {
            this.panelTitulo = new System.Windows.Forms.Panel();
            this.lblTitulo = new System.Windows.Forms.Label();
            this.lblBuscar = new System.Windows.Forms.Label();
            this.txtBuscar = new System.Windows.Forms.TextBox();
            this.dgvUsuarios = new System.Windows.Forms.DataGridView();
            this.panelEdicion = new System.Windows.Forms.Panel();
            this.lblNombreUsuario = new System.Windows.Forms.Label();
            this.txtNombreUsuario = new System.Windows.Forms.TextBox();
            this.lblRol = new System.Windows.Forms.Label();
            this.cmbRol = new System.Windows.Forms.ComboBox();
            this.chkActivo = new System.Windows.Forms.CheckBox();
            this.chkCambiarClave = new System.Windows.Forms.CheckBox();
            this.lblClave = new System.Windows.Forms.Label();
            this.txtClave = new System.Windows.Forms.TextBox();
            this.lblConfirmarClave = new System.Windows.Forms.Label();
            this.txtConfirmarClave = new System.Windows.Forms.TextBox();
            this.btnNuevo = new System.Windows.Forms.Button();
            this.btnGuardar = new System.Windows.Forms.Button();
            this.btnEliminar = new System.Windows.Forms.Button();
            this.btnCancelar = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvUsuarios)).BeginInit();
            this.panelTitulo.SuspendLayout();
            this.panelEdicion.SuspendLayout();
            this.SuspendLayout();

            // panelTitulo
            this.panelTitulo.BackColor = System.Drawing.ColorTranslator.FromHtml("#16284B");
            this.panelTitulo.Controls.Add(this.lblTitulo);
            this.panelTitulo.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelTitulo.Location = new System.Drawing.Point(0, 0);
            this.panelTitulo.Name = "panelTitulo";
            this.panelTitulo.Size = new System.Drawing.Size(980, 50);
            this.panelTitulo.TabIndex = 0;

            // lblTitulo
            this.lblTitulo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblTitulo.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold);
            this.lblTitulo.ForeColor = System.Drawing.Color.White;
            this.lblTitulo.Name = "lblTitulo";
            this.lblTitulo.Text = "Gestión de Usuarios";
            this.lblTitulo.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;

            // lblBuscar
            this.lblBuscar.AutoSize = true;
            this.lblBuscar.ForeColor = System.Drawing.Color.White;
            this.lblBuscar.Location = new System.Drawing.Point(20, 65);
            this.lblBuscar.Name = "lblBuscar";
            this.lblBuscar.Size = new System.Drawing.Size(48, 15);
            this.lblBuscar.TabIndex = 1;
            this.lblBuscar.Text = "Buscar:";

            // txtBuscar
            this.txtBuscar.Location = new System.Drawing.Point(85, 62);
            this.txtBuscar.Name = "txtBuscar";
            this.txtBuscar.Size = new System.Drawing.Size(300, 23);
            this.txtBuscar.TabIndex = 2;
            this.txtBuscar.TextChanged += new System.EventHandler(this.txtBuscar_TextChanged);

            // dgvUsuarios
            this.dgvUsuarios.AllowUserToAddRows = false;
            this.dgvUsuarios.AllowUserToDeleteRows = false;
            this.dgvUsuarios.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvUsuarios.AutoGenerateColumns = false;
            this.dgvUsuarios.BackgroundColor = System.Drawing.Color.White;
            this.dgvUsuarios.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvUsuarios.Location = new System.Drawing.Point(20, 100);
            this.dgvUsuarios.MultiSelect = false;
            this.dgvUsuarios.Name = "dgvUsuarios";
            this.dgvUsuarios.ReadOnly = true;
            this.dgvUsuarios.RowHeadersVisible = false;
            this.dgvUsuarios.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvUsuarios.Size = new System.Drawing.Size(580, 480);
            this.dgvUsuarios.TabIndex = 3;
            this.dgvUsuarios.SelectionChanged += new System.EventHandler(this.dgvUsuarios_SelectionChanged);

            // panelEdicion
            this.panelEdicion.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Right))));
            this.panelEdicion.BackColor = System.Drawing.ColorTranslator.FromHtml("#16284B");
            this.panelEdicion.Controls.Add(this.lblNombreUsuario);
            this.panelEdicion.Controls.Add(this.txtNombreUsuario);
            this.panelEdicion.Controls.Add(this.lblRol);
            this.panelEdicion.Controls.Add(this.cmbRol);
            this.panelEdicion.Controls.Add(this.chkActivo);
            this.panelEdicion.Controls.Add(this.chkCambiarClave);
            this.panelEdicion.Controls.Add(this.lblClave);
            this.panelEdicion.Controls.Add(this.txtClave);
            this.panelEdicion.Controls.Add(this.lblConfirmarClave);
            this.panelEdicion.Controls.Add(this.txtConfirmarClave);
            this.panelEdicion.Controls.Add(this.btnNuevo);
            this.panelEdicion.Controls.Add(this.btnGuardar);
            this.panelEdicion.Controls.Add(this.btnEliminar);
            this.panelEdicion.Controls.Add(this.btnCancelar);
            this.panelEdicion.Location = new System.Drawing.Point(620, 100);
            this.panelEdicion.Name = "panelEdicion";
            this.panelEdicion.Size = new System.Drawing.Size(340, 480);
            this.panelEdicion.TabIndex = 4;

            // lblNombreUsuario
            this.lblNombreUsuario.AutoSize = true;
            this.lblNombreUsuario.ForeColor = System.Drawing.Color.White;
            this.lblNombreUsuario.Location = new System.Drawing.Point(10, 10);
            this.lblNombreUsuario.Name = "lblNombreUsuario";
            this.lblNombreUsuario.Text = "Nombre de usuario:";

            // txtNombreUsuario
            this.txtNombreUsuario.Location = new System.Drawing.Point(10, 28);
            this.txtNombreUsuario.Name = "txtNombreUsuario";
            this.txtNombreUsuario.Size = new System.Drawing.Size(300, 23);
            this.txtNombreUsuario.TabIndex = 0;

            // lblRol
            this.lblRol.AutoSize = true;
            this.lblRol.ForeColor = System.Drawing.Color.White;
            this.lblRol.Location = new System.Drawing.Point(10, 62);
            this.lblRol.Name = "lblRol";
            this.lblRol.Text = "Rol:";

            // cmbRol
            this.cmbRol.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbRol.Location = new System.Drawing.Point(10, 80);
            this.cmbRol.Name = "cmbRol";
            this.cmbRol.Size = new System.Drawing.Size(200, 23);
            this.cmbRol.TabIndex = 1;

            // chkActivo
            this.chkActivo.AutoSize = true;
            this.chkActivo.Checked = true;
            this.chkActivo.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkActivo.ForeColor = System.Drawing.Color.White;
            this.chkActivo.Location = new System.Drawing.Point(10, 115);
            this.chkActivo.Name = "chkActivo";
            this.chkActivo.Size = new System.Drawing.Size(95, 19);
            this.chkActivo.TabIndex = 2;
            this.chkActivo.Text = "Usuario activo";

            // chkCambiarClave
            // (Solo se hace Visible=true en código cuando se está EDITANDO un usuario existente)
            this.chkCambiarClave.AutoSize = true;
            this.chkCambiarClave.ForeColor = System.Drawing.Color.White;
            this.chkCambiarClave.Location = new System.Drawing.Point(10, 145);
            this.chkCambiarClave.Name = "chkCambiarClave";
            this.chkCambiarClave.Size = new System.Drawing.Size(130, 19);
            this.chkCambiarClave.TabIndex = 3;
            this.chkCambiarClave.Text = "Cambiar contraseña";
            this.chkCambiarClave.Visible = false;
            this.chkCambiarClave.CheckedChanged += new System.EventHandler(this.chkCambiarClave_CheckedChanged);

            // lblClave
            this.lblClave.AutoSize = true;
            this.lblClave.ForeColor = System.Drawing.Color.White;
            this.lblClave.Location = new System.Drawing.Point(10, 175);
            this.lblClave.Name = "lblClave";
            this.lblClave.Text = "Contraseña:";

            // txtClave
            this.txtClave.Location = new System.Drawing.Point(10, 193);
            this.txtClave.Name = "txtClave";
            this.txtClave.PasswordChar = '*';
            this.txtClave.Size = new System.Drawing.Size(300, 23);
            this.txtClave.TabIndex = 4;

            // lblConfirmarClave
            this.lblConfirmarClave.AutoSize = true;
            this.lblConfirmarClave.ForeColor = System.Drawing.Color.White;
            this.lblConfirmarClave.Location = new System.Drawing.Point(10, 225);
            this.lblConfirmarClave.Name = "lblConfirmarClave";
            this.lblConfirmarClave.Text = "Confirmar contraseña:";

            // txtConfirmarClave
            this.txtConfirmarClave.Location = new System.Drawing.Point(10, 243);
            this.txtConfirmarClave.Name = "txtConfirmarClave";
            this.txtConfirmarClave.PasswordChar = '*';
            this.txtConfirmarClave.Size = new System.Drawing.Size(300, 23);
            this.txtConfirmarClave.TabIndex = 5;

            // btnNuevo
            this.btnNuevo.BackColor = System.Drawing.ColorTranslator.FromHtml("#3498DB");
            this.btnNuevo.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnNuevo.ForeColor = System.Drawing.Color.White;
            this.btnNuevo.Location = new System.Drawing.Point(10, 320);
            this.btnNuevo.Name = "btnNuevo";
            this.btnNuevo.Size = new System.Drawing.Size(150, 35);
            this.btnNuevo.TabIndex = 6;
            this.btnNuevo.Text = "Nuevo";
            this.btnNuevo.Click += new System.EventHandler(this.btnNuevo_Click);

            // btnGuardar
            this.btnGuardar.BackColor = System.Drawing.ColorTranslator.FromHtml("#27AE60");
            this.btnGuardar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnGuardar.ForeColor = System.Drawing.Color.White;
            this.btnGuardar.Location = new System.Drawing.Point(170, 320);
            this.btnGuardar.Name = "btnGuardar";
            this.btnGuardar.Size = new System.Drawing.Size(150, 35);
            this.btnGuardar.TabIndex = 7;
            this.btnGuardar.Text = "Guardar";
            this.btnGuardar.Click += new System.EventHandler(this.btnGuardar_Click);

            // btnEliminar
            this.btnEliminar.BackColor = System.Drawing.ColorTranslator.FromHtml("#E74C3C");
            this.btnEliminar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnEliminar.ForeColor = System.Drawing.Color.White;
            this.btnEliminar.Location = new System.Drawing.Point(10, 365);
            this.btnEliminar.Name = "btnEliminar";
            this.btnEliminar.Size = new System.Drawing.Size(150, 35);
            this.btnEliminar.TabIndex = 8;
            this.btnEliminar.Text = "Eliminar";
            this.btnEliminar.Click += new System.EventHandler(this.btnEliminar_Click);

            // btnCancelar
            this.btnCancelar.BackColor = System.Drawing.ColorTranslator.FromHtml("#7F8C8D");
            this.btnCancelar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancelar.ForeColor = System.Drawing.Color.White;
            this.btnCancelar.Location = new System.Drawing.Point(170, 365);
            this.btnCancelar.Name = "btnCancelar";
            this.btnCancelar.Size = new System.Drawing.Size(150, 35);
            this.btnCancelar.TabIndex = 9;
            this.btnCancelar.Text = "Cancelar";
            this.btnCancelar.Click += new System.EventHandler(this.btnCancelar_Click);

            // FrmUsuarios
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.ColorTranslator.FromHtml("#1E3A5F");
            this.ClientSize = new System.Drawing.Size(980, 600);
            this.Controls.Add(this.panelEdicion);
            this.Controls.Add(this.dgvUsuarios);
            this.Controls.Add(this.lblBuscar);
            this.Controls.Add(this.txtBuscar);
            this.Controls.Add(this.panelTitulo);
            this.MinimumSize = new System.Drawing.Size(900, 600);
            this.Name = "FrmUsuarios";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Gestión de Usuarios";
            this.Load += new System.EventHandler(this.FrmUsuarios_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvUsuarios)).EndInit();
            this.panelTitulo.ResumeLayout(false);
            this.panelEdicion.ResumeLayout(false);
            this.panelEdicion.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}