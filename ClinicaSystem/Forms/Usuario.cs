using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing;
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
        private readonly MedicoRepository _medicoRepositorio = new MedicoRepository();

        private List<Usuario> _listaUsuarios = new List<Usuario>();
        private List<Rol> _roles = new List<Rol>();

        // Usuario que tiene la sesión activa - se usa para evitar que alguien
        // se elimine o se desactive a sí mismo por accidente.
        private readonly Usuario _usuarioActual;

        private int _idSeleccionado = 0;
        private bool _cargandoDatos = false;

        // NUEVO: guarda el médico vinculado al usuario que está cargado en el
        // formulario (si lo hay). Null = este usuario no tiene médico asociado.
        private Medico _medicoVinculado = null;

        // ---------- Controles del panel de datos de médico (creados por código) ----------
        private Panel pnlDatosMedico;
        private Label lblCedulaMedico;
        private TextBox txtCedulaMedico;
        private Label lblEspecialidadMedico;
        private TextBox txtEspecialidadMedico;
        private Label lblTelefonoMedico;
        private TextBox txtTelefonoMedico;
        private Label lblTurnoMedico;
        private ComboBox cmbTurnoMedico;

        public FrmUsuarios(Usuario usuarioActual)
        {
            InitializeComponent();
            _usuarioActual = usuarioActual;
        }

        private void FrmUsuarios_Load(object sender, EventArgs e)
        {
            ConfigurarGrid();
            ConfigurarPanelMedico();
            CargarRoles();
            CargarUsuarios();
            LimpiarCampos();

            // Se conecta por código para no depender del Designer.
            cmbRol.SelectedIndexChanged += cmbRol_SelectedIndexChanged;
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

        // NUEVO: crea el panel con los campos extra que solo aplican a un
        // usuario con rol "Medico" (cédula, especialidad, teléfono, turno).
        // Se agrega bajo el checkbox "Activo" - AJUSTA pnlDatosMedico.Top /
        // pnlDatosMedico.Left si se monta encima de tus botones Guardar/Eliminar.
        private void ConfigurarPanelMedico()
        {
            pnlDatosMedico = new Panel
            {
                Name = "pnlDatosMedico",
                BackColor = Color.FromArgb(22, 40, 75),   // #16284B - mismo tema oscuro
                BorderStyle = BorderStyle.FixedSingle,
                Location = new Point(chkActivo.Left, chkActivo.Bottom + 15),
                Size = new Size(280, 190),
                Visible = false
            };

            var lblTitulo = new Label
            {
                Text = "Datos de médico",
                ForeColor = Color.FromArgb(201, 168, 76), // dorado #C9A84C
                Font = new Font(this.Font, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(10, 8)
            };

            lblCedulaMedico = new Label { Text = "Cédula:", ForeColor = Color.White, AutoSize = true, Location = new Point(10, 38) };
            txtCedulaMedico = new TextBox { Location = new Point(120, 35), Width = 140 };

            lblEspecialidadMedico = new Label { Text = "Especialidad:", ForeColor = Color.White, AutoSize = true, Location = new Point(10, 68) };
            txtEspecialidadMedico = new TextBox { Location = new Point(120, 65), Width = 140 };

            lblTelefonoMedico = new Label { Text = "Teléfono:", ForeColor = Color.White, AutoSize = true, Location = new Point(10, 98) };
            txtTelefonoMedico = new TextBox { Location = new Point(120, 95), Width = 140 };

            lblTurnoMedico = new Label { Text = "Turno:", ForeColor = Color.White, AutoSize = true, Location = new Point(10, 128) };
            cmbTurnoMedico = new ComboBox
            {
                Location = new Point(120, 125),
                Width = 140,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            // ASUNCIÓN: confirmar que estos textos coinciden exactamente con
            // los que usa FrmMedicos, para no generar datos inconsistentes.
            cmbTurnoMedico.Items.AddRange(new object[] { "Matutino", "Vespertino", "Nocturno" });

            pnlDatosMedico.Controls.Add(lblTitulo);
            pnlDatosMedico.Controls.Add(lblCedulaMedico);
            pnlDatosMedico.Controls.Add(txtCedulaMedico);
            pnlDatosMedico.Controls.Add(lblEspecialidadMedico);
            pnlDatosMedico.Controls.Add(txtEspecialidadMedico);
            pnlDatosMedico.Controls.Add(lblTelefonoMedico);
            pnlDatosMedico.Controls.Add(txtTelefonoMedico);
            pnlDatosMedico.Controls.Add(lblTurnoMedico);
            pnlDatosMedico.Controls.Add(cmbTurnoMedico);

            this.Controls.Add(pnlDatosMedico);
            pnlDatosMedico.BringToFront();
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

        // NUEVO: true si el rol actualmente seleccionado en el combo es "Medico"
        private bool RolSeleccionadoEsMedico()
        {
            if (cmbRol.SelectedItem == null) return false;
            var rol = (Rol)cmbRol.SelectedItem;
            return rol.NombreRol == "Medico";
        }

        // NUEVO: muestra/oculta el panel de datos de médico según el rol elegido
        private void cmbRol_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_cargandoDatos) return;
            pnlDatosMedico.Visible = RolSeleccionadoEsMedico();
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

            // NUEVO: si el usuario es Medico, buscar y precargar sus datos
            CargarDatosMedicoVinculado(usuario.IdUsuario);
        }

        // NUEVO: busca si este usuario ya tiene un médico vinculado y llena el panel.
        // Si el rol es Medico pero NO existe médico vinculado (usuario creado antes
        // de este cambio), muestra el panel vacío para que lo completen al guardar.
        private void CargarDatosMedicoVinculado(int idUsuario)
        {
            _medicoVinculado = null;
            txtCedulaMedico.Clear();
            txtEspecialidadMedico.Clear();
            txtTelefonoMedico.Clear();
            cmbTurnoMedico.SelectedIndex = -1;

            if (!RolSeleccionadoEsMedico())
            {
                pnlDatosMedico.Visible = false;
                return;
            }

            pnlDatosMedico.Visible = true;
            _medicoVinculado = _medicoRepositorio.ObtenerPorIdUsuario(idUsuario);

            if (_medicoVinculado != null)
            {
                txtCedulaMedico.Text = _medicoVinculado.Cedula;
                txtEspecialidadMedico.Text = _medicoVinculado.Especialidad;
                txtTelefonoMedico.Text = _medicoVinculado.Telefono;
                cmbTurnoMedico.SelectedItem = _medicoVinculado.Turno;
            }
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

            // NUEVO: validar cédula duplicada ANTES de tocar la base de datos,
            // solo si el rol elegido es Medico
            if (RolSeleccionadoEsMedico())
            {
                int idMedicoExcluir = _medicoVinculado?.IdMedico ?? 0;
                if (_medicoRepositorio.ExisteCedula(txtCedulaMedico.Text.Trim(), idMedicoExcluir))
                {
                    MessageBox.Show("Ya existe un médico registrado con esa cédula.",
                        "Cédula duplicada", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
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
                int idUsuarioGuardado;

                if (_idSeleccionado == 0)
                {
                    // Usuario nuevo: Insertar() recibe la clave en texto plano y la hashea internamente
                    idUsuarioGuardado = _repositorio.Insertar(usuario, txtClave.Text);
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
                    idUsuarioGuardado = usuario.IdUsuario;

                    // Actualizar() nunca toca la clave; si se marcó "Cambiar contraseña", se hace aparte
                    if (chkCambiarClave.Checked)
                    {
                        _repositorio.CambiarClave(usuario.IdUsuario, txtClave.Text);
                    }

                    MessageBox.Show("Usuario actualizado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                // NUEVO: sincronizar el registro de médico según el rol elegido
                SincronizarMedico(idUsuarioGuardado);

                CargarUsuarios();
                LimpiarCampos();
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Error al guardar el usuario: " + ex.Message,
                    "Error de base de datos", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // NUEVO: crea, actualiza o desvincula el médico asociado a este usuario,
        // dependiendo del rol que haya quedado seleccionado al guardar.
        private void SincronizarMedico(int idUsuario)
        {
            bool esMedico = RolSeleccionadoEsMedico();

            if (esMedico)
            {
                var medico = new Medico
                {
                    Nombre = txtNombreUsuario.Text.Trim(),
                    Cedula = txtCedulaMedico.Text.Trim(),
                    Especialidad = txtEspecialidadMedico.Text.Trim(),
                    Telefono = string.IsNullOrWhiteSpace(txtTelefonoMedico.Text) ? null : txtTelefonoMedico.Text.Trim(),
                    Turno = cmbTurnoMedico.SelectedItem?.ToString(),
                    IdUsuario = idUsuario
                };

                if (_medicoVinculado == null)
                {
                    // No existía médico vinculado a este usuario -> crear
                    _medicoRepositorio.Insertar(medico);
                }
                else
                {
                    // Ya existía -> actualizar sus datos
                    medico.IdMedico = _medicoVinculado.IdMedico;
                    _medicoRepositorio.Actualizar(medico);
                }
            }
            else if (_medicoVinculado != null)
            {
                // El usuario TENÍA un médico vinculado pero le cambiaron el rol
                // a algo distinto de Medico. Preguntar si se debe eliminar.
                var confirmacion = MessageBox.Show(
                    "Este usuario tenía un registro de médico asociado y su rol ya no es Medico.\n" +
                    "¿Deseas eliminar también su registro como médico?\n\n" +
                    "Si tiene citas asociadas, no se podrá eliminar.",
                    "Rol cambiado", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (confirmacion == DialogResult.Yes)
                {
                    try
                    {
                        _medicoRepositorio.Eliminar(_medicoVinculado.IdMedico);
                    }
                    catch (SqlException ex) when (ex.Number == 547)
                    {
                        MessageBox.Show(
                            "No se pudo eliminar el registro de médico porque tiene citas asociadas.\n" +
                            "El usuario cambió de rol, pero el médico permanece en el sistema.",
                            "No se puede eliminar", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
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
                // NUEVO: si tenía médico vinculado, avisar / intentar limpiar primero.
                if (_medicoVinculado != null)
                {
                    var confirmacionMedico = MessageBox.Show(
                        "Este usuario tiene un registro de médico asociado.\n" +
                        "¿Deseas eliminar también su registro como médico?\n\n" +
                        "Si tiene citas asociadas, no se podrá eliminar y quedará sin usuario de acceso.",
                        "Eliminar médico asociado", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (confirmacionMedico == DialogResult.Yes)
                    {
                        try
                        {
                            _medicoRepositorio.Eliminar(_medicoVinculado.IdMedico);
                        }
                        catch (SqlException ex) when (ex.Number == 547)
                        {
                            MessageBox.Show(
                                "No se pudo eliminar el registro de médico porque tiene citas asociadas.",
                                "No se puede eliminar", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                }

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

            // NUEVO: validar los campos del médico si el rol elegido es Medico
            if (RolSeleccionadoEsMedico())
            {
                if (string.IsNullOrWhiteSpace(txtCedulaMedico.Text))
                {
                    MessageBox.Show("La cédula del médico es obligatoria.", "Campo requerido",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtCedulaMedico.Focus();
                    return false;
                }

                if (string.IsNullOrWhiteSpace(txtEspecialidadMedico.Text))
                {
                    MessageBox.Show("La especialidad del médico es obligatoria.", "Campo requerido",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtEspecialidadMedico.Focus();
                    return false;
                }

                if (cmbTurnoMedico.SelectedIndex == -1)
                {
                    MessageBox.Show("Selecciona el turno del médico.", "Campo requerido",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    cmbTurnoMedico.Focus();
                    return false;
                }
            }

            return true;
        }

        private void LimpiarCampos()
        {
            _idSeleccionado = 0;
            _medicoVinculado = null;
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

            // NUEVO: limpiar y ocultar panel de médico; se vuelve a mostrar
            // solo si el rol por defecto (índice 0) resulta ser Medico
            txtCedulaMedico.Clear();
            txtEspecialidadMedico.Clear();
            txtTelefonoMedico.Clear();
            cmbTurnoMedico.SelectedIndex = -1;
            pnlDatosMedico.Visible = RolSeleccionadoEsMedico();

            dgvUsuarios.ClearSelection();
        }
    }
}