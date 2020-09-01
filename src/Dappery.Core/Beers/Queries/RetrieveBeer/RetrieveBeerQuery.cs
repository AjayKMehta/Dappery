using Dappery.Domain.Media;
using MediatR;

namespace Dappery.Core.Beers.Queries.RetrieveBeer
{
    public class RetrieveBeerQuery : IRequest<BeerResource>
    {
        public RetrieveBeerQuery(int id) => this.Id = id;

        public int Id { get; }
    }
}
