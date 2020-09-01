using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Dappery.Core.Data;
using Dappery.Domain.Media;
using Dappery.Core.Exceptions;
using Dappery.Core.Extensions;
using MediatR;

namespace Dappery.Core.Breweries.Queries.RetrieveBrewery
{
    public class RetrieveBreweryQueryHandler : IRequestHandler<RetrieveBreweryQuery, BreweryResource>
    {
        private readonly IUnitOfWork unitOfWork;

        public RetrieveBreweryQueryHandler(IUnitOfWork unitOfWork) => this.unitOfWork = unitOfWork;

        public async Task<BreweryResource> Handle(RetrieveBreweryQuery request, CancellationToken cancellationToken)
        {
            // Retrieve the brewery and clean up our resources
            var brewery = await this.unitOfWork.BreweryRepository.GetBreweryById(request.Id, cancellationToken).ConfigureAwait(false);
            this.unitOfWork.Commit();

            // Invalidate the request if no brewery is found
            if (brewery is null)
            {
                throw new DapperyApiException($"No brewery found with ID {request.Id}", HttpStatusCode.NotFound);
            }

            // Map and return the brewery
            return new BreweryResource(brewery.ToBreweryDto());
        }
    }
}
