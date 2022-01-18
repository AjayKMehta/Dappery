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
    private readonly IUnitOfWork unitOfWork;

    public DeleteBeerCommandHandler(IUnitOfWork unitOfWork) => this.unitOfWork = unitOfWork;

    public async Task<Unit> Handle(DeleteBeerCommand request, CancellationToken cancellationToken)
    {
        if (request is null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        // Retrieve the beer from the request
        var existingBeer = await this.unitOfWork.BeerRepository.GetBeerByIdAsync(request.BeerId, cancellationToken).ConfigureAwait(false);

        // Invalidate the request if no beer is found
        if (existingBeer is null)
            throw new DapperyApiException($"No beer found with ID {request.BeerId}", HttpStatusCode.NotFound);

        // Remove the beer from the database
        await this.unitOfWork.BeerRepository.DeleteBeerAsync(request.BeerId, cancellationToken).ConfigureAwait(false);
        this.unitOfWork.Commit();

        return Unit.Value;
    }
}
