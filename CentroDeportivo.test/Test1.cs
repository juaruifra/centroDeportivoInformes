using centroDeportivo.Model;
using CentroDeportivo.ViewModel;
using System;
using System.Linq;

namespace CentroDeportivo.test
{
    [TestClass]
    public sealed class Test1
    {

        // PRIMER TEST: Validación de mail del socio
        [TestMethod]
        public void TestSociosMailValido()
        {
            // Creamos los dos socios. Uno con el  mail correcto y otro con el mail incorrecto
            Socios s = new Socios
            {
                Email = "usuario@dominio.com",
                Nombre = "Pedro",
                Activo = true
            };

            SociosViewModel SVM = new SociosViewModel();

            // comprobar que el email del socio es valido utilizando el metodo del SociosViewModel
            bool correcto = SVM.EmailValido(s.Email);

            // Generar el resultado del test
            Assert.IsTrue(correcto);

        }

        [TestMethod]
        public void TestSociosMailInvalido()
        {
            // Creamos socio con mail incorrecto
            Socios s = new Socios
            {
                Email = "prueba.com",
                Nombre = "Maria",
                Activo = true
            };

            SociosViewModel SVM = new SociosViewModel();

            // comprobar que el email del socio es valido utilizando el metodo del SociosViewModel
            bool incorrecto = SVM.EmailValido(s.Email);

            // Generar el resultado del test
            Assert.IsFalse(incorrecto);

        }

        // SEGUNDO TEST: Validación de fechas en reservas

        [TestMethod]
        public void TestReservaNoPermiteFechaAnterior()
        {
            
            // Crear repositorios para manipular la BD
            SociosRepository sociosRepo = new SociosRepository();
            ActividadesRepository actividadesRepo = new ActividadesRepository();
            ReservasRepository reservasRepo = new ReservasRepository();
            
            // Crear socio de prueba. Usamos los ticks para definir un identificador único
            Socios socioTest = new Socios
            {
                Nombre = "SOCIO_TEST_FECHA_" + DateTime.Now.Ticks,
                Email = "test_fecha_" + DateTime.Now.Ticks + "@test.com",
                Activo = true
            };
            sociosRepo.Save(socioTest);
            int socioTestId = socioTest.Id;
            
            // Crear actividad de prueba con aforo suficiente
            Actividades actividadTest = new Actividades
            {
                Nombre = "TEST_FECHA_" + DateTime.Now.Ticks,
                AforoMaximo = 10
            };
            actividadesRepo.Save(actividadTest);
            int actividadTestId = actividadTest.Id;
            
            
            // Intentar crear una reserva con fecha ANTERIOR a hoy (ayer)
            ReservasViewModel reservasVM = new ReservasViewModel();
            reservasVM.NuevaReserva = new Reservas
            {
                SocioId = socioTestId,
                ActividadId = actividadTestId,
                Fecha = DateTime.Today.AddDays(-1)
            };

            bool resultado = reservasVM.Guardar(true);
            
            // Test
            
            Assert.IsFalse(resultado, "No se debería permitir crear una reserva con fecha anterior a hoy");
            
            // Limpiar
            
            // Verificar que no se creó ninguna reserva con estos datos
            Reservas reservaCreada = reservasRepo.GetAll()
                .FirstOrDefault(r => r.SocioId == socioTestId && r.ActividadId == actividadTestId);
            
            // Si por algún error se creó la reserva, la eliminamos
            if (reservaCreada != null)
            {
                reservasRepo.Delete(reservaCreada);
            }
            
            // Borrar la actividad
            Actividades actividadABorrar = actividadesRepo.GetById(actividadTestId);
            if (actividadABorrar != null)
            {
                actividadesRepo.Delete(actividadABorrar);
            }
            
            // Borrar el socio
            Socios socioABorrar = sociosRepo.GetAll().FirstOrDefault(s => s.Id == socioTestId);
            if (socioABorrar != null)
            {
                sociosRepo.Delete(socioABorrar);
            }
        }

        // TERCER TEST: Validación superación aforo no permitido

        [TestMethod]
        public void TestReservaExcedeAforoActividad()
        {
          
            // Crear repositorios para manipular la BD
            SociosRepository sociosRepo = new SociosRepository();
            ActividadesRepository actividadesRepo = new ActividadesRepository();
            ReservasRepository reservasRepo = new ReservasRepository();
            
            // Crear socio de prueba. Usamos los ticks para definir un identificador unico
            Socios socioTest = new Socios
            {
                Nombre = "SOCIO_TEST_AFORO_" + DateTime.Now.Ticks,
                Email = "test_aforo_" + DateTime.Now.Ticks + "@test.com",
                Activo = true
            };
            sociosRepo.Save(socioTest);
            int socioTestId = socioTest.Id;

            // Crear actividad de prueba con AforoMaximo = 1.Usamos los ticks para definir un identificador unico
            Actividades actividadTest = new Actividades
            {
                Nombre = "TEST_AFORO_" + DateTime.Now.Ticks,
                AforoMaximo = 1
            };
            actividadesRepo.Save(actividadTest);
            int actividadTestId = actividadTest.Id;
            
            // Fecha de prueba. Añadimos un dia al de hoy para evitar ese error
            DateTime fechaPrueba = DateTime.Today.AddDays(1);
          
            // Crear PRIMERA reserva (debe tener éxito - hay 1 plaza disponible)
            ReservasViewModel reservasVM1 = new ReservasViewModel();
            reservasVM1.NuevaReserva = new Reservas
            {
                SocioId = socioTestId,
                ActividadId = actividadTestId,
                Fecha = fechaPrueba
            };
            bool primeraReservaOk = reservasVM1.Guardar(true);
            
            // Obtener el ID de la primera reserva para poder borrarla después
            Reservas primeraReserva = reservasRepo.GetAll()
                .Where(r => r.ActividadId == actividadTestId && r.SocioId == socioTestId && r.Fecha.Date == fechaPrueba.Date)
                .OrderByDescending(r => r.Id)
                .FirstOrDefault();
            
            int? primeraReservaId = primeraReserva?.Id;
                     
            // Crear SEGUNDA reserva (debe FALLAR - aforo completo)
            ReservasViewModel reservasVM2 = new ReservasViewModel();
            reservasVM2.NuevaReserva = new Reservas
            {
                SocioId = socioTestId,
                ActividadId = actividadTestId,
                Fecha = fechaPrueba
            };
            bool segundaReservaOk = reservasVM2.Guardar(true);
            
            // Comparación de los tests
            
            Assert.IsTrue(primeraReservaOk, "La primera reserva debería crearse correctamente porque hay plaza disponible");
            Assert.IsFalse(segundaReservaOk, "La segunda reserva debería fallar porque el aforo está completo (1/1)");
                      
            // Limpiar datos de prueba en orden

            if (primeraReservaId.HasValue)
            {
                Reservas reservaABorrar = reservasRepo.GetAll()
                    .FirstOrDefault(r => r.Id == primeraReservaId.Value);
                if (reservaABorrar != null)
                {
                    reservasRepo.Delete(reservaABorrar);
                }
            }
            
            // Borrar la actividad
            Actividades actividadABorrar = actividadesRepo.GetById(actividadTestId);
            if (actividadABorrar != null)
            {
                actividadesRepo.Delete(actividadABorrar);
            }
            
            // Borrar el socio
            Socios socioABorrar = sociosRepo.GetAll()
                .FirstOrDefault(s => s.Id == socioTestId);
            if (socioABorrar != null)
            {
                sociosRepo.Delete(socioABorrar);
            }
        }
    }
}
