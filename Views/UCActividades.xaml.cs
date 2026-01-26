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
    /// UserControl para la gestión de Actividades
    /// Proporciona la interfaz de usuario para crear, editar, visualizar y eliminar actividades
    /// El DataContext se establece automáticamente desde el MainViewModel cuando se navega a esta vista
    /// </summary>
    public partial class UCActividades : UserControl
    {
        /// <summary>
        /// Constructor del UserControl de Actividades
        /// Inicializa todos los componentes visuales definidos en el XAML
        /// </summary>
        public UCActividades()
        {
            InitializeComponent();
        }
    }
}
