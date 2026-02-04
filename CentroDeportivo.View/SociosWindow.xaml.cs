using CentroDeportivo.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using CentroDeportivo.Reports;
using CentroDeportivo.ReportsView;

namespace CentroDeportivo.View
{
    /// <summary>
    /// Ventana para gestionar socios del centro deportivo.
    /// Permite crear, editar y eliminar socios, además de acceder a sus reservas.
    /// </summary>
    public partial class SociosWindow : Window
    {
        /// <summary>
        /// Inicializa la ventana y conecta con su ViewModel
        /// </summary>
        public SociosWindow()
        {
            InitializeComponent();
            DataContext = new SociosViewModel();
        }

        /// <summary>
        /// Cierra la ventana cuando se pulsa el botón cerrar
        /// </summary>
        private void Cerrar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Permite mover la ventana arrastrándola con el ratón.
        /// Útil en ventanas sin barra de título estándar.
        /// </summary>
        private void Window_DragMove(object sender, MouseButtonEventArgs e)
        {
            // Solo movemos si el botón izquierdo está presionado
            if (e.ButtonState == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }

        /// <summary>
        /// Abre la ventana de reservas con el socio actual preseleccionado.
        /// Permite crear rápidamente una reserva para el socio que estamos viendo.
        /// </summary>
        private void AbrirReservas_Click(object sender, RoutedEventArgs e)
        {
            var vm = DataContext as SociosViewModel;

            // Nos aseguramos de que hay un socio seleccionado
            if (vm == null || vm.SocioSeleccionado == null)
                return;

            // Creamos el ViewModel de reservas pasándole el ID del socio actual
            var reservasVm = new ReservasViewModel(socioId: vm.SocioSeleccionado.Id);

            // Abrimos la ventana de reservas
            var ventana = new ReservasWindow
            {
                DataContext = reservasVm
            };

            ventana.Show();
        }

        /// <summary>
        /// Abre la ventana de informes mostrando el informe maestro de socios.
        /// </summary>
        private void InformeSocios_Click(object sender, RoutedEventArgs e)
        {
            // Creamos la ventana del visor indicando que queremos el informe de socios
            var visor = new Window1(ReportType.Socios);

            // Mostramos la ventana como modal (bloquea la ventana actual hasta cerrarla)
            visor.ShowDialog();
        }
    }
}
