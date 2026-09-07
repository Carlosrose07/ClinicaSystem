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
            string query = "SELECT id_medico, nombre, cedula, especialidad, telefono, turno, fecha_registro, id_usuario FROM medicos ORDER BY nombre";

            return DbHelper.EjecutarConsulta(query, null, MapearMedico);
        }

        // Obtener un médico por su Id
        public Medico ObtenerPorId(int idMedico)
        {
            string query = "SELECT id_medico, nombre, cedula, especialidad, telefono, turno, fecha_registro, id_usuario FROM medicos WHERE id_medico = @id";

            var parametros = new Dictionary<string, object>
            {
                { "@id", idMedico }
            };

            return DbHelper.EjecutarConsultaUnica(query, parametros, MapearMedico);
        }

        // Obtener el médico vinculado a un usuario específico.
        // Devuelve null si ese usuario no tiene médico asociado
        // (por ejemplo, si es un usuario con rol Medico creado antes de este cambio).
        public Medico ObtenerPorIdUsuario(int idUsuario)
        {
            string query = "SELECT id_medico, nombre, cedula, especialidad, telefono, turno, fecha_registro, id_usuario FROM medicos WHERE id_usuario = @idUsuario";

            var parametros = new Dictionary<string, object>
            {
                { "@idUsuario", idUsuario }
            };

            return DbHelper.EjecutarConsultaUnica(query, parametros, MapearMedico);
        }

        // Insertar un nuevo médico. Devuelve el Id generado.
        public int Insertar(Medico medico)
        {
            string query = @"INSERT INTO medicos (nombre, cedula, especialidad, telefono, turno, id_usuario)
                              OUTPUT INSERTED.id_medico
                              VALUES (@nombre, @cedula, @especialidad, @telefono, @turno, @idUsuario)";

            var parametros = ConstruirParametros(medico);

            return DbHelper.EjecutarEscalar<int>(query, parametros);
        }

        // Actualizar un médico existente
        public bool Actualizar(Medico medico)
        {
            string query = @"UPDATE medicos SET
                                nombre = @nombre,
                                cedula = @cedula,
                                especialidad = @especialidad,
                                telefono = @telefono,
                                turno = @turno,
                                id_usuario = @idUsuario
                              WHERE id_medico = @id";

            var parametros = ConstruirParametros(medico);
            parametros["@id"] = medico.IdMedico;

            return DbHelper.EjecutarNonQuery(query, parametros) > 0;
        }

        // Eliminar un médico por Id
        public bool Eliminar(int idMedico)
        {
            string query = "DELETE FROM medicos WHERE id_medico = @id";

            var parametros = new Dictionary<string, object>
            {
                { "@id", idMedico }
            };

            // Si el médico tiene citas asignadas, SQL Server rechaza el DELETE
            // por la FK (error 547). DbHelper ya traduce eso a un mensaje en
            // español dentro de DatosException; el Form solo necesita un
            // catch (DatosException ex) para mostrarlo sin lógica adicional.
            return DbHelper.EjecutarNonQuery(query, parametros) > 0;
        }

        // Verificar si ya existe un médico con esa cédula (para validar antes de insertar)
        public bool ExisteCedula(string cedula, int idMedicoExcluir = 0)
        {
            string query = "SELECT COUNT(1) FROM medicos WHERE cedula = @cedula AND id_medico <> @idExcluir";

            var parametros = new Dictionary<string, object>
            {
                { "@cedula", cedula },
                { "@idExcluir", idMedicoExcluir }
            };

            return DbHelper.EjecutarEscalar<int>(query, parametros) > 0;
        }

        // ---------- Métodos privados de apoyo ----------

        // Arma el diccionario de parámetros compartido entre Insertar y Actualizar.
        private Dictionary<string, object> ConstruirParametros(Medico medico)
        {
            return new Dictionary<string, object>
            {
                { "@nombre", medico.Nombre },
                { "@cedula", medico.Cedula },
                { "@especialidad", medico.Especialidad },
                { "@telefono", string.IsNullOrEmpty(medico.Telefono) ? null : medico.Telefono },
                { "@turno", medico.Turno },
                // IdUsuario es int? (nullable): un médico puede no tener cuenta
                // de usuario vinculada todavía. Si HasValue es false, se guarda
                // como DBNull vía el "?? DBNull.Value" que ya aplica DbHelper
                // internamente al recibir null en el diccionario.
                { "@idUsuario", medico.IdUsuario.HasValue ? (object)medico.IdUsuario.Value : null }
            };
        }

        // Convierte una fila del reader en un objeto Medico. Se pasa como
        // delegado a DbHelper.EjecutarConsulta / EjecutarConsultaUnica.
        private Medico MapearMedico(SqlDataReader reader)
        {
            int ordinalIdUsuario = reader.GetOrdinal("id_usuario");

            return new Medico
            {
                IdMedico = reader.GetInt32(reader.GetOrdinal("id_medico")),
                Nombre = reader.GetString(reader.GetOrdinal("nombre")),
                Cedula = reader.GetString(reader.GetOrdinal("cedula")),
                Especialidad = reader.GetString(reader.GetOrdinal("especialidad")),
                Telefono = reader.IsDBNull(reader.GetOrdinal("telefono")) ? null : reader.GetString(reader.GetOrdinal("telefono")),
                Turno = reader.GetString(reader.GetOrdinal("turno")),
                FechaRegistro = reader.GetDateTime(reader.GetOrdinal("fecha_registro")),
                IdUsuario = reader.IsDBNull(ordinalIdUsuario) ? (int?)null : reader.GetInt32(ordinalIdUsuario)
            };
        }
    }
}