using System;

namespace ClinicaSystem.Models
{
    public class Paciente
    {
        public int IdPaciente { get; set; }
        public string Nombre { get; set; }
        public string Cedula { get; set; }
        public DateTime FechaNacimiento { get; set; }
        public string Telefono { get; set; }
        public string Direccion { get; set; }
        public string TipoSangre { get; set; }
        public DateTime FechaRegistro { get; set; }
    }
}