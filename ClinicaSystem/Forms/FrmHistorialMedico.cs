using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Windows.Forms;
using ClinicaSystem.Data;
using ClinicaSystem.Models;

namespace ClinicaSystem.Forms
{
    public partial class FrmHistorialMedico : Form
    {
        private readonly HistorialMedicoRepository _repositorio = new HistorialMedicoRepository();
        private readonly PacienteRepository _pacienteRepositorio = new PacienteRepository();
        private readonly MedicoRepository _medicoRepositorio = new MedicoRepository();
        private readonly CitaRepository _citaRepositorio = new CitaRepository();

        private List<HistorialMedico> _listaHistorial = new List<HistorialMedico>();
        private List<Paciente> _pacientes = new List<Paciente>();
        private List<Medico> _medicos = new List<Medico>();

        private int _idSeleccionado = 0;
        private bool _cargandoDatos = false;

        // Clase auxiliar SOLO para mostrar las citas en cmbCita con un texto legible
        private class ItemCita
        {
            public int IdCita { get; set; }
            public string Texto { get; set; }
        }

        public FrmHistorialMedico()
        {
            InitializeComponent();
        }

        private void FrmHistorialMedico_Load(object sender, EventArgs e)
        {
            ConfigurarGrid();
            CargarListasAuxiliares();
            CargarHistorial();
            LimpiarCampos();
        }

        private void ConfigurarGrid()
        {
            dgvHistorial.Columns.Clear();
            dgvHistorial.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colId",
                DataPropertyName = "IdHistorial",
                HeaderText = "ID",
                Width = 40
            });
            dgvHistorial.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colPaciente",
                DataPropertyName = "NombrePaciente",
                HeaderText = "Paciente",
                Width = 130
            });
            dgvHistorial.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colMedico",
                DataPropertyName = "NombreMedico",
                HeaderText = "Médico",
                Width = 130
            });
            dgvHistorial.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colFecha",
                DataPropertyName = "FechaConsulta",
                HeaderText = "Fecha consulta",
                Width = 100,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "dd/MM/yyyy HH:mm" }
            });
            dgvHistorial.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colDiagnostico",
                DataPropertyName = "Diagnostico",
                HeaderText = "Diagnóstico",
                Width = 180
            });
        }

        // Carga los combos de Paciente y Médico una sola vez al abrir el formulario.
        // IMPORTANTE: DisplayMember/ValueMember se asignan ANTES que DataSource.
        // Si se asignan después (como en el primer intento), al fijar DataSource el
        // combo dispara SelectedIndexChanged de inmediato y, como ValueMember aún no
        // existía, SelectedValue devolvía el objeto Paciente completo en vez del int
        // -> InvalidCastException en CargarCitasDelPaciente().
        private void CargarListasAuxiliares()
        {
            _pacientes = _pacienteRepositorio.ObtenerTodos();
            _medicos = _medicoRepositorio.ObtenerTodos();

            cmbPaciente.DisplayMember = "Nombre";
            cmbPaciente.ValueMember = "IdPaciente";
            cmbPaciente.DataSource = _pacientes;

            cmbMedico.DisplayMember = "Nombre";
            cmbMedico.ValueMember = "IdMedico";
            cmbMedico.DataSource = _medicos;

            if (_pacientes.Count == 0 || _medicos.Count == 0)
            {
                MessageBox.Show(
                    "Debes registrar al menos un paciente y un médico antes de crear historial médico.",
                    "Datos insuficientes", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        // Cada vez que cambia el paciente seleccionado, se vuelve a consultar
        // SOLO las citas de ese paciente y se arma el combo cmbCita con
        // "(Sin cita asociada)" como primera opción (id_cita es opcional/nullable).
        private void cmbPaciente_SelectedIndexChanged(object sender, EventArgs e)
        {
            CargarCitasDelPaciente();
        }

        private void CargarCitasDelPaciente()
        {
            var items = new List<ItemCita>
            {
                new ItemCita { IdCita = 0, Texto = "(Sin cita asociada)" }
            };

            // Usamos "is int" en vez de un cast directo: si este método se llegara a
            // disparar antes de que ValueMember esté completamente listo, SelectedValue
            // podría no ser un int, y así evitamos el InvalidCastException en vez de
            // depender 100% del orden de asignación.
            if (cmbPaciente.SelectedValue is int idPaciente)
            {
                var citasPaciente = _citaRepositorio.ObtenerPorPaciente(idPaciente);

                items.AddRange(citasPaciente.Select(c => new ItemCita
                {
                    IdCita = c.IdCita,
                    Texto = $"{c.Fecha:dd/MM/yyyy} {c.Hora:hh\\:mm} - {c.Estado}"
                }));
            }

            cmbCita.DataSource = items;
            cmbCita.DisplayMember = "Texto";
            cmbCita.ValueMember = "IdCita";
        }

        private void CargarHistorial(List<HistorialMedico> lista = null)
        {
            _cargandoDatos = true;
            _listaHistorial = lista ?? _repositorio.ObtenerTodos();
            dgvHistorial.DataSource = null;
            dgvHistorial.DataSource = _listaHistorial;
            _cargandoDatos = false;
        }

        private void dgvHistorial_SelectionChanged(object sender, EventArgs e)
        {
            if (_cargandoDatos) return;

            if (dgvHistorial.CurrentRow == null || dgvHistorial.CurrentRow.DataBoundItem == null)
            {
                _idSeleccionado = 0;
                return;
            }

            var historial = (HistorialMedico)dgvHistorial.CurrentRow.DataBoundItem;
            _idSeleccionado = historial.IdHistorial;

            // Al fijar cmbPaciente.SelectedValue se dispara cmbPaciente_SelectedIndexChanged,
            // que repuebla cmbCita con las citas de ESE paciente. Por eso cmbCita.SelectedValue
            // se asigna DESPUÉS, una vez que ya existe la opción correcta en el combo.
            cmbPaciente.SelectedValue = historial.IdPaciente;
            cmbMedico.SelectedValue = historial.IdMedico;
            cmbCita.SelectedValue = historial.IdCita ?? 0;

            txtSintomas.Text = historial.Sintomas;
            txtDiagnostico.Text = historial.Diagnostico;
            txtTratamiento.Text = historial.Tratamiento;
        }

        private void txtBuscar_TextChanged(object sender, EventArgs e)
        {
            string filtro = txtBuscar.Text.Trim().ToLower();

            if (string.IsNullOrEmpty(filtro))
            {
                CargarHistorial();
                return;
            }

            var filtrados = _repositorio.ObtenerTodos()
                .Where(h => h.NombrePaciente.ToLower().Contains(filtro)
                         || h.NombreMedico.ToLower().Contains(filtro)
                         || (h.Diagnostico != null && h.Diagnostico.ToLower().Contains(filtro)))
                .ToList();

            CargarHistorial(filtrados);
        }

        private void btnNuevo_Click(object sender, EventArgs e)
        {
            LimpiarCampos();
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (!ValidarCampos()) return;

            int idCitaSeleccionada = (int)cmbCita.SelectedValue;

            var historial = new HistorialMedico
            {
                IdHistorial = _idSeleccionado,
                IdPaciente = (int)cmbPaciente.SelectedValue,
                IdMedico = (int)cmbMedico.SelectedValue,
                IdCita = idCitaSeleccionada == 0 ? (int?)null : idCitaSeleccionada,
                Sintomas = string.IsNullOrWhiteSpace(txtSintomas.Text) ? null : txtSintomas.Text.Trim(),
                Diagnostico = txtDiagnostico.Text.Trim(),
                Tratamiento = string.IsNullOrWhiteSpace(txtTratamiento.Text) ? null : txtTratamiento.Text.Trim()
            };

            try
            {
                if (_idSeleccionado == 0)
                {
                    _repositorio.Insertar(historial);
                    MessageBox.Show("Registro de historial guardado correctamente.",
                        "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    _repositorio.Actualizar(historial);
                    MessageBox.Show("Registro de historial actualizado correctamente.",
                        "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                CargarHistorial();
                LimpiarCampos();
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Error al guardar el historial: " + ex.Message,
                    "Error de base de datos", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (_idSeleccionado == 0)
            {
                MessageBox.Show("Selecciona un registro de la lista primero.",
                    "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var confirmacion = MessageBox.Show(
                "¿Seguro que deseas eliminar este registro de historial médico?\nEsta acción no se puede deshacer.",
                "Confirmar eliminación", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (confirmacion != DialogResult.Yes) return;

            try
            {
                _repositorio.Eliminar(_idSeleccionado);
                MessageBox.Show("Registro eliminado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                CargarHistorial();
                LimpiarCampos();
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Error al eliminar el registro: " + ex.Message,
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

            if (string.IsNullOrWhiteSpace(txtDiagnostico.Text))
            {
                MessageBox.Show("El diagnóstico es obligatorio.", "Campo requerido",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtDiagnostico.Focus();
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
            if (cmbCita.Items.Count > 0) cmbCita.SelectedValue = 0;

            txtSintomas.Clear();
            txtDiagnostico.Clear();
            txtTratamiento.Clear();

            _cargandoDatos = false;
            dgvHistorial.ClearSelection();
        }
    }
}