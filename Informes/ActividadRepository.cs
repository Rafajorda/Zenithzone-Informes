using System;
using System.Data;
using System.Data.SqlClient;

namespace Informes
{
    /// <summary>
    /// Repositorio para obtener reservas por actividad (para informes)
    /// Devuelve DsActividades.ActividadesDataTable y rellena filas usando NewActividadesRow .
    /// </summary>
    public class ActividadRepository
    {
        private readonly string _connectionString =
      @"Data Source=DESKTOP-H7T9KDB\SQLEXPRESS;
      Initial Catalog=zenithzone;
      Integrated Security=True;
      Encrypt=False;
      MultipleActiveResultSets=True;";

        /// <summary>
        /// Obtiene las reservas/actividades para una actividad concreta en formato tipado DsActividades.ActividadesDataTable.
        /// </summary>
        /// <param name="actividadId">Id de actividad (se usa como parámetro en la consulta)</param>

        public DsActividades.ActividadesDataTable ObtenerActividades(int actividadId)
        {
            var tabla = new DsActividades.ActividadesDataTable();

            const string sql = @"
                SELECT
                  a.Id           AS Id,
                  a.Nombre       AS Nombre,
                  s.Nombre       AS Socio,
                  r.Fecha        AS Fecha,
                  a.AforoMaximo  AS AforoMaximo,
                  (SELECT COUNT(*) FROM Reserva r2 WHERE r2.ActividadId = r.ActividadId AND r2.Fecha = r.Fecha) AS ReservasEnFecha
                FROM Reserva r
                INNER JOIN Actividad a ON r.ActividadId = a.Id
                INNER JOIN Socio s ON r.SocioId = s.Id
                WHERE r.ActividadId = @ActividadId
                ORDER BY r.Fecha, s.Nombre;";

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.Add(new SqlParameter("@ActividadId", SqlDbType.Int) { Value = actividadId });

                    conn.Open();

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var fila = tabla.NewActividadesRow();

                       
                            fila.Id = reader.GetInt32(0);
                            fila.Nombre = reader.GetString(1);
                            fila.Socio = reader.GetString(2);       
                            fila.Fecha = reader.GetDateTime(3);
                            fila.AforoMaximo = reader.GetInt32(4);

                            tabla.Rows.Add(fila);
                        }
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                throw new InvalidOperationException($"Error SQL al obtener actividades para Id={actividadId}: {sqlEx.Message}", sqlEx);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error al obtener actividades para Id={actividadId}: {ex.Message}", ex);
            }

            return tabla;
        }
    }
}
