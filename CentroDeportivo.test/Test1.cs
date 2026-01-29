using centroDeportivo.Model;
using CentroDeportivo.ViewModel;
using System;
using System.Linq;

namespace CentroDeportivo.test
{
    [TestClass]
    public sealed class Test1
    {
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

        [TestMethod]
        public void TestReservaFechaAnteriorNoPermitida()
        {
            // Crear el ViewModel de Reservas
            ReservasViewModel reservasVM = new ReservasViewModel();

            // Crear una nueva reserva con fecha anterior a hoy
            reservasVM.NuevaReserva = new Reservas
            {
                SocioId = 1,
                ActividadId = 1, 
                Fecha = DateTime.Today.AddDays(-1)  // Fecha de ayer
            };

            // Intentar guardar la reserva
            bool resultado = reservasVM.Guardar();

            // La operación debería fallar (devolver false)
            Assert.IsFalse(resultado, "No se debería permitir crear una reserva con fecha anterior a hoy");
        }

        [TestMethod]
        public void TestReservaFechaActualPermitida()
        {
            //  Crear el ViewModel de Reservas
            ReservasViewModel reservasVM = new ReservasViewModel();

            // Crear una nueva reserva con la fecha de hoy
            reservasVM.NuevaReserva = new Reservas
            {
                SocioId = 1,  
                ActividadId = 1,  
                Fecha = DateTime.Today  // Fecha de hoy
            };

            // Intentar guardar la reserva
            bool resultado = reservasVM.Guardar();

            // Assert: La operación debería tener éxito (devolver true) si hay aforo disponible
            Assert.IsTrue(resultado, "Se debería permitir crear una reserva con la fecha de hoy");
        }

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
            bool primeraReservaOk = reservasVM1.Guardar();
            
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
            bool segundaReservaOk = reservasVM2.Guardar();
            
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
