using System.Linq;

using Dappery.Core.Extensions;
using Dappery.Domain.Dtos.Brewery;
using Dappery.Domain.Entities;

using Shouldly;

using Xunit;

namespace Dappery.Core.Tests.Extensions;

public class BreweryExtensionsTest
{
    [Fact]
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

        var addressDto = breweryDto.Address.ShouldNotBeNull();
        addressDto.City.ShouldBe(breweryToMap.Address?.City);
        addressDto.State.ShouldBe(breweryToMap.Address?.State);
        addressDto.StreetAddress.ShouldBe(breweryToMap.Address?.StreetAddress);
        addressDto.ZipCode.ShouldBe(breweryToMap.Address?.ZipCode);

        var beers = mappedBrewery.Beers.ShouldNotBeNull();
        beers.ShouldNotBeEmpty();

        var firstBeer = beers.First(b => b.Id == 1);
        firstBeer.Name.ShouldBe("Test Beer 1");
        firstBeer.Style.ShouldBe("Lager");

        var secondBeer = beers.First(b => b.Id == 2);
        secondBeer.Name.ShouldBe("Test Beer 2");
        secondBeer.Style.ShouldBe("Ipa");

        var thirdBeer = beers.First(b => b.Id == 3);
        thirdBeer.Name.ShouldBe("Test Beer 3");
        thirdBeer.Style.ShouldBe("DoubleIpa");
    }

    [Fact]
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
        var addressDto = mappedBrewery.Address.ShouldNotBeNull();
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

    [Fact]
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
        var breweryDto = mappedBrewery.ShouldNotBeNull();
        breweryDto.Id.ShouldBe(breweryToMap.Id);
        breweryDto.Name.ShouldBe(breweryToMap.Name);
        breweryDto.Beers.ShouldBeNull();
        breweryDto.BeerCount.ShouldBeNull();

        var addressDto = mappedBrewery.Address.ShouldNotBeNull();
        addressDto.City.ShouldBe(breweryToMap.Address?.City);
        addressDto.State.ShouldBe(breweryToMap.Address?.State);
        addressDto.StreetAddress.ShouldBe(breweryToMap.Address?.StreetAddress);
        addressDto.ZipCode.ShouldBe(breweryToMap.Address?.ZipCode);
    }

    [Fact]
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
        var breweryDto = mappedBrewery.ShouldNotBeNull();
        breweryDto.Id.ShouldBe(breweryToMap.Id);
        breweryDto.Name.ShouldBe(breweryToMap.Name);
        breweryDto.Beers.ShouldBeNull();
        breweryDto.BeerCount.ShouldBeNull();

        var addressDto = mappedBrewery.Address.ShouldNotBeNull();
        addressDto.City.ShouldBe(breweryToMap.Address?.City);
        addressDto.State.ShouldBe(breweryToMap.Address?.State);
        addressDto.StreetAddress.ShouldBe(breweryToMap.Address?.StreetAddress);
        addressDto.ZipCode.ShouldBe(breweryToMap.Address?.ZipCode);
    }
}
