using System;
using System.Threading;
using Dappery.Data;
using Dappery.Core.Data;

namespace Dappery.Core.Tests
{
    public class TestFixture : IDisposable
    {
        public TestFixture()
        {
            // Initialize our test database with our seed data
            this.UnitOfWork = new UnitOfWork(null);
        }

        protected IUnitOfWork UnitOfWork { get; }

        protected static CancellationToken CancellationTestToken => CancellationToken.None;

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.UnitOfWork.Dispose();
            }
        }
    }
}
