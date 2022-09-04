using System;
using System.Threading;

using Dappery.Core.Data;
using Dappery.Data;

namespace Dappery.Core.Tests;

public class TestFixture : IDisposable
{
    public TestFixture() => UnitOfWork = new UnitOfWork(null);

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
        {
            UnitOfWork.Dispose();
        }
    }
}
