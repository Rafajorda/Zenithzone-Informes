using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Informes
{
    /// <summary>
    /// Repositorio para obtener datos de Socios para informes
    /// Se conecta directamente a la base de datos sin pasar por Entity Framework
    /// Retorna DataTables tipados para uso con Crystal Reports
    /// </summary>
    public class SocioRepository
    {
        private readonly string _connectionString =
      @"Data Source=DESKTOP-H7T9KDB\SQLEXPRESS;
      Initial Catalog=zenithzone;
      Integrated Security=True;
      Encrypt=False;
      MultipleActiveResultSets=True;";

        /// <summary>
        /// Obtiene todos los socios de la base de datos en formato DataTable para Crystal Reports
        /// </summary>
        /// <returns>DataTable tipado con los datos de socios</returns>
        public DsSocios.SociosDataTable ObtenerSocios()
        {
            var tabla = new DsSocios.SociosDataTable();

            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand(
                "SELECT Id, Nombre, Email, Activo FROM Socio", conn))
            {
                conn.Open();

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var fila = tabla.NewSociosRow();

                        fila.Id = reader.GetInt32(0);
                        fila.Nombre = reader.GetString(1);
                        fila.Email = reader.GetString(2);
                        fila.Activo = reader.GetBoolean(3);

                        tabla.Rows.Add(fila);
                    }
                }
            }

            return tabla;
        }
    }
}
