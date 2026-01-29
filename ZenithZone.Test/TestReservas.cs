using Models;
using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ViewModel;

namespace ZenithZone.Test;

[TestClass]
public class TestReservas
{


    /// <summary>
    ///  Validar que no se puedan hacer reservas para fechas pasadas.
    /// </summary>
    [TestMethod]
    public void Validacion_FechaReserva()
    {
        var repo = new ReservaRepository();
        var ayer = DateTime.Today.AddDays(-1);

        // La validación en el repositorio debe rechazar fechas pasadas
        Assert.IsFalse(repo.ValidarReserva(1, 1, ayer));
    }


    /// <summary>
    /// Validar que no se pueda superar el aforo máximo de una actividad.
    /// </summary>

    [TestMethod]
    public void Control_Aforo_Maximo()
    {
        int actividadId;
        int socioId;

        // Buscar la actividad "test" existente (case-insensitive). Si no existe, marcar como inconcluso.
        using (var entity = new zenithzoneEntities())
        {
            var actividad = entity.Actividad
                              .AsEnumerable()
                              .FirstOrDefault(a => string.Equals(a.Nombre, "test", StringComparison.OrdinalIgnoreCase));
            if (actividad == null)
            {
                Assert.Inconclusive("No se encontró la actividad de prueba con Nombre = 'test'.");
                return;
            }

            actividadId = actividad.Id;

            // Crear un socio de prueba (se eliminará en finally)
            var socio = new Socio { Nombre = "Test Socio", Email = "test@test.local", Activo = true };
            entity.Socio.Add(socio);
            entity.SaveChanges();
            socioId = socio.Id;
        }

        try
        {
            var repo = new ReservaRepository();

            // Primera reserva debe poder añadirse
            var reserva1 = new Reserva { SocioId = socioId, ActividadId = actividadId, Fecha = DateTime.Today };
            repo.Add(reserva1);

            // Comprobar que ya no hay aforo disponible para la misma actividad y fecha
            bool hayAforo = repo.ValidarAforoDisponible(actividadId, DateTime.Today);
            Assert.IsFalse(hayAforo);
        }
        finally
        {
            // Limpiar datos creados por el test: eliminar reservas del socio creado y el socio
            using (var entity = new zenithzoneEntities())
            {
                var reservas = entity.Reserva.Where(r => r.ActividadId == actividadId && r.SocioId == socioId).ToList();
                foreach (var r in reservas) entity.Reserva.Remove(r);

                var soc = entity.Socio.Find(socioId);
                if (soc != null) entity.Socio.Remove(soc);

                entity.SaveChanges();
            }
        }
    }
}
