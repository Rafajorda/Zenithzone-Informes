using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;

namespace Models
{
    /// <summary>
    /// Repositorio para gestionar las operaciones CRUD de Socios
    /// Implementa el patrón Repository para abstraer el acceso a datos
    /// </summary>
    public class SocioRepository
    {
        /// <summary>
        /// Obtiene todos los socios de la base de datos
        /// </summary>
        /// <returns>Lista de todos los socios</returns>
        public List<Socio> GetAll()
        {
            using (var context = new zenithzoneEntities())
            {
                return context.Socio.ToList();
            }
        }

        /// <summary>
        /// Obtiene un socio específico por su ID
        /// </summary>
        /// <param name="id">Identificador único del socio</param>
        /// <returns>El socio encontrado o null si no existe</returns>
        public Socio GetById(int id)
        {
            using (var context = new zenithzoneEntities())
            {
                return context.Socio.Find(id);
            }
        }

        /// <summary>
        /// Agrega un nuevo socio a la base de datos
        /// </summary>
        /// <param name="socio">Objeto Socio a agregar</param>
        public void Add(Socio socio)
        {
            using (var context = new zenithzoneEntities())
            {
                context.Socio.Add(socio);
                var result = context.SaveChanges();
                
                // Log para debugging
                System.Diagnostics.Debug.WriteLine($"Socio agregado con ID: {socio.Id}, Rows affected: {result}");
            }
        }

        /// <summary>
        /// Actualiza un socio existente en la base de datos
        /// </summary>
        /// <param name="socio">Objeto Socio con los datos actualizados</param>
        public void Update(Socio socio)
        {
            using (var context = new zenithzoneEntities())
            {
                var existing = context.Socio.Find(socio.Id);
                if (existing != null)
                {
                    // Actualizar solo las propiedades modificables
                    existing.Nombre = socio.Nombre;
                    existing.Email = socio.Email;
                    existing.Activo = socio.Activo;
                    context.SaveChanges();
                }
            }
        }

        /// <summary>
        /// Verifica si un socio tiene reservas asociadas
        /// </summary>
        /// <param name="id">Identificador único del socio</param>
        /// <returns>True si el socio tiene reservas, False en caso contrario</returns>
        public bool TieneReservas(int id)
        {
            using (var context = new zenithzoneEntities())
            {
                return context.Reserva.Any(r => r.SocioId == id);
            }
        }

        /// <summary>
        /// Elimina un socio de la base de datos
        /// Valida que el socio no tenga reservas asociadas antes de eliminar
        /// </summary>
        /// <param name="id">Identificador único del socio a eliminar</param>
        /// <returns>Tupla con (éxito, mensaje): true y mensaje vacío si se eliminó correctamente, false y mensaje de error si no se pudo eliminar</returns>
        public (bool exito, string mensaje) Delete(int id)
        {
            using (var context = new zenithzoneEntities())
            {
                var socio = context.Socio.Find(id);
                if (socio == null)
                {
                    return (false, "El socio no existe.");
                }

                // Verificar si tiene reservas
                if (TieneReservas(id))
                {
                    return (false, "No se puede eliminar el socio porque tiene reservas asociadas. Primero debe eliminar o reasignar las reservas.");
                }

                context.Socio.Remove(socio);
                context.SaveChanges();
                return (true, string.Empty);
            }
        }
    }
}
