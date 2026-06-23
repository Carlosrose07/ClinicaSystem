using System;
using System.Drawing;
using System.Windows.Forms;

namespace ClinicaSystem.Forms
{
    public partial class FrmLogin : Form
    {
        public FrmLogin()
        {
            InitializeComponent();
        }

        private void FrmLogin_Load(object sender, EventArgs e)
        {
            // ===================================================================
            // TEMA OSCURO: igual que el resto de los formularios del sistema
            // Colores: fondo #1E3A5F, controles #16284B, texto blanco, botón azul
            // ===================================================================

            // Formulario
            this.Text = "ClinicaSystem — Iniciar Sesión";
            this.BackColor = ColorTranslator.FromHtml("#1E3A5F");
            this.ForeColor = Color.White;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Font = new Font("Segoe UI", 10f);

            // Labels
            label1.ForeColor = Color.White;
            label1.BackColor = Color.Transparent;
            label2.ForeColor = Color.White;
            label2.BackColor = Color.Transparent;

            // TextBoxes
            txtUsuario.BackColor = ColorTranslator.FromHtml("#16284B");
            txtUsuario.ForeColor = Color.White;
            txtUsuario.BorderStyle = BorderStyle.FixedSingle;

            txtClave.BackColor = ColorTranslator.FromHtml("#16284B");
            txtClave.ForeColor = Color.White;
            txtClave.BorderStyle = BorderStyle.FixedSingle;

            // Botón Ingresar — azul igual que los demás formularios
            btnIngresar.BackColor = ColorTranslator.FromHtml("#3498DB");
            btnIngresar.ForeColor = Color.White;
            btnIngresar.FlatStyle = FlatStyle.Flat;
            btnIngresar.FlatAppearance.BorderSize = 0;
            btnIngresar.Cursor = Cursors.Hand;
        }

        private void btnIngresar_Click(object sender, EventArgs e)
        {
            string usuario = txtUsuario.Text.Trim();
            string clave = txtClave.Text;

            if (string.IsNullOrEmpty(usuario) || string.IsNullOrEmpty(clave))
            {
                MessageBox.Show("Debes ingresar usuario y contraseña.", "Aviso",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var repo = new ClinicaSystem.Data.UsuarioRepository();
            var usuarioValido = repo.ValidarCredenciales(usuario, clave);

            if (usuarioValido == null)
            {
                MessageBox.Show("Usuario o contraseña incorrectos.", "Error de acceso",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var menu = new FrmMenuPrincipal(usuarioValido);
            menu.Show();
            this.Hide();
        }
    }
}