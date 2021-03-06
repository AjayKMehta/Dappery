using System.Net;
using System.Threading.Tasks;
using Dappery.Core.Beers.Commands.CreateBeer;
using Dappery.Core.Exceptions;
using Dappery.Domain.Dtos;
using Dappery.Domain.Entities;
using Dappery.Domain.Media;
using Shouldly;
using Xunit;

namespace Dappery.Core.Tests.Beers
{
    public class CreateBeerCommandHandlerTest : TestFixture
    {
        [Fact]
        public async Task GivenValidRequestWhenBreweryExistsReturnsMappedAndCreatedBeer()
        {
            // Arrange
            using var unitOfWork = this.UnitOfWork;
            var beerCommand = new CreateBeerCommand(new CreateBeerDto
            {
                Name = "Test Beer",
                Style = "Lager",
                BreweryId = 1
            });
            var handler = new CreateBeerCommandHandler(unitOfWork);

            // Act
            var result = await handler.Handle(beerCommand, CancellationTestToken).ConfigureAwait(false);

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBeOfType<BeerResource>();
            result.Self.ShouldNotBeNull();
            result.Self.Brewery.ShouldNotBeNull();
            result.Self.Brewery?.Address.ShouldNotBeNull();
            result.Self.Brewery?.Address?.StreetAddress.ShouldBe("1030 E Cypress Ave Ste D");
            result.Self.Brewery?.Address?.City.ShouldBe("Redding");
            result.Self.Brewery?.Address?.State.ShouldBe("CA");
            result.Self.Brewery?.Address?.ZipCode.ShouldBe("96002");
            result.Self.Brewery?.Beers.ShouldBeNull();
            result.Self.Brewery?.Id.ShouldBe(1);
            result.Self.Brewery?.Name.ShouldBe("Fall River Brewery");
            result.Self.Name.ShouldBe(beerCommand.Dto.Name);
            result.Self.Style.ShouldBe(beerCommand.Dto.Style);
        }

        [Fact]
        public async Task GivenValidRequestWhenBreweryDoesNotExistThrowsApiExceptionForBadRequest()
        {
            // Arrange
            using var unitOfWork = this.UnitOfWork;
            var beerCommand = new CreateBeerCommand(new CreateBeerDto
            {
                Name = "Test Beer",
                Style = "Lager",
                BreweryId = 11
            });
            var handler = new CreateBeerCommandHandler(unitOfWork);

            // Act
            var result = await Should.ThrowAsync<DapperyApiException>(async () => await handler.Handle(beerCommand, CancellationTestToken).ConfigureAwait(false)).ConfigureAwait(false);

            // Assert
            result.ShouldNotBeNull();
            result.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task GivenValidRequestWithInvalidBeerStyleReturnsMappedAndCreatedBeerWithOtherAsStyle()
        {
            // Arrange
            using var unitOfWork = this.UnitOfWork;
            var beerCommand = new CreateBeerCommand(new CreateBeerDto
            {
                Name = "Test Beer",
                Style = "Not defined!",
                BreweryId = 1
            });
            var handler = new CreateBeerCommandHandler(unitOfWork);

            // Act
            var result = await handler.Handle(beerCommand, CancellationTestToken).ConfigureAwait(false);

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBeOfType<BeerResource>();
            result.Self.ShouldNotBeNull();
            result.Self.Brewery?.ShouldNotBeNull();
            result.Self.Brewery?.Address.ShouldNotBeNull();
            result.Self.Brewery?.Address?.StreetAddress.ShouldBe("1030 E Cypress Ave Ste D");
            result.Self.Brewery?.Address?.City.ShouldBe("Redding");
            result.Self.Brewery?.Address?.State.ShouldBe("CA");
            result.Self.Brewery?.Address?.ZipCode.ShouldBe("96002");
            result.Self.Brewery?.Beers.ShouldBeNull();
            result.Self.Brewery?.Id.ShouldBe(1);
            result.Self.Brewery?.Name.ShouldBe("Fall River Brewery");
            result.Self.Name.ShouldBe(beerCommand.Dto.Name);
            result.Self.Style.ShouldBe(nameof(BeerStyle.Other));
        }
    }
}
