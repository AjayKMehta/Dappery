using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Dappery.Domain.Entities;

using Shouldly;

using Xunit;

namespace Dappery.Data.Tests;

public class BeerRepositoryTest : TestFixture
{
    [Fact]
    public async Task GetAllBeersWhenInvokedAndBeersExistReturnsValidListOfBeers()
    {
        // Arrange
        using var unitOfWork = this.UnitOfWork;

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
    public async Task GetAllBeersWhenNoBeersExistReturnsEmptyListOfBeers()
    {
        // Arrange, remove all the beers from our database
        using var unitOfWork = this.UnitOfWork;
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
    public async Task GetBeerByIdWhenInvokedAndBeerExistsReturnsValidBeer()
    {
        // Arrange
        using var unitOfWork = this.UnitOfWork;

        // Act
        var beer = await unitOfWork.BeerRepository.GetBeerByIdAsync(1, CancellationTestToken).ConfigureAwait(false);
        unitOfWork.Commit();

        // Assert, validate a few properties
        beer = beer
            .ShouldNotBeNull()
            .ShouldBeOfType<Beer>();
        beer.Name.ShouldBe("Hexagenia");
        beer.BeerStyle.ShouldBe(BeerStyle.Ipa);

        var brewery = beer.Brewery.ShouldNotBeNull();
        brewery.Name.ShouldBe("Fall River Brewery");
        brewery.Address.ShouldNotBeNull().City.ShouldBe("Redding");
    }

    [Fact]
    public async Task GetBeerByIdWhenInvokedAndBeerDoesNotExistReturnsNull()
    {
        // Arrange
        using var unitOfWork = this.UnitOfWork;

        // Act
        var beer = await unitOfWork.BeerRepository.GetBeerByIdAsync(10, CancellationTestToken).ConfigureAwait(false);
        unitOfWork.Commit();

        // Assert, validate a few properties
        beer.ShouldBeNull();
    }

    [Fact]
    public async Task CreateBeerWhenBeerIsValidReturnsNewlyInsertedBeer()
    {
        // Arrange
        using var unitOfWork = this.UnitOfWork;
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
        var insertedBeer = await unitOfWork.BeerRepository.GetBeerByIdAsync(beerId, CancellationTestToken).ConfigureAwait(false);
        unitOfWork.Commit();

        var beer = insertedBeer
            .ShouldNotBeNull()
            .ShouldBeOfType<Beer>();
        var brewery = beer.Brewery.ShouldNotBeNull();
        _ = brewery.Address.ShouldNotBeNull();
        brewery.Beers.ShouldNotBeEmpty();
        brewery.Beers.Count.ShouldBe(4);
        brewery.Beers.ShouldContain(b => b.Id == insertedBeer.Id);
        brewery.Beers.FirstOrDefault(b => b.Id == insertedBeer.Id)?.Name.ShouldBe(beerToInsert.Name);
    }

    [Fact]
    public async Task UpdateBeerWhenBeerIsValidReturnsUpdateBeer()
    {
        // Arrange
        using var unitOfWork = this.UnitOfWork;
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
        var updatedBeer = await unitOfWork.BeerRepository.GetBeerByIdAsync(beerToUpdate.Id, CancellationTestToken).ConfigureAwait(false);
        unitOfWork.Commit();

        var brewery = updatedBeer
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
    public async Task DeleteBeerWhenBeerExistsRemovesBeerFromDatabase()
    {
        // Arrange
        using var unitOfWork = this.UnitOfWork;
        (await unitOfWork.BeerRepository.GetAllBeersAsync(CancellationTestToken).ConfigureAwait(false))?.Count().ShouldBe(5);

        // Act
        await unitOfWork.BeerRepository.DeleteBeerAsync(1, CancellationTestToken).ConfigureAwait(false);
        var breweryOfRemovedBeer = await unitOfWork.BreweryRepository.GetBreweryById(1, CancellationTestToken).ConfigureAwait(false);
        (await unitOfWork.BeerRepository.GetAllBeersAsync(CancellationTestToken).ConfigureAwait(false))?.Count().ShouldBe(4);
        unitOfWork.Commit();

        // Assert
        var beers = breweryOfRemovedBeer
            .ShouldNotBeNull()
            .Beers
            .ShouldNotBeNull();
        beers.ShouldNotBeEmpty();
        beers.ShouldNotContain(b => b.Name == "Hexagenia");
    }
}
