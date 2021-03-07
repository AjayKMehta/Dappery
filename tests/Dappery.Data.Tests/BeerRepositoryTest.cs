using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dappery.Domain.Entities;
using Shouldly;
using Xunit;

namespace Dappery.Data.Tests
{
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
            beers.ShouldNotBeNull();
            beers.ShouldBeOfType<List<Beer>>();
            beers.ShouldNotBeEmpty();
            beers.All(b => b.Brewery != null).ShouldBeTrue();
            beers.All(b => b.Brewery!.Address != null).ShouldBeTrue();
            beers.All(b => b.Brewery!.Address!.BreweryId == b.Brewery.Id).ShouldBeTrue();
            beers.ShouldContain(b => b.Name == "Hexagenia");
            beers.Find(b => b.Name == "Hexagenia")?.BeerStyle.ShouldBe(BeerStyle.Ipa);
            beers.ShouldContain(b => b.Name == "Widowmaker");
            beers.Find(b => b.Name == "Widowmaker")?.BeerStyle.ShouldBe(BeerStyle.DoubleIpa);
            beers.ShouldContain(b => b.Name == "Hooked");
            beers.Find(b => b.Name == "Hooked")?.BeerStyle.ShouldBe(BeerStyle.Lager);
            beers.ShouldContain(b => b.Name == "Pale Ale");
            beers.Find(b => b.Name == "Pale Ale")?.BeerStyle.ShouldBe(BeerStyle.PaleAle);
            beers.ShouldContain(b => b.Name == "Hazy Little Thing");
            beers.Find(b => b.Name == "Hazy Little Thing")?.BeerStyle.ShouldBe(BeerStyle.NewEnglandIpa);
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
            beers.ShouldNotBeNull();
            beers.ShouldBeOfType<List<Beer>>();
            beers.ShouldBeEmpty();
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
            beer.ShouldNotBeNull();
            beer.ShouldBeOfType<Beer>();
            beer.Name.ShouldBe("Hexagenia");
            beer.BeerStyle.ShouldBe(BeerStyle.Ipa);
            beer.Brewery?.ShouldNotBeNull();
            beer.Brewery?.Name.ShouldBe("Fall River Brewery");
            beer.Brewery?.Address?.ShouldNotBeNull();
            beer.Brewery?.Address?.City.ShouldBe("Redding");
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

            insertedBeer.ShouldNotBeNull();
            insertedBeer.ShouldBeOfType<Beer>();
            insertedBeer.Brewery!.ShouldNotBeNull();
            insertedBeer.Brewery!.Address!.ShouldNotBeNull();
            insertedBeer.Brewery!.Beers.ShouldNotBeEmpty();
            insertedBeer.Brewery!.Beers.Count.ShouldBe(4);
            insertedBeer.Brewery!.Beers.ShouldContain(b => b.Id == insertedBeer.Id);
            insertedBeer.Brewery!.Beers.FirstOrDefault(b => b.Id == insertedBeer.Id)?.Name.ShouldBe(beerToInsert.Name);
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

            updatedBeer.ShouldNotBeNull();
            updatedBeer.ShouldNotBeNull();
            updatedBeer.ShouldBeOfType<Beer>();
            updatedBeer.Brewery?.ShouldNotBeNull();
            updatedBeer.Brewery?.Address?.ShouldNotBeNull();
            updatedBeer.Brewery?.Beers.ShouldNotBeEmpty();
            updatedBeer.Brewery?.Beers.Count.ShouldBe(3);
            updatedBeer.Brewery?.Beers.ShouldContain(b => b.Id == beerToUpdate.Id);
            updatedBeer.Brewery?.Beers.ShouldNotContain(b => b.Name == "Hexagenia");
            updatedBeer.Brewery?.Beers.FirstOrDefault(b => b.Id == beerToUpdate.Id)?.Name.ShouldBe(beerToUpdate.Name);
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
            breweryOfRemovedBeer.ShouldNotBeNull();
            breweryOfRemovedBeer.Beers.ShouldNotBeNull();
            breweryOfRemovedBeer.Beers.ShouldNotBeEmpty();
            breweryOfRemovedBeer.Beers.ShouldNotContain(b => b.Name == "Hexagenia");
        }
    }
}
