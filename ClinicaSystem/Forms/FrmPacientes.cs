using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using ClinicaSystem.Data;
using ClinicaSystem.Models;

namespace ClinicaSystem.Forms
{
    public partial class FrmPacientes : Form
    {
        private readonly PacienteRepository _repositorio = new PacienteRepository();
        private List<Paciente> _listaPacientes = new List<Paciente>();
        private int _idSeleccionado = 0;
        private bool _cargandoDatos = false;

        public FrmPacientes()
        {
            InitializeComponent();
        }

        private void FrmPacientes_Load(object sender, EventArgs e)
        {
            ConfigurarGrid();
            CargarPacientes();
            LimpiarCampos();
        }

        private void ConfigurarGrid()
        {
            dgvPacientes.Columns.Clear();
            dgvPacientes.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colId",
                DataPropertyName = "IdPaciente",
                HeaderText = "ID",
                Width = 40
            });
            dgvPacientes.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colNombre",
                DataPropertyName = "Nombre",
                HeaderText = "Nombre",
                Width = 180
            });
            dgvPacientes.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colCedula",
                DataPropertyName = "Cedula",
                HeaderText = "Cédula",
                Width = 120
            });
            dgvPacientes.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colFechaNacimiento",
                DataPropertyName = "FechaNacimiento",
                HeaderText = "Nacimiento",
                Width = 90,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "dd/MM/yyyy" }
            });
            dgvPacientes.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colTelefono",
                DataPropertyName = "Telefono",
                HeaderText = "Teléfono",
                Width = 100
            });
            dgvPacientes.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colTipoSangre",
                DataPropertyName = "TipoSangre",
                HeaderText = "Sangre",
                Width = 60
            });
        }

        private void CargarPacientes(List<Paciente> lista = null)
        {
            _cargandoDatos = true;
            _listaPacientes = lista ?? _repositorio.ObtenerTodos();
            dgvPacientes.DataSource = null;
            dgvPacientes.DataSource = _listaPacientes;
            _cargandoDatos = false;
        }

        private void dgvPacientes_SelectionChanged(object sender, EventArgs e)
        {
            if (_cargandoDatos) return;

            if (dgvPacientes.CurrentRow == null || dgvPacientes.CurrentRow.DataBoundItem == null)
            {
                _idSeleccionado = 0;
                return;
            }

            var paciente = (Paciente)dgvPacientes.CurrentRow.DataBoundItem;
            _idSeleccionado = paciente.IdPaciente;

            txtNombre.Text = paciente.Nombre;
            txtCedula.Text = paciente.Cedula;
            dtpFechaNacimiento.Value = paciente.FechaNacimiento;
            txtTelefono.Text = paciente.Telefono;
            txtDireccion.Text = paciente.Direccion;
            cmbTipoSangre.Text = string.IsNullOrEmpty(paciente.TipoSangre)
                ? cmbTipoSangre.Items[0].ToString()
                : paciente.TipoSangre;
        }

        private void txtBuscar_TextChanged(object sender, EventArgs e)
        {
            string filtro = txtBuscar.Text.Trim().ToLower();

            if (string.IsNullOrEmpty(filtro))
            {
                CargarPacientes();
                return;
            }

            var filtrados = _repositorio.ObtenerTodos()
                .Where(p => p.Nombre.ToLower().Contains(filtro) || p.Cedula.Contains(filtro))
                .ToList();

            CargarPacientes(filtrados);
        }

        private void btnNuevo_Click(object sender, EventArgs e)
        {
            LimpiarCampos();
            txtNombre.Focus();
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (!ValidarCampos()) return;

            var paciente = new Paciente
            {
                IdPaciente = _idSeleccionado,
                Nombre = txtNombre.Text.Trim(),
                Cedula = txtCedula.Text.Trim(),
                FechaNacimiento = dtpFechaNacimiento.Value.Date,
                Telefono = string.IsNullOrWhiteSpace(txtTelefono.Text) ? null : txtTelefono.Text.Trim(),
                Direccion = string.IsNullOrWhiteSpace(txtDireccion.Text) ? null : txtDireccion.Text.Trim(),
                TipoSangre = cmbTipoSangre.SelectedIndex <= 0 ? null : cmbTipoSangre.Text
            };

            try
            {
                if (_repositorio.ExisteCedula(paciente.Cedula, paciente.IdPaciente))
                {
                    MessageBox.Show("Ya existe un paciente registrado con esa cédula.",
                        "Cédula duplicada", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (_idSeleccionado == 0)
                {
                    _repositorio.Insertar(paciente);
                    MessageBox.Show("Paciente registrado correctamente.",
                        "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    _repositorio.Actualizar(paciente);
                    MessageBox.Show("Paciente actualizado correctamente.",
                        "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                CargarPacientes();
                LimpiarCampos();
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Error al guardar el paciente: " + ex.Message,
                    "Error de base de datos", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (_idSeleccionado == 0)
            {
                MessageBox.Show("Selecciona un paciente de la lista primero.",
                    "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var confirmacion = MessageBox.Show(
                $"¿Seguro que deseas eliminar al paciente \"{txtNombre.Text}\"?\nEsta acción no se puede deshacer.",
                "Confirmar eliminación",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (confirmacion != DialogResult.Yes) return;

            try
            {
                _repositorio.Eliminar(_idSeleccionado);
                MessageBox.Show("Paciente eliminado correctamente.",
                    "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                CargarPacientes();
                LimpiarCampos();
            }
            catch (SqlException ex) when (ex.Number == 547)
            {
                MessageBox.Show(
                    "No se puede eliminar este paciente porque tiene citas o historial médico asociado.\n" +
                    "Elimina primero esos registros relacionados.",
                    "No se puede eliminar",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Error al eliminar el paciente: " + ex.Message,
                    "Error de base de datos", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            LimpiarCampos();
        }

        private bool ValidarCampos()
        {
            if (string.IsNullOrWhiteSpace(txtNombre.Text))
            {
                MessageBox.Show("El nombre es obligatorio.", "Campo requerido",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtNombre.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtCedula.Text))
            {
                MessageBox.Show("La cédula es obligatoria.", "Campo requerido",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtCedula.Focus();
                return false;
            }

            if (!Regex.IsMatch(txtCedula.Text.Trim(), @"^\d{3}-\d{7}-\d{1}$"))
            {
                MessageBox.Show("La cédula debe tener el formato 000-0000000-0.",
                    "Formato inválido", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtCedula.Focus();
                return false;
            }

            if (dtpFechaNacimiento.Value.Date > DateTime.Today)
            {
                MessageBox.Show("La fecha de nacimiento no puede ser futura.",
                    "Fecha inválida", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        private void LimpiarCampos()
        {
            _idSeleccionado = 0;
            txtNombre.Clear();
            txtCedula.Clear();
            dtpFechaNacimiento.Value = DateTime.Today;
            txtTelefono.Clear();
            txtDireccion.Clear();
            cmbTipoSangre.SelectedIndex = 0;
            dgvPacientes.ClearSelection();
        }
    }
}