using System.Net;
using System.Threading;
using System.Threading.Tasks;

using Dappery.Core.Data;
using Dappery.Core.Exceptions;

using MediatR;

namespace Dappery.Core.Breweries.Commands.DeleteBrewery
{
    public class DeleteBreweryCommandHandler : IRequestHandler<DeleteBreweryCommand, Unit>
    {
        private readonly IUnitOfWork unitOfWork;

        public DeleteBreweryCommandHandler(IUnitOfWork unitOfWork) => this.unitOfWork = unitOfWork;

        public async Task<Unit> Handle(DeleteBreweryCommand request, CancellationToken cancellationToken)
        {
            // Retrieve the brewery and invalidate the request if none is found
            var breweryToDelete = await this.unitOfWork.BreweryRepository.GetBreweryById(request.BreweryId, cancellationToken).ConfigureAwait(false);

            // Invalidate the request if no brewery is found
            if (breweryToDelete is null)
                throw new DapperyApiException($"No brewery was found with ID {request.BreweryId}", HttpStatusCode.NotFound);

            // Delete the brewery from the database and clean up our resources once we know we have a valid beer
            await this.unitOfWork.BreweryRepository.DeleteBrewery(request.BreweryId, cancellationToken).ConfigureAwait(false);
            this.unitOfWork.Commit();

            return Unit.Value;
        }
    }
}
