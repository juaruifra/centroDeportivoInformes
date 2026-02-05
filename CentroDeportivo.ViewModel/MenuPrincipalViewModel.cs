using centroDeportivo.Model;
using centroDeportivo.Model.Repositories;
using System.Linq;

namespace CentroDeportivo.ViewModel
{
    /// <summary>
    /// ViewModel del menú principal. Muestra las estadísticas generales del centro deportivo.
    /// </summary>
    public class MenuPrincipalViewModel : BaseViewModel
    {
        // Repositorios para acceder a los datos
        private readonly SociosRepository _sociosRepository;
        private readonly ActividadesRepository _actividadesRepository;
        private readonly ReservasRepository _reservasRepository;

        // Número total de socios registrados
        private int _totalSocios;
        public int TotalSocios
        {
            get => _totalSocios;
            set
            {
                _totalSocios = value;
                OnPropertyChanged(nameof(TotalSocios));
            }
        }

        // Número total de actividades disponibles
        private int _totalActividades;
        public int TotalActividades
        {
            get => _totalActividades;
            set
            {
                _totalActividades = value;
                OnPropertyChanged(nameof(TotalActividades));
            }
        }

        // Nombre de la actividad con más reservas
        private string _actividadMasReservada;
        public string ActividadMasReservada
        {
            get => _actividadMasReservada;
            set
            {
                _actividadMasReservada = value;
                OnPropertyChanged(nameof(ActividadMasReservada));
            }
        }

        /// <summary>
        /// Inicializa los repositorios y carga las estadísticas al arrancar
        /// </summary>
        public MenuPrincipalViewModel()
        {
            _sociosRepository = new SociosRepository();
            _actividadesRepository = new ActividadesRepository();
            _reservasRepository = new ReservasRepository();

            CargarEstadisticas();
        }

        /// <summary>
        /// Carga o recarga todas las estadísticas
        /// </summary>
        public void CargarEstadisticas()
        {
            // Cuenta todos los socios
            TotalSocios = _sociosRepository.GetAll().Count;
            
            // Cuenta todas las actividades
            TotalActividades = _actividadesRepository.GetAll().Count;

            // Obtiene todas las reservas
            var reservas = _reservasRepository.GetAll();

            // Agrupa por actividad y encuentra la que tiene más reservas
            var actividadTop = reservas
                .GroupBy(r => r.Actividades.Nombre)
                .OrderByDescending(g => g.Count())
                .Select(g => g.Key)
                .FirstOrDefault();

            // Si no hay reservas, muestra un mensaje por defecto
            ActividadMasReservada = actividadTop ?? "Sin reservas";
        }
    }
}
