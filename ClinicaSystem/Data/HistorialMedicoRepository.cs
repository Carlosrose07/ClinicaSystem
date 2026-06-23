using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using ClinicaSystem.Models;

namespace ClinicaSystem.Data
{
    public class HistorialMedicoRepository
    {
        // Query base reutilizada en varios métodos: trae nombre de paciente y médico via JOIN
        private const string SelectBase = @"
            SELECT h.id_historial, h.id_paciente, h.id_medico, h.id_cita, h.fecha_consulta,
                   h.sintomas, h.diagnostico, h.tratamiento,
                   p.nombre AS nombre_paciente, m.nombre AS nombre_medico
            FROM historial_medico h
            INNER JOIN pacientes p ON h.id_paciente = p.id_paciente
            INNER JOIN medicos m ON h.id_medico = m.id_medico";

        // Obtener todo el historial
        public List<HistorialMedico> ObtenerTodos()
        {
            var lista = new List<HistorialMedico>();

            using (var conexion = ConexionDB.ObtenerConexion())
            {
                string query = SelectBase + " ORDER BY h.fecha_consulta DESC";
                using (var comando = new SqlCommand(query, conexion))
                {
                    conexion.Open();
                    using (var reader = comando.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(MapearHistorial(reader));
                        }
                    }
                }
            }

            return lista;
        }

        // Obtener un registro de historial por su Id
        public HistorialMedico ObtenerPorId(int idHistorial)
        {
            HistorialMedico historial = null;

            using (var conexion = ConexionDB.ObtenerConexion())
            {
                string query = SelectBase + " WHERE h.id_historial = @id";
                using (var comando = new SqlCommand(query, conexion))
                {
                    comando.Parameters.AddWithValue("@id", idHistorial);
                    conexion.Open();
                    using (var reader = comando.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            historial = MapearHistorial(reader);
                        }
                    }
                }
            }

            return historial;
        }

        // Obtener todo el historial clínico de un paciente específico (lo más usado en la práctica)
        public List<HistorialMedico> ObtenerPorPaciente(int idPaciente)
        {
            var lista = new List<HistorialMedico>();

            using (var conexion = ConexionDB.ObtenerConexion())
            {
                string query = SelectBase + " WHERE h.id_paciente = @idPaciente ORDER BY h.fecha_consulta DESC";
                using (var comando = new SqlCommand(query, conexion))
                {
                    comando.Parameters.AddWithValue("@idPaciente", idPaciente);
                    conexion.Open();
                    using (var reader = comando.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(MapearHistorial(reader));
                        }
                    }
                }
            }

            return lista;
        }

        // Insertar un nuevo registro de historial. Devuelve el Id generado.
        public int Insertar(HistorialMedico historial)
        {
            using (var conexion = ConexionDB.ObtenerConexion())
            {
                string query = @"INSERT INTO historial_medico (id_paciente, id_medico, id_cita, sintomas, diagnostico, tratamiento)
                                  OUTPUT INSERTED.id_historial
                                  VALUES (@idPaciente, @idMedico, @idCita, @sintomas, @diagnostico, @tratamiento)";

                using (var comando = new SqlCommand(query, conexion))
                {
                    AgregarParametros(comando, historial);
                    conexion.Open();
                    return (int)comando.ExecuteScalar();
                }
            }
        }

        // Actualizar un registro de historial existente
        public bool Actualizar(HistorialMedico historial)
        {
            using (var conexion = ConexionDB.ObtenerConexion())
            {
                string query = @"UPDATE historial_medico SET
                                    id_paciente = @idPaciente,
                                    id_medico = @idMedico,
                                    id_cita = @idCita,
                                    sintomas = @sintomas,
                                    diagnostico = @diagnostico,
                                    tratamiento = @tratamiento
                                  WHERE id_historial = @id";

                using (var comando = new SqlCommand(query, conexion))
                {
                    AgregarParametros(comando, historial);
                    comando.Parameters.AddWithValue("@id", historial.IdHistorial);
                    conexion.Open();
                    return comando.ExecuteNonQuery() > 0;
                }
            }
        }

        // Eliminar un registro de historial por Id
        public bool Eliminar(int idHistorial)
        {
            using (var conexion = ConexionDB.ObtenerConexion())
            {
                string query = "DELETE FROM historial_medico WHERE id_historial = @id";
                using (var comando = new SqlCommand(query, conexion))
                {
                    comando.Parameters.AddWithValue("@id", idHistorial);
                    conexion.Open();
                    return comando.ExecuteNonQuery() > 0;
                }
            }
        }

        // ---------- Métodos privados de apoyo ----------

        private void AgregarParametros(SqlCommand comando, HistorialMedico historial)
        {
            comando.Parameters.AddWithValue("@idPaciente", historial.IdPaciente);
            comando.Parameters.AddWithValue("@idMedico", historial.IdMedico);
            comando.Parameters.AddWithValue("@idCita", historial.IdCita.HasValue ? (object)historial.IdCita.Value : DBNull.Value);
            comando.Parameters.AddWithValue("@sintomas", string.IsNullOrEmpty(historial.Sintomas) ? (object)DBNull.Value : historial.Sintomas);
            comando.Parameters.AddWithValue("@diagnostico", string.IsNullOrEmpty(historial.Diagnostico) ? (object)DBNull.Value : historial.Diagnostico);
            comando.Parameters.AddWithValue("@tratamiento", string.IsNullOrEmpty(historial.Tratamiento) ? (object)DBNull.Value : historial.Tratamiento);
        }

        private HistorialMedico MapearHistorial(SqlDataReader reader)
        {
            return new HistorialMedico
            {
                IdHistorial = reader.GetInt32(reader.GetOrdinal("id_historial")),
                IdPaciente = reader.GetInt32(reader.GetOrdinal("id_paciente")),
                IdMedico = reader.GetInt32(reader.GetOrdinal("id_medico")),
                IdCita = reader.IsDBNull(reader.GetOrdinal("id_cita")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("id_cita")),
                FechaConsulta = reader.GetDateTime(reader.GetOrdinal("fecha_consulta")),
                Sintomas = reader.IsDBNull(reader.GetOrdinal("sintomas")) ? null : reader.GetString(reader.GetOrdinal("sintomas")),
                Diagnostico = reader.IsDBNull(reader.GetOrdinal("diagnostico")) ? null : reader.GetString(reader.GetOrdinal("diagnostico")),
                Tratamiento = reader.IsDBNull(reader.GetOrdinal("tratamiento")) ? null : reader.GetString(reader.GetOrdinal("tratamiento")),
                NombrePaciente = reader.GetString(reader.GetOrdinal("nombre_paciente")),
                NombreMedico = reader.GetString(reader.GetOrdinal("nombre_medico"))
            };
        }
    }
}