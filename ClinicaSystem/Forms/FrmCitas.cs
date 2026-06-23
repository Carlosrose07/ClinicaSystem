using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Windows.Forms;
using ClinicaSystem.Data;
using ClinicaSystem.Models;

namespace ClinicaSystem.Forms
{
    public partial class FrmCitas : Form
    {
        private readonly CitaRepository _repositorio = new CitaRepository();
        private readonly PacienteRepository _pacienteRepositorio = new PacienteRepository();
        private readonly MedicoRepository _medicoRepositorio = new MedicoRepository();

        private List<Cita> _listaCitas = new List<Cita>();
        private List<Paciente> _pacientes = new List<Paciente>();
        private List<Medico> _medicos = new List<Medico>();

        private int _idSeleccionado = 0;
        private bool _cargandoDatos = false;

        public FrmCitas()
        {
            InitializeComponent();
        }

        private void FrmCitas_Load(object sender, EventArgs e)
        {
            ConfigurarGrid();
            CargarListasAuxiliares();
            CargarCitas();
            LimpiarCampos();
        }

        private void ConfigurarGrid()
        {
            dgvCitas.Columns.Clear();
            dgvCitas.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colId",
                DataPropertyName = "IdCita",
                HeaderText = "ID",
                Width = 40
            });
            dgvCitas.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colPaciente",
                DataPropertyName = "NombrePaciente",
                HeaderText = "Paciente",
                Width = 150
            });
            dgvCitas.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colMedico",
                DataPropertyName = "NombreMedico",
                HeaderText = "Médico",
                Width = 150
            });
            dgvCitas.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colFecha",
                DataPropertyName = "Fecha",
                HeaderText = "Fecha",
                Width = 80,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "dd/MM/yyyy" }
            });
            dgvCitas.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colHora",
                DataPropertyName = "Hora",
                HeaderText = "Hora",
                Width = 60,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "hh\\:mm" }
            });
            dgvCitas.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colEstado",
                DataPropertyName = "Estado",
                HeaderText = "Estado",
                Width = 90
            });
        }

        private void CargarListasAuxiliares()
        {
            _cargandoDatos = true;

            _pacientes = _pacienteRepositorio.ObtenerTodos();
            _medicos = _medicoRepositorio.ObtenerTodos();

            cmbPaciente.DataSource = _pacientes;
            cmbPaciente.DisplayMember = "Nombre";
            cmbPaciente.ValueMember = "IdPaciente";

            cmbMedico.DataSource = _medicos;
            cmbMedico.DisplayMember = "Nombre";
            cmbMedico.ValueMember = "IdMedico";

            _cargandoDatos = false;

            if (_pacientes.Count == 0 || _medicos.Count == 0)
            {
                MessageBox.Show(
                    "Debes registrar al menos un paciente y un médico antes de agendar citas.",
                    "Datos insuficientes", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void CargarCitas(List<Cita> lista = null)
        {
            _cargandoDatos = true;
            _listaCitas = lista ?? _repositorio.ObtenerTodas();
            dgvCitas.DataSource = null;
            dgvCitas.DataSource = _listaCitas;
            _cargandoDatos = false;
        }

        private void dgvCitas_SelectionChanged(object sender, EventArgs e)
        {
            if (_cargandoDatos) return;

            if (dgvCitas.CurrentRow == null || dgvCitas.CurrentRow.DataBoundItem == null)
            {
                _idSeleccionado = 0;
                return;
            }

            var cita = (Cita)dgvCitas.CurrentRow.DataBoundItem;
            _idSeleccionado = cita.IdCita;

            _cargandoDatos = true;
            cmbPaciente.SelectedValue = cita.IdPaciente;
            cmbMedico.SelectedValue = cita.IdMedico;
            dtpFecha.Value = cita.Fecha;
            dtpHora.Value = DateTime.Today.Add(cita.Hora);
            cmbEstado.Text = cita.Estado;
            txtObservaciones.Text = cita.Observaciones;
            _cargandoDatos = false;

            VerificarConflicto();
        }

        private void cmbMedico_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_cargandoDatos) return;
            VerificarConflicto();
        }

        private void dtpFecha_ValueChanged(object sender, EventArgs e)
        {
            if (_cargandoDatos) return;
            VerificarConflicto();
        }

        private void dtpHora_ValueChanged(object sender, EventArgs e)
        {
            if (_cargandoDatos) return;
            VerificarConflicto();
        }

        private void VerificarConflicto()
        {
            if (cmbMedico.SelectedValue == null)
            {
                lblConflicto.Visible = false;
                return;
            }

            int idMedico = (int)cmbMedico.SelectedValue;
            DateTime fecha = dtpFecha.Value.Date;
            TimeSpan hora = dtpHora.Value.TimeOfDay;

            bool conflicto = _repositorio.ExisteConflictoHorario(idMedico, fecha, hora, _idSeleccionado);

            lblConflicto.Visible = conflicto;
            lblConflicto.Text = conflicto
                ? "⚠ El médico ya tiene una cita programada en este horario."
                : "";
        }

        private void txtBuscar_TextChanged(object sender, EventArgs e)
        {
            string filtro = txtBuscar.Text.Trim().ToLower();

            if (string.IsNullOrEmpty(filtro))
            {
                CargarCitas();
                return;
            }

            var filtradas = _repositorio.ObtenerTodas()
                .Where(c => c.NombrePaciente.ToLower().Contains(filtro)
                         || c.NombreMedico.ToLower().Contains(filtro)
                         || c.Estado.ToLower().Contains(filtro))
                .ToList();

            CargarCitas(filtradas);
        }

        private void btnNuevo_Click(object sender, EventArgs e)
        {
            LimpiarCampos();
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (!ValidarCampos()) return;

            var cita = new Cita
            {
                IdCita = _idSeleccionado,
                IdPaciente = (int)cmbPaciente.SelectedValue,
                IdMedico = (int)cmbMedico.SelectedValue,
                Fecha = dtpFecha.Value.Date,
                Hora = dtpHora.Value.TimeOfDay,
                Estado = cmbEstado.Text,
                Observaciones = string.IsNullOrWhiteSpace(txtObservaciones.Text) ? null : txtObservaciones.Text.Trim()
            };

            try
            {
                if (_repositorio.ExisteConflictoHorario(cita.IdMedico, cita.Fecha, cita.Hora, cita.IdCita))
                {
                    MessageBox.Show(
                        "El médico seleccionado ya tiene una cita programada en esa fecha y hora.",
                        "Conflicto de horario", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (_idSeleccionado == 0)
                {
                    _repositorio.Insertar(cita);
                    MessageBox.Show("Cita agendada correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    _repositorio.Actualizar(cita);
                    MessageBox.Show("Cita actualizada correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                CargarCitas();
                LimpiarCampos();
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Error al guardar la cita: " + ex.Message,
                    "Error de base de datos", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (_idSeleccionado == 0)
            {
                MessageBox.Show("Selecciona una cita de la lista primero.",
                    "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var confirmacion = MessageBox.Show(
                "¿Seguro que deseas eliminar esta cita?\nEsta acción no se puede deshacer.",
                "Confirmar eliminación", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (confirmacion != DialogResult.Yes) return;

            try
            {
                _repositorio.Eliminar(_idSeleccionado);
                MessageBox.Show("Cita eliminada correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                CargarCitas();
                LimpiarCampos();
            }
            catch (SqlException ex) when (ex.Number == 547)
            {
                MessageBox.Show(
                    "No se puede eliminar esta cita porque tiene historial médico asociado.\n" +
                    "Elimina primero esos registros relacionados.",
                    "No se puede eliminar", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Error al eliminar la cita: " + ex.Message,
                    "Error de base de datos", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            LimpiarCampos();
        }

        private bool ValidarCampos()
        {
            if (cmbPaciente.SelectedIndex == -1)
            {
                MessageBox.Show("Selecciona un paciente.", "Campo requerido",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (cmbMedico.SelectedIndex == -1)
            {
                MessageBox.Show("Selecciona un médico.", "Campo requerido",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (cmbEstado.SelectedIndex == -1)
            {
                MessageBox.Show("Selecciona un estado.", "Campo requerido",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (_idSeleccionado == 0 && dtpFecha.Value.Date < DateTime.Today)
            {
                MessageBox.Show("No puedes agendar una cita en una fecha pasada.",
                    "Fecha inválida", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        private void LimpiarCampos()
        {
            _idSeleccionado = 0;
            _cargandoDatos = true;

            if (cmbPaciente.Items.Count > 0) cmbPaciente.SelectedIndex = 0;
            if (cmbMedico.Items.Count > 0) cmbMedico.SelectedIndex = 0;
            dtpFecha.Value = DateTime.Today;
            dtpHora.Value = DateTime.Today.AddHours(8);
            cmbEstado.SelectedIndex = 0;
            txtObservaciones.Clear();

            _cargandoDatos = false;
            lblConflicto.Visible = false;
            dgvCitas.ClearSelection();
        }
    }
}