using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using ClinicaSystem.Models;

namespace ClinicaSystem.Data
{
    public class CitaRepository
    {
        // Query base reutilizada en varios métodos: trae nombre de paciente y médico via JOIN
        private const string SelectBase = @"
            SELECT c.id_cita, c.id_paciente, c.id_medico, c.fecha, c.hora, c.estado, c.observaciones,
                   p.nombre AS nombre_paciente, m.nombre AS nombre_medico
            FROM citas c
            INNER JOIN pacientes p ON c.id_paciente = p.id_paciente
            INNER JOIN medicos m ON c.id_medico = m.id_medico";

        // Obtener todas las citas
        public List<Cita> ObtenerTodas()
        {
            var lista = new List<Cita>();

            using (var conexion = ConexionDB.ObtenerConexion())
            {
                string query = SelectBase + " ORDER BY c.fecha DESC, c.hora DESC";
                using (var comando = new SqlCommand(query, conexion))
                {
                    conexion.Open();
                    using (var reader = comando.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(MapearCita(reader));
                        }
                    }
                }
            }

            return lista;
        }

        // Obtener una cita por su Id
        public Cita ObtenerPorId(int idCita)
        {
            Cita cita = null;

            using (var conexion = ConexionDB.ObtenerConexion())
            {
                string query = SelectBase + " WHERE c.id_cita = @id";
                using (var comando = new SqlCommand(query, conexion))
                {
                    comando.Parameters.AddWithValue("@id", idCita);
                    conexion.Open();
                    using (var reader = comando.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            cita = MapearCita(reader);
                        }
                    }
                }
            }

            return cita;
        }

        // Obtener todas las citas de un paciente (para su historial)
        public List<Cita> ObtenerPorPaciente(int idPaciente)
        {
            var lista = new List<Cita>();

            using (var conexion = ConexionDB.ObtenerConexion())
            {
                string query = SelectBase + " WHERE c.id_paciente = @idPaciente ORDER BY c.fecha DESC, c.hora DESC";
                using (var comando = new SqlCommand(query, conexion))
                {
                    comando.Parameters.AddWithValue("@idPaciente", idPaciente);
                    conexion.Open();
                    using (var reader = comando.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(MapearCita(reader));
                        }
                    }
                }
            }

            return lista;
        }

        // Obtener todas las citas de un médico en una fecha (para ver su agenda del día)
        public List<Cita> ObtenerPorMedicoYFecha(int idMedico, DateTime fecha)
        {
            var lista = new List<Cita>();

            using (var conexion = ConexionDB.ObtenerConexion())
            {
                string query = SelectBase + " WHERE c.id_medico = @idMedico AND c.fecha = @fecha ORDER BY c.hora";
                using (var comando = new SqlCommand(query, conexion))
                {
                    comando.Parameters.AddWithValue("@idMedico", idMedico);
                    comando.Parameters.AddWithValue("@fecha", fecha.Date);
                    conexion.Open();
                    using (var reader = comando.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(MapearCita(reader));
                        }
                    }
                }
            }

            return lista;
        }

        // Verifica si el médico ya tiene una cita en esa fecha/hora (para no duplicar el horario)
        // idCitaExcluir se usa al editar, para que la cita no choque consigo misma
        public bool ExisteConflictoHorario(int idMedico, DateTime fecha, TimeSpan hora, int idCitaExcluir = 0)
        {
            using (var conexion = ConexionDB.ObtenerConexion())
            {
                string query = @"SELECT COUNT(1) FROM citas
                                  WHERE id_medico = @idMedico
                                    AND fecha = @fecha
                                    AND hora = @hora
                                    AND estado <> 'Cancelada'
                                    AND id_cita <> @idExcluir";
                using (var comando = new SqlCommand(query, conexion))
                {
                    comando.Parameters.AddWithValue("@idMedico", idMedico);
                    comando.Parameters.AddWithValue("@fecha", fecha.Date);
                    comando.Parameters.AddWithValue("@hora", hora);
                    comando.Parameters.AddWithValue("@idExcluir", idCitaExcluir);
                    conexion.Open();
                    int count = (int)comando.ExecuteScalar();
                    return count > 0;
                }
            }
        }

        // Insertar una nueva cita. Devuelve el Id generado.
        public int Insertar(Cita cita)
        {
            using (var conexion = ConexionDB.ObtenerConexion())
            {
                string query = @"INSERT INTO citas (id_paciente, id_medico, fecha, hora, estado, observaciones)
                                  OUTPUT INSERTED.id_cita
                                  VALUES (@idPaciente, @idMedico, @fecha, @hora, @estado, @observaciones)";

                using (var comando = new SqlCommand(query, conexion))
                {
                    AgregarParametros(comando, cita);
                    conexion.Open();
                    return (int)comando.ExecuteScalar();
                }
            }
        }

        // Actualizar una cita existente
        public bool Actualizar(Cita cita)
        {
            using (var conexion = ConexionDB.ObtenerConexion())
            {
                string query = @"UPDATE citas SET
                                    id_paciente = @idPaciente,
                                    id_medico = @idMedico,
                                    fecha = @fecha,
                                    hora = @hora,
                                    estado = @estado,
                                    observaciones = @observaciones
                                  WHERE id_cita = @id";

                using (var comando = new SqlCommand(query, conexion))
                {
                    AgregarParametros(comando, cita);
                    comando.Parameters.AddWithValue("@id", cita.IdCita);
                    conexion.Open();
                    return comando.ExecuteNonQuery() > 0;
                }
            }
        }

        // Cambiar solo el estado de una cita (ej. Pendiente -> Confirmada -> Completada/Cancelada)
        public bool CambiarEstado(int idCita, string nuevoEstado)
        {
            using (var conexion = ConexionDB.ObtenerConexion())
            {
                string query = "UPDATE citas SET estado = @estado WHERE id_cita = @id";
                using (var comando = new SqlCommand(query, conexion))
                {
                    comando.Parameters.AddWithValue("@estado", nuevoEstado);
                    comando.Parameters.AddWithValue("@id", idCita);
                    conexion.Open();
                    return comando.ExecuteNonQuery() > 0;
                }
            }
        }

        // Eliminar una cita por Id
        public bool Eliminar(int idCita)
        {
            using (var conexion = ConexionDB.ObtenerConexion())
            {
                string query = "DELETE FROM citas WHERE id_cita = @id";
                using (var comando = new SqlCommand(query, conexion))
                {
                    comando.Parameters.AddWithValue("@id", idCita);
                    conexion.Open();
                    return comando.ExecuteNonQuery() > 0;
                }
            }
        }

        // ---------- Métodos privados de apoyo ----------

        private void AgregarParametros(SqlCommand comando, Cita cita)
        {
            comando.Parameters.AddWithValue("@idPaciente", cita.IdPaciente);
            comando.Parameters.AddWithValue("@idMedico", cita.IdMedico);
            comando.Parameters.AddWithValue("@fecha", cita.Fecha.Date);
            comando.Parameters.AddWithValue("@hora", cita.Hora);
            comando.Parameters.AddWithValue("@estado", cita.Estado);
            comando.Parameters.AddWithValue("@observaciones", string.IsNullOrEmpty(cita.Observaciones) ? (object)DBNull.Value : cita.Observaciones);
        }

        private Cita MapearCita(SqlDataReader reader)
        {
            return new Cita
            {
                IdCita = reader.GetInt32(reader.GetOrdinal("id_cita")),
                IdPaciente = reader.GetInt32(reader.GetOrdinal("id_paciente")),
                IdMedico = reader.GetInt32(reader.GetOrdinal("id_medico")),
                Fecha = reader.GetDateTime(reader.GetOrdinal("fecha")),
                Hora = reader.GetTimeSpan(reader.GetOrdinal("hora")),
                Estado = reader.GetString(reader.GetOrdinal("estado")),
                Observaciones = reader.IsDBNull(reader.GetOrdinal("observaciones")) ? null : reader.GetString(reader.GetOrdinal("observaciones")),
                NombrePaciente = reader.GetString(reader.GetOrdinal("nombre_paciente")),
                NombreMedico = reader.GetString(reader.GetOrdinal("nombre_medico"))
            };
        }
    }
}