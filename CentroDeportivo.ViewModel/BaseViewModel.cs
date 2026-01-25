using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CentroDeportivo.ViewModel
{
    /// <summary>
    /// Clase base para todos los ViewModels.
    /// Implementa INotifyPropertyChanged para que WPF
    /// se entere cuando cambian las propiedades.
    /// </summary>
    public abstract class BaseViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Llama a este método cuando una propiedad cambia.
        /// WPF actualizará la interfaz automáticamente.
        /// </summary>
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(
                this,
                new PropertyChangedEventArgs(propertyName));
        }
    }


}
