using System.Collections.Generic;

namespace Dappery.Domain.Entities;

public class Brewery : TimeStampedEntity
{
    public Brewery() => this.Beers = new List<Beer>();

    public string? Name { get; set; }

    public Address? Address { get; set; }

    public ICollection<Beer> Beers { get; }

    public int BeerCount => this.Beers.Count;
}
