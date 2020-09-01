using Dappery.Domain.Dtos.Beer;
using Dappery.Domain.Entities;

namespace Dappery.Core.Extensions
{
    public static class BeerExtensions
    {
        public static BeerDto ToBeerDto(this Beer beer) =>
        new BeerDto
        {
            Id = beer.Id,
            Name = beer.Name,
            Style = beer.BeerStyle.ToString(),
            Brewery = beer.Brewery?.ToBreweryDto(false),
        };
    }
}
