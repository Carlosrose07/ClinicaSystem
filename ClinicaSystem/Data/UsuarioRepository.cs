using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Security.Cryptography;
using ClinicaSystem.Models;

namespace ClinicaSystem.Data
{
    public class UsuarioRepository
    {
        // Parámetros del hash PBKDF2 (Rfc2898DeriveBytes). 100,000 iteraciones
        // es un valor razonable para 2026 en un equipo normal: suficientemente
        // lento para dificultar ataques de fuerza bruta, pero rápido para el
        // usuario real que hace login una vez.
        private const int PBKDF2_ITERACIONES = 100000;
        private const int SALT_BYTES = 16;   // 128 bits
        private const int HASH_BYTES = 32;   // 256 bits

        // ---- Parámetros del bloqueo por intentos fallidos ----
        // Tras MAX_INTENTOS_FALLIDOS logins incorrectos seguidos, la cuenta
        // se bloquea por MINUTOS_BLOQUEO minutos. Esto dificulta ataques de
        // fuerza bruta/diccionario contra el login sin afectar demasiado a
        // un usuario real que simplemente se equivocó escribiendo la clave.
        private const int MAX_INTENTOS_FALLIDOS = 5;
        private const int MINUTOS_BLOQUEO = 15;

        // Query base reutilizada: trae el nombre del rol via JOIN.
        // Incluye clave_salt para poder verificar/migrar el hash, e
        // intentos_fallidos/bloqueado_hasta para el control de bloqueo.
        private const string SelectBase = @"
            SELECT u.id_usuario, u.nombre_usuario, u.clave, u.clave_salt, u.id_rol, u.activo, r.nombre_rol,
                   u.intentos_fallidos, u.bloqueado_hasta
            FROM usuarios u
            INNER JOIN roles r ON u.id_rol = r.id_rol";

        // Validar credenciales de login. Devuelve un ResultadoLogin indicando
        // si fue exitoso y, si no, POR QUÉ falló (credenciales incorrectas,
        // cuenta bloqueada por intentos fallidos, o cuenta inactiva), para
        // que la UI (FrmLogin) pueda mostrar el mensaje correcto en cada caso.
        //
        // IMPORTANTE: ya no se compara la clave dentro del WHERE del SELECT
        // (como antes), porque ahora cada usuario puede tener un salt distinto
        // y el hash depende de ese salt. Por eso primero se busca al usuario
        // por nombre, y LUEGO se verifica la clave en memoria.
        public ResultadoLogin ValidarCredenciales(string nombreUsuario, string claveSinHash)
        {
            Usuario usuario = ObtenerPorNombreUsuario(nombreUsuario);

            // Usuario no existe: se devuelve el mismo resultado genérico que
            // credenciales inválidas, para no revelar si el usuario existe o no.
            if (usuario == null)
                return new ResultadoLogin { Tipo = TipoResultadoLogin.CredencialesInvalidas };

            // ---- Verificar si la cuenta está bloqueada ----
            if (usuario.BloqueadoHasta.HasValue)
            {
                if (usuario.BloqueadoHasta.Value > DateTime.Now)
                {
                    // Todavía dentro de la ventana de bloqueo: no se valida
                    // la clave ni se cuenta como intento nuevo.
                    return new ResultadoLogin
                    {
                        Tipo = TipoResultadoLogin.CuentaBloqueada,
                        BloqueadoHasta = usuario.BloqueadoHasta
                    };
                }

                // El bloqueo ya venció: se desbloquea automáticamente antes
                // de continuar, sin necesidad de que un administrador lo haga a mano.
                DesbloquearUsuario(usuario.IdUsuario);
                usuario.BloqueadoHasta = null;
                usuario.IntentosFallidos = 0;
            }

            if (!usuario.Activo)
                return new ResultadoLogin { Tipo = TipoResultadoLogin.CuentaInactiva };

            bool claveValida;

            if (string.IsNullOrEmpty(usuario.ClaveSalt))
            {
                // ---- Usuario "viejo", todavía con hash SHA256 sin salt ----
                // Se valida contra el esquema anterior por compatibilidad.
                claveValida = usuario.Clave == HashearClaveLegacySinSalt(claveSinHash);

                if (claveValida)
                {
                    // Migración transparente: como el login fue correcto y
                    // tenemos la clave en texto plano en este momento, se
                    // aprovecha para generarle un salt y recalcular su hash
                    // con PBKDF2. Así, con el uso normal del sistema, todos
                    // los usuarios activos terminan migrados sin intervención
                    // manual ni reseteo de contraseñas.
                    string nuevoSalt = GenerarSalt();
                    string nuevoHash = HashearClave(claveSinHash, nuevoSalt);
                    ActualizarHashYSalt(usuario.IdUsuario, nuevoHash, nuevoSalt);
                    usuario.Clave = nuevoHash;
                    usuario.ClaveSalt = nuevoSalt;
                }
            }
            else
            {
                // ---- Usuario ya migrado: hash PBKDF2 con salt propio ----
                string hashCalculado = HashearClave(claveSinHash, usuario.ClaveSalt);
                claveValida = usuario.Clave == hashCalculado;
            }

            if (claveValida)
            {
                // Login correcto: si venía con intentos fallidos acumulados
                // de antes, se resetean para que arranque "limpio".
                if (usuario.IntentosFallidos > 0)
                {
                    ResetearIntentosFallidos(usuario.IdUsuario);
                    usuario.IntentosFallidos = 0;
                }

                return new ResultadoLogin { Tipo = TipoResultadoLogin.Exito, Usuario = usuario };
            }

            // ---- Login incorrecto: contabilizar el intento fallido ----
            int intentosActualizados = IncrementarIntentosFallidos(usuario.IdUsuario);

            if (intentosActualizados >= MAX_INTENTOS_FALLIDOS)
            {
                // Se alcanzó el límite: bloquear la cuenta y reiniciar el
                // contador (para que al desbloquearse arranque en 0 de nuevo).
                DateTime hasta = DateTime.Now.AddMinutes(MINUTOS_BLOQUEO);
                BloquearUsuario(usuario.IdUsuario, hasta);

                return new ResultadoLogin
                {
                    Tipo = TipoResultadoLogin.CuentaBloqueada,
                    BloqueadoHasta = hasta
                };
            }

            return new ResultadoLogin { Tipo = TipoResultadoLogin.CredencialesInvalidas };
        }

        // Obtener todos los usuarios (para pantalla de administración)
        public List<Usuario> ObtenerTodos()
        {
            var lista = new List<Usuario>();

            using (var conexion = ConexionDB.ObtenerConexion())
            {
                string query = SelectBase + " ORDER BY u.nombre_usuario";
                using (var comando = new SqlCommand(query, conexion))
                {
                    conexion.Open();
                    using (var reader = comando.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(MapearUsuario(reader));
                        }
                    }
                }
            }

            return lista;
        }

        // Obtener un usuario por su Id
        public Usuario ObtenerPorId(int idUsuario)
        {
            Usuario usuario = null;

            using (var conexion = ConexionDB.ObtenerConexion())
            {
                string query = SelectBase + " WHERE u.id_usuario = @id";
                using (var comando = new SqlCommand(query, conexion))
                {
                    comando.Parameters.AddWithValue("@id", idUsuario);
                    conexion.Open();
                    using (var reader = comando.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            usuario = MapearUsuario(reader);
                        }
                    }
                }
            }

            return usuario;
        }

        // Obtener un usuario por su nombre de usuario (usado internamente por el login)
        public Usuario ObtenerPorNombreUsuario(string nombreUsuario)
        {
            Usuario usuario = null;

            using (var conexion = ConexionDB.ObtenerConexion())
            {
                string query = SelectBase + " WHERE u.nombre_usuario = @nombreUsuario";
                using (var comando = new SqlCommand(query, conexion))
                {
                    comando.Parameters.AddWithValue("@nombreUsuario", nombreUsuario);
                    conexion.Open();
                    using (var reader = comando.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            usuario = MapearUsuario(reader);
                        }
                    }
                }
            }

            return usuario;
        }

        // Insertar un nuevo usuario. Recibe la clave SIN hashear; genera un
        // salt nuevo y guarda clave+salt ya hasheados con PBKDF2.
        public int Insertar(Usuario usuario, string claveSinHash)
        {
            string salt = GenerarSalt();
            string hash = HashearClave(claveSinHash, salt);

            using (var conexion = ConexionDB.ObtenerConexion())
            {
                string query = @"INSERT INTO usuarios (nombre_usuario, clave, clave_salt, id_rol, activo)
                                  OUTPUT INSERTED.id_usuario
                                  VALUES (@nombreUsuario, @clave, @claveSalt, @idRol, @activo)";

                using (var comando = new SqlCommand(query, conexion))
                {
                    comando.Parameters.AddWithValue("@nombreUsuario", usuario.NombreUsuario);
                    comando.Parameters.AddWithValue("@clave", hash);
                    comando.Parameters.AddWithValue("@claveSalt", salt);
                    comando.Parameters.AddWithValue("@idRol", usuario.IdRol);
                    comando.Parameters.AddWithValue("@activo", usuario.Activo);
                    conexion.Open();
                    return (int)comando.ExecuteScalar();
                }
            }
        }

        // Actualizar datos del usuario (NO cambia la clave; para eso usar CambiarClave)
        public bool Actualizar(Usuario usuario)
        {
            using (var conexion = ConexionDB.ObtenerConexion())
            {
                string query = @"UPDATE usuarios SET
                                    nombre_usuario = @nombreUsuario,
                                    id_rol = @idRol,
                                    activo = @activo
                                  WHERE id_usuario = @id";

                using (var comando = new SqlCommand(query, conexion))
                {
                    comando.Parameters.AddWithValue("@nombreUsuario", usuario.NombreUsuario);
                    comando.Parameters.AddWithValue("@idRol", usuario.IdRol);
                    comando.Parameters.AddWithValue("@activo", usuario.Activo);
                    comando.Parameters.AddWithValue("@id", usuario.IdUsuario);
                    conexion.Open();
                    return comando.ExecuteNonQuery() > 0;
                }
            }
        }

        // Cambiar la clave de un usuario. Siempre genera un salt NUEVO
        // (no se reutiliza el anterior), buena práctica al rotar contraseñas.
        public bool CambiarClave(int idUsuario, string nuevaClaveSinHash)
        {
            string nuevoSalt = GenerarSalt();
            string nuevoHash = HashearClave(nuevaClaveSinHash, nuevoSalt);
            return ActualizarHashYSalt(idUsuario, nuevoHash, nuevoSalt);
        }

        // Eliminar un usuario por Id
        public bool Eliminar(int idUsuario)
        {
            using (var conexion = ConexionDB.ObtenerConexion())
            {
                string query = "DELETE FROM usuarios WHERE id_usuario = @id";
                using (var comando = new SqlCommand(query, conexion))
                {
                    comando.Parameters.AddWithValue("@id", idUsuario);
                    conexion.Open();
                    return comando.ExecuteNonQuery() > 0;
                }
            }
        }

        // Verificar si ya existe ese nombre de usuario (para validar antes de insertar)
        public bool ExisteNombreUsuario(string nombreUsuario, int idUsuarioExcluir = 0)
        {
            using (var conexion = ConexionDB.ObtenerConexion())
            {
                string query = "SELECT COUNT(1) FROM usuarios WHERE nombre_usuario = @nombreUsuario AND id_usuario <> @idExcluir";
                using (var comando = new SqlCommand(query, conexion))
                {
                    comando.Parameters.AddWithValue("@nombreUsuario", nombreUsuario);
                    comando.Parameters.AddWithValue("@idExcluir", idUsuarioExcluir);
                    conexion.Open();
                    int count = (int)comando.ExecuteScalar();
                    return count > 0;
                }
            }
        }

        // ---------- Métodos privados de apoyo ----------

        // ---- Manejo del bloqueo por intentos fallidos ----

        // Suma 1 al contador de intentos fallidos del usuario y devuelve el
        // valor YA actualizado. Se usa OUTPUT en vez de leer-luego-escribir
        // para que el incremento sea atómico a nivel de base de datos.
        private int IncrementarIntentosFallidos(int idUsuario)
        {
            using (var conexion = ConexionDB.ObtenerConexion())
            {
                string query = @"UPDATE usuarios
                                  SET intentos_fallidos = intentos_fallidos + 1
                                  OUTPUT INSERTED.intentos_fallidos
                                  WHERE id_usuario = @id";
                using (var comando = new SqlCommand(query, conexion))
                {
                    comando.Parameters.AddWithValue("@id", idUsuario);
                    conexion.Open();
                    return (int)comando.ExecuteScalar();
                }
            }
        }

        // Marca la cuenta como bloqueada hasta la fecha/hora indicada, y
        // reinicia el contador de intentos (para que al desbloquearse
        // arranque de nuevo en 0, no siga sumando desde el límite).
        private bool BloquearUsuario(int idUsuario, DateTime bloqueadoHasta)
        {
            using (var conexion = ConexionDB.ObtenerConexion())
            {
                string query = @"UPDATE usuarios
                                  SET bloqueado_hasta = @bloqueadoHasta, intentos_fallidos = 0
                                  WHERE id_usuario = @id";
                using (var comando = new SqlCommand(query, conexion))
                {
                    comando.Parameters.AddWithValue("@bloqueadoHasta", bloqueadoHasta);
                    comando.Parameters.AddWithValue("@id", idUsuario);
                    conexion.Open();
                    return comando.ExecuteNonQuery() > 0;
                }
            }
        }

        // Quita el bloqueo de la cuenta (bloqueado_hasta = NULL). Se llama
        // automáticamente cuando el bloqueo ya venció, y también podría
        // usarse a futuro desde una pantalla de administración para
        // desbloquear una cuenta manualmente antes de tiempo.
        private bool DesbloquearUsuario(int idUsuario)
        {
            using (var conexion = ConexionDB.ObtenerConexion())
            {
                string query = "UPDATE usuarios SET bloqueado_hasta = NULL, intentos_fallidos = 0 WHERE id_usuario = @id";
                using (var comando = new SqlCommand(query, conexion))
                {
                    comando.Parameters.AddWithValue("@id", idUsuario);
                    conexion.Open();
                    return comando.ExecuteNonQuery() > 0;
                }
            }
        }

        // Resetea el contador de intentos fallidos a 0 (sin tocar el bloqueo).
        // Se usa tras un login exitoso.
        private bool ResetearIntentosFallidos(int idUsuario)
        {
            using (var conexion = ConexionDB.ObtenerConexion())
            {
                string query = "UPDATE usuarios SET intentos_fallidos = 0 WHERE id_usuario = @id";
                using (var comando = new SqlCommand(query, conexion))
                {
                    comando.Parameters.AddWithValue("@id", idUsuario);
                    conexion.Open();
                    return comando.ExecuteNonQuery() > 0;
                }
            }
        }

        // Guarda un hash+salt nuevos para un usuario (usado por CambiarClave
        // y por la migración automática dentro de ValidarCredenciales).
        private bool ActualizarHashYSalt(int idUsuario, string nuevoHash, string nuevoSalt)
        {
            using (var conexion = ConexionDB.ObtenerConexion())
            {
                string query = "UPDATE usuarios SET clave = @clave, clave_salt = @claveSalt WHERE id_usuario = @id";
                using (var comando = new SqlCommand(query, conexion))
                {
                    comando.Parameters.AddWithValue("@clave", nuevoHash);
                    comando.Parameters.AddWithValue("@claveSalt", nuevoSalt);
                    comando.Parameters.AddWithValue("@id", idUsuario);
                    conexion.Open();
                    return comando.ExecuteNonQuery() > 0;
                }
            }
        }

        // Genera un salt aleatorio criptográficamente seguro y lo devuelve en Base64
        // (listo para guardar en la columna clave_salt, que es NVARCHAR).
        private string GenerarSalt()
        {
            byte[] saltBytes = new byte[SALT_BYTES];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(saltBytes);
            }
            return Convert.ToBase64String(saltBytes);
        }

        // Deriva el hash de una clave usando PBKDF2 (Rfc2898DeriveBytes) con el
        // salt indicado. Devuelve el resultado en Base64.
        private string HashearClave(string claveSinHash, string saltBase64)
        {
            byte[] saltBytes = Convert.FromBase64String(saltBase64);

            using (var pbkdf2 = new Rfc2898DeriveBytes(claveSinHash, saltBytes, PBKDF2_ITERACIONES, HashAlgorithmName.SHA256))
            {
                byte[] hashBytes = pbkdf2.GetBytes(HASH_BYTES);
                return Convert.ToBase64String(hashBytes);
            }
        }

        // Hash SHA256 "viejo" (sin salt), tal como se guardaban las claves
        // antes de esta mejora. Se mantiene SOLO para poder validar el login
        // de usuarios que aún no se han migrado (ver ValidarCredenciales).
        // No se usa para nada nuevo: todo lo nuevo pasa por HashearClave().
        private string HashearClaveLegacySinSalt(string claveSinHash)
        {
            using (var sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(claveSinHash));
                var sb = new System.Text.StringBuilder();
                foreach (byte b in bytes)
                {
                    sb.Append(b.ToString("x2"));
                }
                return sb.ToString();
            }
        }

        private Usuario MapearUsuario(SqlDataReader reader)
        {
            int ordSalt = reader.GetOrdinal("clave_salt");
            int ordBloqueadoHasta = reader.GetOrdinal("bloqueado_hasta");

            return new Usuario
            {
                IdUsuario = reader.GetInt32(reader.GetOrdinal("id_usuario")),
                NombreUsuario = reader.GetString(reader.GetOrdinal("nombre_usuario")),
                Clave = reader.GetString(reader.GetOrdinal("clave")),
                ClaveSalt = reader.IsDBNull(ordSalt) ? null : reader.GetString(ordSalt),
                IdRol = reader.GetInt32(reader.GetOrdinal("id_rol")),
                Activo = reader.GetBoolean(reader.GetOrdinal("activo")),
                NombreRol = reader.GetString(reader.GetOrdinal("nombre_rol")),
                IntentosFallidos = reader.GetInt32(reader.GetOrdinal("intentos_fallidos")),
                BloqueadoHasta = reader.IsDBNull(ordBloqueadoHasta) ? (DateTime?)null : reader.GetDateTime(ordBloqueadoHasta)
            };
        }
    }
}