using System.Collections.Generic;

namespace Dappery.Domain.Entities;

public class Brewery : TimeStampedEntity
{
    public Brewery() => Beers = [];

    public string? Name { get; set; }

    public Address? Address { get; set; }

    public ICollection<Beer> Beers { get; }

    public int BeerCount => Beers.Count;
}
