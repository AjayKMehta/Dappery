using Dappery.Core.Extensions;
using Dappery.Domain.Dtos;
using Dappery.Domain.Dtos.Beer;
using Dappery.Domain.Dtos.Brewery;
using Dappery.Domain.Entities;

using Shouldly;

namespace Dappery.Core.Tests.Extensions;

internal sealed class BeerExtensionsTest
{
    [Test]
    public void ToBeerDtoGivenValidBeerReturnsMappedBeerDtoWithoutBreweryBeerListAndCount()
    {
        // Arrange
        var beerToMap = new Beer
        {
            Id = 1,
            Name = "Test Beer",
            BeerStyle = BeerStyle.Ipa,
            Brewery = new Brewery
            {
                Id = 1,
                Name = "Test Brewery",
                Address = new Address
                {
                    City = "Redding",
                    State = "CA",
                    ZipCode = "96002",
                    StreetAddress = "123 Redding St."
                }
            }
        };

        // Act
        var mappedBeer = beerToMap.ToBeerDto();

        // Assert
        BeerDto beerDto = mappedBeer.ShouldNotBeNull();
        beerDto.Id.ShouldBe(beerToMap.Id);
        beerDto.Name.ShouldBe(beerToMap.Name);
        beerDto.Style.ShouldBe(beerToMap.BeerStyle.ToString());

        BreweryDto? breweryDto = beerDto.Brewery.ShouldNotBeNull();
        breweryDto.Id.ShouldBe(beerToMap.Brewery.Id);
        breweryDto.Name.ShouldBe(beerToMap.Brewery.Name);
        // Validate the beers were not recursively mapped
        breweryDto.Beers.ShouldBeNull();
        breweryDto.BeerCount.ShouldBeNull();

        AddressDto? addressDto = breweryDto.Address.ShouldNotBeNull();
        addressDto.City.ShouldBe(beerToMap.Brewery.Address?.City);
        addressDto.State.ShouldBe(beerToMap.Brewery.Address?.State);
        addressDto.ZipCode.ShouldBe(beerToMap.Brewery.Address?.ZipCode);
        addressDto.StreetAddress.ShouldBe(beerToMap.Brewery.Address?.StreetAddress);
    }

    [Test]
    public void ToBeerDtoGivenValidBeerWithoutBreweryReturnsMappedBeerDtoWithoutBrewery()
    {
        // Arrange
        var beerToMap = new Beer
        {
            Id = 1,
            Name = "Test Beer",
            BeerStyle = BeerStyle.Ipa,
        };

        // Act
        var mappedBeer = beerToMap.ToBeerDto();

        // Assert
        BeerDto? beerDto = mappedBeer.ShouldNotBeNull();
        beerDto.Id.ShouldBe(beerToMap.Id);
        beerDto.Name.ShouldBe(beerToMap.Name);
        beerDto.Style.ShouldBe(beerToMap.BeerStyle.ToString());
        beerDto.Brewery.ShouldBeNull();
    }
}
