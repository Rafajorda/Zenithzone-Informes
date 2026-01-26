using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;

namespace Models
{
    /// <summary>
    /// Repositorio para gestionar las operaciones CRUD de Actividades
    /// Implementa el patrón Repository para abstraer el acceso a datos
    /// </summary>
    public class ActividadRepository
    {
        /// <summary>
        /// Obtiene todas las actividades de la base de datos
        /// </summary>
        /// <returns>Lista de todas las actividades</returns>
        public List<Actividad> GetAll()
        {
            using (var context = new zenithzoneEntities())
            {
                return context.Actividad.ToList();
            }
        }

        /// <summary>
        /// Obtiene una actividad específica por su ID
        /// </summary>
        /// <param name="id">Identificador único de la actividad</param>
        /// <returns>La actividad encontrada o null si no existe</returns>
        public Actividad GetById(int id)
        {
            using (var context = new zenithzoneEntities())
            {
                return context.Actividad.Find(id);
            }
        }

        /// <summary>
        /// Agrega una nueva actividad a la base de datos
        /// </summary>
        /// <param name="actividad">Objeto Actividad a agregar</param>
        public void Add(Actividad actividad)
        {
            using (var context = new zenithzoneEntities())
            {
                context.Actividad.Add(actividad);
                var result = context.SaveChanges();
                
                // Log para debugging
                System.Diagnostics.Debug.WriteLine($"Actividad agregada con ID: {actividad.Id}, Rows affected: {result}");
            }
        }

        /// <summary>
        /// Actualiza una actividad existente en la base de datos
        /// </summary>
        /// <param name="actividad">Objeto Actividad con los datos actualizados</param>
        public void Update(Actividad actividad)
        {
            using (var context = new zenithzoneEntities())
            {
                var existing = context.Actividad.Find(actividad.Id);
                if (existing != null)
                {
                    // Actualizar solo las propiedades modificables
                    existing.Nombre = actividad.Nombre;
                    existing.AforoMaximo = actividad.AforoMaximo;
                    context.SaveChanges();
                }
            }
        }

        /// <summary>
        /// Verifica si una actividad tiene reservas asociadas
        /// </summary>
        /// <param name="id">Identificador único de la actividad</param>
        /// <returns>True si la actividad tiene reservas, False en caso contrario</returns>
        public bool TieneReservas(int id)
        {
            using (var context = new zenithzoneEntities())
            {
                return context.Reserva.Any(r => r.ActividadId == id);
            }
        }

        /// <summary>
        /// Elimina una actividad de la base de datos
        /// Valida que la actividad no tenga reservas asociadas antes de eliminar
        /// </summary>
        /// <param name="id">Identificador único de la actividad a eliminar</param>
        /// <returns>Tupla con (éxito, mensaje): true y mensaje vacío si se eliminó correctamente, false y mensaje de error si no se pudo eliminar</returns>
        public (bool exito, string mensaje) Delete(int id)
        {
            using (var context = new zenithzoneEntities())
            {
                var actividad = context.Actividad.Find(id);
                if (actividad == null)
                {
                    return (false, "La actividad no existe.");
                }

                // Verificar si tiene reservas
                if (TieneReservas(id))
                {
                    return (false, "No se puede eliminar la actividad porque tiene reservas asociadas. Primero debe eliminar las reservas.");
                }

                context.Actividad.Remove(actividad);
                context.SaveChanges();
                return (true, string.Empty);
            }
        }
    }
}
