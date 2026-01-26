using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ViewModel;


namespace Views
{
    /// <summary>
    /// Ventana principal de la aplicación ZenithZone
    /// Implementa el patrón MVVM estableciendo el MainViewModel como DataContext
    /// Gestiona la navegación entre las diferentes secciones de la aplicación
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Constructor de la ventana principal
        /// Inicializa los componentes y establece el DataContext con el MainViewModel
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            // Establecer el ViewModel principal como contexto de datos
            // Esto permite el binding entre la vista y el ViewModel
            DataContext = new MainViewModel();
        }
    }
}