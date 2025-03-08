using System.Collections.Generic;
using System.Diagnostics;

using Dappery.Core.Extensions;
using Dappery.Domain.Dtos;
using Dappery.Domain.Dtos.Beer;
using Dappery.Domain.Dtos.Brewery;
using Dappery.Domain.Entities;

using Shouldly;

namespace Dappery.Core.Tests.Extensions;

internal sealed class BreweryExtensionsTest
{
    [Test]
    public void ToBreweryDtoGivenValidBreweryWithAListOfBeersReturnsMappedBreweryDto()
    {
        // Arrange
        var breweryToMap = new Brewery
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
        };

        // Add our beers since they are initialized within the domain
        breweryToMap.Beers.Add(new Beer { Id = 1, Name = "Test Beer 1", BeerStyle = BeerStyle.Lager });
        breweryToMap.Beers.Add(new Beer { Id = 2, Name = "Test Beer 2", BeerStyle = BeerStyle.Ipa });
        breweryToMap.Beers.Add(new Beer { Id = 3, Name = "Test Beer 3", BeerStyle = BeerStyle.DoubleIpa });

        // Act
        var mappedBrewery = breweryToMap.ToBreweryDto();

        // Assert
        BreweryDto breweryDto = mappedBrewery.ShouldNotBeNull();
        breweryDto.Id.ShouldBe(breweryToMap.Id);
        breweryDto.Name.ShouldBe(breweryToMap.Name);
        breweryDto.BeerCount.ShouldNotBeNull().ShouldBe(3);

        AddressDto addressDto = breweryDto.Address.ShouldNotBeNull();
        addressDto.City.ShouldBe(breweryToMap.Address?.City);
        addressDto.State.ShouldBe(breweryToMap.Address?.State);
        addressDto.StreetAddress.ShouldBe(breweryToMap.Address?.StreetAddress);
        addressDto.ZipCode.ShouldBe(breweryToMap.Address?.ZipCode);

        IEnumerable<BeerDto> beers = mappedBrewery.Beers.ShouldNotBeNull();
        beers.ShouldNotBeEmpty();

        BeerDto? firstBeer = null;
        BeerDto? secondBeer = null;
        BeerDto? thirdBeer = null;
        foreach (BeerDto beer in beers)
        {
            switch (beer.Id)
            {
                case 1:
                    firstBeer = beer;
                    break;
                case 2:
                    secondBeer = beer;
                    break;
                case 3:
                    thirdBeer = beer;
                    break;
                default:
                    throw new UnreachableException();
            }
        }

        _ = firstBeer.ShouldNotBeNull();
        firstBeer.Name.ShouldBe("Test Beer 1");
        firstBeer.Style.ShouldBe("Lager");

        _ = secondBeer.ShouldNotBeNull();
        secondBeer.Name.ShouldBe("Test Beer 2");
        secondBeer.Style.ShouldBe("Ipa");

        _ = thirdBeer.ShouldNotBeNull();
        thirdBeer.Name.ShouldBe("Test Beer 3");
        thirdBeer.Style.ShouldBe("DoubleIpa");
    }

    [Test]
    public void ToBreweryDtoGivenValidBreweryWithoutListOfBeersReturnsMappedBreweryDtoWithEmptyBeerListAndZeroCount()
    {
        // Arrange
        var breweryToMap = new Brewery
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
        };

        // Act
        var mappedBrewery = breweryToMap.ToBreweryDto();

        // Assert
        _ = mappedBrewery.ShouldNotBeNull();
        mappedBrewery.Id.ShouldBe(breweryToMap.Id);
        mappedBrewery.Name.ShouldBe(breweryToMap.Name);
        AddressDto addressDto = mappedBrewery.Address.ShouldNotBeNull();
        addressDto.City.ShouldBe(breweryToMap.Address?.City);
        addressDto.State.ShouldBe(breweryToMap.Address?.State);
        addressDto.StreetAddress.ShouldBe(breweryToMap.Address?.StreetAddress);
        addressDto.ZipCode.ShouldBe(breweryToMap.Address?.ZipCode);
        mappedBrewery
            .Beers
            .ShouldNotBeNull()
            .ShouldBeEmpty();
        mappedBrewery
            .BeerCount
            .ShouldNotBeNull()
            .ShouldBe(0);
    }

    [Test]
    public void ToBreweryDtoGivenValidBreweryWithoutBeerListIncludedReturnsMappedBreweryDtoWithoutBeerListOrCount()
    {
        // Arrange
        var breweryToMap = new Brewery
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
        };

        // Add our beers since they are initialized within the domain
        breweryToMap.Beers.Add(new Beer { Id = 1, Name = "Test Beer 1", BeerStyle = BeerStyle.Lager });
        breweryToMap.Beers.Add(new Beer { Id = 2, Name = "Test Beer 2", BeerStyle = BeerStyle.Ipa });
        breweryToMap.Beers.Add(new Beer { Id = 3, Name = "Test Beer 3", BeerStyle = BeerStyle.DoubleIpa });

        // Act
        var mappedBrewery = breweryToMap.ToBreweryDto(false);

        // Assert
        BreweryDto breweryDto = mappedBrewery.ShouldNotBeNull();
        breweryDto.Id.ShouldBe(breweryToMap.Id);
        breweryDto.Name.ShouldBe(breweryToMap.Name);
        breweryDto.Beers.ShouldBeNull();
        breweryDto.BeerCount.ShouldBeNull();

        AddressDto addressDto = mappedBrewery.Address.ShouldNotBeNull();
        addressDto.City.ShouldBe(breweryToMap.Address?.City);
        addressDto.State.ShouldBe(breweryToMap.Address?.State);
        addressDto.StreetAddress.ShouldBe(breweryToMap.Address?.StreetAddress);
        addressDto.ZipCode.ShouldBe(breweryToMap.Address?.ZipCode);
    }

    [Test]
    public void ToBreweryDtoGivenValidBreweryWithNoBeerListAndWithoutBeerListIncludedReturnsMappedBreweryDtoWithoutBeerListOrCount()
    {
        // Arrange
        var breweryToMap = new Brewery
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
        };

        // Act
        var mappedBrewery = breweryToMap.ToBreweryDto(false);

        // Assert
        BreweryDto breweryDto = mappedBrewery.ShouldNotBeNull();
        breweryDto.Id.ShouldBe(breweryToMap.Id);
        breweryDto.Name.ShouldBe(breweryToMap.Name);
        breweryDto.Beers.ShouldBeNull();
        breweryDto.BeerCount.ShouldBeNull();

        AddressDto addressDto = mappedBrewery.Address.ShouldNotBeNull();
        addressDto.City.ShouldBe(breweryToMap.Address?.City);
        addressDto.State.ShouldBe(breweryToMap.Address?.State);
        addressDto.StreetAddress.ShouldBe(breweryToMap.Address?.StreetAddress);
        addressDto.ZipCode.ShouldBe(breweryToMap.Address?.ZipCode);
    }
}
