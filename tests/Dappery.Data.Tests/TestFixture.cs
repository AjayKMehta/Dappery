using System;
using System.Threading;

using Dappery.Core.Data;

namespace Dappery.Data.Tests;

public class TestFixture : IDisposable
{
    protected TestFixture() => UnitOfWork = new UnitOfWork(null);

    protected IUnitOfWork UnitOfWork { get; }

    protected static CancellationToken CancellationTestToken => CancellationToken.None;

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
            UnitOfWork.Dispose();
    }
}
