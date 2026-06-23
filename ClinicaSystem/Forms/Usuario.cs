using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Windows.Forms;
using ClinicaSystem.Data;
using ClinicaSystem.Models;

namespace ClinicaSystem.Forms
{
    public partial class FrmUsuarios : Form
    {
        private readonly UsuarioRepository _repositorio = new UsuarioRepository();
        private readonly RolRepository _rolRepositorio = new RolRepository();

        private List<Usuario> _listaUsuarios = new List<Usuario>();
        private List<Rol> _roles = new List<Rol>();

        // Usuario que tiene la sesión activa - se usa para evitar que alguien
        // se elimine o se desactive a sí mismo por accidente.
        private readonly Usuario _usuarioActual;

        private int _idSeleccionado = 0;
        private bool _cargandoDatos = false;

        public FrmUsuarios(Usuario usuarioActual)
        {
            InitializeComponent();
            _usuarioActual = usuarioActual;
        }

        private void FrmUsuarios_Load(object sender, EventArgs e)
        {
            ConfigurarGrid();
            CargarRoles();
            CargarUsuarios();
            LimpiarCampos();
        }

        private void ConfigurarGrid()
        {
            dgvUsuarios.Columns.Clear();
            dgvUsuarios.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colId",
                DataPropertyName = "IdUsuario",
                HeaderText = "ID",
                Width = 40
            });
            dgvUsuarios.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colNombreUsuario",
                DataPropertyName = "NombreUsuario",
                HeaderText = "Usuario",
                Width = 160
            });
            dgvUsuarios.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colRol",
                DataPropertyName = "NombreRol",
                HeaderText = "Rol",
                Width = 140
            });
            dgvUsuarios.Columns.Add(new DataGridViewCheckBoxColumn
            {
                Name = "colActivo",
                DataPropertyName = "Activo",
                HeaderText = "Activo",
                Width = 60
            });
        }

        // DisplayMember/ValueMember se asignan ANTES que DataSource (lección aprendida
        // en FrmHistorialMedico: si se asignan después, el primer SelectedIndexChanged
        // puede dispararse sin ValueMember listo y causar problemas con SelectedValue).
        private void CargarRoles()
        {
            _roles = _rolRepositorio.ObtenerTodos();
            cmbRol.DisplayMember = "NombreRol";
            cmbRol.ValueMember = "IdRol";
            cmbRol.DataSource = _roles;
        }

        private void CargarUsuarios(List<Usuario> lista = null)
        {
            _cargandoDatos = true;
            _listaUsuarios = lista ?? _repositorio.ObtenerTodos();
            dgvUsuarios.DataSource = null;
            dgvUsuarios.DataSource = _listaUsuarios;
            _cargandoDatos = false;
        }

        private void dgvUsuarios_SelectionChanged(object sender, EventArgs e)
        {
            if (_cargandoDatos) return;

            if (dgvUsuarios.CurrentRow == null || dgvUsuarios.CurrentRow.DataBoundItem == null)
            {
                _idSeleccionado = 0;
                return;
            }

            var usuario = (Usuario)dgvUsuarios.CurrentRow.DataBoundItem;
            _idSeleccionado = usuario.IdUsuario;

            txtNombreUsuario.Text = usuario.NombreUsuario;
            cmbRol.SelectedValue = usuario.IdRol;
            chkActivo.Checked = usuario.Activo;

            // En modo edición la contraseña NO se muestra ni se pide por defecto;
            // solo aparece si el usuario marca "Cambiar contraseña".
            chkCambiarClave.Visible = true;
            chkCambiarClave.Checked = false;
            lblClave.Visible = false;
            txtClave.Visible = false;
            lblConfirmarClave.Visible = false;
            txtConfirmarClave.Visible = false;
            txtClave.Clear();
            txtConfirmarClave.Clear();
        }

        // Muestra/oculta los campos de contraseña según el checkbox "Cambiar contraseña"
        private void chkCambiarClave_CheckedChanged(object sender, EventArgs e)
        {
            bool mostrar = chkCambiarClave.Checked;
            lblClave.Visible = mostrar;
            txtClave.Visible = mostrar;
            lblConfirmarClave.Visible = mostrar;
            txtConfirmarClave.Visible = mostrar;

            if (!mostrar)
            {
                txtClave.Clear();
                txtConfirmarClave.Clear();
            }
        }

        private void txtBuscar_TextChanged(object sender, EventArgs e)
        {
            string filtro = txtBuscar.Text.Trim().ToLower();

            if (string.IsNullOrEmpty(filtro))
            {
                CargarUsuarios();
                return;
            }

            var filtrados = _repositorio.ObtenerTodos()
                .Where(u => u.NombreUsuario.ToLower().Contains(filtro)
                         || u.NombreRol.ToLower().Contains(filtro))
                .ToList();

            CargarUsuarios(filtrados);
        }

        private void btnNuevo_Click(object sender, EventArgs e)
        {
            LimpiarCampos();
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (!ValidarCampos()) return;

            string nombreUsuario = txtNombreUsuario.Text.Trim();

            if (_repositorio.ExisteNombreUsuario(nombreUsuario, _idSeleccionado))
            {
                MessageBox.Show("Ya existe un usuario con ese nombre.",
                    "Nombre duplicado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var usuario = new Usuario
            {
                IdUsuario = _idSeleccionado,
                NombreUsuario = nombreUsuario,
                IdRol = (int)cmbRol.SelectedValue,
                Activo = chkActivo.Checked
            };

            try
            {
                if (_idSeleccionado == 0)
                {
                    // Usuario nuevo: Insertar() recibe la clave en texto plano y la hashea internamente
                    _repositorio.Insertar(usuario, txtClave.Text);
                    MessageBox.Show("Usuario creado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    // Bloqueo de seguridad: no permitir que el usuario logueado se desactive a sí mismo
                    if (usuario.IdUsuario == _usuarioActual.IdUsuario && !usuario.Activo)
                    {
                        MessageBox.Show("No puedes desactivar tu propio usuario mientras tienes la sesión activa.",
                            "Operación no permitida", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    _repositorio.Actualizar(usuario);

                    // Actualizar() nunca toca la clave; si se marcó "Cambiar contraseña", se hace aparte
                    if (chkCambiarClave.Checked)
                    {
                        _repositorio.CambiarClave(usuario.IdUsuario, txtClave.Text);
                    }

                    MessageBox.Show("Usuario actualizado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                CargarUsuarios();
                LimpiarCampos();
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Error al guardar el usuario: " + ex.Message,
                    "Error de base de datos", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (_idSeleccionado == 0)
            {
                MessageBox.Show("Selecciona un usuario de la lista primero.",
                    "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Bloqueo de seguridad: no permitir que el usuario logueado se elimine a sí mismo
            if (_idSeleccionado == _usuarioActual.IdUsuario)
            {
                MessageBox.Show("No puedes eliminar tu propio usuario mientras tienes la sesión activa.",
                    "Operación no permitida", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var confirmacion = MessageBox.Show(
                $"¿Seguro que deseas eliminar al usuario \"{txtNombreUsuario.Text}\"?\nEsta acción no se puede deshacer.",
                "Confirmar eliminación", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (confirmacion != DialogResult.Yes) return;

            try
            {
                _repositorio.Eliminar(_idSeleccionado);
                MessageBox.Show("Usuario eliminado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                CargarUsuarios();
                LimpiarCampos();
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Error al eliminar el usuario: " + ex.Message,
                    "Error de base de datos", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            LimpiarCampos();
        }

        private bool ValidarCampos()
        {
            if (string.IsNullOrWhiteSpace(txtNombreUsuario.Text))
            {
                MessageBox.Show("El nombre de usuario es obligatorio.", "Campo requerido",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtNombreUsuario.Focus();
                return false;
            }

            if (cmbRol.SelectedIndex == -1)
            {
                MessageBox.Show("Selecciona un rol.", "Campo requerido",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            // La contraseña es obligatoria al crear (_idSeleccionado == 0) o al editar
            // SOLO si se marcó "Cambiar contraseña"
            bool requiereClave = (_idSeleccionado == 0) || chkCambiarClave.Checked;

            if (requiereClave)
            {
                if (string.IsNullOrWhiteSpace(txtClave.Text))
                {
                    MessageBox.Show("La contraseña es obligatoria.", "Campo requerido",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtClave.Focus();
                    return false;
                }

                if (txtClave.Text.Length < 6)
                {
                    MessageBox.Show("La contraseña debe tener al menos 6 caracteres.",
                        "Contraseña débil", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtClave.Focus();
                    return false;
                }

                if (txtClave.Text != txtConfirmarClave.Text)
                {
                    MessageBox.Show("Las contraseñas no coinciden.", "Error de validación",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtConfirmarClave.Focus();
                    return false;
                }
            }

            return true;
        }

        private void LimpiarCampos()
        {
            _idSeleccionado = 0;
            txtNombreUsuario.Clear();
            if (cmbRol.Items.Count > 0) cmbRol.SelectedIndex = 0;
            chkActivo.Checked = true;

            // Modo "usuario nuevo": la contraseña siempre es visible y obligatoria,
            // así que ocultamos el checkbox de "cambiar contraseña" (no aplica aquí).
            chkCambiarClave.Visible = false;
            chkCambiarClave.Checked = false;
            lblClave.Visible = true;
            txtClave.Visible = true;
            lblConfirmarClave.Visible = true;
            txtConfirmarClave.Visible = true;
            txtClave.Clear();
            txtConfirmarClave.Clear();

            dgvUsuarios.ClearSelection();
        }
    }
}