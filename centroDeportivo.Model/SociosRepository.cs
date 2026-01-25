using centroDeportivo.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

public class SociosRepository : RepositoryBase
{
    /// <summary>
    /// Obtener todos los socios
    /// </summary>
    /// <returns></returns>
    public List<Socios> GetAll()
    {
        return Context.Socios.ToList();
    }

    /// <summary>
    /// Guardar socio ya sea editando o añadiendo
    /// Depende de si existe o no el id
    /// </summary>
    /// <param name="so"></param>
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
        catch (Exception ex) {
            throw new Exception("Error guardar socio BBDD.", ex);
        }
          
    }

    /// <summary>
    /// Eliminar socio
    /// </summary>
    /// <param name="socio"></param>
    /// <exception cref="Exception"></exception>
    public void Delete(Socios socio)
    {
        try { 
            Context.Socios.Remove(socio); // Marcar para eliminación
            Context.SaveChanges(); // Ejecutar los cambios
        }
        catch (Exception ex)
        {
            throw new Exception("Error borrar socio BBDD.", ex);
        }
    }


}

