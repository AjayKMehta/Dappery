namespace Dappery.Domain.Dtos
{
    public record AddressDto
    {
        public string? StreetAddress { get; init; }

        public string? City { get; init; }

        public string? State { get; init; }

        public string? ZipCode { get; init; }
    }
}
