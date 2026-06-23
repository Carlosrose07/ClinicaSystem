using System;

namespace ClinicaSystem.Models
{
    public class HistorialMedico
    {
        public int IdHistorial { get; set; }
        public int IdPaciente { get; set; }
        public int IdMedico { get; set; }
        public int? IdCita { get; set; }
        public DateTime FechaConsulta { get; set; }
        public string Sintomas { get; set; }
        public string Diagnostico { get; set; }
        public string Tratamiento { get; set; }

        // Propiedades de conveniencia para mostrar en grids (se llenan con JOIN)
        public string NombrePaciente { get; set; }
        public string NombreMedico { get; set; }
    }
}