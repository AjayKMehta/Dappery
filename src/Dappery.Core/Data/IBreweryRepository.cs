using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Dappery.Domain.Entities;

namespace Dappery.Core.Data
{
    public interface IBreweryRepository
    {
        Task<IEnumerable<Brewery>> GetAllBreweries(CancellationToken cancellationToken);

        Task<Brewery> GetBreweryById(int id, CancellationToken cancellationToken);

        Task<int> CreateBrewery(Brewery brewery, CancellationToken cancellationToken);

        Task UpdateBrewery(Brewery brewery, CancellationToken cancellationToken, bool updateAddress = false);

        Task DeleteBrewery(int breweryId, CancellationToken cancellationToken);
    }
}
