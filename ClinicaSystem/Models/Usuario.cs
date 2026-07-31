using System;

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

        // ---- Bloqueo por intentos fallidos de login ----
        // Cuántos intentos fallidos consecutivos lleva el usuario.
        // Se resetea a 0 en cada login exitoso.
        public int IntentosFallidos { get; set; }

        // Fecha/hora hasta la cual la cuenta está bloqueada. Null = no bloqueada.
        public DateTime? BloqueadoHasta { get; set; }
    }

    // Resultado detallado de un intento de login. Se usa en vez de devolver
    // solo un Usuario (o null) para que la UI pueda distinguir POR QUÉ falló
    // el login (credenciales incorrectas, cuenta bloqueada, cuenta inactiva)
    // y mostrar un mensaje específico en cada caso.
    public enum TipoResultadoLogin
    {
        Exito,
        CredencialesInvalidas,
        CuentaBloqueada,
        CuentaInactiva
    }

    public class ResultadoLogin
    {
        public TipoResultadoLogin Tipo { get; set; }
        public Usuario Usuario { get; set; }

        // Solo tiene valor cuando Tipo == CuentaBloqueada; indica hasta
        // cuándo debe esperar el usuario para volver a intentar.
        public DateTime? BloqueadoHasta { get; set; }
    }
}