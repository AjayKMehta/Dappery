using Dappery.Domain.Dtos.Brewery;

namespace Dappery.Domain.Dtos.Beer;

public class BeerDto
{
    public int Id { get; init; }

    public string? Name { get; init; }

    public string? Style { get; init; }

    public BreweryDto? Brewery { get; init; }
}
