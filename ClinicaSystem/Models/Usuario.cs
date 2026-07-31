namespace ClinicaSystem.Models
{
    public class Usuario
    {
        public int IdUsuario { get; set; }
        public string NombreUsuario { get; set; }
        public string Clave { get; set; }

        // Sal (salt) única por usuario, usada junto con la clave para generar
        // el hash. Se guarda en Base64. Puede venir null en usuarios "viejos"
        // que todavía no se han migrado al nuevo esquema (ver UsuarioRepository).
        public string ClaveSalt { get; set; }

        public int IdRol { get; set; }
        public bool Activo { get; set; }

        // Propiedad de conveniencia para mostrar en grids (se llena con JOIN a roles)
        public string NombreRol { get; set; }
    }
}