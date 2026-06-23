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
    public partial class FrmMedicos : Form
    {
        private readonly MedicoRepository _repositorio = new MedicoRepository();
        private List<Medico> _listaMedicos = new List<Medico>();
        private int _idSeleccionado = 0;
        private bool _cargandoDatos = false;

        public FrmMedicos()
        {
            InitializeComponent();
        }

        private void FrmMedicos_Load(object sender, EventArgs e)
        {
            ConfigurarGrid();
            CargarMedicos();
            LimpiarCampos();
        }

        private void ConfigurarGrid()
        {
            dgvMedicos.Columns.Clear();
            dgvMedicos.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colId",
                DataPropertyName = "IdMedico",
                HeaderText = "ID",
                Width = 40
            });
            dgvMedicos.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colNombre",
                DataPropertyName = "Nombre",
                HeaderText = "Nombre",
                Width = 150
            });
            dgvMedicos.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colCedula",
                DataPropertyName = "Cedula",
                HeaderText = "Cédula",
                Width = 110
            });
            dgvMedicos.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colEspecialidad",
                DataPropertyName = "Especialidad",
                HeaderText = "Especialidad",
                Width = 120
            });
            dgvMedicos.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colTelefono",
                DataPropertyName = "Telefono",
                HeaderText = "Teléfono",
                Width = 90
            });
            dgvMedicos.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colTurno",
                DataPropertyName = "Turno",
                HeaderText = "Turno",
                Width = 70
            });
        }

        private void CargarMedicos(List<Medico> lista = null)
        {
            _cargandoDatos = true;
            _listaMedicos = lista ?? _repositorio.ObtenerTodos();
            dgvMedicos.DataSource = null;
            dgvMedicos.DataSource = _listaMedicos;
            _cargandoDatos = false;
        }

        private void dgvMedicos_SelectionChanged(object sender, EventArgs e)
        {
            if (_cargandoDatos) return;

            if (dgvMedicos.CurrentRow == null || dgvMedicos.CurrentRow.DataBoundItem == null)
            {
                _idSeleccionado = 0;
                return;
            }

            var medico = (Medico)dgvMedicos.CurrentRow.DataBoundItem;
            _idSeleccionado = medico.IdMedico;

            txtNombre.Text = medico.Nombre;
            txtCedula.Text = medico.Cedula;
            txtEspecialidad.Text = medico.Especialidad;
            txtTelefono.Text = medico.Telefono;
            cmbTurno.Text = medico.Turno;
        }

        private void txtBuscar_TextChanged(object sender, EventArgs e)
        {
            string filtro = txtBuscar.Text.Trim().ToLower();

            if (string.IsNullOrEmpty(filtro))
            {
                CargarMedicos();
                return;
            }

            var filtrados = _repositorio.ObtenerTodos()
                .Where(m => m.Nombre.ToLower().Contains(filtro)
                         || m.Cedula.Contains(filtro)
                         || m.Especialidad.ToLower().Contains(filtro))
                .ToList();

            CargarMedicos(filtrados);
        }

        private void btnNuevo_Click(object sender, EventArgs e)
        {
            LimpiarCampos();
            txtNombre.Focus();
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (!ValidarCampos()) return;

            var medico = new Medico
            {
                IdMedico = _idSeleccionado,
                Nombre = txtNombre.Text.Trim(),
                Cedula = txtCedula.Text.Trim(),
                Especialidad = txtEspecialidad.Text.Trim(),
                Telefono = string.IsNullOrWhiteSpace(txtTelefono.Text) ? null : txtTelefono.Text.Trim(),
                Turno = cmbTurno.Text
            };

            try
            {
                if (_repositorio.ExisteCedula(medico.Cedula, medico.IdMedico))
                {
                    MessageBox.Show("Ya existe un médico registrado con esa cédula.",
                        "Cédula duplicada", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (_idSeleccionado == 0)
                {
                    _repositorio.Insertar(medico);
                    MessageBox.Show("Médico registrado correctamente.",
                        "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    _repositorio.Actualizar(medico);
                    MessageBox.Show("Médico actualizado correctamente.",
                        "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                CargarMedicos();
                LimpiarCampos();
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Error al guardar el médico: " + ex.Message,
                    "Error de base de datos", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (_idSeleccionado == 0)
            {
                MessageBox.Show("Selecciona un médico de la lista primero.",
                    "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var confirmacion = MessageBox.Show(
                $"¿Seguro que deseas eliminar al Dr(a). \"{txtNombre.Text}\"?\nEsta acción no se puede deshacer.",
                "Confirmar eliminación",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (confirmacion != DialogResult.Yes) return;

            try
            {
                _repositorio.Eliminar(_idSeleccionado);
                MessageBox.Show("Médico eliminado correctamente.",
                    "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                CargarMedicos();
                LimpiarCampos();
            }
            catch (SqlException ex) when (ex.Number == 547)
            {
                MessageBox.Show(
                    "No se puede eliminar este médico porque tiene citas o historial médico asociado.\n" +
                    "Elimina primero esos registros relacionados.",
                    "No se puede eliminar",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Error al eliminar el médico: " + ex.Message,
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

            if (string.IsNullOrWhiteSpace(txtEspecialidad.Text))
            {
                MessageBox.Show("La especialidad es obligatoria.", "Campo requerido",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtEspecialidad.Focus();
                return false;
            }

            if (cmbTurno.SelectedIndex == -1)
            {
                MessageBox.Show("Selecciona un turno.", "Campo requerido",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cmbTurno.Focus();
                return false;
            }

            return true;
        }

        private void LimpiarCampos()
        {
            _idSeleccionado = 0;
            txtNombre.Clear();
            txtCedula.Clear();
            txtEspecialidad.Clear();
            txtTelefono.Clear();
            cmbTurno.SelectedIndex = 0;
            dgvMedicos.ClearSelection();
        }
    }
}