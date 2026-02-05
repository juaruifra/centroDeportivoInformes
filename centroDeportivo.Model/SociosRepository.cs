using centroDeportivo.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;



/// <summary>
/// Repositorio para gestionar las operaciones de acceso a datos de Socios.
/// Permite consultar, crear, actualizar y eliminar socios en la base de datos.
/// </summary>
namespace centroDeportivo.Model.Repositories
{
    /// <summary>
    /// Repositorio para gestionar las operaciones de acceso a datos de Socios.
    /// Permite consultar, crear, actualizar y eliminar socios en la base de datos.
    /// </summary>
    public class SociosRepository : RepositoryBase
    {
        /// <summary>
        /// Obtener todos los socios
        /// </summary>
        /// <returns>Todos los socios</returns>
        public List<Socios> GetAll()
        {
            return Context.Socios.ToList();
        }

        /// <summary>
        /// Guardar socio ya sea editando o añadiendo
        /// Depende de si existe o no el id
        /// </summary>
        /// <param name="so"> Objeto socio a guardar</param>
        /// <exception cref="Exception"></exception>
        public void Save(Socios so)
        {
            try
            {
                if (so.Id < 1)
                {
                    Context.Socios.Add(so);  // Insertar
                }
                else
                {
                    // Actualizar
                    var s = Context.Socios.Find(so.Id);

                    // Modificamos los datos
                    s.Nombre = so.Nombre;
                    s.Email = so.Email;
                    s.Activo = so.Activo;

                }

                Context.SaveChanges(); // Guardar cambios 
            }
            catch (Exception ex)
            {
                throw new Exception("Error guardar socio BBDD.", ex);
            }

        }

        /// <summary>
        /// Eliminar socio
        /// </summary>
        /// <param name="socio">Objeto socio a eliminar</param>
        /// <exception cref="Exception"></exception>
        public void Delete(Socios socio)
        {
            try
            {
                Context.Socios.Remove(socio); // Marcar para eliminación
                Context.SaveChanges(); // Ejecutar los cambios
            }
            catch (Exception ex)
            {
                throw new Exception("Error borrar socio BBDD.", ex);
            }
        }
    }
}

