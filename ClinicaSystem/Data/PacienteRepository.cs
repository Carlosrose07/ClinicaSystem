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
            string query = "SELECT id_paciente, nombre, cedula, fecha_nacimiento, telefono, direccion, tipo_sangre, fecha_registro FROM pacientes ORDER BY nombre";

            // Sin parámetros: se pasa null y DbHelper simplemente no agrega nada al comando.
            return DbHelper.EjecutarConsulta(query, null, MapearPaciente);
        }

        // Obtener un paciente por su Id
        public Paciente ObtenerPorId(int idPaciente)
        {
            string query = "SELECT id_paciente, nombre, cedula, fecha_nacimiento, telefono, direccion, tipo_sangre, fecha_registro FROM pacientes WHERE id_paciente = @id";

            var parametros = new Dictionary<string, object>
            {
                { "@id", idPaciente }
            };

            // EjecutarConsultaUnica devuelve default(T) (null, para Paciente) si no
            // hay filas, igual que el "if (reader.Read())" original.
            return DbHelper.EjecutarConsultaUnica(query, parametros, MapearPaciente);
        }

        // Insertar un nuevo paciente. Devuelve el Id generado.
        public int Insertar(Paciente paciente)
        {
            string query = @"INSERT INTO pacientes (nombre, cedula, fecha_nacimiento, telefono, direccion, tipo_sangre)
                              OUTPUT INSERTED.id_paciente
                              VALUES (@nombre, @cedula, @fechaNacimiento, @telefono, @direccion, @tipoSangre)";

            var parametros = ConstruirParametros(paciente);

            // EjecutarEscalar<int> reemplaza el (int)comando.ExecuteScalar() manual.
            // Si el OUTPUT no devolviera nada (no debería pasar con este INSERT),
            // devolvería 0 en vez de lanzar una excepción de cast como antes.
            return DbHelper.EjecutarEscalar<int>(query, parametros);
        }

        // Actualizar un paciente existente
        public bool Actualizar(Paciente paciente)
        {
            string query = @"UPDATE pacientes SET
                                nombre = @nombre,
                                cedula = @cedula,
                                fecha_nacimiento = @fechaNacimiento,
                                telefono = @telefono,
                                direccion = @direccion,
                                tipo_sangre = @tipoSangre
                              WHERE id_paciente = @id";

            var parametros = ConstruirParametros(paciente);
            parametros["@id"] = paciente.IdPaciente;

            // EjecutarNonQuery devuelve las filas afectadas; > 0 confirma que
            // el id_paciente existía y se actualizó (igual que antes).
            return DbHelper.EjecutarNonQuery(query, parametros) > 0;
        }

        // Eliminar un paciente por Id
        public bool Eliminar(int idPaciente)
        {
            string query = "DELETE FROM pacientes WHERE id_paciente = @id";

            var parametros = new Dictionary<string, object>
            {
                { "@id", idPaciente }
            };

            // Nota: si el paciente tiene citas o historial médico asociados,
            // SQL Server rechaza el DELETE por la FK (error 547). DbHelper ya
            // traduce eso a un mensaje en español dentro de DatosException,
            // así que el Form solo necesita un catch (DatosException ex).
            return DbHelper.EjecutarNonQuery(query, parametros) > 0;
        }

        // Verificar si ya existe un paciente con esa cédula (para validar antes de insertar)
        public bool ExisteCedula(string cedula, int idPacienteExcluir = 0)
        {
            string query = "SELECT COUNT(1) FROM pacientes WHERE cedula = @cedula AND id_paciente <> @idExcluir";

            var parametros = new Dictionary<string, object>
            {
                { "@cedula", cedula },
                { "@idExcluir", idPacienteExcluir }
            };

            // idPacienteExcluir = 0 por defecto: al insertar un paciente nuevo
            // (que aún no tiene id) no excluye a nadie real, ya que ningún
            // paciente existente puede tener id_paciente = 0.
            return DbHelper.EjecutarEscalar<int>(query, parametros) > 0;
        }

        // ---------- Métodos privados de apoyo ----------

        // Arma el diccionario de parámetros compartido entre Insertar y Actualizar.
        // Centralizarlo acá evita que ambos métodos se desincronicen si mañana
        // se agrega un campo nuevo al modelo Paciente.
        private Dictionary<string, object> ConstruirParametros(Paciente paciente)
        {
            return new Dictionary<string, object>
            {
                { "@nombre", paciente.Nombre },
                { "@cedula", paciente.Cedula },
                { "@fechaNacimiento", paciente.FechaNacimiento },
                // Campos opcionales: cadena vacía o null se guardan como DBNull
                // en la base para no ensuciar reportes/búsquedas con "".
                { "@telefono", string.IsNullOrEmpty(paciente.Telefono) ? null : paciente.Telefono },
                { "@direccion", string.IsNullOrEmpty(paciente.Direccion) ? null : paciente.Direccion },
                { "@tipoSangre", string.IsNullOrEmpty(paciente.TipoSangre) ? null : paciente.TipoSangre }
            };
        }

        // Convierte una fila del reader en un objeto Paciente. Se pasa como
        // delegado (Func<SqlDataReader, T>) a DbHelper.EjecutarConsulta /
        // EjecutarConsultaUnica; DbHelper no sabe nada de Paciente, solo
        // invoca este mapeador fila por fila.
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