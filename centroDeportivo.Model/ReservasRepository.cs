using centroDeportivo.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

public class ReservasRepository : RepositoryBase
{
    /// <summary>
    /// Obtener reservas enlanzandolas con los socios y las actividades para mostrar los nombres
    /// </summary>
    /// <returns></returns>
    public List<Reservas> GetAll()
    {
        return Context.Reservas
            .Include(r => r.Socios)
            .Include(r => r.Actividades)
            .ToList();
    }

    //public int GetReservasCount(int actividadId, DateTime fecha)
    //{
    //    return Context.Reservas
    //        .Count(r =>
    //            r.ActividadId == actividadId &&
    //            DbFunctions.TruncateTime(r.Fecha) == fecha.Date);
    //}

    /// <summary>
    /// Obtenener las reservas que hay de una actividad
    /// a una fecha
    /// pudiendo excluir una reserva concreta para las ediciones
    /// </summary>
    /// <param name="actividadId"></param>
    /// <param name="fecha"></param>
    /// <param name="reservaIdExcluir"></param>
    /// <returns></returns>
    public int GetReservasCount(int actividadId, DateTime fecha, int? reservaIdExcluir)
    {
        var query = Context.Reservas.Where(r =>
            r.ActividadId == actividadId &&
            DbFunctions.TruncateTime(r.Fecha) == fecha.Date);

        // Si estamos editando, excluimos esa reserva
        if (reservaIdExcluir.HasValue)
        {
            query = query.Where(r => r.Id != reservaIdExcluir.Value);
        }

        return query.Count();
    }

    //public void Add(Reservas reserva)
    //{
    //    Context.Reservas.Add(reserva);
    //    Context.SaveChanges();
    //}

    /// <summary>
    /// Añadir o editar una reserva
    /// </summary>
    /// <param name="re"></param>
    /// <exception cref="Exception"></exception>
    public void Save(Reservas re)
    {
        try { 
            if (re.Id < 1)
            {
                // INSERTAR
                Context.Reservas.Add(new Reservas
                {
                    SocioId = re.SocioId,
                    ActividadId = re.ActividadId,
                    Fecha = re.Fecha
                });
            }
            else
            {
                // ACTUALIZAR
                //var r = Context.Reservas.Find(re.Id);

                // ACTUALIZAR: buscar con ESTE contexto
                var r = Context.Reservas.FirstOrDefault(x => x.Id == re.Id);

                if (r != null)
                {
                    r.SocioId = re.SocioId;
                    r.ActividadId = re.ActividadId;
                    r.Fecha = re.Fecha;
                }
            }

            Context.SaveChanges();
        }
        catch (Exception ex) {
            throw new Exception("Error guardar reserva BBDD.", ex);
        }
    }

    /// <summary>
    /// Eliminar una reserva
    /// </summary>
    /// <param name="reserva"></param>
    /// <exception cref="Exception"></exception>
    public void Delete(Reservas reserva)
    {
        try { 

            // Hacemos esto para evitar problemas de contexto
            // Obtenemos el objeto del mismo contexto
            // y borramos ese objeto
            var r = Context.Reservas.FirstOrDefault(x => x.Id == reserva.Id);

            if (r != null) { 
                Context.Reservas.Remove(r);
                Context.SaveChanges();
            }

        }
        catch (Exception ex) {
            throw new Exception("Error borrar reserva BBDD.", ex);
        }


    }
    /// <summary>
    /// Comprobar si existen reservas con un socio concreto
    /// </summary>
    /// <param name="socioId"></param>
    /// <returns></returns>
    public bool ExisteReservaConSocio(int socioId)
    {
        return Context.Reservas.Any(r => r.SocioId == socioId);
    }

    /// <summary>
    /// Comprobar si existen reserevas con un actividad concreta
    /// </summary>
    /// <param name="actividadId"></param>
    /// <returns></returns>
    public bool ExisteReservaConActividad(int actividadId)
    {
        return Context.Reservas.Any(r => r.ActividadId == actividadId);
    }




}

