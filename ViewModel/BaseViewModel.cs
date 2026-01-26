using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModel
{
    /// <summary>
    /// Clase base abstracta para todos los ViewModels
    /// Implementa INotifyPropertyChanged para el patrón MVVM
    /// Proporciona funcionalidad común para notificar cambios de propiedades a la vista
    /// </summary>
    public abstract class BaseViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// Evento que se dispara cuando una propiedad cambia su valor
        /// Permite que la interfaz de usuario se actualice automáticamente
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Método protegido para notificar cambios en las propiedades
        /// Debe ser llamado en el setter de cada propiedad que afecte a la vista
        /// </summary>
        /// <param name="propertyName">Nombre de la propiedad que cambió</param>
        protected void OnPropertyChanged(string propertyName)
        {
            // Invoca el evento si hay suscriptores (uso del operador null-conditional)
            PropertyChanged?.Invoke(
                this,
                new PropertyChangedEventArgs(propertyName));
        }
    }
}
