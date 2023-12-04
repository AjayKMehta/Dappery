using System.Net;
using System.Threading.Tasks;

using Dappery.Core.Beers.Commands.UpdateBeer;
using Dappery.Core.Data;
using Dappery.Core.Exceptions;
using Dappery.Domain.Dtos;
using Dappery.Domain.Dtos.Beer;
using Dappery.Domain.Dtos.Brewery;
using Dappery.Domain.Entities;
using Dappery.Domain.Media;

using Shouldly;

using Xunit;

namespace Dappery.Core.Tests.Beers;

public class UpdateBeerCommandHandlerTest : TestFixture
{
    [Fact]
    public async Task GivenValidRequestWhenBeerExistsAndBreweryIsNotUpdatedUpdatesAndReturnsMappedBeerAsync()
    {
        // Arrange
        using IUnitOfWork unitOfWork = UnitOfWork;
        var updateCommand = new UpdateBeerCommand(new UpdateBeerDto
        {
            Name = "Updated Beer Name",
            Style = "Ipa"
        }, 1);
        var handler = new UpdateBeerCommandHandler(unitOfWork);

        // Act
        BeerResource result = await handler.Handle(updateCommand, CancellationTestToken);

        // Assert
        BeerDto beerDto = result
            .ShouldNotBeNull()
            .Self
            .ShouldNotBeNull();
        beerDto.Id.ShouldBe(updateCommand.BeerId);
        beerDto.Name.ShouldBe(updateCommand.Dto.Name);
        beerDto.Style.ShouldBe(updateCommand.Dto.Style);

        BreweryDto breweryDto = beerDto.Brewery.ShouldNotBeNull();
        breweryDto.Id.ShouldBe(1);
        breweryDto.Name.ShouldBe("Fall River Brewery");
        breweryDto.Beers.ShouldBeNull();

        AddressDto addressDto = breweryDto.Address.ShouldNotBeNull();
        addressDto.StreetAddress.ShouldBe("1030 E Cypress Ave Ste D");
        addressDto.City.ShouldBe("Redding");
        addressDto.State.ShouldBe("CA");
        addressDto.ZipCode.ShouldBe("96002");
    }

    [Fact]
    public async Task GivenValidRequestWhenBeerExistsAndStyleIsUnknownUpdatesAndReturnsMappedBeerWithOtherStyleAsync()
    {
        // Arrange
        using IUnitOfWork unitOfWork = UnitOfWork;
        var updateCommand = new UpdateBeerCommand(new UpdateBeerDto
        {
            Name = "Updated Beer Name",
            Style = "Not a valid beer style!"
        }, 1);
        var handler = new UpdateBeerCommandHandler(unitOfWork);

        // Act
        BeerResource result = await handler.Handle(updateCommand, CancellationTestToken);

        // Assert
        BeerDto beerDto = result
            .ShouldNotBeNull()
            .Self
            .ShouldNotBeNull();
        beerDto.Id.ShouldBe(updateCommand.BeerId);
        beerDto.Name.ShouldBe(updateCommand.Dto.Name);
        beerDto.Style.ShouldBe(nameof(BeerStyle.Other));

        BreweryDto breweryDto = beerDto.Brewery.ShouldNotBeNull();
        breweryDto.Id.ShouldBe(1);
        breweryDto.Name.ShouldBe("Fall River Brewery");
        breweryDto.Beers.ShouldBeNull();

        AddressDto addressDto = breweryDto.Address.ShouldNotBeNull();
        addressDto.StreetAddress.ShouldBe("1030 E Cypress Ave Ste D");
        addressDto.City.ShouldBe("Redding");
        addressDto.State.ShouldBe("CA");
        addressDto.ZipCode.ShouldBe("96002");
    }

    [Fact]
    public async Task GivenValidRequestWhenBeerExistsAndExistingBreweryIsUpdatedUpdatesAndReturnsMappedBeerAsync()
    {
        // Arrange
        using IUnitOfWork unitOfWork = UnitOfWork;
        var updateCommand = new UpdateBeerCommand(new UpdateBeerDto
        {
            Name = "Updated Beer Name",
            Style = "Ipa",
            BreweryId = 2
        }, 1);
        var handler = new UpdateBeerCommandHandler(unitOfWork);

        // Act
        BeerResource result = await handler.Handle(updateCommand, CancellationTestToken);

        // Assert
        BeerDto beerDto = result
            .ShouldNotBeNull()
            .Self
            .ShouldNotBeNull();
        beerDto.Id.ShouldBe(updateCommand.BeerId);
        beerDto.Name.ShouldBe(updateCommand.Dto.Name);
        beerDto.Style.ShouldBe(updateCommand.Dto.Style);

        BreweryDto breweryDto = beerDto.Brewery.ShouldNotBeNull();
        breweryDto.Beers.ShouldBeNull();
        breweryDto.Id.ShouldBe(2);
        breweryDto.Name.ShouldBe("Sierra Nevada Brewing Company");

        AddressDto addressDto = breweryDto.Address.ShouldNotBeNull();
        addressDto.StreetAddress.ShouldBe("1075 E 20th St");
        addressDto.City.ShouldBe("Chico");
        addressDto.State.ShouldBe("CA");
        addressDto.ZipCode.ShouldBe("95928");
    }

    [Fact]
    public async Task GivenValidRequestWhenBeerExistsAndNonExistingBreweryThrowsApiExceptionForBadRequestAsync()
    {
        // Arrange
        using IUnitOfWork unitOfWork = UnitOfWork;
        var updateCommand = new UpdateBeerCommand(new UpdateBeerDto
        {
            Name = "Updated Beer Name",
            Style = "Ipa",
            BreweryId = 22
        }, 1);
        var handler = new UpdateBeerCommandHandler(unitOfWork);

        // Act
        DapperyApiException result = await Should.ThrowAsync<DapperyApiException>(async () => await handler.Handle(updateCommand, CancellationTestToken));

        // Assert
        result.ShouldNotBeNull().StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }
}
