using Dappery.Domain.Media;

using MediatR;

namespace Dappery.Core.Breweries.Queries.RetrieveBrewery;

public class RetrieveBreweryQuery : IRequest<BreweryResource>
{
    public RetrieveBreweryQuery(int id) => Id = id;

    public int Id { get; }
}
