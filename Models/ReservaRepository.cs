using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;

namespace Models
{
    /// <summary>
    /// Repositorio para gestionar las operaciones CRUD de Reservas
    /// Implementa el patrón Repository para abstraer el acceso a datos
    /// Incluye validaciones de negocio y consultas relacionadas
    /// </summary>
    public class ReservaRepository
    {
        /// <summary>
        /// Obtiene todas las reservas con sus relaciones (Socio y Actividad) cargadas
        /// </summary>
        /// <returns>Lista de todas las reservas con navegación eager loading</returns>
        public List<Reserva> GetAll()
        {
            using (var context = new zenithzoneEntities())
            {
                return context.Reserva.Include(r => r.Socio).Include(r => r.Actividad).ToList();
            }
        }

        /// <summary>
        /// Obtiene una reserva específica por su ID con sus relaciones cargadas
        /// </summary>
        /// <param name="id">Identificador único de la reserva</param>
        /// <returns>La reserva encontrada o null si no existe</returns>
        public Reserva GetById(int id)
        {
            using (var context = new zenithzoneEntities())
            {
                return context.Reserva.Include(r => r.Socio).Include(r => r.Actividad).FirstOrDefault(r => r.Id == id);
            }
        }

        /// <summary>
        /// Valida si un socio puede hacer una reserva en una fecha específica
        /// Regla de negocio: Un socio solo puede tener una reserva por día
        /// </summary>
        /// <param name="socioId">ID del socio</param>
        /// <param name="actividadId">ID de la actividad</param>
        /// <param name="fecha">Fecha de la reserva</param>
        /// <param name="reservaIdExcluir">ID de reserva a excluir (para validar en edición)</param>
        /// <returns>True si la reserva es válida, False si ya existe una reserva para ese socio en esa fecha</returns>
        public bool ValidarReserva(int socioId, int actividadId, DateTime fecha, int? reservaIdExcluir = null)
        {
            using (var context = new zenithzoneEntities())
            {
                // Truncar la hora para comparar solo fechas
                var fechaSoloFecha = fecha.Date;
                
                var query = context.Reserva.Where(r => 
                    r.SocioId == socioId && 
                    DbFunctions.TruncateTime(r.Fecha) == fechaSoloFecha);

                // Si estamos editando, excluir la reserva actual
                if (reservaIdExcluir.HasValue)
                {
                    query = query.Where(r => r.Id != reservaIdExcluir.Value);
                }

                return !query.Any();
            }
        }

        /// <summary>
        /// Valida si hay aforo disponible para una actividad en una fecha específica
        /// Regla de negocio: No se pueden hacer más reservas que el aforo máximo de la actividad por día
        /// </summary>
        /// <param name="actividadId">ID de la actividad</param>
        /// <param name="fecha">Fecha de la reserva</param>
        /// <param name="reservaIdExcluir">ID de reserva a excluir (para validar en edición)</param>
        /// <returns>True si hay aforo disponible, False si se alcanzó el aforo máximo</returns>
        public bool ValidarAforoDisponible(int actividadId, DateTime fecha, int? reservaIdExcluir = null)
        {
            using (var context = new zenithzoneEntities())
            {
                // Obtener la actividad y su aforo máximo
                var actividad = context.Actividad.Find(actividadId);
                if (actividad == null)
                {
                    return false;
                }

                // Truncar la hora para comparar solo fechas
                var fechaSoloFecha = fecha.Date;
                
                // Contar reservas para esta actividad en esta fecha
                var queryReservas = context.Reserva.Where(r => 
                    r.ActividadId == actividadId && 
                    DbFunctions.TruncateTime(r.Fecha) == fechaSoloFecha);

                // Si estamos editando, excluir la reserva actual del conteo
                if (reservaIdExcluir.HasValue)
                {
                    queryReservas = queryReservas.Where(r => r.Id != reservaIdExcluir.Value);
                }

                int reservasActuales = queryReservas.Count();
                
                // Verificar si hay espacio disponible
                return reservasActuales < actividad.AforoMaximo;
            }
        }

        /// <summary>
        /// Agrega una nueva reserva a la base de datos
        /// </summary>
        /// <param name="reserva">Objeto Reserva a agregar</param>
        public void Add(Reserva reserva)
        {
            using (var context = new zenithzoneEntities())
            {
                context.Reserva.Add(reserva);
                var result = context.SaveChanges();
                
                // Log para debugging
                System.Diagnostics.Debug.WriteLine($"Reserva agregada con ID: {reserva.Id}, Rows affected: {result}");
            }
        }

        /// <summary>
        /// Actualiza una reserva existente en la base de datos
        /// </summary>
        /// <param name="reserva">Objeto Reserva con los datos actualizados</param>
        public void Update(Reserva reserva)
        {
            using (var context = new zenithzoneEntities())
            {
                var existing = context.Reserva.Find(reserva.Id);
                if (existing != null)
                {
                    // Actualizar las propiedades modificables
                    existing.SocioId = reserva.SocioId;
                    existing.ActividadId = reserva.ActividadId;
                    existing.Fecha = reserva.Fecha;
                    context.SaveChanges();
                }
            }
        }

        /// <summary>
        /// Elimina una reserva de la base de datos
        /// </summary>
        /// <param name="id">Identificador único de la reserva a eliminar</param>
        public void Delete(int id)
        {
            using (var context = new zenithzoneEntities())
            {
                var reserva = context.Reserva.Find(id);
                if (reserva != null)
                {
                    context.Reserva.Remove(reserva);
                    context.SaveChanges();
                }
            }
        }

        /// <summary>
        /// Obtiene la lista de socios activos para mostrar en el ComboBox
        /// </summary>
        /// <returns>Lista de socios con estado Activo = true</returns>
        public List<Socio> GetSociosActivos()
        {
            using (var context = new zenithzoneEntities())
            {
                return context.Socio.Where(s => s.Activo).ToList();
            }
        }

        /// <summary>
        /// Obtiene todas las actividades disponibles para mostrar en el ComboBox
        /// </summary>
        /// <returns>Lista de todas las actividades</returns>
        public List<Actividad> GetAllActividades()
        {
            using (var context = new zenithzoneEntities())
            {
                return context.Actividad.ToList();
            }
        }
    }
}
