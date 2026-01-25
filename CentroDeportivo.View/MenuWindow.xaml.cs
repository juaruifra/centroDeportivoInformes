using CentroDeportivo.ViewModel;
using System.Windows;
using System.Windows.Input;

namespace CentroDeportivo.View
{
    public partial class MenuWindow : Window
    {
        private MenuPrincipalViewModel _vm;

        public MenuWindow()
        {
            InitializeComponent();

            _vm = new MenuPrincipalViewModel();
            DataContext = _vm;

            // Evento cuando la ventana recibe el foco
            Activated += MenuPrincipalWindow_Activated;
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

        private void AbrirSocios_Click(object sender, MouseButtonEventArgs e)
        {
            new SociosWindow().Show();
        }

        private void AbrirActividades_Click(object sender, MouseButtonEventArgs e)
        {
            new ActividadesWindow().Show();
        }

        private void AbrirReservas_Click(object sender, MouseButtonEventArgs e)
        {
            new ReservasWindow().Show();
        }

        private void MenuPrincipalWindow_Activated(object sender, System.EventArgs e)
        {
            _vm.CargarEstadisticas();
        }



    }
}

