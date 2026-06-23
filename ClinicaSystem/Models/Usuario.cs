namespace ClinicaSystem.Models
{
    public class Usuario
    {
        public int IdUsuario { get; set; }
        public string NombreUsuario { get; set; }
        public string Clave { get; set; }
        public int IdRol { get; set; }
        public bool Activo { get; set; }

        // Propiedad de conveniencia para mostrar en grids (se llena con JOIN a roles)
        public string NombreRol { get; set; }
    }
}