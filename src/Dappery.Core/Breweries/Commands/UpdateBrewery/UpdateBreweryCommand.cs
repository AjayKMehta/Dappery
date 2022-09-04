using Dappery.Domain.Dtos.Brewery;
using Dappery.Domain.Media;

using MediatR;

namespace Dappery.Core.Breweries.Commands.UpdateBrewery;

public class UpdateBreweryCommand : IRequest<BreweryResource>
{
    public UpdateBreweryCommand(UpdateBreweryDto dto, int breweryId) => (Dto, BreweryId) = (dto, breweryId);

    public int BreweryId { get; }

    public UpdateBreweryDto Dto { get; }
}
