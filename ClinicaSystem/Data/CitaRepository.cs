using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using ClinicaSystem.Models;

namespace ClinicaSystem.Data
{
    public class CitaRepository
    {
        // Query base reutilizada en varios métodos: trae nombre de paciente y médico via JOIN.
        // Se mantiene como constante para que todos los SELECT devuelvan las mismas columnas
        // y MapearCita funcione igual en cualquiera de ellos.
        private const string SelectBase = @"
            SELECT c.id_cita, c.id_paciente, c.id_medico, c.fecha, c.hora, c.estado, c.observaciones,
                   p.nombre AS nombre_paciente, m.nombre AS nombre_medico
            FROM citas c
            INNER JOIN pacientes p ON c.id_paciente = p.id_paciente
            INNER JOIN medicos m ON c.id_medico = m.id_medico";

        // Obtener todas las citas
        public List<Cita> ObtenerTodas()
        {
            // DbHelper abre/cierra la conexión y traduce errores SQL a DatosException,
            // por eso aquí ya no hay using/Open/try-catch repetidos.
            return DbHelper.EjecutarConsulta(
                SelectBase + " ORDER BY c.fecha DESC, c.hora DESC",
                null,
                MapearCita);
        }

        // Obtener una cita por su Id (devuelve null si no existe)
        public Cita ObtenerPorId(int idCita)
        {
            return DbHelper.EjecutarConsultaUnica(
                SelectBase + " WHERE c.id_cita = @id",
                new Dictionary<string, object> { { "@id", idCita } },
                MapearCita);
        }

        // Obtener todas las citas de un paciente (para su historial)
        public List<Cita> ObtenerPorPaciente(int idPaciente)
        {
            return DbHelper.EjecutarConsulta(
                SelectBase + " WHERE c.id_paciente = @idPaciente ORDER BY c.fecha DESC, c.hora DESC",
                new Dictionary<string, object> { { "@idPaciente", idPaciente } },
                MapearCita);
        }

        // Obtener todas las citas de un médico en una fecha (para ver su agenda del día)
        public List<Cita> ObtenerPorMedicoYFecha(int idMedico, DateTime fecha)
        {
            return DbHelper.EjecutarConsulta(
                SelectBase + " WHERE c.id_medico = @idMedico AND c.fecha = @fecha ORDER BY c.hora",
                new Dictionary<string, object>
                {
                    { "@idMedico", idMedico },
                    // .Date descarta la hora: la columna fecha se compara solo por día
                    { "@fecha", fecha.Date }
                },
                MapearCita);
        }

        // Verifica si el médico ya tiene una cita en esa fecha/hora (para no duplicar el horario).
        // idCitaExcluir se usa al editar, para que la cita no choque consigo misma.
        // Las citas 'Cancelada' no cuentan: liberan el horario.
        public bool ExisteConflictoHorario(int idMedico, DateTime fecha, TimeSpan hora, int idCitaExcluir = 0)
        {
            string query = @"SELECT COUNT(1) FROM citas
                              WHERE id_medico = @idMedico
                                AND fecha = @fecha
                                AND hora = @hora
                                AND estado <> 'Cancelada'
                                AND id_cita <> @idExcluir";

            int count = DbHelper.EjecutarEscalar<int>(query, new Dictionary<string, object>
            {
                { "@idMedico", idMedico },
                { "@fecha", fecha.Date },
                { "@hora", hora },
                { "@idExcluir", idCitaExcluir }
            });

            return count > 0;
        }

        // Insertar una nueva cita. Devuelve el Id generado.
        public int Insertar(Cita cita)
        {
            // OUTPUT INSERTED devuelve el nuevo Id en una sola ida a la base,
            // por eso se usa EjecutarEscalar en lugar de EjecutarNonQuery.
            string query = @"INSERT INTO citas (id_paciente, id_medico, fecha, hora, estado, observaciones)
                              OUTPUT INSERTED.id_cita
                              VALUES (@idPaciente, @idMedico, @fecha, @hora, @estado, @observaciones)";

            return DbHelper.EjecutarEscalar<int>(query, ConstruirParametros(cita));
        }

        // Actualizar una cita existente
        public bool Actualizar(Cita cita)
        {
            string query = @"UPDATE citas SET
                                id_paciente = @idPaciente,
                                id_medico = @idMedico,
                                fecha = @fecha,
                                hora = @hora,
                                estado = @estado,
                                observaciones = @observaciones
                              WHERE id_cita = @id";

            var parametros = ConstruirParametros(cita);
            parametros.Add("@id", cita.IdCita); // solo el UPDATE necesita el Id

            return DbHelper.EjecutarNonQuery(query, parametros) > 0;
        }

        // Cambiar solo el estado de una cita (ej. Pendiente -> Confirmada -> Completada/Cancelada)
        public bool CambiarEstado(int idCita, string nuevoEstado)
        {
            return DbHelper.EjecutarNonQuery(
                "UPDATE citas SET estado = @estado WHERE id_cita = @id",
                new Dictionary<string, object>
                {
                    { "@estado", nuevoEstado },
                    { "@id", idCita }
                }) > 0;
        }

        // Eliminar una cita por Id
        public bool Eliminar(int idCita)
        {
            return DbHelper.EjecutarNonQuery(
                "DELETE FROM citas WHERE id_cita = @id",
                new Dictionary<string, object> { { "@id", idCita } }) > 0;
        }

        // ---------- Métodos privados de apoyo ----------

        // Arma el diccionario de parámetros común a Insertar y Actualizar.
        // Observaciones es opcional: si viene vacía se guarda NULL en la base (no cadena vacía).
        private Dictionary<string, object> ConstruirParametros(Cita cita)
        {
            return new Dictionary<string, object>
            {
                { "@idPaciente", cita.IdPaciente },
                { "@idMedico", cita.IdMedico },
                { "@fecha", cita.Fecha.Date },
                { "@hora", cita.Hora },
                { "@estado", cita.Estado },
                { "@observaciones", string.IsNullOrEmpty(cita.Observaciones) ? (object)DBNull.Value : cita.Observaciones }
            };
        }

        // Convierte una fila del reader en un objeto Cita. DbHelper la invoca por cada fila.
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