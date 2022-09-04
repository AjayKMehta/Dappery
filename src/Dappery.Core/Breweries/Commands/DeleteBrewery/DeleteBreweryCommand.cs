using MediatR;

namespace Dappery.Core.Breweries.Commands.DeleteBrewery;

public class DeleteBreweryCommand : IRequest<Unit>
{
    public DeleteBreweryCommand(int id) => BreweryId = id;

    public int BreweryId { get; }
}
