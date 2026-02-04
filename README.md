# ZenithZone Informes

Resumen corto
Proyecto de gestión y generación de informes para actividades, socios y reservas. Sigue arquitectura MVVM con Entity Framework 6 para acceso a datos y un proyecto de tests para validación automática.

Requisitos
- Visual Studio 2022 (o superior) con .NET Framework 4.8
- SQL Server accesible con la base de datos `zenithzone` (o configurar cadena de conexión)


Cómo compilar y ejecutar
1. Abrir la solución en Visual Studio.
2. Restaurar paquetes NuGet (Restore).
3. Compilar la solución.
4. Ejecutar desde Visual Studio o ejecutar los proyectos deseados.

Estructura del repositorio (resumen)
- `Models/` — Entidades EF (`Socio`, `Reserva`, `Actividad`) y repositorios (`ReservaRepository`, `ActividadRepository`, `SocioRepository`).
- `ViewModel/` — ViewModels (por ejemplo `ReservasViewModel`, `SociosViewModel`), `BaseViewModel`, `RelayCommand`.
- `Views/` — Vistas XAML y recursos UI.
- `Informes/` — DataSets tipados y repositorios orientados a la generación de informes.
- `ZenithZone.Test/` — Pruebas unitarias / de integración.
- `documentacion/` — Documentación técnica y recursos 

Arquitectura (resumen)
- Patrón MVVM: Views ↔ ViewModels ↔ Models/Repositories → EF → DB.
- Diagrama de capas:

<img width="2076" height="1350" alt="mermaid-diagram-2026-02-04-191604" src="https://github.com/user-attachments/assets/aa5378ee-bf9f-493a-904b-a8c21a3e228f" />


Componentes clave
- Models: representan el dominio y encapsulan acceso a datos. Repositorios implementan CRUD y validaciones que requieren consultas (por ejemplo `ValidarReserva`, `ValidarAforoDisponible`).
- ViewModels: exponen propiedades observables (`ObservableCollection<T>`), implementan `INotifyPropertyChanged` y comandos (`ICommand`). Contienen validaciones y orquestan llamadas a repositorios.
- Commands: `RelayCommand` (acción + `CanExecute`) usado para enlazar botones/acciones desde la UI.
- Acceso a datos: Entity Framework 6 con el contexto `zenithzoneEntities`. Cadenas de conexión en `App.config` del ensamblado que ejecuta las pruebas/ejecutable.

Diagrama de la base de datos

<img width="2004" height="1254" alt="mermaid-diagram-2026-02-04-194725" src="https://github.com/user-attachments/assets/14a72215-ada9-4253-97bf-c841ae52d57c" />


Contacto / autor
- Proyecto: `ZenithZone Informes`
- Repo: https://github.com/Rafajorda/Zenithzone-Informes
