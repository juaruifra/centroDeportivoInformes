using centroDeportivo.Model;
using CentroDeportivo.Reports;
using CentroDeportivo.ReportsView;
using CentroDeportivo.ViewModel;
using System.Windows;
using System.Windows.Input;


namespace CentroDeportivo.View
{
    public partial class ActividadesWindow : Window
    {
        public ActividadesWindow()
        {
            InitializeComponent();
            DataContext = new ActividadesViewModel();
        }

        private void Cerrar_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_DragMove(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }

        /// <summary>
        /// Abre el informe de reservas para la actividad seleccionada.
        /// </summary>
        private void InformeReservasActividad_Click(object sender, RoutedEventArgs e)
        {
            // Obtenemos el ViewModel asociado a la vista
            var vm = DataContext as ActividadesViewModel;

            // Obtenemos la actividad seleccionada en el DataGrid
            //var actividadSeleccionada = dataGridActividades.SelectedItem as Actividades;

            if (vm == null || vm.ActividadSeleccionada == null)
            {
                MessageBox.Show("Debes seleccionar una actividad.", "Error informe", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Obtenemos el Id desde el ViewModel
            int actividadId = vm.ActividadSeleccionada.Id;

            // Creamos el visor de informes pasando el Id de la actividad
            // Le pasamos la actividad por el constructor, aunque no me gusta este metodo
            // no he encontrado otra forma de hacerlo
            var visor = new Window1(ReportType.ReservasPorActividad, actividadId);

            visor.ShowDialog();

        }




    }
}
