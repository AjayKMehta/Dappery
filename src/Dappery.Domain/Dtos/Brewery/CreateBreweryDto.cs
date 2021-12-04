namespace Dappery.Domain.Dtos.Brewery
{
    public record CreateBreweryDto
    {
        public string? Name { get; init; }

        public AddressDto? Address { get; init; }
    }
}
