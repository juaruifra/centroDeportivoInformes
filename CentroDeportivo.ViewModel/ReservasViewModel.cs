using centroDeportivo.Model;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Windows;

namespace CentroDeportivo.ViewModel
{
    /// <summary>
    /// ViewModel para gestionar las reservas del centro deportivo.
    /// Controla la creación, edición y eliminación de reservas, validando aforos y fechas.
    /// </summary>
    public class ReservasViewModel : BaseViewModel
    {
        // Repositorios para acceder a los datos
        private readonly SociosRepository _sociosRepository;
        private readonly ActividadesRepository _actividadesRepository;
        private readonly ReservasRepository _reservasRepository;

        // Listas que se muestran en los ComboBox y DataGrid
        public ObservableCollection<Socios> ListaSocios { get; set; }
        public ObservableCollection<Actividades> ListaActividades { get; set; }
        public ObservableCollection<Reservas> ListaReservas { get; set; }

        // Reserva que el usuario selecciona en la tabla
        private Reservas _reservaSeleccionada;
        public Reservas ReservaSeleccionada
        {
            get => _reservaSeleccionada;
            set
            {
                _reservaSeleccionada = value;
                OnPropertyChanged(nameof(ReservaSeleccionada));

                // Cuando seleccionamos una reserva, copiamos sus datos al formulario para editarla
                if (_reservaSeleccionada != null)
                {
                    NuevaReserva = new Reservas
                    {
                        Id = _reservaSeleccionada.Id,
                        SocioId = _reservaSeleccionada.SocioId,
                        ActividadId = _reservaSeleccionada.ActividadId,
                        Fecha = _reservaSeleccionada.Fecha
                    };
                }

                // Actualiza el estado del botón eliminar
                EliminarCommand.RaiseCanExecuteChanged();
            }
        }

        // Reserva que estamos creando o editando en el formulario
        private Reservas _nuevaReserva;
        public Reservas NuevaReserva
        {
            get => _nuevaReserva;
            set
            {
                _nuevaReserva = value;
                OnPropertyChanged(nameof(NuevaReserva));

                // Notificamos a los ComboBox que se actualicen con los valores correctos
                OnPropertyChanged(nameof(SocioSeleccionadoId));
                OnPropertyChanged(nameof(ActividadSeleccionadaId));

                // Habilitamos/deshabilitamos el formulario según si hay reserva activa
                OnPropertyChanged(nameof(FormularioHabilitado));
                GuardarCommand.RaiseCanExecuteChanged();
            }
        }

        // Enlace con el ComboBox de socios. Sincroniza la selección con la reserva
        public int SocioSeleccionadoId
        {
            get => NuevaReserva?.SocioId ?? 0;
            set
            {
                if (NuevaReserva == null) return;

                NuevaReserva.SocioId = value;

                // Conectamos el objeto socio completo para que aparezca bien en la tabla
                NuevaReserva.Socios = ListaSocios.FirstOrDefault(s => s.Id == value);

                OnPropertyChanged(nameof(NuevaReserva));
            }
        }

        // Enlace con el ComboBox de actividades. Sincroniza la selección con la reserva
        public int ActividadSeleccionadaId
        {
            get => NuevaReserva?.ActividadId ?? 0;
            set
            {
                if (NuevaReserva == null) return;

                NuevaReserva.ActividadId = value;

                // Conectamos el objeto actividad completo para que aparezca bien en la tabla
                NuevaReserva.Actividades = ListaActividades.FirstOrDefault(a => a.Id == value);

                OnPropertyChanged(nameof(NuevaReserva));
            }
        }

        // El formulario solo está activo si hay una reserva en edición
        public bool FormularioHabilitado => NuevaReserva != null;

        // Comandos que se enlazan con los botones
        public RelayCommand NuevoCommand { get; }
        public RelayCommand GuardarCommand { get; }
        public RelayCommand EliminarCommand { get; }

        /// <summary>
        /// Constructor por defecto. Carga todo y deja el formulario listo para usar
        /// </summary>
        public ReservasViewModel()
        {
            _sociosRepository = new SociosRepository();
            _actividadesRepository = new ActividadesRepository();
            _reservasRepository = new ReservasRepository();

            // Inicializamos los comandos
            NuevoCommand = new RelayCommand(Nueva);
            GuardarCommand = new RelayCommand(GuardarCommand_Execute, PuedeGuardar);
            EliminarCommand = new RelayCommand(Eliminar, PuedeEliminar);

            // Cargamos los datos de la base de datos
            CargarDatos();
        }

        /// <summary>
        /// Constructor alternativo. Permite crear una reserva ya con socio o actividad preseleccionados.
        /// Útil cuando vienes desde la pantalla de socios o actividades.
        /// </summary>
        public ReservasViewModel(int? socioId = null, int? actividadId = null)
        {
            _sociosRepository = new SociosRepository();
            _actividadesRepository = new ActividadesRepository();
            _reservasRepository = new ReservasRepository();

            NuevoCommand = new RelayCommand(Nueva);
            GuardarCommand = new RelayCommand(GuardarCommand_Execute, PuedeGuardar);
            EliminarCommand = new RelayCommand(Eliminar, PuedeEliminar);

            CargarDatos();

            // Creamos una reserva nueva automáticamente con la fecha de hoy
            NuevaReserva = new Reservas
            {
                Fecha = DateTime.Today
            };

            // Si nos han pasado un socio o actividad, los preseleccionamos
            if (socioId.HasValue)
                NuevaReserva.SocioId = socioId.Value;

            if (actividadId.HasValue)
                NuevaReserva.ActividadId = actividadId.Value;
        }

        /// <summary>
        /// Carga socios, actividades y reservas desde la base de datos
        /// </summary>
        private void CargarDatos()
        {
            // Cargamos y ordenamos alfabéticamente
            ListaSocios = new ObservableCollection<Socios>(_sociosRepository.GetAll().OrderBy(s => s.Nombre));

            ListaActividades = new ObservableCollection<Actividades>(_actividadesRepository.GetAll().OrderBy(a => a.Nombre));

            // Las reservas las ordenamos por fecha, las más recientes primero
            ListaReservas = new ObservableCollection<Reservas>(_reservasRepository.GetAll().OrderByDescending(r => r.Fecha));

            // Notificamos que las listas están listas
            OnPropertyChanged(nameof(ListaSocios));
            OnPropertyChanged(nameof(ListaActividades));
            OnPropertyChanged(nameof(ListaReservas));
        }

        /// <summary>
        /// Prepara el formulario para crear una reserva nueva desde cero
        /// </summary>
        private void Nueva()
        {
            NuevaReserva = new Reservas
            {
                Fecha = DateTime.Today
            };
        }

        /// <summary>
        /// Adaptador para que el comando pueda llamar a Guardar sin problemas
        /// </summary>
        private void GuardarCommand_Execute()
        {
            Guardar();
        }

        /// <summary>
        /// Valida y guarda la reserva en la base de datos.
        /// Comprueba que el socio, actividad y fecha sean válidos, y que haya plazas disponibles.
        /// </summary>
        /// <param name="test">Si es true, no muestra mensajes (para tests automáticos)</param>
        /// <returns>true si se guardó correctamente, false si hubo errores</returns>
        public bool Guardar(bool test = false)
        {
            try
            {
                bool ok = true;

                // Validamos que se haya seleccionado un socio
                if (NuevaReserva.SocioId == 0)
                {
                    MessageBox.Show(
                        "Debe seleccionar un socio.",
                        "Error de validación",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);

                    ok = false;
                }

                // Validamos que se haya seleccionado una actividad
                if (ok && NuevaReserva.ActividadId == 0)
                {
                    MessageBox.Show(
                        "Debe seleccionar una actividad.",
                        "Error de validación",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);

                    ok = false;
                }

                // Validamos que se haya seleccionado una fecha
                if (ok && NuevaReserva.Fecha == DateTime.MinValue)
                {
                    MessageBox.Show(
                        "Debe seleccionar una fecha.",
                        "Error de validación",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);

                    ok = false;
                }

                // No permitimos reservas en el pasado
                if (ok && NuevaReserva.Fecha.Date < DateTime.Today)
                {
                    if (!test)
                    {
                        MessageBox.Show(
                            "La fecha no puede ser anterior a hoy.",
                            "Error de validación",
                            MessageBoxButton.OK,
                            MessageBoxImage.Error);
                    }

                    ok = false;
                }

                // Comprobamos que la actividad no esté llena ese día
                if (ok && ActividadSinAforo())
                {
                    if (!test)
                    {
                        MessageBox.Show(
                            "No hay plazas disponibles para esta actividad en la fecha seleccionada.",
                            "Aforo completo",
                            MessageBoxButton.OK,
                            MessageBoxImage.Error);
                    }

                    ok = false;
                }

                if (ok)
                {
                    // Si todo está bien, guardamos en la base de datos
                    _reservasRepository.Save(NuevaReserva);

                    // Recargamos la lista para mostrar los cambios
                    ListaReservas = new ObservableCollection<Reservas>(_reservasRepository.GetAll().OrderByDescending(r => r.Fecha));

                    OnPropertyChanged(nameof(ListaReservas));

                    // Limpiamos el formulario
                    ReservaSeleccionada = null;
                    NuevaReserva = null;
                }

                return ok;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error guardar reserva", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        /// <summary>
        /// Elimina la reserva seleccionada después de pedir confirmación
        /// </summary>
        private void Eliminar()
        {
            try
            {
                var resultado = MessageBox.Show(
                    "¿Seguro que quieres eliminar esta reserva?",
                    "Confirmar eliminación",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                // Solo borramos si el usuario confirma
                if (resultado == MessageBoxResult.Yes)
                {
                    _reservasRepository.Delete(ReservaSeleccionada);
                    ListaReservas.Remove(ReservaSeleccionada);
                    ReservaSeleccionada = null;
                    NuevaReserva = null;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error eliminar reserva", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Comprueba si el botón guardar debe estar habilitado
        /// </summary>
        /// <returns> True o false dependiendo de si se puede guardar o no</returns>
        private bool PuedeGuardar()
        {
            return NuevaReserva != null;
        }

        /// <summary>
        /// Comprueba si el botón eliminar debe estar habilitado
        /// </summary>
        /// <returns> True o false dependiendo de si se puede guardar o no</returns>
        private bool PuedeEliminar()
        {
            return ReservaSeleccionada != null;
        }

        /// <summary>
        /// Verifica si ya no quedan plazas disponibles para la actividad en la fecha elegida.
        /// Cuenta las reservas existentes y las compara con el aforo máximo.
        /// </summary>
        /// <returns>true si está completo, false si aún hay plazas</returns>
        private bool ActividadSinAforo()
        {
            // Cuenta cuántas reservas ya hay para esa actividad ese día
            // (excluyendo la actual si estamos editando)
            int reservasActuales = _reservasRepository.GetReservasCount(
                NuevaReserva.ActividadId,
                NuevaReserva.Fecha,
                NuevaReserva.Id > 0 ? NuevaReserva.Id : (int?)null);

            // Buscamos el aforo máximo de la actividad
            int aforoMaximo = ListaActividades
                .First(a => a.Id == NuevaReserva.ActividadId)
                .AforoMaximo;

            // Si ya hay tantas reservas como plazas, está completo
            return reservasActuales >= aforoMaximo;
        }
    }
}

