using Dappery.Domain.Dtos.Beer;
using Dappery.Domain.Media;
using MediatR;

namespace Dappery.Core.Beers.Commands.UpdateBeery
{
    public class UpdateBeerCommand : IRequest<BeerResource>
    {
        public UpdateBeerCommand(UpdateBeerDto beerDto, int requestId) => (this.Dto, this.BeerId) = (beerDto, requestId);

        public UpdateBeerDto Dto { get; }

        public int BeerId { get; }
    }
}
