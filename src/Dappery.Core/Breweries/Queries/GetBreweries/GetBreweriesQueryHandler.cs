using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dappery.Core.Data;
using Dappery.Core.Extensions;
using Dappery.Domain.Media;
using MediatR;

namespace Dappery.Core.Breweries.Queries.GetBreweries
{
    public class GetBreweriesQueryHandler : IRequestHandler<GetBreweriesQuery, BreweryResourceList>
    {
        private readonly IUnitOfWork unitOfWork;

        public GetBreweriesQueryHandler(IUnitOfWork unitOfWork) => this.unitOfWork = unitOfWork;

        public async Task<BreweryResourceList> Handle(GetBreweriesQuery request, CancellationToken cancellationToken)
        {
            // Retrieve the breweries and clean up our resources
            var breweries = await this.unitOfWork.BreweryRepository.GetAllBreweries(cancellationToken).ConfigureAwait(false);
            this.unitOfWork.Commit();

            // Map our breweries from the returned query
            var mappedBreweries = breweries.Select(b => b.ToBreweryDto());

            // Map each brewery to its corresponding DTO
            return new BreweryResourceList(mappedBreweries);
        }
    }
}
