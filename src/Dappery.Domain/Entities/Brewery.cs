using System.Collections.Generic;

namespace Dappery.Domain.Entities
{
    public record Brewery : TimeStampedEntity
    {
        public Brewery() => this.Beers = new List<Beer>();

        public string? Name { get; init; }

        public Address? Address { get; init; }

        public ICollection<Beer> Beers { get; }

        public int BeerCount => this.Beers.Count;
    }
}
