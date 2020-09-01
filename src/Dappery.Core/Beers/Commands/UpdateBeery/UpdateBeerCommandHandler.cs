using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Dappery.Core.Data;
using Dappery.Domain.Entities;
using Dappery.Domain.Media;
using Dappery.Core.Exceptions;
using Dappery.Core.Extensions;
using MediatR;

namespace Dappery.Core.Beers.Commands.UpdateBeery
{
    public class UpdateBeerCommandHandler : IRequestHandler<UpdateBeerCommand, BeerResource>
    {
        private readonly IUnitOfWork unitOfWork;

        public UpdateBeerCommandHandler(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public async Task<BeerResource> Handle(UpdateBeerCommand request, CancellationToken cancellationToken)
        {
            // First validate both the beer
            var existingBeer = await this.unitOfWork.BeerRepository.GetBeerByIdAsync(request.BeerId, cancellationToken);

            // Invalidate the request if no beer was found
            if (existingBeer is null)
            {
                throw new DapperyApiException($"Beer with ID {request.BeerId} was not found", HttpStatusCode.NotFound);
            }

            // Attempt to parse the incoming BeerStyle enumeration value (exactly how we parse in the CreateBeerCommandHandler)
            var parsedBeerStyle = Enum.TryParse(request.Dto.Style, true, out BeerStyle beerStyle);

            // Update the fields on the existing beer
            existingBeer.BeerStyle = parsedBeerStyle ? beerStyle : BeerStyle.Other;
            existingBeer.Name = request.Dto.Name;
            existingBeer.UpdatedAt = DateTime.UtcNow;

            // If the user wants to update the brewery the beer is attached to, verify first that it exists
            if (request.Dto.BreweryId.HasValue)
            {
                // Retrieve the brewery, or invalidate the request if none is returned
                var existingBrewery = await this.unitOfWork.BreweryRepository.GetBreweryById(request.Dto.BreweryId.Value, cancellationToken);

                if (existingBrewery is null)
                {
                    throw new DapperyApiException($"Cannot update brewery as brewery with ID {request.Dto.BreweryId.Value} does not exists", HttpStatusCode.BadRequest);
                }

                existingBeer.BreweryId = request.Dto.BreweryId.Value;
            }

            // Perform the update and grab the newly return beer for the response
            await this.unitOfWork.BeerRepository.UpdateBeerAsync(existingBeer, cancellationToken);
            var updatedBeer = await this.unitOfWork.BeerRepository.GetBeerByIdAsync(existingBeer.Id, cancellationToken);
            this.unitOfWork.Commit();

            // Return the response with the updated beer
            return new BeerResource(updatedBeer.ToBeerDto());
        }
    }
}
