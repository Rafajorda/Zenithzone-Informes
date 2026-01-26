using System;
using System.Data;
using System.Data.SqlClient;

namespace Informes
{
    /// <summary>
    /// Repositorio para obtener reservas  (para informes)
    /// Devuelve DsReservas.ReservassDataTable y rellena filas usando NewReservasRow
    /// </summary>
    public class ReservaRepository
    {
        private readonly string _connectionString =
      @"Data Source=DESKTOP-H7T9KDB\SQLEXPRESS;
      Initial Catalog=zenithzone;
      Integrated Security=True;
      Encrypt=False;
      MultipleActiveResultSets=True;";

        /// <summary>
        /// Obtiene las reservas.
        /// </summary>
        public DsReservas.ReservasDataTable ObtenerReservas()
        {
            var tabla = new DsReservas.ReservasDataTable();

            const string sql = @"
            SELECT
                s.Id          AS IdSocio,
                s.Nombre      AS Socio,
                a.Nombre      AS Actividad,
                r.Fecha       AS FechaReserva
            FROM Reserva r
            INNER JOIN Socio s
                ON r.SocioId = s.Id
            INNER JOIN Actividad a
                ON r.ActividadId = a.Id
            ORDER BY
                s.Nombre,
                r.Fecha ASC;";

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                using (var cmd = new SqlCommand(sql, conn))
                {
                    
                    conn.Open();

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var fila = tabla.NewReservasRow();


                            fila.IdSocio = reader.GetInt32(0);
                            fila.Socio = reader.GetString(1);
                            fila.Actividad = reader.GetString(2);
                            fila.FechaReserva = reader.GetDateTime(3);
                            tabla.Rows.Add(fila);
                        }
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                throw new InvalidOperationException($"Error SQL al obtener actividades Reservas: {sqlEx.Message}", sqlEx);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error al obtener  reservas {ex.Message}", ex);
            }

            return tabla;
        }
    }
}
