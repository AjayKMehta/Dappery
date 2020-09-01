using System.Linq;
using Dappery.Domain.Dtos;
using Dappery.Domain.Dtos.Beer;
using Dappery.Domain.Dtos.Brewery;
using Dappery.Domain.Entities;

namespace Dappery.Core.Extensions
{
    public static class BreweryExtensions
    {
        public static BreweryDto ToBreweryDto(this Brewery brewery, bool includeBeerList = true) =>
        new BreweryDto
        {
            Id = brewery.Id,
            Name = brewery.Name,
            Beers = includeBeerList ? brewery.Beers.Select(b => new BeerDto
            {
                Id = b.Id,
                Name = b.Name,
                Style = b.BeerStyle.ToString()
            }) : default,
            Address = new AddressDto
            {
                City = brewery.Address?.City,
                State = brewery.Address?.State,
                StreetAddress = brewery.Address?.StreetAddress,
                ZipCode = brewery.Address?.ZipCode
            },
            BeerCount = includeBeerList ? brewery.BeerCount : (int?)null
        };
    }
}
