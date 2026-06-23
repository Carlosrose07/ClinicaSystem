using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using ClinicaSystem.Models;

namespace ClinicaSystem.Data
{
    public class PacienteRepository
    {
        // Obtener todos los pacientes
        public List<Paciente> ObtenerTodos()
        {
            var lista = new List<Paciente>();

            using (var conexion = ConexionDB.ObtenerConexion())
            {
                string query = "SELECT id_paciente, nombre, cedula, fecha_nacimiento, telefono, direccion, tipo_sangre, fecha_registro FROM pacientes ORDER BY nombre";
                using (var comando = new SqlCommand(query, conexion))
                {
                    conexion.Open();
                    using (var reader = comando.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(MapearPaciente(reader));
                        }
                    }
                }
            }

            return lista;
        }

        // Obtener un paciente por su Id
        public Paciente ObtenerPorId(int idPaciente)
        {
            Paciente paciente = null;

            using (var conexion = ConexionDB.ObtenerConexion())
            {
                string query = "SELECT id_paciente, nombre, cedula, fecha_nacimiento, telefono, direccion, tipo_sangre, fecha_registro FROM pacientes WHERE id_paciente = @id";
                using (var comando = new SqlCommand(query, conexion))
                {
                    comando.Parameters.AddWithValue("@id", idPaciente);
                    conexion.Open();
                    using (var reader = comando.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            paciente = MapearPaciente(reader);
                        }
                    }
                }
            }

            return paciente;
        }

        // Insertar un nuevo paciente. Devuelve el Id generado.
        public int Insertar(Paciente paciente)
        {
            using (var conexion = ConexionDB.ObtenerConexion())
            {
                string query = @"INSERT INTO pacientes (nombre, cedula, fecha_nacimiento, telefono, direccion, tipo_sangre)
                                  OUTPUT INSERTED.id_paciente
                                  VALUES (@nombre, @cedula, @fechaNacimiento, @telefono, @direccion, @tipoSangre)";

                using (var comando = new SqlCommand(query, conexion))
                {
                    AgregarParametros(comando, paciente);
                    conexion.Open();
                    return (int)comando.ExecuteScalar();
                }
            }
        }

        // Actualizar un paciente existente
        public bool Actualizar(Paciente paciente)
        {
            using (var conexion = ConexionDB.ObtenerConexion())
            {
                string query = @"UPDATE pacientes SET
                                    nombre = @nombre,
                                    cedula = @cedula,
                                    fecha_nacimiento = @fechaNacimiento,
                                    telefono = @telefono,
                                    direccion = @direccion,
                                    tipo_sangre = @tipoSangre
                                  WHERE id_paciente = @id";

                using (var comando = new SqlCommand(query, conexion))
                {
                    AgregarParametros(comando, paciente);
                    comando.Parameters.AddWithValue("@id", paciente.IdPaciente);
                    conexion.Open();
                    return comando.ExecuteNonQuery() > 0;
                }
            }
        }

        // Eliminar un paciente por Id
        public bool Eliminar(int idPaciente)
        {
            using (var conexion = ConexionDB.ObtenerConexion())
            {
                string query = "DELETE FROM pacientes WHERE id_paciente = @id";
                using (var comando = new SqlCommand(query, conexion))
                {
                    comando.Parameters.AddWithValue("@id", idPaciente);
                    conexion.Open();
                    return comando.ExecuteNonQuery() > 0;
                }
            }
        }

        // Verificar si ya existe un paciente con esa cédula (para validar antes de insertar)
        public bool ExisteCedula(string cedula, int idPacienteExcluir = 0)
        {
            using (var conexion = ConexionDB.ObtenerConexion())
            {
                string query = "SELECT COUNT(1) FROM pacientes WHERE cedula = @cedula AND id_paciente <> @idExcluir";
                using (var comando = new SqlCommand(query, conexion))
                {
                    comando.Parameters.AddWithValue("@cedula", cedula);
                    comando.Parameters.AddWithValue("@idExcluir", idPacienteExcluir);
                    conexion.Open();
                    int count = (int)comando.ExecuteScalar();
                    return count > 0;
                }
            }
        }

        // ---------- Métodos privados de apoyo ----------

        private void AgregarParametros(SqlCommand comando, Paciente paciente)
        {
            comando.Parameters.AddWithValue("@nombre", paciente.Nombre);
            comando.Parameters.AddWithValue("@cedula", paciente.Cedula);
            comando.Parameters.AddWithValue("@fechaNacimiento", paciente.FechaNacimiento);
            comando.Parameters.AddWithValue("@telefono", string.IsNullOrEmpty(paciente.Telefono) ? (object)DBNull.Value : paciente.Telefono);
            comando.Parameters.AddWithValue("@direccion", string.IsNullOrEmpty(paciente.Direccion) ? (object)DBNull.Value : paciente.Direccion);
            comando.Parameters.AddWithValue("@tipoSangre", string.IsNullOrEmpty(paciente.TipoSangre) ? (object)DBNull.Value : paciente.TipoSangre);
        }

        private Paciente MapearPaciente(SqlDataReader reader)
        {
            return new Paciente
            {
                IdPaciente = reader.GetInt32(reader.GetOrdinal("id_paciente")),
                Nombre = reader.GetString(reader.GetOrdinal("nombre")),
                Cedula = reader.GetString(reader.GetOrdinal("cedula")),
                FechaNacimiento = reader.GetDateTime(reader.GetOrdinal("fecha_nacimiento")),
                Telefono = reader.IsDBNull(reader.GetOrdinal("telefono")) ? null : reader.GetString(reader.GetOrdinal("telefono")),
                Direccion = reader.IsDBNull(reader.GetOrdinal("direccion")) ? null : reader.GetString(reader.GetOrdinal("direccion")),
                TipoSangre = reader.IsDBNull(reader.GetOrdinal("tipo_sangre")) ? null : reader.GetString(reader.GetOrdinal("tipo_sangre")),
                FechaRegistro = reader.GetDateTime(reader.GetOrdinal("fecha_registro"))
            };
        }
    }
}