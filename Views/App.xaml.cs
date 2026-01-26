using System.Configuration;
using System.Data;
using System.Windows;

namespace Views
{
    /// <summary>
    /// Clase principal de la aplicación ZenithZone
    /// Punto de entrada y configuración global de la aplicación WPF
    /// Maneja eventos del ciclo de vida de la aplicación y recursos globales
    /// </summary>
    public partial class App : Application
    {
        // La configuración de inicio y recursos se define en App.xaml
        public static ReportWindow ReportWindowInstance;
    }
}
