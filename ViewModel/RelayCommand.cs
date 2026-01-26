using System;
using System.Windows.Input;

namespace ViewModel
{
    /// <summary>
    /// Implementación reutilizable de ICommand para el patrón MVVM
    /// Permite enlazar acciones del ViewModel con controles de la Vista (botones, menús, etc.)
    /// Proporciona una forma sencilla de ejecutar lógica y controlar cuándo un comando está habilitado
    /// </summary>
    public class RelayCommand : ICommand
    {
        private readonly Action _execute;
        private readonly Func<bool> _canExecute;

        /// <summary>
        /// Constructor del RelayCommand
        /// </summary>
        /// <param name="execute">Acción a ejecutar cuando se invoca el comando (requerido)</param>
        /// <param name="canExecute">Función que determina si el comando puede ejecutarse (opcional)</param>
        public RelayCommand(Action execute, Func<bool> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        /// <summary>
        /// Determina si el comando puede ejecutarse en su estado actual
        /// </summary>
        /// <param name="parameter">Parámetro del comando (no utilizado en esta implementación)</param>
        /// <returns>True si el comando puede ejecutarse, False en caso contrario</returns>
        public bool CanExecute(object parameter) => _canExecute == null || _canExecute();

        /// <summary>
        /// Ejecuta la acción asociada al comando
        /// </summary>
        /// <param name="parameter">Parámetro del comando (no utilizado en esta implementación)</param>
        public void Execute(object parameter) => _execute();

        /// <summary>
        /// Evento que notifica cuando el estado de CanExecute puede haber cambiado
        /// WPF escucha este evento para reevaluar automáticamente si los controles deben estar habilitados/deshabilitados
        /// </summary>
        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }
}