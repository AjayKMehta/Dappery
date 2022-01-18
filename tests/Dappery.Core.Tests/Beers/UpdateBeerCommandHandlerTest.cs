using System.Net;
using System.Threading.Tasks;

using Dappery.Core.Beers.Commands.UpdateBeer;
using Dappery.Core.Exceptions;
using Dappery.Domain.Dtos.Beer;
using Dappery.Domain.Entities;

using Shouldly;

using Xunit;

namespace Dappery.Core.Tests.Beers;

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
        var beerDto = result
            .ShouldNotBeNull()
            .Self
            .ShouldNotBeNull();
        beerDto.Id.ShouldBe(updateCommand.BeerId);
        beerDto.Name.ShouldBe(updateCommand.Dto.Name);
        beerDto.Style.ShouldBe(updateCommand.Dto.Style);

        var breweryDto = beerDto.Brewery.ShouldNotBeNull();
        breweryDto.Id.ShouldBe(1);
        breweryDto.Name.ShouldBe("Fall River Brewery");
        breweryDto.Beers.ShouldBeNull();

        var addressDto = breweryDto.Address.ShouldNotBeNull();
        addressDto.StreetAddress.ShouldBe("1030 E Cypress Ave Ste D");
        addressDto.City.ShouldBe("Redding");
        addressDto.State.ShouldBe("CA");
        addressDto.ZipCode.ShouldBe("96002");
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
        var beerDto = result
            .ShouldNotBeNull()
            .Self
            .ShouldNotBeNull();
        beerDto.Id.ShouldBe(updateCommand.BeerId);
        beerDto.Name.ShouldBe(updateCommand.Dto.Name);
        beerDto.Style.ShouldBe(nameof(BeerStyle.Other));

        var breweryDto = beerDto.Brewery.ShouldNotBeNull();
        breweryDto.Id.ShouldBe(1);
        breweryDto.Name.ShouldBe("Fall River Brewery");
        breweryDto.Beers.ShouldBeNull();

        var addressDto = breweryDto.Address.ShouldNotBeNull();
        addressDto.StreetAddress.ShouldBe("1030 E Cypress Ave Ste D");
        addressDto.City.ShouldBe("Redding");
        addressDto.State.ShouldBe("CA");
        addressDto.ZipCode.ShouldBe("96002");
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
        var beerDto = result
            .ShouldNotBeNull()
            .Self
            .ShouldNotBeNull();
        beerDto.Id.ShouldBe(updateCommand.BeerId);
        beerDto.Name.ShouldBe(updateCommand.Dto.Name);
        beerDto.Style.ShouldBe(updateCommand.Dto.Style);

        var breweryDto = beerDto.Brewery.ShouldNotBeNull();
        breweryDto.Beers.ShouldBeNull();
        breweryDto.Id.ShouldBe(2);
        breweryDto.Name.ShouldBe("Sierra Nevada Brewing Company");

        var addressDto = breweryDto.Address.ShouldNotBeNull();
        addressDto.StreetAddress.ShouldBe("1075 E 20th St");
        addressDto.City.ShouldBe("Chico");
        addressDto.State.ShouldBe("CA");
        addressDto.ZipCode.ShouldBe("95928");
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
        result.ShouldNotBeNull().StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }
}
