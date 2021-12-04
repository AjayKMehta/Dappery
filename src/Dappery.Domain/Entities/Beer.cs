namespace Dappery.Domain.Entities
{
    public record Beer : TimeStampedEntity
    {
        public string? Name { get; init; }

        public BeerStyle BeerStyle { get; init; }

        public int BreweryId { get; init; }

        public Brewery? Brewery { get; init; }
    }
}
