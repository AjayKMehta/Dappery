using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dappery.Domain.Entities;
using Shouldly;
using Xunit;

namespace Dappery.Data.Tests
{
    public class BreweryRepositoryTest : TestFixture
    {
        [Fact]
        public async Task GetAllBreweries_WhenInvokedAndBreweriesExist_ReturnsValidListOfBreweries()
        {
            // Arrange
            using var unitOfWork = this.UnitOfWork;

            // Act
            var breweries = (await unitOfWork.BreweryRepository.GetAllBreweries(CancellationTestToken).ConfigureAwait(false)).ToList();
            unitOfWork.Commit();

            // Assert
            breweries.ShouldNotBeNull();
            breweries.ShouldNotBeEmpty();
            breweries.Count.ShouldBe(2);
            breweries.All(br => br.Address != null).ShouldBeTrue();
            breweries.All(br => br.Beers != null).ShouldBeTrue();
            breweries.All(br => br.Beers.Count > 0).ShouldBeTrue();

            var brewery = breweries.Find(br => br.Name == "Fall River Brewery");
            brewery?.Beers.ShouldContain(b => b.Name == "Hexagenia");
            brewery?.Beers.ShouldContain(b => b.Name == "Widowmaker");
            brewery?.Beers.ShouldContain(b => b.Name == "Hooked");

            brewery = breweries.Find(br => br.Name == "Sierra Nevada Brewing Company");
            brewery?.Beers.ShouldContain(b => b.Name == "Pale Ale");
            brewery?.Beers.ShouldContain(b => b.Name == "Hazy Little Thing");
        }

        [Fact]
        public async Task GetAllBreweries_WhenInvokedAndNoBreweriesExist_ReturnsEmptyList()
        {
            // Arrange
            using var unitOfWork = this.UnitOfWork;
            await unitOfWork.BreweryRepository.DeleteBrewery(1, CancellationTestToken).ConfigureAwait(false);
            await unitOfWork.BreweryRepository.DeleteBrewery(2, CancellationTestToken).ConfigureAwait(false);

            // Act
            var breweries = (await unitOfWork.BreweryRepository.GetAllBreweries(CancellationTestToken).ConfigureAwait(false)).ToList();
            unitOfWork.Commit();

            // Assert
            breweries.ShouldNotBeNull();
            breweries.ShouldBeOfType<List<Brewery>>();
            breweries.ShouldBeEmpty();
        }

        [Fact]
        public async Task GetBreweryById_WhenInvokedAndBreweryExist_ReturnsValidBreweryWithBeersAndAddress()
        {
            // Arrange
            using var unitOfWork = this.UnitOfWork;

            // Act
            var brewery = await unitOfWork.BreweryRepository.GetBreweryById(1, CancellationTestToken).ConfigureAwait(false);
            unitOfWork.Commit();

            // Assert
            brewery.ShouldNotBeNull();
            brewery.ShouldBeOfType<Brewery>();
            brewery.Address.ShouldNotBeNull();
            brewery.Beers.ShouldNotBeNull();
            brewery.Beers.ShouldNotBeEmpty();
            brewery.BeerCount.ShouldBe(3);
            brewery.Beers.ShouldContain(b => b.Name == "Hexagenia");
            brewery.Beers.ShouldContain(b => b.Name == "Widowmaker");
            brewery.Beers.ShouldContain(b => b.Name == "Hooked");
        }

        [Fact]
        public async Task GetBreweryById_WhenInvokedAndNoBreweryExist_ReturnsNull()
        {
            // Arrange
            using var unitOfWork = this.UnitOfWork;

            // Act
            var brewery = await unitOfWork.BreweryRepository.GetBreweryById(11, CancellationTestToken).ConfigureAwait(false);
            unitOfWork.Commit();

            // Assert
            brewery.ShouldBeNull();
        }

        [Fact]
        public async Task CreateBrewery_WhenBreweryIsValid_ReturnsNewlyInsertedBrewery()
        {
            // Arrange
            using var unitOfWork = this.UnitOfWork;
            var breweryToInsert = new Brewery
            {
                Name = "Bike Dog Brewing Company",
                Address = new Address
                {
                    StreetAddress = "123 Sacramento St.",
                    City = "Sacramento",
                    State = "CA",
                    ZipCode = "95811",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            // Act
            var breweryId = await unitOfWork.BreweryRepository.CreateBrewery(breweryToInsert, CancellationTestToken).ConfigureAwait(false);
            var insertedBrewery = await unitOfWork.BreweryRepository.GetBreweryById(breweryId, CancellationTestToken).ConfigureAwait(false);
            unitOfWork.Commit();

            // Assert
            insertedBrewery.ShouldNotBeNull();
            insertedBrewery.ShouldBeOfType<Brewery>();
            insertedBrewery.Address?.ShouldNotBeNull();
            insertedBrewery.Address?.StreetAddress.ShouldBe(breweryToInsert.Address.StreetAddress);
            insertedBrewery.Address?.BreweryId.ShouldBe(3);
            insertedBrewery.Beers.ShouldBeEmpty();
        }

        [Fact]
        public async Task UpdateBrewery_WhenBreweryIsValidAndAddressIsNotUpdated_ReturnsUpdatedBrewery()
        {
            // Arrange
            using var unitOfWork = this.UnitOfWork;
            var breweryToUpdate = new Brewery
            {
                Id = 2,
                Name = "Sierra Nevada Brewing Company Of Brewing",
                Address = new Address
                {
                    StreetAddress = "1075 E 20th St",
                    City = "Chico",
                    State = "CA",
                    ZipCode = "95928",
                    UpdatedAt = DateTime.UtcNow,
                    BreweryId = 2
                },
                UpdatedAt = DateTime.UtcNow
            };

            // Act
            await unitOfWork.BreweryRepository.UpdateBrewery(breweryToUpdate, CancellationTestToken).ConfigureAwait(false);
            var updatedBrewery = await unitOfWork.BreweryRepository.GetBreweryById(breweryToUpdate.Id, CancellationTestToken).ConfigureAwait(false);
            unitOfWork.Commit();

            // Assert
            updatedBrewery.ShouldNotBeNull();
            updatedBrewery.ShouldBeOfType<Brewery>();
            updatedBrewery.Address?.ShouldNotBeNull();
            updatedBrewery.Address?.StreetAddress.ShouldBe(breweryToUpdate.Address.StreetAddress);
            updatedBrewery.Address?.BreweryId.ShouldBe(2);
            updatedBrewery.Beers.ShouldNotBeNull();
            updatedBrewery.Beers.ShouldNotBeEmpty();
        }

        [Fact]
        public async Task UpdateBrewery_WhenBreweryIsValidAndAddressIsUpdated_ReturnsUpdatedBrewery()
        {
            // Arrange
            using var unitOfWork = this.UnitOfWork;
            var breweryToUpdate = new Brewery
            {
                Id = 2,
                Name = "Sierra Nevada Brewing Company Of Brewing",
                Address = new Address
                {
                    Id = 2,
                    StreetAddress = "123 Happy St.",
                    City = "Redding",
                    State = "CA",
                    ZipCode = "96002",
                    UpdatedAt = DateTime.UtcNow,
                    BreweryId = 2
                },
                UpdatedAt = DateTime.UtcNow
            };

            // Act
            await unitOfWork.BreweryRepository.UpdateBrewery(breweryToUpdate, CancellationTestToken, true).ConfigureAwait(false);
            var updatedBrewery = await unitOfWork.BreweryRepository.GetBreweryById(breweryToUpdate.Id, CancellationTestToken).ConfigureAwait(false);
            unitOfWork.Commit();

            // Assert
            updatedBrewery.ShouldNotBeNull();
            updatedBrewery.ShouldBeOfType<Brewery>();
            updatedBrewery.Address?.ShouldNotBeNull();
            updatedBrewery.Address?.StreetAddress.ShouldBe(breweryToUpdate.Address.StreetAddress);
            updatedBrewery.Address?.ZipCode.ShouldBe(breweryToUpdate.Address.ZipCode);
            updatedBrewery.Address?.City.ShouldBe(breweryToUpdate.Address.City);
            updatedBrewery.Address?.BreweryId.ShouldBe(2);
            updatedBrewery.Beers.ShouldNotBeNull();
            updatedBrewery.Beers.ShouldNotBeEmpty();
        }

        [Fact]
        public async Task DeleteBrewery_WhenBreweryExists_RemovesBreweryAndAllAssociatedBeersAndAddress()
        {
            // Arrange
            using var unitOfWork = this.UnitOfWork;
            (await unitOfWork.BreweryRepository.GetAllBreweries(CancellationTestToken).ConfigureAwait(false))?.Count().ShouldBe(2);
            (await unitOfWork.BeerRepository.GetAllBeersAsync(CancellationToken.None).ConfigureAwait(false))?.Count().ShouldBe(5);

            // Act
            await unitOfWork.BreweryRepository.DeleteBrewery(1, CancellationTestToken).ConfigureAwait(false);
            var breweries = (await unitOfWork.BreweryRepository.GetAllBreweries(CancellationTestToken).ConfigureAwait(false)).ToList();
            (await unitOfWork.BeerRepository.GetAllBeersAsync(CancellationToken.None).ConfigureAwait(false))?.Count().ShouldBe(2);
            unitOfWork.Commit();

            // Assert
            breweries.ShouldNotBeNull();
            breweries.Count.ShouldBe(1);
            breweries.ShouldNotContain(br => br.Name == "Fall River Brewery");
        }
    }
}
