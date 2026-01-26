using System;
using System.Windows;
using System.Windows.Input;

namespace ViewModel
{
    /// <summary>
    /// ViewModel principal de la aplicación
    /// Gestiona la navegación entre las diferentes vistas (Reservas, Actividades, Socios, Informes)
    /// Implementa el patrón de navegación mediante cambio de ViewModel actual
    /// </summary>
    public class MainViewModel : BaseViewModel
    {
        private BaseViewModel _currentViewModel;
        
        /// <summary>
        /// ViewModel actualmente visible en la vista principal
        /// Al cambiar esta propiedad, la vista se actualiza automáticamente para mostrar el contenido correspondiente
        /// </summary>
        public BaseViewModel CurrentViewModel
        {
            get => _currentViewModel;
            set
            {
                _currentViewModel = value;
                OnPropertyChanged(nameof(CurrentViewModel));
            }
        }

        /// <summary>
        /// Comando para navegar a la vista de Reservas
        /// </summary>
        public ICommand ShowReservasCommand { get; }
        
        /// <summary>
        /// Comando para navegar a la vista de Actividades
        /// </summary>
        public ICommand ShowActividadesCommand { get; }
        
        /// <summary>
        /// Comando para navegar a la vista de Socios
        /// </summary>
        public ICommand ShowSociosCommand { get; }
        
        /// <summary>
        /// Comando para navegar a la vista de Informes
        /// </summary>
        public ICommand ShowInformesCommand { get; }

        /// <summary>
        /// Comando para cerrar la aplicación
        /// </summary>
        public ICommand SalirCommand { get; }

        /// <summary>
        /// Constructor del MainViewModel
        /// Inicializa todos los comandos de navegación y establece la vista inicial
        /// </summary>
        public MainViewModel()
        {
            // Comando para mostrar la vista de Reservas
            ShowReservasCommand = new RelayCommand(() =>
                CurrentViewModel = new ReservasViewModel());

            // Comando para mostrar la vista de Actividades
            ShowActividadesCommand = new RelayCommand(() =>
                CurrentViewModel = new ActividadesViewModel());

            // Comando para mostrar la vista de Socios
            ShowSociosCommand = new RelayCommand(() =>
                CurrentViewModel = new SociosViewModel());

            // Comando para mostrar la vista de Informes
            ShowInformesCommand = new RelayCommand(() =>
                CurrentViewModel = new InformesViewModel());

            // Comando para cerrar la aplicación
            SalirCommand = new RelayCommand(() =>
                Application.Current.Shutdown());

            // Establecer la vista inicial con la pantalla de bienvenida
            CurrentViewModel = new InicioViewModel();
        }
    }
}
