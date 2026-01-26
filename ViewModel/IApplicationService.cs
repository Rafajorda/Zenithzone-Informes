using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModel
{
    /// <summary>
    /// Interfaz que define servicios de la aplicación
    /// Permite al ViewModel comunicarse con funcionalidades de la aplicación sin dependencia directa
    /// Facilita el testing y mantiene la separación de responsabilidades del patrón MVVM
    /// </summary>
    public interface IApplicationService
    {
        /// <summary>
        /// Cierra la aplicación de forma controlada
        /// </summary>
        void Shutdown();
    }
}
