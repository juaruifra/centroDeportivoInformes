using centroDeportivo.Model;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Windows;

namespace CentroDeportivo.ViewModel
{
    public class ReservasViewModel : BaseViewModel
    {
        private readonly SociosRepository _sociosRepository;
        private readonly ActividadesRepository _actividadesRepository;
        private readonly ReservasRepository _reservasRepository;

        public ObservableCollection<Socios> ListaSocios { get; set; }
        public ObservableCollection<Actividades> ListaActividades { get; set; }
        public ObservableCollection<Reservas> ListaReservas { get; set; }

        // Reserva seleccionada en el DataGrid
        private Reservas _reservaSeleccionada;
        public Reservas ReservaSeleccionada
        {
            get => _reservaSeleccionada;
            set
            {
                _reservaSeleccionada = value;
                OnPropertyChanged(nameof(ReservaSeleccionada));

                // Si se selecciona una reserva, la cargamos para editar
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

                EliminarCommand.RaiseCanExecuteChanged();
            }
        }


        // Reserva nueva que se está creando
        private Reservas _nuevaReserva;
        public Reservas NuevaReserva
        {
            get => _nuevaReserva;
            set
            {
                _nuevaReserva = value;
                OnPropertyChanged(nameof(NuevaReserva));

                // ESTO ES CLAVE
                OnPropertyChanged(nameof(SocioSeleccionadoId));
                OnPropertyChanged(nameof(ActividadSeleccionadaId));

                OnPropertyChanged(nameof(FormularioHabilitado));
                GuardarCommand.RaiseCanExecuteChanged();
            }
        }

        public int SocioSeleccionadoId
        {
            get => NuevaReserva?.SocioId ?? 0;
            set
            {
                if (NuevaReserva == null) return;

                NuevaReserva.SocioId = value;

                // Actualizamos la navegación para que el DataGrid se refresque
                NuevaReserva.Socios = ListaSocios.FirstOrDefault(s => s.Id == value);

                OnPropertyChanged(nameof(NuevaReserva));
            }
        }

        public int ActividadSeleccionadaId
        {
            get => NuevaReserva?.ActividadId ?? 0;
            set
            {
                if (NuevaReserva == null) return;

                NuevaReserva.ActividadId = value;

                // Actualizamos la navegación para que el DataGrid se refresque
                NuevaReserva.Actividades = ListaActividades.FirstOrDefault(a => a.Id == value);

                OnPropertyChanged(nameof(NuevaReserva));
            }
        }


        public bool FormularioHabilitado => NuevaReserva != null;

        public RelayCommand NuevoCommand { get; }
        public RelayCommand GuardarCommand { get; }
        public RelayCommand EliminarCommand { get; }

        public ReservasViewModel()
        {
            _sociosRepository = new SociosRepository();
            _actividadesRepository = new ActividadesRepository();
            _reservasRepository = new ReservasRepository();

            // Primero crear los comandos
            NuevoCommand = new RelayCommand(Nueva);
            GuardarCommand = new RelayCommand(Guardar, PuedeGuardar);
            EliminarCommand = new RelayCommand(Eliminar, PuedeEliminar);

            // Luego cargar datos y crear la reserva inicial
            CargarDatos();
        }

        public ReservasViewModel(int? socioId = null, int? actividadId = null)
        {
            _sociosRepository = new SociosRepository();
            _actividadesRepository = new ActividadesRepository();
            _reservasRepository = new ReservasRepository();

            NuevoCommand = new RelayCommand(Nueva);
            GuardarCommand = new RelayCommand(Guardar, PuedeGuardar);
            EliminarCommand = new RelayCommand(Eliminar, PuedeEliminar);

            CargarDatos();

            // Crear nueva reserva automáticamente
            NuevaReserva = new Reservas
            {
                Fecha = DateTime.Today
            };

            if (socioId.HasValue)
                NuevaReserva.SocioId = socioId.Value;

            if (actividadId.HasValue)
                NuevaReserva.ActividadId = actividadId.Value;
        }


        /// <summary>
        /// Carga los datos iniciales
        /// </summary>
        private void CargarDatos()
        {
            ListaSocios = new ObservableCollection<Socios>(_sociosRepository.GetAll().OrderBy(s => s.Nombre));

            ListaActividades = new ObservableCollection<Actividades>( _actividadesRepository.GetAll().OrderBy(a => a.Nombre));

            ListaReservas = new ObservableCollection<Reservas>(_reservasRepository.GetAll().OrderByDescending(r => r.Fecha));

            OnPropertyChanged(nameof(ListaSocios));
            OnPropertyChanged(nameof(ListaActividades));
            OnPropertyChanged(nameof(ListaReservas));

        }

        /// <summary>
        /// Se genera el nuevo objeto reservas con los datos iniciales asignados
        /// si se pueden asignar
        /// </summary>
        private void Nueva()
        {
            NuevaReserva = new Reservas
            {
                Fecha = DateTime.Today
            };

        }


        /// <summary>
        /// Añade una reserva despues de validar todo lo necesario
        /// </summary>
        private void Guardar()
        {
            try
            {
                bool ok = true;

                // Validación: socio obligatorio
                if (NuevaReserva.SocioId == 0)
                {
                    MessageBox.Show(
                        "Debe seleccionar un socio.",
                        "Error de validación",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);

                    ok = false;
                }

                // Validación: actividad obligatoria
                if (ok && NuevaReserva.ActividadId == 0)
                {
                    MessageBox.Show(
                        "Debe seleccionar una actividad.",
                        "Error de validación",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);

                    ok = false;
                }

                // Validación: fecha obligatoria
                if (ok && NuevaReserva.Fecha == DateTime.MinValue)
                {
                    MessageBox.Show(
                        "Debe seleccionar una fecha.",
                        "Error de validación",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);

                    ok = false;
                }

                // Validación: fecha no anterior a hoy
                if (ok && NuevaReserva.Fecha.Date < DateTime.Today)
                {
                    MessageBox.Show(
                        "La fecha no puede ser anterior a hoy.",
                        "Error de validación",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);

                    ok = false; ;
                }

                // Validación de aforo por actividad y día
                if (ok && ActividadSinAforo())
                {
                    MessageBox.Show(
                        "No hay plazas disponibles para esta actividad en la fecha seleccionada.",
                        "Aforo completo",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);

                    ok = false;
                }

                if (ok)
                {

                    // Guardar (inserta o actualiza según Id)
                    _reservasRepository.Save(NuevaReserva);

                    // Volver a cargar reservas desde BBDD
                    ListaReservas = new ObservableCollection<Reservas>(_reservasRepository.GetAll().OrderByDescending(r => r.Fecha));

                    OnPropertyChanged(nameof(ListaReservas));

                    // Limpiamos selección
                    ReservaSeleccionada = null;
                    NuevaReserva = null;

                }
            }catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Error guardar reserva", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Eliminar reserva
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

                // Si el usuario acepta, borramos
                if (resultado == MessageBoxResult.Yes)
                {
                    _reservasRepository.Delete(ReservaSeleccionada);
                    ListaReservas.Remove(ReservaSeleccionada);
                    ReservaSeleccionada = null;
                    NuevaReserva = null;
                }

            }catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Error eliminar reserva", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        private bool PuedeGuardar()
        {
            return NuevaReserva != null;
        }

        private bool PuedeEliminar()
        {
            return ReservaSeleccionada != null;
        }

        /// <summary>
        /// Comprueba si la actividad ha superado su aforo máximo
        /// para la fecha seleccionada, consultando la base de datos.
        /// </summary>
        private bool ActividadSinAforo()
        {
            int reservasActuales = _reservasRepository.GetReservasCount(
                NuevaReserva.ActividadId,
                NuevaReserva.Fecha,
                NuevaReserva.Id > 0 ? NuevaReserva.Id : (int?)null);

            int aforoMaximo = ListaActividades
                .First(a => a.Id == NuevaReserva.ActividadId)
                .AforoMaximo;

            return reservasActuales >= aforoMaximo;
        }

    }
}

