using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using ClinicaSystem.Models;

namespace ClinicaSystem.Data
{
    public class MedicoRepository
    {
        // Obtener todos los médicos
        public List<Medico> ObtenerTodos()
        {
            var lista = new List<Medico>();

            using (var conexion = ConexionDB.ObtenerConexion())
            {
                string query = "SELECT id_medico, nombre, cedula, especialidad, telefono, turno, fecha_registro FROM medicos ORDER BY nombre";
                using (var comando = new SqlCommand(query, conexion))
                {
                    conexion.Open();
                    using (var reader = comando.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(MapearMedico(reader));
                        }
                    }
                }
            }

            return lista;
        }

        // Obtener un médico por su Id
        public Medico ObtenerPorId(int idMedico)
        {
            Medico medico = null;

            using (var conexion = ConexionDB.ObtenerConexion())
            {
                string query = "SELECT id_medico, nombre, cedula, especialidad, telefono, turno, fecha_registro FROM medicos WHERE id_medico = @id";
                using (var comando = new SqlCommand(query, conexion))
                {
                    comando.Parameters.AddWithValue("@id", idMedico);
                    conexion.Open();
                    using (var reader = comando.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            medico = MapearMedico(reader);
                        }
                    }
                }
            }

            return medico;
        }

        // Insertar un nuevo médico. Devuelve el Id generado.
        public int Insertar(Medico medico)
        {
            using (var conexion = ConexionDB.ObtenerConexion())
            {
                string query = @"INSERT INTO medicos (nombre, cedula, especialidad, telefono, turno)
                                  OUTPUT INSERTED.id_medico
                                  VALUES (@nombre, @cedula, @especialidad, @telefono, @turno)";

                using (var comando = new SqlCommand(query, conexion))
                {
                    AgregarParametros(comando, medico);
                    conexion.Open();
                    return (int)comando.ExecuteScalar();
                }
            }
        }

        // Actualizar un médico existente
        public bool Actualizar(Medico medico)
        {
            using (var conexion = ConexionDB.ObtenerConexion())
            {
                string query = @"UPDATE medicos SET
                                    nombre = @nombre,
                                    cedula = @cedula,
                                    especialidad = @especialidad,
                                    telefono = @telefono,
                                    turno = @turno
                                  WHERE id_medico = @id";

                using (var comando = new SqlCommand(query, conexion))
                {
                    AgregarParametros(comando, medico);
                    comando.Parameters.AddWithValue("@id", medico.IdMedico);
                    conexion.Open();
                    return comando.ExecuteNonQuery() > 0;
                }
            }
        }

        // Eliminar un médico por Id
        public bool Eliminar(int idMedico)
        {
            using (var conexion = ConexionDB.ObtenerConexion())
            {
                string query = "DELETE FROM medicos WHERE id_medico = @id";
                using (var comando = new SqlCommand(query, conexion))
                {
                    comando.Parameters.AddWithValue("@id", idMedico);
                    conexion.Open();
                    return comando.ExecuteNonQuery() > 0;
                }
            }
        }

        // Verificar si ya existe un médico con esa cédula (para validar antes de insertar)
        public bool ExisteCedula(string cedula, int idMedicoExcluir = 0)
        {
            using (var conexion = ConexionDB.ObtenerConexion())
            {
                string query = "SELECT COUNT(1) FROM medicos WHERE cedula = @cedula AND id_medico <> @idExcluir";
                using (var comando = new SqlCommand(query, conexion))
                {
                    comando.Parameters.AddWithValue("@cedula", cedula);
                    comando.Parameters.AddWithValue("@idExcluir", idMedicoExcluir);
                    conexion.Open();
                    int count = (int)comando.ExecuteScalar();
                    return count > 0;
                }
            }
        }

        // ---------- Métodos privados de apoyo ----------

        private void AgregarParametros(SqlCommand comando, Medico medico)
        {
            comando.Parameters.AddWithValue("@nombre", medico.Nombre);
            comando.Parameters.AddWithValue("@cedula", medico.Cedula);
            comando.Parameters.AddWithValue("@especialidad", medico.Especialidad);
            comando.Parameters.AddWithValue("@telefono", string.IsNullOrEmpty(medico.Telefono) ? (object)DBNull.Value : medico.Telefono);
            comando.Parameters.AddWithValue("@turno", medico.Turno);
        }

        private Medico MapearMedico(SqlDataReader reader)
        {
            return new Medico
            {
                IdMedico = reader.GetInt32(reader.GetOrdinal("id_medico")),
                Nombre = reader.GetString(reader.GetOrdinal("nombre")),
                Cedula = reader.GetString(reader.GetOrdinal("cedula")),
                Especialidad = reader.GetString(reader.GetOrdinal("especialidad")),
                Telefono = reader.IsDBNull(reader.GetOrdinal("telefono")) ? null : reader.GetString(reader.GetOrdinal("telefono")),
                Turno = reader.GetString(reader.GetOrdinal("turno")),
                FechaRegistro = reader.GetDateTime(reader.GetOrdinal("fecha_registro"))
            };
        }
    }
}