namespace Dappery.Domain.Dtos.Brewery
{
    public record UpdateBreweryDto
    {
        public string? Name { get; init; }

        public AddressDto? Address { get; init; }
    }
}
