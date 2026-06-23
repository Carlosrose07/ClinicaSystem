using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using ClinicaSystem.Models;

namespace ClinicaSystem.Data
{
    public class UsuarioRepository
    {
        // Query base reutilizada: trae el nombre del rol via JOIN
        private const string SelectBase = @"
            SELECT u.id_usuario, u.nombre_usuario, u.clave, u.id_rol, u.activo, r.nombre_rol
            FROM usuarios u
            INNER JOIN roles r ON u.id_rol = r.id_rol";

        // Validar credenciales de login. Devuelve el Usuario si son correctas, null si no.
        public Usuario ValidarCredenciales(string nombreUsuario, string claveSinHash)
        {
            Usuario usuario = null;
            string claveHasheada = HashearClave(claveSinHash);

            using (var conexion = ConexionDB.ObtenerConexion())
            {
                string query = SelectBase + " WHERE u.nombre_usuario = @nombreUsuario AND u.clave = @clave AND u.activo = 1";
                using (var comando = new SqlCommand(query, conexion))
                {
                    comando.Parameters.AddWithValue("@nombreUsuario", nombreUsuario);
                    comando.Parameters.AddWithValue("@clave", claveHasheada);
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

        // Insertar un nuevo usuario. Recibe la clave SIN hashear y la hashea antes de guardar.
        public int Insertar(Usuario usuario, string claveSinHash)
        {
            using (var conexion = ConexionDB.ObtenerConexion())
            {
                string query = @"INSERT INTO usuarios (nombre_usuario, clave, id_rol, activo)
                                  OUTPUT INSERTED.id_usuario
                                  VALUES (@nombreUsuario, @clave, @idRol, @activo)";

                using (var comando = new SqlCommand(query, conexion))
                {
                    comando.Parameters.AddWithValue("@nombreUsuario", usuario.NombreUsuario);
                    comando.Parameters.AddWithValue("@clave", HashearClave(claveSinHash));
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

        // Cambiar la clave de un usuario
        public bool CambiarClave(int idUsuario, string nuevaClaveSinHash)
        {
            using (var conexion = ConexionDB.ObtenerConexion())
            {
                string query = "UPDATE usuarios SET clave = @clave WHERE id_usuario = @id";
                using (var comando = new SqlCommand(query, conexion))
                {
                    comando.Parameters.AddWithValue("@clave", HashearClave(nuevaClaveSinHash));
                    comando.Parameters.AddWithValue("@id", idUsuario);
                    conexion.Open();
                    return comando.ExecuteNonQuery() > 0;
                }
            }
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

        // Convierte una clave en texto plano a su hash SHA256 (en hexadecimal)
        private string HashearClave(string claveSinHash)
        {
            using (var sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(claveSinHash));
                var sb = new StringBuilder();
                foreach (byte b in bytes)
                {
                    sb.Append(b.ToString("x2"));
                }
                return sb.ToString();
            }
        }

        private Usuario MapearUsuario(SqlDataReader reader)
        {
            return new Usuario
            {
                IdUsuario = reader.GetInt32(reader.GetOrdinal("id_usuario")),
                NombreUsuario = reader.GetString(reader.GetOrdinal("nombre_usuario")),
                Clave = reader.GetString(reader.GetOrdinal("clave")),
                IdRol = reader.GetInt32(reader.GetOrdinal("id_rol")),
                Activo = reader.GetBoolean(reader.GetOrdinal("activo")),
                NombreRol = reader.GetString(reader.GetOrdinal("nombre_rol"))
            };
        }
    }
}