using System;

namespace Dappery.Core.Data;

public interface IUnitOfWork : IDisposable
{
    IBeerRepository BeerRepository { get; }

    IBreweryRepository BreweryRepository { get; }

    void Commit();
}
