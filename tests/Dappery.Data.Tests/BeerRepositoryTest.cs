using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Dappery.Core.Data;
using Dappery.Domain.Entities;

using Shouldly;

using Xunit;

namespace Dappery.Data.Tests;

public class BeerRepositoryTest : TestFixture
{
    [Fact]
    public async Task GetAllBeersWhenInvokedAndBeersExistReturnsValidListOfBeersAsync()
    {
        // Arrange
        using IUnitOfWork unitOfWork = UnitOfWork;

        // Act
        var beers = (await unitOfWork.BeerRepository.GetAllBeersAsync(CancellationTestToken).ConfigureAwait(false)).ToList();
        unitOfWork.Commit();

        // Assert
        _ = beers
            .ShouldNotBeNull()
            .ShouldBeOfType<List<Beer>>();
        beers.ShouldNotBeEmpty();
        beers.All(b => b.Brewery is not null).ShouldBeTrue();
        beers.All(b => b.Brewery!.Address is not null).ShouldBeTrue();
        beers.All(b => b.Brewery!.Address!.BreweryId == b.Brewery.Id).ShouldBeTrue();
        beers.Find(b => b.Name == "Hexagenia").ShouldNotBeNull().BeerStyle.ShouldBe(BeerStyle.Ipa);
        beers.Find(b => b.Name == "Widowmaker").ShouldNotBeNull().BeerStyle.ShouldBe(BeerStyle.DoubleIpa);
        beers.Find(b => b.Name == "Hooked").ShouldNotBeNull().BeerStyle.ShouldBe(BeerStyle.Lager);
        beers.Find(b => b.Name == "Pale Ale").ShouldNotBeNull().BeerStyle.ShouldBe(BeerStyle.PaleAle);
        beers.Find(b => b.Name == "Hazy Little Thing").ShouldNotBeNull().BeerStyle.ShouldBe(BeerStyle.NewEnglandIpa);
    }

    [Fact]
    public async Task GetAllBeersWhenNoBeersExistReturnsEmptyListOfBeersAsync()
    {
        // Arrange, remove all the beers from our database
        using IUnitOfWork unitOfWork = UnitOfWork;
        await unitOfWork.BeerRepository.DeleteBeerAsync(1, CancellationTestToken).ConfigureAwait(false);
        await unitOfWork.BeerRepository.DeleteBeerAsync(2, CancellationTestToken).ConfigureAwait(false);
        await unitOfWork.BeerRepository.DeleteBeerAsync(3, CancellationTestToken).ConfigureAwait(false);
        await unitOfWork.BeerRepository.DeleteBeerAsync(4, CancellationTestToken).ConfigureAwait(false);
        await unitOfWork.BeerRepository.DeleteBeerAsync(5, CancellationTestToken).ConfigureAwait(false);

        // Act
        var beers = (await unitOfWork.BeerRepository.GetAllBeersAsync(CancellationTestToken).ConfigureAwait(false)).ToList();
        unitOfWork.Commit();

        // Assert
        beers.ShouldNotBeNull().ShouldBeOfType<List<Beer>>().ShouldBeEmpty();
    }

    [Fact]
    public async Task GetBeerByIdWhenInvokedAndBeerExistsReturnsValidBeerAsync()
    {
        // Arrange
        using IUnitOfWork unitOfWork = UnitOfWork;

        // Act
        Beer? beer = await unitOfWork.BeerRepository.GetBeerByIdAsync(1, CancellationTestToken).ConfigureAwait(false);
        unitOfWork.Commit();

        // Assert, validate a few properties
        beer = beer
            .ShouldNotBeNull()
            .ShouldBeOfType<Beer>();
        beer.Name.ShouldBe("Hexagenia");
        beer.BeerStyle.ShouldBe(BeerStyle.Ipa);

        Brewery brewery = beer.Brewery.ShouldNotBeNull();
        brewery.Name.ShouldBe("Fall River Brewery");
        brewery.Address.ShouldNotBeNull().City.ShouldBe("Redding");
    }

    [Fact]
    public async Task GetBeerByIdWhenInvokedAndBeerDoesNotExistReturnsNullAsync()
    {
        // Arrange
        using IUnitOfWork unitOfWork = UnitOfWork;

        // Act
        Beer? beer = await unitOfWork.BeerRepository.GetBeerByIdAsync(10, CancellationTestToken).ConfigureAwait(false);
        unitOfWork.Commit();

        // Assert, validate a few properties
        beer.ShouldBeNull();
    }

    [Fact]
    public async Task CreateBeerWhenBeerIsValidReturnsNewlyInsertedBeerAsync()
    {
        // Arrange
        using IUnitOfWork unitOfWork = UnitOfWork;
        var beerToInsert = new Beer
        {
            Name = "Lazy Hazy",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            BreweryId = 1,
            BeerStyle = BeerStyle.NewEnglandIpa
        };

        // Act
        var beerId = await unitOfWork.BeerRepository.CreateBeerAsync(beerToInsert, CancellationTestToken).ConfigureAwait(false);
        Beer? insertedBeer = await unitOfWork.BeerRepository.GetBeerByIdAsync(beerId, CancellationTestToken).ConfigureAwait(false);
        unitOfWork.Commit();

        Beer beer = insertedBeer
            .ShouldNotBeNull()
            .ShouldBeOfType<Beer>();
        Brewery brewery = beer.Brewery.ShouldNotBeNull();
        _ = brewery.Address.ShouldNotBeNull();
        brewery.Beers.ShouldNotBeEmpty();
        brewery.Beers.Count.ShouldBe(4);
        brewery.Beers.ShouldContain(b => b.Id == insertedBeer.Id);
        brewery.Beers.FirstOrDefault(b => b.Id == insertedBeer.Id)?.Name.ShouldBe(beerToInsert.Name);
    }

    [Fact]
    public async Task UpdateBeerWhenBeerIsValidReturnsUpdateBeerAsync()
    {
        // Arrange
        using IUnitOfWork unitOfWork = UnitOfWork;
        var beerToUpdate = new Beer
        {
            Id = 1,
            Name = "Colossus Imperial Stout",
            UpdatedAt = DateTime.UtcNow,
            BeerStyle = BeerStyle.Stout,
            BreweryId = 1,
        };

        // Act
        await unitOfWork.BeerRepository.UpdateBeerAsync(beerToUpdate, CancellationTestToken).ConfigureAwait(false);
        Beer? updatedBeer = await unitOfWork.BeerRepository.GetBeerByIdAsync(beerToUpdate.Id, CancellationTestToken).ConfigureAwait(false);
        unitOfWork.Commit();

        Brewery brewery = updatedBeer
            .ShouldNotBeNull()
            .ShouldBeOfType<Beer>()
            .Brewery
            .ShouldNotBeNull();

        _ = brewery.Address.ShouldNotBeNull();
        brewery.Beers.ShouldNotBeEmpty();
        brewery.Beers.Count.ShouldBe(3);
        brewery.Beers.ShouldContain(b => b.Id == beerToUpdate.Id);
        brewery.Beers.ShouldNotContain(b => b.Name == "Hexagenia");
        brewery.Beers.FirstOrDefault(b => b.Id == beerToUpdate.Id)?.Name.ShouldBe(beerToUpdate.Name);
    }

    [Fact]
    public async Task DeleteBeerWhenBeerExistsRemovesBeerFromDatabaseAsync()
    {
        // Arrange
        using IUnitOfWork unitOfWork = UnitOfWork;
        (await unitOfWork.BeerRepository.GetAllBeersAsync(CancellationTestToken).ConfigureAwait(false))?.Count().ShouldBe(5);

        // Act
        await unitOfWork.BeerRepository.DeleteBeerAsync(1, CancellationTestToken).ConfigureAwait(false);
        Brewery? breweryOfRemovedBeer = await unitOfWork.BreweryRepository.GetBreweryById(1, CancellationTestToken).ConfigureAwait(false);
        (await unitOfWork.BeerRepository.GetAllBeersAsync(CancellationTestToken).ConfigureAwait(false))?.Count().ShouldBe(4);
        unitOfWork.Commit();

        // Assert
        ICollection<Beer> beers = breweryOfRemovedBeer
            .ShouldNotBeNull()
            .Beers
            .ShouldNotBeNull();
        beers.ShouldNotBeEmpty();
        beers.ShouldNotContain(b => b.Name == "Hexagenia");
    }
}
