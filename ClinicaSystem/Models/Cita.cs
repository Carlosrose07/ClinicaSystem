using System;

namespace ClinicaSystem.Models
{
    public class Cita
    {
        public int IdCita { get; set; }
        public int IdPaciente { get; set; }
        public int IdMedico { get; set; }
        public DateTime Fecha { get; set; }
        public TimeSpan Hora { get; set; }
        public string Estado { get; set; }
        public string Observaciones { get; set; }

        // Propiedades de conveniencia para mostrar en grids (se llenan con JOIN)
        public string NombrePaciente { get; set; }
        public string NombreMedico { get; set; }
    }
}