using Dappery.Domain.Dtos.Brewery;
using Dappery.Domain.Media;
using MediatR;

namespace Dappery.Core.Breweries.Commands.CreateBrewery
{
    public class CreateBreweryCommand : IRequest<BreweryResource>
    {
        public CreateBreweryCommand(CreateBreweryDto dto) => this.Dto = dto;

        public CreateBreweryDto Dto { get; }
    }
}
