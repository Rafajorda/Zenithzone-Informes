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
    /// ViewModel para la gestión de Actividades
    /// Maneja la lógica de negocio y la interacción entre la vista de actividades y el repositorio
    /// Implementa operaciones CRUD completas
    /// </summary>
    public class ActividadesViewModel : BaseViewModel
    {
        private readonly ActividadRepository _repository;

        private ObservableCollection<Actividad> _actividades;
        
        /// <summary>
        /// Colección observable de todas las actividades
        /// Se actualiza automáticamente en la vista cuando cambia
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

        private Actividad _selectedActividad;
        
        /// <summary>
        /// Actividad actualmente seleccionada en el DataGrid
        /// Controla el estado del formulario y los comandos disponibles
        /// </summary>
        public Actividad SelectedActividad
        {
            get => _selectedActividad;
            set
            {
                _selectedActividad = value;
                OnPropertyChanged(nameof(SelectedActividad));
                OnPropertyChanged(nameof(IsEditingExistingActividad));
                OnPropertyChanged(nameof(IsFormEnabled));
                
                // Forzar reevaluación de los comandos
                System.Windows.Input.CommandManager.InvalidateRequerySuggested();
            }
        }

        /// <summary>
        /// Indica si se está editando una actividad existente (Id != 0)
        /// </summary>
        public bool IsEditingExistingActividad => SelectedActividad != null && SelectedActividad.Id != 0;
        
        /// <summary>
        /// Indica si el formulario debe estar habilitado
        /// </summary>
        public bool IsFormEnabled => SelectedActividad != null;

        // Comandos para las operaciones CRUD
        public ICommand AgregarCommand { get; }
        public ICommand GuardarCommand { get; }
        public ICommand EditarCommand { get; }
        public ICommand EliminarCommand { get; }
        public ICommand CancelarCommand { get; }

        /// <summary>
        /// Constructor del ActividadesViewModel
        /// Inicializa el repositorio, los comandos y carga los datos iniciales
        /// </summary>
        public ActividadesViewModel()
        {
            _repository = new ActividadRepository();
            Actividades = new ObservableCollection<Actividad>();

            // Inicializar comandos con sus respectivas acciones y condiciones
            AgregarCommand = new RelayCommand(Agregar);
            GuardarCommand = new RelayCommand(Guardar, () => SelectedActividad != null);
            EditarCommand = new RelayCommand(Editar, () => SelectedActividad != null);
            EliminarCommand = new RelayCommand(Eliminar, () => SelectedActividad != null);
            CancelarCommand = new RelayCommand(Cancelar);

            // Cargar datos iniciales
            CargarActividades();
        }

        /// <summary>
        /// Carga todas las actividades desde la base de datos
        /// Limpia y recarga la colección observable
        /// </summary>
        private void CargarActividades()
        {
            Actividades.Clear();
            var actividades = _repository.GetAll();
            foreach (var actividad in actividades)
            {
                Actividades.Add(actividad);
            }
        }

        /// <summary>
        /// Crea una nueva actividad vacía y la prepara para edición
        /// </summary>
        private void Agregar()
        {
            var nuevaActividad = new Actividad
            {
                Nombre = "",
                AforoMaximo = 0
            };
            Actividades.Add(nuevaActividad);
            SelectedActividad = nuevaActividad;
        }

        /// <summary>
        /// Guarda la actividad actual (nueva o modificada) en la base de datos
        /// Realiza validaciones antes de guardar
        /// </summary>
        private void Guardar()
        {
            if (SelectedActividad == null) return;

            // Validación: nombre obligatorio
            if (string.IsNullOrWhiteSpace(SelectedActividad.Nombre))
            {
                System.Windows.MessageBox.Show("El nombre es obligatorio.", "Validación", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
                return;
            }

            // Validación: aforo mayor a 0
            if (SelectedActividad.AforoMaximo <= 0)
            {
                System.Windows.MessageBox.Show("El aforo máximo debe ser mayor a 0.", "Validación", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
                return;
            }

            try
            {
                // Determinar si es inserción o actualización según el ID
                if (SelectedActividad.Id == 0)
                {
                    _repository.Add(SelectedActividad);
                }
                else
                {
                    _repository.Update(SelectedActividad);
                }
                
                // Recargar y limpiar selección
                CargarActividades();
                SelectedActividad = null;
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Error al guardar: {ex.Message}", "Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Prepara la actividad seleccionada para edición
        /// En este caso no realiza ninguna acción adicional ya que la edición es directa en el formulario
        /// </summary>
        private void Editar()
        {
            // La edición se realiza directamente en el formulario vinculado
        }

        /// <summary>
        /// Elimina la actividad seleccionada de la base de datos
        /// Valida que la actividad no tenga reservas asociadas antes de eliminar
        /// </summary>
        private void Eliminar()
        {
            if (SelectedActividad == null) return;

            // Solicitar confirmación al usuario
            var resultado = System.Windows.MessageBox.Show(
                $"¿Está seguro que desea eliminar la actividad '{SelectedActividad.Nombre}'?", 
                "Confirmar eliminación", 
                System.Windows.MessageBoxButton.YesNo, 
                System.Windows.MessageBoxImage.Question);

            if (resultado == System.Windows.MessageBoxResult.Yes)
            {
                (bool exito, string mensaje) = _repository.Delete(SelectedActividad.Id);
                
                if (exito)
                {
                    CargarActividades();
                    SelectedActividad = null;
                }
                else
                {
                    System.Windows.MessageBox.Show(
                        mensaje, 
                        "No se puede eliminar", 
                        System.Windows.MessageBoxButton.OK, 
                        System.Windows.MessageBoxImage.Warning);
                }
            }
        }

        /// <summary>
        /// Cancela la operación actual y recarga los datos originales
        /// Descarta cualquier cambio no guardado
        /// </summary>
        private void Cancelar()
        {
            SelectedActividad = null;
            CargarActividades();
        }
    }
}
