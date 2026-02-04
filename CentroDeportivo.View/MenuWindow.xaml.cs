using CentroDeportivo.ViewModel;
using System.Windows;
using System.Windows.Input;

namespace CentroDeportivo.View
{
    /// <summary>
    /// Ventana principal de la aplicación.
    /// Muestra las estadísticas del centro y permite acceder a las demás ventanas.
    /// </summary>
    public partial class MenuWindow : Window
    {
        private MenuPrincipalViewModel _vm;

        /// <summary>
        /// Inicializa la ventana principal y configura las estadísticas.
        /// También prepara el evento para recargar datos cuando volvemos a esta ventana.
        /// </summary>
        public MenuWindow()
        {
            InitializeComponent();

            _vm = new MenuPrincipalViewModel();
            DataContext = _vm;

            // Cada vez que la ventana recibe el foco, recargamos las estadísticas
            // Esto permite ver los cambios cuando volvemos de otras ventanas
            Activated += MenuPrincipalWindow_Activated;
        }

        /// <summary>
        /// Cierra la aplicación
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
        /// Abre la ventana de gestión de socios como diálogo modal.
        /// La ventana actual queda bloqueada hasta que se cierre.
        /// </summary>
        private void AbrirSocios_Click(object sender, MouseButtonEventArgs e)
        {
            new SociosWindow().ShowDialog();
        }

        /// <summary>
        /// Abre la ventana de gestión de actividades como diálogo modal
        /// </summary>
        private void AbrirActividades_Click(object sender, MouseButtonEventArgs e)
        {
            new ActividadesWindow().ShowDialog();
        }

        /// <summary>
        /// Abre la ventana de gestión de reservas como diálogo modal
        /// </summary>
        private void AbrirReservas_Click(object sender, MouseButtonEventArgs e)
        {
            new ReservasWindow().ShowDialog();
        }

        /// <summary>
        /// Se ejecuta cada vez que la ventana se activa (recibe el foco).
        /// Recarga las estadísticas para mostrar los datos actualizados
        /// después de crear, editar o eliminar socios, actividades o reservas.
        /// </summary>
        private void MenuPrincipalWindow_Activated(object sender, System.EventArgs e)
        {
            _vm.CargarEstadisticas();
        }
    }
}

