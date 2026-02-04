using CentroDeportivo.Reports;
using CentroDeportivo.ReportsView;
using CentroDeportivo.ViewModel;
using System.Windows;
using System.Windows.Input;

namespace CentroDeportivo.View
{
    /// <summary>
    /// Ventana para gestionar las reservas del centro deportivo.
    /// </summary>
    public partial class ReservasWindow : Window
    {
        /// <summary>
        /// Inicializa la ventana y conecta con su ViewModel
        /// </summary>
        public ReservasWindow()
        {
            InitializeComponent();
            DataContext = new ReservasViewModel();
        }

        /// <summary>
        /// Cierra la ventana cuando se pulsa el botón cerrar
        /// </summary>
        private void Cerrar_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        /// <summary>
        /// Permite arrastrar la ventana con el ratón
        /// </summary>
        private void Window_DragMove(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }

        /// <summary>
        /// Abre el informe con el historial completo de todas las reservas.
        /// Muestra un informe con todas las reservas registradas en el sistema.
        /// </summary>
        private void InformeHistorialReservas_Click(object sender, RoutedEventArgs e)
        {
            // Creamos el visor de informes para mostrar el historial de reservas
            var visor = new Window1(ReportType.HistorialReservas);

            // Mostramos la ventana como modal
            visor.ShowDialog();
        }
    }
}
