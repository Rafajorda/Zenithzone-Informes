using Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ViewModel
{
    /// <summary>
    /// ViewModel para la gestión de Socios
    /// Maneja la lógica de negocio y la interacción entre la vista de socios y el repositorio
    /// Implementa operaciones CRUD completas
    /// </summary>
    public class SociosViewModel : BaseViewModel
    {
        private readonly SocioRepository _repository;

        private ObservableCollection<Socio> _socios;
        
        /// <summary>
        /// Colección observable de todos los socios
        /// Se actualiza automáticamente en la vista cuando cambia
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

        private Socio _selectedSocio;
        
        /// <summary>
        /// Socio actualmente seleccionado en el DataGrid
        /// Controla el estado del formulario y los comandos disponibles
        /// </summary>
        public Socio SelectedSocio
        {
            get => _selectedSocio;
            set
            {
                _selectedSocio = value;
                OnPropertyChanged(nameof(SelectedSocio));
                OnPropertyChanged(nameof(IsEditingExistingSocio));
                OnPropertyChanged(nameof(IsFormEnabled));
                
                // Forzar reevaluación de los comandos
                System.Windows.Input.CommandManager.InvalidateRequerySuggested();
            }
        }

        /// <summary>
        /// Indica si se está editando un socio existente (Id != 0)
        /// </summary>
        public bool IsEditingExistingSocio => SelectedSocio != null && SelectedSocio.Id != 0;
        
        /// <summary>
        /// Indica si el formulario debe estar habilitado
        /// </summary>
        public bool IsFormEnabled => SelectedSocio != null;

        // Comandos para las operaciones CRUD

        /// <summary>
        /// Agrega un nuevo socio
        /// </summary>
        public ICommand AgregarSocCommand { get; }

        /// <summary>
        /// Guarda el socio actual (nuevo o modificado)
        /// </summary>
        public ICommand GuardarSocCommand { get; }
        /// <summary>
        /// Edita el socio seleccionado
        /// </summary>
        public ICommand EditarSocCommand { get; }
        /// <summary>
        /// Elimina el socio seleccionado
        /// </summary>  
        public ICommand EliminarSocCommand { get; }
        public ICommand CancelarSocCommand { get; }

        /// <summary>
        /// Constructor del SociosViewModel
        /// Inicializa el repositorio, los comandos y carga los datos iniciales
        /// </summary>
        public SociosViewModel()
        {
            _repository = new SocioRepository();
            Socios = new ObservableCollection<Socio>();

            // Inicializar comandos con sus respectivas acciones y condiciones
            AgregarSocCommand = new RelayCommand(Agregar);
            GuardarSocCommand = new RelayCommand(Guardar, () => SelectedSocio != null);
            EditarSocCommand = new RelayCommand(Editar, () => SelectedSocio != null);
            EliminarSocCommand = new RelayCommand(Eliminar, () => SelectedSocio != null);
            CancelarSocCommand = new RelayCommand(Cancelar);

            // Cargar datos iniciales
            CargarSocios();
        }

        /// <summary>
        /// Carga todos los socios desde la base de datos
        /// Limpia y recarga la colección observable
        /// </summary>
        private void CargarSocios()
        {
            Socios.Clear();
            var socios = _repository.GetAll();
            foreach (var socio in socios)
            {
                Socios.Add(socio);
            }
        }

        /// <summary>
        /// Crea un nuevo socio vacío y lo prepara para edición
        /// Por defecto, el nuevo socio está activo
        /// </summary>
        private void Agregar()
        {
            var nuevoSocio = new Socio
            {
                Nombre = "",
                Email = "",
                Activo = true
            };
            Socios.Add(nuevoSocio);
            SelectedSocio = nuevoSocio;
        }

        /// <summary>
        /// Guarda el socio current (nuevo o modificado) en la base de datos
        /// Realiza validaciones antes de guardar
        /// </summary>
        private void Guardar()
        {
            if (SelectedSocio == null) return;

            // Validación: nombre obligatorio
            if (string.IsNullOrWhiteSpace(SelectedSocio.Nombre))
            {
                System.Windows.MessageBox.Show("El nombre es obligatorio.", "Validación", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
                return;
            }

            // Validación: email obligatorio
            if (string.IsNullOrWhiteSpace(SelectedSocio.Email))
            {
                System.Windows.MessageBox.Show("El email es obligatorio.", "Validación", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
                return;
            }

            // Validación: formato de email
            if (!EsEmailValido(SelectedSocio.Email))
            {
                System.Windows.MessageBox.Show("El email no tiene un formato válido.", "Validación", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
                return;
            }

            try
            {
                // Determinar si es inserción o actualización según el ID
                if (SelectedSocio.Id == 0)
                {
                    _repository.Add(SelectedSocio);
                }
                else
                {
                    _repository.Update(SelectedSocio);
                }

                // Recargar y limpiar selección
                CargarSocios();
                SelectedSocio = null;
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Error al guardar: {ex.Message}", "Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Valida el formato del email usando System.Net.Mail.MailAddress
        /// </summary>
        /// <param name="email">Email a validar</param>
        /// <returns>True si el formato parece válido, False en caso contrario</returns>
        public bool EsEmailValido(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return false;
            try
            {
                var addr = new MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }


        /// <summary>
        /// Prepara el socio seleccionado para edición
        /// En este caso no realiza ninguna acción adicional ya que la edición es directa en el formulario
        /// </summary>
        private void Editar()
        {
            // La edición se realiza directamente en el formulario vinculado
        }

        /// <summary>
        /// Elimina el socio seleccionado de la base de datos
        /// Valida que el socio no tenga reservas asociadas antes de eliminar
        /// </summary>
        private void Eliminar()
        {
            if (SelectedSocio == null) return;

            // Solicitar confirmación al usuario
            var resultado = System.Windows.MessageBox.Show(
                $"¿Está seguro que desea eliminar al socio '{SelectedSocio.Nombre}'?", 
                "Confirmar eliminación", 
                System.Windows.MessageBoxButton.YesNo, 
                System.Windows.MessageBoxImage.Question);

            if (resultado == System.Windows.MessageBoxResult.Yes)
            {
                (bool exito, string mensaje) = _repository.Delete(SelectedSocio.Id);
                
                if (exito)
                {
                    CargarSocios();
                    SelectedSocio = null;
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
            SelectedSocio = null;
            CargarSocios();
        }
    }
}
