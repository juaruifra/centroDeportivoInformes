using CentroDeportivo.Reports;
using CentroDeportivo.ReportsView;
using CentroDeportivo.ViewModel;
using System.Windows;
using System.Windows.Input;

namespace CentroDeportivo.View
{
    public partial class ReservasWindow : Window
    {
        public ReservasWindow()
        {
            InitializeComponent();
            DataContext = new ReservasViewModel();
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

        private void InformeHistorialReservas_Click(object sender, RoutedEventArgs e)
        {
            // Creamos la ventana del visor de informes,
            // indicando que queremos mostrar el informe de socios.
            var visor = new Window1(ReportType.HistorialReservas);

            // Mostramos la ventana como diálogo modal.
            visor.ShowDialog();
        }


    }
}
