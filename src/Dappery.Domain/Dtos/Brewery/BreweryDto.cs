using System.Collections.Generic;

namespace Dappery.Domain.Dtos
{
    public class BreweryDto
    {
        public int Id { get; set; }

        public string? Name { get; set; }

        public AddressDto? Address { get; set; }

        public IEnumerable<BeerDto>? Beers { get; set; }

        public int? BeerCount { get; set; }
    }
}
