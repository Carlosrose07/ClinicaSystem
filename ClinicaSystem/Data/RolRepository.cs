using System.Collections.Generic;
using System.Data.SqlClient;
using ClinicaSystem.Models;

namespace ClinicaSystem.Data
{
    // Repositorio de solo lectura: los roles (Administrador, Recepcionista, Medico)
    // ya están sembrados en la base de datos y no se crean/editan/eliminan desde la app.
    // Esto solo sirve para cargar el combo de roles en FrmUsuarios.
    public class RolRepository
    {
        // Obtener todos los roles disponibles
        public List<Rol> ObtenerTodos()
        {
            var lista = new List<Rol>();

            using (var conexion = ConexionDB.ObtenerConexion())
            {
                string query = "SELECT id_rol, nombre_rol FROM roles ORDER BY nombre_rol";
                using (var comando = new SqlCommand(query, conexion))
                {
                    conexion.Open();
                    using (var reader = comando.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(new Rol
                            {
                                IdRol = reader.GetInt32(reader.GetOrdinal("id_rol")),
                                NombreRol = reader.GetString(reader.GetOrdinal("nombre_rol"))
                            });
                        }
                    }
                }
            }

            return lista;
        }
    }
}