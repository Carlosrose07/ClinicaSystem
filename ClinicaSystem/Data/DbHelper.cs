using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace ClinicaSystem.Data
{
    // =======================================================================
    // Helper genérico de acceso a datos.
    //
    // Objetivo: eliminar la duplicación de
    //     using (var conexion = ConexionDB.ObtenerConexion())
    //     using (var comando = new SqlCommand(query, conexion))
    //     {
    //         comando.Parameters.AddWithValue(...);
    //         conexion.Open();
    //         ... ExecuteReader / ExecuteNonQuery / ExecuteScalar ...
    //     }
    // que antes se repetía casi textual en cada método de cada Repository.
    //
    // De paso, centraliza el manejo de SqlException: en vez de que cada
    // Repository (o peor, cada Form) reciba el mensaje técnico crudo de
    // SQL Server, acá se traducen los códigos de error más comunes a
    // mensajes en español entendibles, y se relanzan como DatosException.
    // Así, en los Forms alcanza con un solo catch (DatosException ex) para
    // mostrar un MessageBox amigable, sin importar qué método falló.
    // =======================================================================
    public static class DbHelper
    {
        // ---------- SELECT que devuelve una LISTA de resultados ----------
        // mapeador: función que convierte UNA fila del reader en un objeto T
        // (normalmente el método privado MapearXxx de cada Repository).
        public static List<T> EjecutarConsulta<T>(string query, Dictionary<string, object> parametros, Func<SqlDataReader, T> mapeador)
        {
            var lista = new List<T>();

            try
            {
                using (var conexion = ConexionDB.ObtenerConexion())
                using (var comando = new SqlCommand(query, conexion))
                {
                    AgregarParametros(comando, parametros);
                    conexion.Open();

                    using (var reader = comando.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(mapeador(reader));
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                throw TraducirExcepcion(ex);
            }

            return lista;
        }

        // ---------- SELECT que devuelve UN SOLO resultado (o default/null) ----------
        public static T EjecutarConsultaUnica<T>(string query, Dictionary<string, object> parametros, Func<SqlDataReader, T> mapeador)
        {
            try
            {
                using (var conexion = ConexionDB.ObtenerConexion())
                using (var comando = new SqlCommand(query, conexion))
                {
                    AgregarParametros(comando, parametros);
                    conexion.Open();

                    using (var reader = comando.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return mapeador(reader);
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                throw TraducirExcepcion(ex);
            }

            return default(T);
        }

        // ---------- INSERT / UPDATE / DELETE sin valor de retorno de datos ----------
        // Devuelve la cantidad de filas afectadas (ExecuteNonQuery).
        public static int EjecutarNonQuery(string query, Dictionary<string, object> parametros)
        {
            try
            {
                using (var conexion = ConexionDB.ObtenerConexion())
                using (var comando = new SqlCommand(query, conexion))
                {
                    AgregarParametros(comando, parametros);
                    conexion.Open();
                    return comando.ExecuteNonQuery();
                }
            }
            catch (SqlException ex)
            {
                throw TraducirExcepcion(ex);
            }
        }

        // ---------- Consultas que devuelven un único valor escalar ----------
        // (COUNT(1), OUTPUT INSERTED.id_usuario, etc.)
        public static T EjecutarEscalar<T>(string query, Dictionary<string, object> parametros)
        {
            try
            {
                using (var conexion = ConexionDB.ObtenerConexion())
                using (var comando = new SqlCommand(query, conexion))
                {
                    AgregarParametros(comando, parametros);
                    conexion.Open();

                    object resultado = comando.ExecuteScalar();
                    if (resultado == null || resultado == DBNull.Value)
                        return default(T);

                    return (T)Convert.ChangeType(resultado, typeof(T));
                }
            }
            catch (SqlException ex)
            {
                throw TraducirExcepcion(ex);
            }
        }

        // ---------- Soporte interno ----------

        private static void AgregarParametros(SqlCommand comando, Dictionary<string, object> parametros)
        {
            if (parametros == null) return;

            foreach (var kvp in parametros)
            {
                // ?? DBNull.Value: por si en algún momento se pasa un valor
                // null real (ej. un campo opcional), ADO.NET lo rechaza si
                // no se convierte explícitamente a DBNull.
                comando.Parameters.AddWithValue(kvp.Key, kvp.Value ?? DBNull.Value);
            }
        }

        // Traduce los códigos de error más comunes de SQL Server a mensajes
        // en español entendibles para el usuario final. La lista cubre los
        // que realmente pueden pasar en este sistema; cualquier otro cae
        // en un mensaje genérico.
        private static DatosException TraducirExcepcion(SqlException ex)
        {
            string mensajeAmigable;

            switch (ex.Number)
            {
                case 547:
                    // Violación de FK: borrar/modificar un registro que
                    // todavía está referenciado por otra tabla
                    // (ej. borrar un médico que tiene citas asignadas).
                    mensajeAmigable = "No se puede completar la operación porque este registro está " +
                                       "relacionado con otros datos del sistema (por ejemplo, citas o historiales asociados).";
                    break;

                case 2627:
                case 2601:
                    // Violación de restricción UNIQUE/PK: valor duplicado
                    // (ej. nombre de usuario que ya existe).
                    mensajeAmigable = "Ya existe un registro con ese mismo valor único (por ejemplo, el nombre de usuario).";
                    break;

                case -2:
                    // Timeout de conexión/ejecución.
                    mensajeAmigable = "La operación tardó demasiado en responder. Verifica tu conexión e intenta de nuevo.";
                    break;

                case 53:
                case 4060:
                case 18456:
                    // No se pudo establecer conexión / login fallido a nivel de motor.
                    mensajeAmigable = "No se pudo conectar a la base de datos. Verifica que el servidor SQL esté disponible.";
                    break;

                default:
                    mensajeAmigable = "Ocurrió un error al comunicarse con la base de datos. Intenta de nuevo o contacta al administrador.";
                    break;
            }

            return new DatosException(mensajeAmigable, ex);
        }
    }
}