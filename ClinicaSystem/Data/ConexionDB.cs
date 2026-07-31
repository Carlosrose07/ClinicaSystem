using System;
using System.Configuration;
using System.Data.SqlClient;

namespace ClinicaSystem.Data
{
    /// <summary>
    /// Clase encargada de proveer la conexión a la base de datos SQL Server.
    /// Obtiene la cadena de conexión centralizada desde el App.config mediante la clave 'ClinicaDB'.
    /// </summary>
    public class ConexionDB
    {
        // La cadena de conexión ya NO está fija en el código: se lee desde
        // App.config (sección <connectionStrings>, entrada "ClinicaDB").
        // Esto permite que el mismo .exe funcione en otra PC con solo editar
        // el App.config (o el ClinicaSystem.exe.config generado al compilar),
        // sin tener que tocar y recompilar el proyecto.
        public static SqlConnection ObtenerConexion()
        {
            var elementoConexion = ConfigurationManager.ConnectionStrings["ClinicaDB"];

            if (elementoConexion == null || string.IsNullOrWhiteSpace(elementoConexion.ConnectionString))
            {
                throw new InvalidOperationException(
                    "No se encontró la cadena de conexión llamada 'ClinicaDB' en el archivo App.config. " +
                    "Asegúrate de que la etiqueta <add name=\"ClinicaDB\" ... /> esté definida dentro de <connectionStrings>.");
            }

            return new SqlConnection(elementoConexion.ConnectionString);
        }
    }
}