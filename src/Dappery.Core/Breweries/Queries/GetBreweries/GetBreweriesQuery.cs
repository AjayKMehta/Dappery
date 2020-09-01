using Dappery.Domain.Media;
using MediatR;

namespace Dappery.Core.Breweries.Queries.GetBreweries
{
    public class GetBreweriesQuery : IRequest<BreweryResourceList>
    {
    }
}
