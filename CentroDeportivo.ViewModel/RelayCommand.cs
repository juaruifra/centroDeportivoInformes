using System;
using System.Windows.Input;

namespace CentroDeportivo.ViewModel
{
    /// <summary>
    /// Implementación sencilla de ICommand.
    /// Permite enlazar botones a métodos del ViewModel.
    /// </summary>
    public class RelayCommand : ICommand
    {
        private readonly Action _execute;
        private readonly Func<bool> _canExecute;

        public RelayCommand(Action execute, Func<bool> canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        /// <summary>
        /// Determina si el comando puede ejecutarse en su estado actual.
        /// </summary>
        /// <param name="parameter">Parámetro del comando (no utilizado).</param>
        /// <returns>true si el comando puede ejecutarse; de lo contrario, false.</returns>
        public bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute();
        }

        /// <summary>
        /// Ejecuta el comando
        /// </summary>
        /// <param name="parameter">Comando a ejecutar</param>
        public void Execute(object parameter)
        {
            _execute();
        }

        public event EventHandler CanExecuteChanged;

        /// <summary>
        /// Llamar a este método cuando cambie la lógica de CanExecute.
        /// </summary>
        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
