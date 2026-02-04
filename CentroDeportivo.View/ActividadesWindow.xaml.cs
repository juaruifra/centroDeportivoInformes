using centroDeportivo.Model;
using CentroDeportivo.Reports;
using CentroDeportivo.ReportsView;
using CentroDeportivo.ViewModel;
using System.Windows;
using System.Windows.Input;


namespace CentroDeportivo.View
{
    /// <summary>
    /// Ventana para gestionar las actividades del centro deportivo.
    /// </summary>
    public partial class ActividadesWindow : Window
    {
        /// <summary>
        /// Inicializa la ventana y conecta con su ViewModel
        /// </summary>
        public ActividadesWindow()
        {
            InitializeComponent();
            DataContext = new ActividadesViewModel();
        }

        /// <summary>
        /// Cierra la ventana cuando se pulsa el botón cerrar
        /// </summary>
        private void Cerrar_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        /// <summary>
        /// Permite arrastrar la ventana con el ratón.
        /// </summary>
        private void Window_DragMove(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }

        /// <summary>
        /// Abre el informe con todas las reservas de la actividad seleccionada.
        /// Muestra un informe con los datos de quién ha reservado y cuándo.
        /// </summary>
        private void InformeReservasActividad_Click(object sender, RoutedEventArgs e)
        {
            // Obtenemos el ViewModel para acceder a la actividad seleccionada
            var vm = DataContext as ActividadesViewModel;

            // Comprobamos que haya una actividad seleccionada
            if (vm == null || vm.ActividadSeleccionada == null)
            {
                MessageBox.Show("Debes seleccionar una actividad.", "Error informe", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Obtenemos el ID de la actividad para filtrar el informe
            int actividadId = vm.ActividadSeleccionada.Id;

            // Abrimos el visor de informes pasándole el tipo de informe y el ID
            // Esto filtrará las reservas solo para esta actividad
            var visor = new Window1(ReportType.ReservasPorActividad, actividadId);

            visor.ShowDialog();
        }
    }
}
