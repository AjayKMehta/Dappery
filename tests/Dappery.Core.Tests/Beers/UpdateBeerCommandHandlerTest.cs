using System.Net;
using System.Threading.Tasks;
using Dappery.Core.Beers.Commands.UpdateBeery;
using Dappery.Core.Exceptions;
using Dappery.Domain.Dtos;
using Dappery.Domain.Entities;
using Shouldly;
using Xunit;

namespace Dappery.Core.Tests.Beers
{
    public class UpdateBeerCommandHandlerTest : TestFixture
    {
        [Fact]
        public async Task GivenValidRequestWhenBeerExistsAndBreweryIsNotUpdatedUpdatesAndReturnsMappedBeer()
        {
            // Arrange
            using var unitOfWork = this.UnitOfWork;
            var updateCommand = new UpdateBeerCommand(new UpdateBeerDto
            {
                Name = "Updated Beer Name",
                Style = "Ipa"
            }, 1);
            var handler = new UpdateBeerCommandHandler(unitOfWork);

            // Act
            var result = await handler.Handle(updateCommand, CancellationTestToken).ConfigureAwait(false);

            // Assert
            result.ShouldNotBeNull();
            result.Self.ShouldNotBeNull();
            result.Self.Id.ShouldBe(updateCommand.BeerId);
            result.Self.Name.ShouldBe(updateCommand.Dto.Name);
            result.Self.Style.ShouldBe(updateCommand.Dto.Style);
            result.Self.Brewery.ShouldNotBeNull();
            result.Self.Brewery?.Address.ShouldNotBeNull();
            result.Self.Brewery?.Address?.StreetAddress.ShouldBe("1030 E Cypress Ave Ste D");
            result.Self.Brewery?.Address?.City.ShouldBe("Redding");
            result.Self.Brewery?.Address?.State.ShouldBe("CA");
            result.Self.Brewery?.Address?.ZipCode.ShouldBe("96002");
            result.Self.Brewery?.Beers.ShouldBeNull();
            result.Self.Brewery?.Id.ShouldBe(1);
            result.Self.Brewery?.Name.ShouldBe("Fall River Brewery");
        }

        [Fact]
        public async Task GivenValidRequestWhenBeerExistsAndStyleIsUnknownUpdatesAndReturnsMappedBeerWithOtherStyle()
        {
            // Arrange
            using var unitOfWork = this.UnitOfWork;
            var updateCommand = new UpdateBeerCommand(new UpdateBeerDto
            {
                Name = "Updated Beer Name",
                Style = "Not a valid beer style!"
            }, 1);
            var handler = new UpdateBeerCommandHandler(unitOfWork);

            // Act
            var result = await handler.Handle(updateCommand, CancellationTestToken).ConfigureAwait(false);

            // Assert
            result.ShouldNotBeNull();
            result.Self.ShouldNotBeNull();
            result.Self.Id.ShouldBe(updateCommand.BeerId);
            result.Self.Name.ShouldBe(updateCommand.Dto.Name);
            result.Self.Style.ShouldBe(nameof(BeerStyle.Other));
            result.Self.Brewery.ShouldNotBeNull();
            result.Self.Brewery?.Address.ShouldNotBeNull();
            result.Self.Brewery?.Address?.StreetAddress.ShouldBe("1030 E Cypress Ave Ste D");
            result.Self.Brewery?.Address?.City.ShouldBe("Redding");
            result.Self.Brewery?.Address?.State.ShouldBe("CA");
            result.Self.Brewery?.Address?.ZipCode.ShouldBe("96002");
            result.Self.Brewery?.Beers.ShouldBeNull();
            result.Self.Brewery?.Id.ShouldBe(1);
            result.Self.Brewery?.Name.ShouldBe("Fall River Brewery");
        }

        [Fact]
        public async Task GivenValidRequestWhenBeerExistsAndExistingBreweryIsUpdatedUpdatesAndReturnsMappedBeer()
        {
            // Arrange
            using var unitOfWork = this.UnitOfWork;
            var updateCommand = new UpdateBeerCommand(new UpdateBeerDto
            {
                Name = "Updated Beer Name",
                Style = "Ipa",
                BreweryId = 2
            }, 1);
            var handler = new UpdateBeerCommandHandler(unitOfWork);

            // Act
            var result = await handler.Handle(updateCommand, CancellationTestToken).ConfigureAwait(false);

            // Assert
            result.ShouldNotBeNull();
            result.Self.ShouldNotBeNull();
            result.Self.Id.ShouldBe(updateCommand.BeerId);
            result.Self.Name.ShouldBe(updateCommand.Dto.Name);
            result.Self.Style.ShouldBe(updateCommand.Dto.Style);
            result.Self.Brewery.ShouldNotBeNull();
            result.Self.Brewery?.Address.ShouldNotBeNull();
            result.Self.Brewery?.Address?.StreetAddress.ShouldBe("1075 E 20th St");
            result.Self.Brewery?.Address?.City.ShouldBe("Chico");
            result.Self.Brewery?.Address?.State.ShouldBe("CA");
            result.Self.Brewery?.Address?.ZipCode.ShouldBe("95928");
            result.Self.Brewery?.Beers.ShouldBeNull();
            result.Self.Brewery?.Id.ShouldBe(2);
            result.Self.Brewery?.Name.ShouldBe("Sierra Nevada Brewing Company");
        }

        [Fact]
        public async Task GivenValidRequestWhenBeerExistsAndNonExistingBreweryThrowsApiExceptionForBadRequest()
        {
            // Arrange
            using var unitOfWork = this.UnitOfWork;
            var updateCommand = new UpdateBeerCommand(new UpdateBeerDto
            {
                Name = "Updated Beer Name",
                Style = "Ipa",
                BreweryId = 22
            }, 1);
            var handler = new UpdateBeerCommandHandler(unitOfWork);

            // Act
            var result = await Should.ThrowAsync<DapperyApiException>(async () => await handler.Handle(updateCommand, CancellationTestToken).ConfigureAwait(false)).ConfigureAwait(false);

            // Assert
            result.ShouldNotBeNull();
            result.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        }
    }
}
