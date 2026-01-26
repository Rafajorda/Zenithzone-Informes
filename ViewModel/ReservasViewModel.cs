using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Models;

namespace ViewModel
{
    /// <summary>
    /// ViewModel para la gestión de Reservas
    /// Maneja la lógica de negocio compleja incluyendo validaciones de reglas de negocio
    /// Gestiona las relaciones entre Reservas, Socios y Actividades
    /// Implementa operaciones CRUD completas con validaciones
    /// </summary>
    public class ReservasViewModel : BaseViewModel
    {
        private readonly ReservaRepository _repository;

        private ObservableCollection<Reserva> _reservas;
        
        /// <summary>
        /// Colección observable de todas las reservas con sus relaciones cargadas
        /// Se actualiza automáticamente en la vista cuando cambia
        /// </summary>
        public ObservableCollection<Reserva> Reservas
        {
            get => _reservas;
            set
            {
                _reservas = value;
                OnPropertyChanged(nameof(Reservas));
            }
        }

        private Reserva _selectedReserva;
        
        /// <summary>
        /// Reserva actualmente seleccionada en el DataGrid
        /// Controla el estado del formulario y los comandos disponibles
        /// </summary>
        public Reserva SelectedReserva
        {
            get => _selectedReserva;
            set
            {
                _selectedReserva = value;
                OnPropertyChanged(nameof(SelectedReserva));
                OnPropertyChanged(nameof(IsEditingExistingReserva));
                OnPropertyChanged(nameof(IsFormEnabled));
                
                // Forzar reevaluación de los comandos
                System.Windows.Input.CommandManager.InvalidateRequerySuggested();
            }
        }

        private ObservableCollection<Socio> _socios;
        
        /// <summary>
        /// Colección de socios activos para el ComboBox
        /// Solo incluye socios con estado Activo = true
        /// </summary>
        public ObservableCollection<Socio> Socios
        {
            get => _socios;
            set
            {
                _socios = value;
                OnPropertyChanged(nameof(Socios));
            }
        }

        private ObservableCollection<Actividad> _actividades;
        
        /// <summary>
        /// Colección de actividades disponibles para el ComboBox
        /// </summary>
        public ObservableCollection<Actividad> Actividades
        {
            get => _actividades;
            set
            {
                _actividades = value;
                OnPropertyChanged(nameof(Actividades));
            }
        }

        /// <summary>
        /// Indica si se está editando una reserva existente (Id != 0)
        /// </summary>
        public bool IsEditingExistingReserva => SelectedReserva != null && SelectedReserva.Id != 0;
        
        /// <summary>
        /// Indica si el formulario debe estar habilitado
        /// </summary>
        public bool IsFormEnabled => SelectedReserva != null;

        // Comandos para las operaciones CRUD
        public ICommand AgregarCommand { get; }
        public ICommand EditarCommand { get; }
        public ICommand GuardarCommand { get; }
        public ICommand EliminarCommand { get; }
        public ICommand CancelarCommand { get; }

        /// <summary>
        /// Constructor del ReservasViewModel
        /// Inicializa el repositorio, los comandos y carga todos los datos necesarios
        /// </summary>
        public ReservasViewModel()
        {
            _repository = new ReservaRepository();
            Reservas = new ObservableCollection<Reserva>();
            Socios = new ObservableCollection<Socio>();
            Actividades = new ObservableCollection<Actividad>();

            // Inicializar comandos con sus respectivas acciones y condiciones
            AgregarCommand = new RelayCommand(Agregar);
            GuardarCommand = new RelayCommand(Guardar, () => SelectedReserva != null);
            EditarCommand = new RelayCommand(Editar, () => SelectedReserva != null);
            EliminarCommand = new RelayCommand(Eliminar, () => SelectedReserva != null);
            CancelarCommand = new RelayCommand(Cancelar);

            // Cargar todos los datos iniciales
            CargarDatos();
        }

        /// <summary>
        /// Carga todos los datos necesarios: reservas, socios activos y actividades
        /// </summary>
        private void CargarDatos()
        {
            CargarReservas();
            CargarSocios();
            CargarActividades();
        }

        /// <summary>
        /// Carga todas las reservas desde la base de datos con sus relaciones
        /// </summary>
        private void CargarReservas()
        {
            Reservas.Clear();
            var reservas = _repository.GetAll();
            foreach (var reserva in reservas)
            {
                Reservas.Add(reserva);
            }
        }

        /// <summary>
        /// Carga solo los socios activos para el ComboBox de selección
        /// </summary>
        private void CargarSocios()
        {
            Socios.Clear();
            var socios = _repository.GetSociosActivos();
            foreach (var socio in socios)
            {
                Socios.Add(socio);
            }
        }

        /// <summary>
        /// Carga todas las actividades disponibles para el ComboBox de selección
        /// </summary>
        private void CargarActividades()
        {
            Actividades.Clear();
            var actividades = _repository.GetAllActividades();
            foreach (var actividad in actividades)
            {
                Actividades.Add(actividad);
            }
        }

        /// <summary>
        /// Crea una nueva reserva vacía y la prepara para edición
        /// Establece la fecha actual como fecha predeterminada
        /// </summary>
        private void Agregar()
        {
            var nuevaReserva = new Reserva
            {
                SocioId = 0,
                ActividadId = 0,
                Fecha = DateTime.Today
            };
            Reservas.Add(nuevaReserva);
            SelectedReserva = nuevaReserva;
        }

        /// <summary>
        /// Guarda la reserva actual (nueva o modificada) en la base de datos
        /// Realiza múltiples validaciones:
        /// - Validación de campos obligatorios (socio y actividad)
        /// - Validación de regla de negocio: un socio solo puede tener una reserva por día
        /// - Validación de aforo: no se puede superar el aforo máximo de la actividad por día
        /// </summary>
        private void Guardar()
        {
            if (SelectedReserva == null) return;

            // Validación: socio obligatorio
            if (SelectedReserva.SocioId == 0)
            {
                System.Windows.MessageBox.Show("Debe seleccionar un socio.", "Validación", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
                return;
            }

            // Validación: actividad obligatoria
            if (SelectedReserva.ActividadId == 0)
            {
                System.Windows.MessageBox.Show("Debe seleccionar una actividad.", "Validación", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
                return;
            }

            // Validación de regla de negocio: un socio solo puede tener una reserva por día
            // Si estamos editando, excluir la reserva actual de la validación
            int? reservaIdExcluir = SelectedReserva.Id == 0 ? (int?)null : SelectedReserva.Id;
            if (!_repository.ValidarReserva(SelectedReserva.SocioId, SelectedReserva.ActividadId, SelectedReserva.Fecha, reservaIdExcluir))
            {
                System.Windows.MessageBox.Show(
                    "El socio ya tiene una reserva para este día. No se permite más de una reserva por día.", 
                    "Validación", 
                    System.Windows.MessageBoxButton.OK, 
                    System.Windows.MessageBoxImage.Warning);
                return;
            }

            // Validación de aforo: verificar que no se supere el aforo máximo de la actividad
            if (!_repository.ValidarAforoDisponible(SelectedReserva.ActividadId, SelectedReserva.Fecha, reservaIdExcluir))
            {
                // Obtener información de la actividad para mostrar en el mensaje
                var actividad = Actividades.FirstOrDefault(a => a.Id == SelectedReserva.ActividadId);
                string nombreActividad = actividad?.Nombre ?? "la actividad seleccionada";
                int aforoMaximo = actividad?.AforoMaximo ?? 0;
                
                System.Windows.MessageBox.Show(
                    $"No hay aforo disponible para {nombreActividad} en la fecha seleccionada.\n" +
                    $"Aforo máximo: {aforoMaximo} persona(s) por día.\n" +
                    $"El aforo ya está completo para este día.",
                    "Aforo completo", 
                    System.Windows.MessageBoxButton.OK, 
                    System.Windows.MessageBoxImage.Warning);
                return;
            }

            try
            {
                // Determinar si es inserción o actualización según el ID
                if (SelectedReserva.Id == 0)
                {
                    _repository.Add(SelectedReserva);
                }
                else
                {
                    _repository.Update(SelectedReserva);
                }
                
                // Recargar y limpiar selección
                CargarReservas();
                SelectedReserva = null;
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Error al guardar: {ex.Message}", "Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Prepara la reserva seleccionada para edición
        /// En este caso no realiza ninguna acción adicional ya que la edición es directa en el formulario
        /// </summary>
        private void Editar()
        {
            // La edición se realiza directamente en el formulario vinculado
        }

        /// <summary>
        /// Elimina la reserva seleccionada de la base de datos
        /// Solicita confirmación antes de eliminar
        /// </summary>
        private void Eliminar()
        {
            if (SelectedReserva == null) return;

            // Solicitar confirmación al usuario
            var resultado = System.Windows.MessageBox.Show(
                "¿Está seguro que desea eliminar esta reserva?", 
                "Confirmar eliminación", 
                System.Windows.MessageBoxButton.YesNo, 
                System.Windows.MessageBoxImage.Question);

            if (resultado == System.Windows.MessageBoxResult.Yes)
            {
                _repository.Delete(SelectedReserva.Id);
                CargarReservas();
                SelectedReserva = null;
            }
        }

        /// <summary>
        /// Cancela la operación actual y recarga los datos originales
        /// Descarta cualquier cambio no guardado
        /// </summary>
        private void Cancelar()
        {
            SelectedReserva = null;
            CargarReservas();
        }
    }
}
