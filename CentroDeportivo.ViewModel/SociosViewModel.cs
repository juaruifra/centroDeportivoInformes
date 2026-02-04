using centroDeportivo.Model;
using System.Collections.ObjectModel;
using System.Windows;
using System.Text.RegularExpressions;
using System;

namespace CentroDeportivo.ViewModel
{
    /// <summary>
    /// ViewModel para la gestión de socios.
    /// Contiene la lógica que usará la ventana de socios.
    /// </summary>
    public class SociosViewModel : BaseViewModel//BaseValidableViewModel // Hereda de la base validable
    {
        // Repositorio que se encarga del acceso a datos
        private readonly SociosRepository _sociosRepository;

        // Repositorio de reservas
        private readonly ReservasRepository _reservasRepository;

        private const string ERROR_NOMBRE = "Nombre";
        private const string ERROR_EMAIL = "Email";

        /// <summary>
        /// Lista de socios que se muestra en el DataGrid.
        /// ObservableCollection notifica cambios a la vista.
        /// </summary>
        public ObservableCollection<Socios> ListaSocios { get; set; }

        /// <summary>
        /// Indica si el formulario debe estar activo.
        /// Será true cuando haya un socio seleccionado (nuevo o existente).
        /// </summary>
        public bool FormularioHabilitado
        {
            get => SocioSeleccionado != null;
        }

        // Socio actualmente seleccionado en la vista
        private Socios _socioSeleccionado;
        public Socios SocioSeleccionado
        {
            get => _socioSeleccionado;
            set
            {
                _socioSeleccionado = value;
                OnPropertyChanged(nameof(SocioSeleccionado));

                // Esto hace que la vista actualice el bloqueo del formulario
                OnPropertyChanged(nameof(FormularioHabilitado));

                //ValidarSocio();

                // Actualizamos el estado de los botones
                GuardarCommand.RaiseCanExecuteChanged();
                EliminarCommand.RaiseCanExecuteChanged();
                //ReservarParaSocioCommand.RaiseCanExecuteChanged();
            }
        }

        // Comandos que se enlazarán a los botones
        public RelayCommand NuevoCommand { get; }
        public RelayCommand GuardarCommand { get; }
        public RelayCommand EliminarCommand { get; }
        public RelayCommand ReservarParaSocioCommand { get; }

        public event Action<int> AbrirReservasParaSocioRequested;


        /// <summary>
        /// Constructor.
        /// Se ejecuta cuando se abre la ventana.
        /// </summary>
        public SociosViewModel()
        {
            // Creamos los repositorios necesarios
            _sociosRepository = new SociosRepository();

            _reservasRepository = new ReservasRepository();

            // Cargamos los socios al iniciar
            CargarSocios();

            // Inicializamos los comandos
            NuevoCommand = new RelayCommand(NuevoSocio);
            GuardarCommand = new RelayCommand(GuardarSocio, PuedeGuardar);
            EliminarCommand = new RelayCommand(EliminarSocio, PuedeEliminar);

            ReservarParaSocioCommand = new RelayCommand(
                AbrirReservasParaSocio,
                PuedeReservar
            );

        }

        /// <summary>
        /// Abre una nueva reserva para el socio seleccionado
        /// </summary>
        private void AbrirReservasParaSocio()
        {
            try
            {
                bool ok = true;
                // Si no hay socio, mostramos mensaje. No tiene que saltar nunca
                // pero lo he puesto por si acaso
                // El tema está en que el botón está inhabilitado aunque visualmente no lo parezca
                if (SocioSeleccionado == null)
                {

                    MessageBox.Show(
                        "Debes seleccionar un socio para poder crear la reserva.",
                        "No permitido",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);

                    ok = false;

                }

                // Avisamos a la vista: "abre reservas con este socio"
                if (ok) AbrirReservasParaSocioRequested?.Invoke(SocioSeleccionado.Id);
            }
            catch (Exception ex) {
                MessageBox.Show(ex.Message,"Error abrir reserva",MessageBoxButton.OK,MessageBoxImage.Error);
            }
        }


        /// <summary>
        /// Carga los socios desde la base de datos.
        /// </summary>
        private void CargarSocios()
        {
            ListaSocios = new ObservableCollection<Socios>(_sociosRepository.GetAll());

            OnPropertyChanged(nameof(ListaSocios));
        }

        /// <summary>
        /// Prepara un socio nuevo para editar.
        /// No se asignan valores por defecto.
        /// </summary>
        private void NuevoSocio()
        {
            SocioSeleccionado = new Socios
            {
                Activo = true
            };
        }

        /// <summary>
        /// Guarda el socio (nuevo o existente) tras validar los datos.
        /// </summary>
        private void GuardarSocio()
        {
            try
            {
                bool ok = true;
                // Validar nombre obligatorio
                if (string.IsNullOrWhiteSpace(SocioSeleccionado.Nombre))
                {
                    MessageBox.Show(
                        "El nombre es obligatorio.",
                        "Validación",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);

                    ok = false;
                }

                // Validar email obligatorio
                if (ok && string.IsNullOrWhiteSpace(SocioSeleccionado.Email))
                {
                    MessageBox.Show(
                        "El email es obligatorio.",
                        "Validación",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);

                    ok = false;
                }

                // Validar formato del email
                if (ok && !EmailValido(SocioSeleccionado.Email))
                {
                    MessageBox.Show(
                        "El formato del email no es correcto.",
                        "Validación",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);

                    ok = false;
                }

                if (ok)
                {
                    // Guardar el socio
                    _sociosRepository.Save(SocioSeleccionado);

                    // Si es un socio nuevo, se añade a la lista
                    if (!ListaSocios.Contains(SocioSeleccionado))
                    {
                        ListaSocios.Add(SocioSeleccionado);
                    }

                    MessageBox.Show(
                        "Socio guardado correctamente.",
                        "Información",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);

                }

            }catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Error guardar socio", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        /// <summary>
        /// Elimina el socio seleccionado previa confirmación.
        /// </summary>
        private void EliminarSocio()
        {
            try
            {
                bool ok = true;

                // Comprobación existencia de reservas con el socio que se va a borrar
                bool conflicto = _reservasRepository.ExisteReservaConSocio(SocioSeleccionado.Id);

                // Si existe conflicto: Mostrar mensaje y no continuar
                if (conflicto)
                {

                    MessageBox.Show(
                        "No se pueden borrar socios que esten asignados a reservas",
                        "Conflicto",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);

                    ok = false;

                }

                if (ok)
                {
                    // Confirmación al usuario
                    var resultado = MessageBox.Show(
                    "¿Seguro que quieres eliminar este socio?",
                    "Confirmar eliminación",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                    // Si el usuario acepta, borramos
                    if (resultado == MessageBoxResult.Yes)
                    {
                        // Eliminamos
                        _sociosRepository.Delete(SocioSeleccionado);
                        ListaSocios.Remove(SocioSeleccionado);
                        SocioSeleccionado = null;
                    }
                }

            }catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Error eliminar socio", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }


        /// <summary>
        /// Indica si se puede guardar.
        /// Se usa para habilitar o deshabilitar el botón Guardar.
        /// </summary>
        private bool PuedeGuardar()
        {
            //ValidarSocio();
            return SocioSeleccionado != null; // && !TieneErrores;
        }

        /// <summary>
        /// Indica si se puede eliminar.
        /// </summary>
        private bool PuedeEliminar()
        {
            return SocioSeleccionado != null && SocioSeleccionado.Id > 0;
        }

        /// <summary>
        /// Indica si se puede eliminar.
        /// </summary>
        private bool PuedeReservar()
        {
            return SocioSeleccionado != null && SocioSeleccionado.Id > 0;
        }

        /// <summary>
        /// Comprueba si el email tiene un formato válido.
        /// </summary>
        public bool EmailValido(string email)
        {
            // Expresión básica para validar un email
            string patron = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            return Regex.IsMatch(email, patron);
        }

        /// <summary>
        /// Valida los datos del socio seleccionado.
        /// </summary>
        //private void ValidarSocio()
        //{
        //    LimpiarErrores();

        //    if (SocioSeleccionado == null)
        //        return;

        //    // Nombre obligatorio
        //    if (string.IsNullOrWhiteSpace(SocioSeleccionado.Nombre))
        //    {
        //        //SetError(nameof(SocioSeleccionado.Nombre),"El nombre es obligatorio.");
        //        SetError(ERROR_NOMBRE, "El nombre es obligatorio.");
        //    }
        //    else
        //    {
        //        //ClearError(nameof(SocioSeleccionado.Nombre));
        //        ClearError(ERROR_NOMBRE);
        //    }

        //    // Email obligatorio
        //    if (string.IsNullOrWhiteSpace(SocioSeleccionado.Email))
        //    {
        //        //SetError(nameof(SocioSeleccionado.Email),"El email es obligatorio.");
        //        SetError(ERROR_EMAIL, "El email es obligatorio.");
        //    }
        //    else if (!EmailValido(SocioSeleccionado.Email))
        //    {
        //        //SetError(nameof(SocioSeleccionado.Email),"El formato del email no es correcto.");
        //        SetError(ERROR_EMAIL, "El formato del email no es correcto.");
        //    }
        //    else
        //    {
        //        //ClearError(nameof(SocioSeleccionado.Email));
        //        ClearError(ERROR_EMAIL);
        //    }

        //    GuardarCommand.RaiseCanExecuteChanged();
        //}


    }
}
