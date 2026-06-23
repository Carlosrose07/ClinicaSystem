using System.Data.SqlClient;

namespace ClinicaSystem.Data
{
    public class ConexionDB
    {
        
        private static readonly string cadenaConexion =
            @"Server=DESKTOP-P6M7LS4\SQLEXPRESS;Database=ClinicaDB;Integrated Security=True;TrustServerCertificate=True;";

        public static SqlConnection ObtenerConexion()
        {
            return new SqlConnection(cadenaConexion);
        }
    }
}