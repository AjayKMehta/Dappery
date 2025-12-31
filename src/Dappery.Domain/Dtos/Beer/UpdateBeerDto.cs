namespace Dappery.Domain.Dtos.Beer;

public class UpdateBeerDto
{
    public string? Name { get; init; }

    public string? Style { get; init; }

    public int? BreweryId { get; init; }
}
