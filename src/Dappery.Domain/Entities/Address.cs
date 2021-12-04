namespace Dappery.Domain.Entities
{
    public record Address : TimeStampedEntity
    {
        public string? StreetAddress { get; init; }

        public string? City { get; init; }

        public string? State { get; init; }

        public string? ZipCode { get; init; }

        public int BreweryId { get; init; }
    }
}
