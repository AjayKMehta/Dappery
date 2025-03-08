using System;
using System.Threading;

using Dappery.Core.Data;

namespace Dappery.Data.Tests;

internal class TestFixture : IDisposable
{
    protected internal TestFixture() => UnitOfWork = new UnitOfWork(null);

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
