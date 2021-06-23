using System;
using Dappery.Domain.Dtos;
using Dappery.Domain.Entities;

namespace Dappery.Core.Extensions
{
    public static class BeerExtensions
    {
        public static BeerDto ToBeerDto(this Beer beer) =>
        (beer is null) ?
        throw new ArgumentNullException(nameof(beer)) :
        new()
        {
            Id = beer.Id,
            Name = beer.Name,
            Style = beer.BeerStyle.ToString(),
            Brewery = beer.Brewery?.ToBreweryDto(false),
        };
    }
}
