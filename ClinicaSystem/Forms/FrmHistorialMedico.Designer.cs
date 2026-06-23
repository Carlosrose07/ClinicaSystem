namespace ClinicaSystem.Forms
{
    partial class FrmHistorialMedico
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
        private System.Windows.Forms.DataGridView dgvHistorial;
        private System.Windows.Forms.Panel panelEdicion;
        private System.Windows.Forms.Label lblPaciente;
        private System.Windows.Forms.ComboBox cmbPaciente;
        private System.Windows.Forms.Label lblMedico;
        private System.Windows.Forms.ComboBox cmbMedico;
        private System.Windows.Forms.Label lblCita;
        private System.Windows.Forms.ComboBox cmbCita;
        private System.Windows.Forms.Label lblSintomas;
        private System.Windows.Forms.TextBox txtSintomas;
        private System.Windows.Forms.Label lblDiagnostico;
        private System.Windows.Forms.TextBox txtDiagnostico;
        private System.Windows.Forms.Label lblTratamiento;
        private System.Windows.Forms.TextBox txtTratamiento;
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
            this.dgvHistorial = new System.Windows.Forms.DataGridView();
            this.panelEdicion = new System.Windows.Forms.Panel();
            this.lblPaciente = new System.Windows.Forms.Label();
            this.cmbPaciente = new System.Windows.Forms.ComboBox();
            this.lblMedico = new System.Windows.Forms.Label();
            this.cmbMedico = new System.Windows.Forms.ComboBox();
            this.lblCita = new System.Windows.Forms.Label();
            this.cmbCita = new System.Windows.Forms.ComboBox();
            this.lblSintomas = new System.Windows.Forms.Label();
            this.txtSintomas = new System.Windows.Forms.TextBox();
            this.lblDiagnostico = new System.Windows.Forms.Label();
            this.txtDiagnostico = new System.Windows.Forms.TextBox();
            this.lblTratamiento = new System.Windows.Forms.Label();
            this.txtTratamiento = new System.Windows.Forms.TextBox();
            this.btnNuevo = new System.Windows.Forms.Button();
            this.btnGuardar = new System.Windows.Forms.Button();
            this.btnEliminar = new System.Windows.Forms.Button();
            this.btnCancelar = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvHistorial)).BeginInit();
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
            this.lblTitulo.Text = "Historial Médico";
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

            // dgvHistorial
            this.dgvHistorial.AllowUserToAddRows = false;
            this.dgvHistorial.AllowUserToDeleteRows = false;
            this.dgvHistorial.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvHistorial.AutoGenerateColumns = false;
            this.dgvHistorial.BackgroundColor = System.Drawing.Color.White;
            this.dgvHistorial.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvHistorial.Location = new System.Drawing.Point(20, 100);
            this.dgvHistorial.MultiSelect = false;
            this.dgvHistorial.Name = "dgvHistorial";
            this.dgvHistorial.ReadOnly = true;
            this.dgvHistorial.RowHeadersVisible = false;
            this.dgvHistorial.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvHistorial.Size = new System.Drawing.Size(580, 480);
            this.dgvHistorial.TabIndex = 3;
            this.dgvHistorial.SelectionChanged += new System.EventHandler(this.dgvHistorial_SelectionChanged);

            // panelEdicion
            this.panelEdicion.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Right))));
            this.panelEdicion.BackColor = System.Drawing.ColorTranslator.FromHtml("#16284B");
            this.panelEdicion.Controls.Add(this.lblPaciente);
            this.panelEdicion.Controls.Add(this.cmbPaciente);
            this.panelEdicion.Controls.Add(this.lblMedico);
            this.panelEdicion.Controls.Add(this.cmbMedico);
            this.panelEdicion.Controls.Add(this.lblCita);
            this.panelEdicion.Controls.Add(this.cmbCita);
            this.panelEdicion.Controls.Add(this.lblSintomas);
            this.panelEdicion.Controls.Add(this.txtSintomas);
            this.panelEdicion.Controls.Add(this.lblDiagnostico);
            this.panelEdicion.Controls.Add(this.txtDiagnostico);
            this.panelEdicion.Controls.Add(this.lblTratamiento);
            this.panelEdicion.Controls.Add(this.txtTratamiento);
            this.panelEdicion.Controls.Add(this.btnNuevo);
            this.panelEdicion.Controls.Add(this.btnGuardar);
            this.panelEdicion.Controls.Add(this.btnEliminar);
            this.panelEdicion.Controls.Add(this.btnCancelar);
            this.panelEdicion.Location = new System.Drawing.Point(620, 100);
            this.panelEdicion.Name = "panelEdicion";
            this.panelEdicion.Size = new System.Drawing.Size(340, 480);
            this.panelEdicion.TabIndex = 4;

            // lblPaciente
            this.lblPaciente.AutoSize = true;
            this.lblPaciente.ForeColor = System.Drawing.Color.White;
            this.lblPaciente.Location = new System.Drawing.Point(10, 5);
            this.lblPaciente.Name = "lblPaciente";
            this.lblPaciente.Text = "Paciente:";

            // cmbPaciente
            this.cmbPaciente.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbPaciente.Location = new System.Drawing.Point(10, 23);
            this.cmbPaciente.Name = "cmbPaciente";
            this.cmbPaciente.Size = new System.Drawing.Size(300, 23);
            this.cmbPaciente.TabIndex = 0;
            // Al cambiar el paciente, se debe refiltrar la lista de citas disponibles (ver cmbPaciente_SelectedIndexChanged)
            this.cmbPaciente.SelectedIndexChanged += new System.EventHandler(this.cmbPaciente_SelectedIndexChanged);

            // lblMedico
            this.lblMedico.AutoSize = true;
            this.lblMedico.ForeColor = System.Drawing.Color.White;
            this.lblMedico.Location = new System.Drawing.Point(10, 55);
            this.lblMedico.Name = "lblMedico";
            this.lblMedico.Text = "Médico:";

            // cmbMedico
            this.cmbMedico.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbMedico.Location = new System.Drawing.Point(10, 73);
            this.cmbMedico.Name = "cmbMedico";
            this.cmbMedico.Size = new System.Drawing.Size(300, 23);
            this.cmbMedico.TabIndex = 1;

            // lblCita
            this.lblCita.AutoSize = true;
            this.lblCita.ForeColor = System.Drawing.Color.White;
            this.lblCita.Location = new System.Drawing.Point(10, 105);
            this.lblCita.Name = "lblCita";
            this.lblCita.Text = "Cita asociada (opcional):";

            // cmbCita
            this.cmbCita.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbCita.Location = new System.Drawing.Point(10, 123);
            this.cmbCita.Name = "cmbCita";
            this.cmbCita.Size = new System.Drawing.Size(300, 23);
            this.cmbCita.TabIndex = 2;

            // lblSintomas
            this.lblSintomas.AutoSize = true;
            this.lblSintomas.ForeColor = System.Drawing.Color.White;
            this.lblSintomas.Location = new System.Drawing.Point(10, 155);
            this.lblSintomas.Name = "lblSintomas";
            this.lblSintomas.Text = "Síntomas:";

            // txtSintomas
            this.txtSintomas.Location = new System.Drawing.Point(10, 173);
            this.txtSintomas.Multiline = true;
            this.txtSintomas.Name = "txtSintomas";
            this.txtSintomas.Size = new System.Drawing.Size(300, 50);
            this.txtSintomas.TabIndex = 3;

            // lblDiagnostico
            this.lblDiagnostico.AutoSize = true;
            this.lblDiagnostico.ForeColor = System.Drawing.Color.White;
            this.lblDiagnostico.Location = new System.Drawing.Point(10, 230);
            this.lblDiagnostico.Name = "lblDiagnostico";
            this.lblDiagnostico.Text = "Diagnóstico:";

            // txtDiagnostico
            this.txtDiagnostico.Location = new System.Drawing.Point(10, 248);
            this.txtDiagnostico.Multiline = true;
            this.txtDiagnostico.Name = "txtDiagnostico";
            this.txtDiagnostico.Size = new System.Drawing.Size(300, 50);
            this.txtDiagnostico.TabIndex = 4;

            // lblTratamiento
            this.lblTratamiento.AutoSize = true;
            this.lblTratamiento.ForeColor = System.Drawing.Color.White;
            this.lblTratamiento.Location = new System.Drawing.Point(10, 305);
            this.lblTratamiento.Name = "lblTratamiento";
            this.lblTratamiento.Text = "Tratamiento:";

            // txtTratamiento
            this.txtTratamiento.Location = new System.Drawing.Point(10, 323);
            this.txtTratamiento.Multiline = true;
            this.txtTratamiento.Name = "txtTratamiento";
            this.txtTratamiento.Size = new System.Drawing.Size(300, 50);
            this.txtTratamiento.TabIndex = 5;

            // btnNuevo
            this.btnNuevo.BackColor = System.Drawing.ColorTranslator.FromHtml("#3498DB");
            this.btnNuevo.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnNuevo.ForeColor = System.Drawing.Color.White;
            this.btnNuevo.Location = new System.Drawing.Point(10, 385);
            this.btnNuevo.Name = "btnNuevo";
            this.btnNuevo.Size = new System.Drawing.Size(150, 35);
            this.btnNuevo.TabIndex = 6;
            this.btnNuevo.Text = "Nuevo";
            this.btnNuevo.Click += new System.EventHandler(this.btnNuevo_Click);

            // btnGuardar
            this.btnGuardar.BackColor = System.Drawing.ColorTranslator.FromHtml("#27AE60");
            this.btnGuardar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnGuardar.ForeColor = System.Drawing.Color.White;
            this.btnGuardar.Location = new System.Drawing.Point(170, 385);
            this.btnGuardar.Name = "btnGuardar";
            this.btnGuardar.Size = new System.Drawing.Size(150, 35);
            this.btnGuardar.TabIndex = 7;
            this.btnGuardar.Text = "Guardar";
            this.btnGuardar.Click += new System.EventHandler(this.btnGuardar_Click);

            // btnEliminar
            this.btnEliminar.BackColor = System.Drawing.ColorTranslator.FromHtml("#E74C3C");
            this.btnEliminar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnEliminar.ForeColor = System.Drawing.Color.White;
            this.btnEliminar.Location = new System.Drawing.Point(10, 430);
            this.btnEliminar.Name = "btnEliminar";
            this.btnEliminar.Size = new System.Drawing.Size(150, 35);
            this.btnEliminar.TabIndex = 8;
            this.btnEliminar.Text = "Eliminar";
            this.btnEliminar.Click += new System.EventHandler(this.btnEliminar_Click);

            // btnCancelar
            this.btnCancelar.BackColor = System.Drawing.ColorTranslator.FromHtml("#7F8C8D");
            this.btnCancelar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancelar.ForeColor = System.Drawing.Color.White;
            this.btnCancelar.Location = new System.Drawing.Point(170, 430);
            this.btnCancelar.Name = "btnCancelar";
            this.btnCancelar.Size = new System.Drawing.Size(150, 35);
            this.btnCancelar.TabIndex = 9;
            this.btnCancelar.Text = "Cancelar";
            this.btnCancelar.Click += new System.EventHandler(this.btnCancelar_Click);

            // FrmHistorialMedico
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.ColorTranslator.FromHtml("#1E3A5F");
            this.ClientSize = new System.Drawing.Size(980, 600);
            this.Controls.Add(this.panelEdicion);
            this.Controls.Add(this.dgvHistorial);
            this.Controls.Add(this.lblBuscar);
            this.Controls.Add(this.txtBuscar);
            this.Controls.Add(this.panelTitulo);
            this.MinimumSize = new System.Drawing.Size(900, 600);
            this.Name = "FrmHistorialMedico";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Historial Médico";
            this.Load += new System.EventHandler(this.FrmHistorialMedico_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvHistorial)).EndInit();
            this.panelTitulo.ResumeLayout(false);
            this.panelEdicion.ResumeLayout(false);
            this.panelEdicion.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}