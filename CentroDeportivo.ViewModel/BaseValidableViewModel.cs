using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace CentroDeportivo.ViewModel
{
    /// <summary>
    /// Clase base para ViewModels con validación.
    /// Implementa IDataErrorInfo de forma reutilizable.
    /// </summary>
    public abstract class BaseValidableViewModel : BaseViewModel, IDataErrorInfo
    {
        // Diccionario con errores por propiedad
        protected Dictionary<string, string> _errores = new Dictionary<string, string>();

        /// <summary>
        /// Error general del objeto (no se usa normalmente).
        /// </summary>
        public string Error => null;

        /// <summary>
        /// Devuelve el error de una propiedad concreta.
        /// WPF llama automáticamente a este indexador.
        /// </summary>
        public string this[string propertyName]
        {
            get
            {
                if (_errores.ContainsKey(propertyName))
                    return _errores[propertyName];

                return null;
            }
        }

        /// <summary>
        /// Indica si el ViewModel tiene errores.
        /// </summary>
        public bool TieneErrores => _errores.Count > 0;

        /// <summary>
        /// Limpia todos los errores.
        /// </summary>
        protected void LimpiarErrores()
        {
            _errores.Clear();
        }

        /// <summary>
        /// Añade o actualiza un error para una propiedad.
        /// </summary>
        protected void SetError(string propiedad, string mensaje)
        {
            _errores[propiedad] = mensaje;
            OnPropertyChanged(propiedad);

            OnPropertyChanged(nameof(TieneErrores));
        }

        /// <summary>
        /// Elimina el error de una propiedad.
        /// </summary>
        protected void ClearError(string propiedad)
        {
            if (_errores.ContainsKey(propiedad))
            {
                _errores.Remove(propiedad);
                OnPropertyChanged(propiedad);

                OnPropertyChanged(nameof(TieneErrores));
            }
        }

        protected void NotificarCambioValidacion()
        {
            OnPropertyChanged(nameof(TieneErrores));
        }



    }
}

