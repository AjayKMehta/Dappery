using Dappery.Domain.Dtos.Beer;
using Dappery.Domain.Media;

using MediatR;

namespace Dappery.Core.Beers.Commands.UpdateBeer;

public class UpdateBeerCommand : IRequest<BeerResource>
{
    public UpdateBeerCommand(UpdateBeerDto beerDto, int requestId) => (Dto, BeerId) = (beerDto, requestId);

    public UpdateBeerDto Dto { get; }

    public int BeerId { get; }
}
