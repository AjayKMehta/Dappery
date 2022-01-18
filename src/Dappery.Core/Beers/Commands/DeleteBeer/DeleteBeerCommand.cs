using MediatR;

namespace Dappery.Core.Beers.Commands.DeleteBeer;

public class DeleteBeerCommand : IRequest<Unit>
{
    public DeleteBeerCommand(int beerId) => this.BeerId = beerId;

    public int BeerId { get; }
}
