using System.Threading;
using System.Threading.Tasks;
using Dappery.Core.Breweries.Commands.CreateBrewery;
using Dappery.Domain.Dtos;
using Dappery.Domain.Dtos.Brewery;
using Dappery.Domain.Media;
using Shouldly;
using Xunit;

namespace Dappery.Core.Tests.Breweries
{
    public class CreateBreweryCommandHandlerTest : TestFixture
    {
        [Fact]
        public async Task CreateBreweryCommandHandlerGivenAValidRequestCreatesBrewery()
        {
            // Arrange
            using var unitOfWork = this.UnitOfWork;
            var createBreweryDto = new CreateBreweryDto
            {
                Name = "Pizza Port Brewing Company",
                Address = new AddressDto
                {
                    City = "San Diego",
                    State = "CA",
                    StreetAddress = "123 San Diego St.",
                    ZipCode = "92109"
                }
            };

            // Act
            var handler = new CreateBreweryCommandHandler(unitOfWork);
            var createdBrewery = await handler.Handle(new CreateBreweryCommand(createBreweryDto), CancellationToken.None).ConfigureAwait(false);

            // Assert
            createdBrewery.ShouldNotBeNull();
            createdBrewery.ShouldBeOfType<BreweryResource>();
            createdBrewery.Self.ShouldNotBeNull();
            createdBrewery.Self.ShouldBeOfType<BreweryDto>();
            createdBrewery.Self.Name.ShouldNotBeNull();
            createdBrewery.Self.Name.ShouldBe(createBreweryDto.Name);
            createdBrewery.Self.Address?.ShouldNotBeNull();
            createdBrewery.Self.Address?.City.ShouldBe(createBreweryDto.Address?.City);
            createdBrewery.Self.Address?.State.ShouldBe(createBreweryDto.Address?.State);
            createdBrewery.Self.Address?.StreetAddress.ShouldBe(createBreweryDto.Address?.StreetAddress);
            createdBrewery.Self.Address?.ZipCode.ShouldBe(createBreweryDto.Address?.ZipCode);
            createdBrewery.Self.Beers.ShouldBeEmpty();
            createdBrewery.Self.BeerCount.ShouldBe(0);
        }
    }
}
