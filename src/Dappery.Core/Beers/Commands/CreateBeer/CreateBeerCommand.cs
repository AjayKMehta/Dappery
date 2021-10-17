using Dappery.Domain.Dtos.Beer;
using Dappery.Domain.Media;
using MediatR;

namespace Dappery.Core.Beers.Commands.CreateBeer
{
    public class CreateBeerCommand : IRequest<BeerResource>
    {
        public CreateBeerCommand(CreateBeerDto beerDto) => this.Dto = beerDto;

        public CreateBeerDto Dto { get; }
    }
}
