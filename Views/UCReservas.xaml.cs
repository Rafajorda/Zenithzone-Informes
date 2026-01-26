using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Views
{
    /// <summary>
    /// UserControl para la gestión de Reservas
    /// Proporciona la interfaz de usuario para crear, editar, visualizar y eliminar reservas
    /// Gestiona la relación entre socios y actividades con validaciones de reglas de negocio
    /// Regla principal: Un socio solo puede tener una reserva por día
    /// El DataContext se establece automáticamente desde el MainViewModel cuando se navega a esta vista
    /// </summary>
    public partial class UCReservas : UserControl
    {
        /// <summary>
        /// Constructor del UserControl de Reservas
        /// Inicializa todos los componentes visuales definidos en el XAML
        /// Incluye ComboBoxes para selección de Socio y Actividad, y DatePicker para la fecha
        /// </summary>
        public UCReservas()
        {
            InitializeComponent();
        }
    }
}
