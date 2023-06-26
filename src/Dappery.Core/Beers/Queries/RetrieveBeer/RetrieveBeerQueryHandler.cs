using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

using Dappery.Core.Data;
using Dappery.Core.Exceptions;
using Dappery.Core.Extensions;
using Dappery.Domain.Media;

using MediatR;

namespace Dappery.Core.Beers.Queries.RetrieveBeer;

public class RetrieveBeerQueryHandler : IRequestHandler<RetrieveBeerQuery, BeerResource>
{
    private readonly IUnitOfWork _unitOfWork;

    public RetrieveBeerQueryHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<BeerResource> Handle(RetrieveBeerQuery request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        // Grab the beer from the ID
        Domain.Entities.Beer? beer = await _unitOfWork.BeerRepository.GetBeerByIdAsync(request.Id, cancellationToken).ConfigureAwait(false) ?? throw new DapperyApiException($"No beer found with ID {request.Id}", HttpStatusCode.NotFound);

        // Commit the query and clean up our resources
        _unitOfWork.Commit();

        // Map and return the query
        return new BeerResource(beer.ToBeerDto());
    }
}
