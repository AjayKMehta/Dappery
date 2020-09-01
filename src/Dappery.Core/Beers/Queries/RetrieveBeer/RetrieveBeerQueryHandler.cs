using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Dappery.Core.Data;
using Dappery.Core.Exceptions;
using Dappery.Core.Extensions;
using Dappery.Domain.Media;
using MediatR;

namespace Dappery.Core.Beers.Queries.RetrieveBeer
{
    public class RetrieveBeerQueryHandler : IRequestHandler<RetrieveBeerQuery, BeerResource>
    {
        private readonly IUnitOfWork unitOfWork;

        public RetrieveBeerQueryHandler(IUnitOfWork unitOfWork) => this.unitOfWork = unitOfWork;

        public async Task<BeerResource> Handle(RetrieveBeerQuery request, CancellationToken cancellationToken)
        {
            // Grab the beer from the ID
            var beer = await this.unitOfWork.BeerRepository.GetBeerByIdAsync(request.Id, cancellationToken).ConfigureAwait(false);

            // Invalidate the request if no beer is found
            if (beer is null)
            {
                throw new DapperyApiException($"No beer found with ID {request.Id}", HttpStatusCode.NotFound);
            }

            // Commit the query and clean up our resources
            this.unitOfWork.Commit();

            // Map and return the query
            return new BeerResource(beer.ToBeerDto());
        }
    }
}
