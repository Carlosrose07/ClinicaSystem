using System;
namespace ClinicaSystem.Models
{
    public class Medico
    {
        public int IdMedico { get; set; }
        public string Nombre { get; set; }
        public string Cedula { get; set; }
        public string Especialidad { get; set; }
        public string Telefono { get; set; }
        public string Turno { get; set; }
        public DateTime FechaRegistro { get; set; }

        // Vincula este médico con su cuenta de acceso en la tabla usuarios.
        // Nullable porque los médicos creados directamente desde FrmMedicos
        // (sin pasar por FrmUsuarios) pueden no tener usuario asociado.
        public int? IdUsuario { get; set; }
    }
}