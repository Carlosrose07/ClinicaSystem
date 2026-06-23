using System;
using System.Windows.Forms;
using ClinicaSystem.Models;

namespace ClinicaSystem.Forms
{
    public partial class FrmMenuPrincipal : Form
    {
        private Usuario _usuarioActual;

        public FrmMenuPrincipal(Usuario usuarioActual)
        {
            InitializeComponent();
            _usuarioActual = usuarioActual;
        }

        private void FrmMenuPrincipal_Load(object sender, EventArgs e)
        {
            lblBienvenida.Text = $"Bienvenido, {_usuarioActual.NombreUsuario}  |  Rol: {_usuarioActual.NombreRol}";

            // Aplicar visibilidad de botones según el rol del usuario logueado
            AplicarPermisosPorRol();
        }

        // =======================================================================
        // CONTROL DE ACCESO: Oculta botones del menú según el rol.
        // Reglas de negocio:
        //   Administrador  → acceso total
        //   Recepcionista  → Pacientes, Médicos, Citas (no ve Historial ni Usuarios)
        //   Medico         → Pacientes, Citas, Historial Médico (no ve Médicos ni Usuarios)
        //   Rol desconocido → bloqueo total por seguridad
        // Se usa NombreRol (string) en lugar de RolId numérico para mayor claridad.
        // =======================================================================
        private void AplicarPermisosPorRol()
        {
            // Partir de todo visible y luego restringir según el rol
            btnPacientes.Visible = true;
            btnMedicos.Visible = true;
            btnCitas.Visible = true;
            btnHistorial.Visible = true;
            btnUsuarios.Visible = true;

            switch (_usuarioActual.NombreRol)
            {
                case "Administrador":
                    // Acceso total — no se oculta nada
                    break;

                case "Recepcionista":
                    // El recepcionista gestiona el front-office pero NO accede
                    // a historiales clínicos ni a la administración de usuarios
                    btnHistorial.Visible = false;
                    btnUsuarios.Visible = false;
                    break;

                case "Medico":
                    // El médico ve sus pacientes, citas e historiales,
                    // pero NO administra otros médicos ni usuarios del sistema
                    btnMedicos.Visible = false;
                    btnUsuarios.Visible = false;
                    break;

                default:
                    // Rol no reconocido: bloquear todo como medida de seguridad
                    btnPacientes.Visible = false;
                    btnMedicos.Visible = false;
                    btnCitas.Visible = false;
                    btnHistorial.Visible = false;
                    btnUsuarios.Visible = false;
                    MessageBox.Show(
                        "Tu rol no tiene permisos configurados. Contacta al administrador.",
                        "Acceso restringido",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    break;
            }
        }

        private void btnPacientes_Click(object sender, EventArgs e)
        {
            var frm = new FrmPacientes();
            frm.ShowDialog();
        }

        private void btnMedicos_Click(object sender, EventArgs e)
        {
            var frm = new FrmMedicos();
            frm.ShowDialog();
        }

        // =======================================================================
        // CONEXIÓN: Abre el formulario de Citas
        // =======================================================================
        private void btnCitas_Click(object sender, EventArgs e)
        {
            var frm = new FrmCitas();
            frm.ShowDialog();
        }

        // =======================================================================
        // CONEXIÓN: Abre el formulario de Historial Médico
        // =======================================================================
        private void btnHistorial_Click(object sender, EventArgs e)
        {
            var frm = new FrmHistorialMedico();
            frm.ShowDialog();
        }

        // =======================================================================
        // CONEXIÓN AGREGADA: Abre el formulario de Gestión de Usuarios
        // OJO: a diferencia de los otros formularios, FrmUsuarios SÍ necesita el
        // usuario actual como parámetro, para poder bloquear que alguien se
        // elimine o se desactive a sí mismo mientras tiene la sesión abierta.
        // =======================================================================
        private void btnUsuarios_Click(object sender, EventArgs e)
        {
            var frm = new FrmUsuarios(_usuarioActual);
            frm.ShowDialog();
        }

        private void btnCerrarSesion_Click(object sender, EventArgs e)
        {
            var resultado = MessageBox.Show("¿Seguro que deseas cerrar sesión?", "Cerrar sesión",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (resultado == DialogResult.Yes)
            {
                var login = new FrmLogin();
                login.Show();
                this.Close();
            }
        }
    }
}