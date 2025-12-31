using System.Collections.Generic;

using Dappery.Domain.Dtos.Beer;

namespace Dappery.Domain.Dtos.Brewery;

public class BreweryDto
{
    public int Id { get; init; }

    public string? Name { get; init; }

    public AddressDto? Address { get; init; }

    public IEnumerable<BeerDto>? Beers { get; init; }

    public int? BeerCount { get; init; }
}
