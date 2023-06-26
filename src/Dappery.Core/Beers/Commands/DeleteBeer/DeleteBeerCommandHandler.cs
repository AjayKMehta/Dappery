using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

using Dappery.Core.Data;
using Dappery.Core.Exceptions;

using MediatR;

namespace Dappery.Core.Beers.Commands.DeleteBeer;

public class DeleteBeerCommandHandler : IRequestHandler<DeleteBeerCommand, Unit>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteBeerCommandHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<Unit> Handle(DeleteBeerCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        // Retrieve the beer from the request
        Domain.Entities.Beer? existingBeer = await _unitOfWork.BeerRepository.GetBeerByIdAsync(request.BeerId, cancellationToken).ConfigureAwait(false) ?? throw new DapperyApiException($"No beer found with ID {request.BeerId}", HttpStatusCode.NotFound);

        // Remove the beer from the database
        await _unitOfWork.BeerRepository.DeleteBeerAsync(request.BeerId, cancellationToken).ConfigureAwait(false);
        _unitOfWork.Commit();

        return Unit.Value;
    }
}
