using centroDeportivo.Model;
using System.Linq;

namespace CentroDeportivo.ViewModel
{
    public class MenuPrincipalViewModel : BaseViewModel
    {
        private readonly SociosRepository _sociosRepository;
        private readonly ActividadesRepository _actividadesRepository;
        private readonly ReservasRepository _reservasRepository;

        // Estadísticas
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
            TotalSocios = _sociosRepository.GetAll().Count;
            TotalActividades = _actividadesRepository.GetAll().Count;

            var reservas = _reservasRepository.GetAll();

            var actividadTop = reservas
                .GroupBy(r => r.Actividades.Nombre)
                .OrderByDescending(g => g.Count())
                .Select(g => g.Key)
                .FirstOrDefault();

            ActividadMasReservada = actividadTop ?? "Sin reservas";
        }
    }
}
