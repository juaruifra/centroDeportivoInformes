using centroDeportivo.Model;
using System;
using System.Data.Entity;

public abstract class RepositoryBase : IDisposable
{
    protected CentroDeportivoEntities Context;

    /// <summary>
    /// Repositorio base para que todos los repositorios compartan el mismo contexto
    /// y no haya que definirlo en cada uno de los repositiorios
    /// </summary>
    protected RepositoryBase()
    {
        Context = new CentroDeportivoEntities();
    }

    public void Dispose()
    {
        Context.Dispose();
    }
}

