using centroDeportivo.Model;
using System;
using System.Collections.ObjectModel;
using System.Windows;

namespace CentroDeportivo.ViewModel
{
    /// <summary>
    /// ViewModel para la gestión de actividades.
    /// </summary>
    public class ActividadesViewModel : BaseViewModel
    {
        // Repositorio de actividades
        private readonly ActividadesRepository _actividadesRepository;

        // Repositorio de reservas
        private readonly ReservasRepository _reservasRepository;

        /// <summary>
        /// Lista de actividades mostrada en el DataGrid.
        /// </summary>
        public ObservableCollection<Actividades> ListaActividades { get; set; }

        /// <summary>
        /// Indica si el formulario está habilitado.
        /// </summary>
        public bool FormularioHabilitado
        {
            get => ActividadSeleccionada != null;
        }

        // Actividad seleccionada
        private Actividades _actividadSeleccionada;
        public Actividades ActividadSeleccionada
        {
            get => _actividadSeleccionada;
            set
            {
                _actividadSeleccionada = value;
                OnPropertyChanged(nameof(ActividadSeleccionada));
                OnPropertyChanged(nameof(FormularioHabilitado));

                GuardarCommand.RaiseCanExecuteChanged();
                EliminarCommand.RaiseCanExecuteChanged();
            }
        }

        // Comandos
        public RelayCommand NuevoCommand { get; }
        public RelayCommand GuardarCommand { get; }
        public RelayCommand EliminarCommand { get; }

        /// <summary>
        /// Constructor.
        /// </summary>
        public ActividadesViewModel()
        {
            // Cargamos los repositorios necesarios
            _actividadesRepository = new ActividadesRepository();

            _reservasRepository = new ReservasRepository();

            CargarActividades();

            NuevoCommand = new RelayCommand(NuevaActividad);
            GuardarCommand = new RelayCommand(GuardarActividad, PuedeGuardar);
            EliminarCommand = new RelayCommand(EliminarActividad, PuedeEliminar);
        }

        /// <summary>
        /// Carga las actividades desde la base de datos.
        /// </summary>
        private void CargarActividades()
        {
            ListaActividades = new ObservableCollection<Actividades>(
                _actividadesRepository.GetAll());

            OnPropertyChanged(nameof(ListaActividades));
        }

        /// <summary>
        /// Prepara una actividad nueva.
        /// </summary>
        private void NuevaActividad()
        {
            ActividadSeleccionada = new Actividades();
        }

        /// <summary>
        /// Guarda la actividad tras validar los datos.
        /// </summary>
        private void GuardarActividad()
        {
            try
            {
                bool ok = true;

                // Validar nombre
                if (string.IsNullOrWhiteSpace(ActividadSeleccionada.Nombre))
                {
                    MessageBox.Show(
                        "El nombre de la actividad es obligatorio.",
                        "Validación",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);

                    ok = false;
                }

                int afVal = 0;

                if (ok && !int.TryParse(ActividadSeleccionada.AforoMaximo.ToString(), out afVal))
                {
                    MessageBox.Show(
                        "El aforo máximo debe ser un número entero",
                        "Validación",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);

                    ok = false;
                }

                // Validar aforo
                if (ok && afVal <= 0)
                {
                    MessageBox.Show(
                        "El aforo máximo debe ser mayor que 0.",
                        "Validación",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);

                    ok = false;
                }

                if (ok)
                {

                    _actividadesRepository.Save(ActividadSeleccionada);

                    if (!ListaActividades.Contains(ActividadSeleccionada))
                    {
                        ListaActividades.Add(ActividadSeleccionada);
                    }

                    MessageBox.Show(
                        "Actividad guardada correctamente.",
                        "Información",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);

                }
            }
            catch (Exception ex) {
                MessageBox.Show(ex.Message, "Error guardar actividad", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        /// <summary>
        /// Elimina la actividad seleccionada previa confirmación.
        /// </summary>
        private void EliminarActividad()
        {
            try
            {
                bool ok = true;

                // Comprobación existencia de reservas con la actividad que se va a borrar
                bool conflicto = _reservasRepository.ExisteReservaConActividad(ActividadSeleccionada.Id);

                // Si existe conflicto: Mostrar mensaje y no continuar
                if (conflicto)
                {

                    MessageBox.Show(
                        "No se pueden borrar actividades que esten asignadas a reservas",
                        "Conflicto ",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);

                    ok = false;

                }

                if (ok)
                {

                    var resultado = MessageBox.Show(
                        "¿Seguro que quieres eliminar esta actividad?",
                        "Confirmar eliminación",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Warning);

                    // Si el usuario acepta, borramos
                    if (resultado == MessageBoxResult.Yes)
                    {
                        _actividadesRepository.Delete(ActividadSeleccionada);
                        ListaActividades.Remove(ActividadSeleccionada);
                        ActividadSeleccionada = null;
                    }

                }

            }catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Error eliminar actividad", MessageBoxButton.OK, MessageBoxImage.Error);
            }



        }

        /// <summary>
        /// Indica si se puede guardar.
        /// </summary>
        private bool PuedeGuardar()
        {
            return ActividadSeleccionada != null;
        }

        /// <summary>
        /// Indica si se puede eliminar.
        /// </summary>
        private bool PuedeEliminar()
        {
            return ActividadSeleccionada != null && ActividadSeleccionada.Id > 0;
        }
    }
}
