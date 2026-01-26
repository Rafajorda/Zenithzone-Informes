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
    /// UserControl para la pantalla de inicio/bienvenida
    /// Muestra el logo de ZenithZone cuando no hay ninguna sección seleccionada
    /// Proporciona una pantalla de bienvenida antes de que el usuario navegue a alguna funcionalidad
    /// </summary>
    public partial class UCInicio : UserControl
    {
        /// <summary>
        /// Constructor del UserControl de Inicio
        /// Inicializa todos los componentes visuales definidos en el XAML
        /// </summary>
        public UCInicio()
        {
            InitializeComponent();
        }
    }
}
