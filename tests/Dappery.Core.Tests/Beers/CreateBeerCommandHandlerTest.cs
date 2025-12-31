using System;
using System.Net;
using System.Threading.Tasks;

using Dappery.Core.Beers.Commands.CreateBeer;
using Dappery.Core.Data;
using Dappery.Core.Exceptions;
using Dappery.Domain.Dtos;
using Dappery.Domain.Dtos.Beer;
using Dappery.Domain.Dtos.Brewery;
using Dappery.Domain.Entities;
using Dappery.Domain.Media;

using Shouldly;

namespace Dappery.Core.Tests.Beers;

internal sealed class CreateBeerCommandHandlerTest : TestFixture
{
    [Test]
    public async Task GivenValidRequestWhenBreweryExistsReturnsMappedAndCreatedBeerAsync()
    {
        // Arrange
        using IUnitOfWork unitOfWork = UnitOfWork;
        var beerCommand = new CreateBeerCommand(new CreateBeerDto
        {
            Name = "Test Beer",
            Style = "Lager",
            BreweryId = 1
        });
        var handler = new CreateBeerCommandHandler(unitOfWork, TimeProvider.System);

        // Act
        BeerResource result = await handler.Handle(beerCommand, CancellationTestToken);

        // Assert
        BeerDto beerDto = result
            .ShouldNotBeNull()
            .ShouldBeOfType<BeerResource>()
            .Self
            .ShouldNotBeNull();
        beerDto.Name.ShouldBe(beerCommand.Dto.Name);
        beerDto.Style.ShouldBe(beerCommand.Dto.Style);

        BreweryDto breweryDto = beerDto.Brewery.ShouldNotBeNull();
        breweryDto.Beers.ShouldBeNull();
        breweryDto.Id.ShouldBe(1);
        breweryDto.Name.ShouldBe("Fall River Brewery");

        AddressDto addressDto = breweryDto.Address.ShouldNotBeNull();
        addressDto.StreetAddress.ShouldBe("1030 E Cypress Ave Ste D");
        addressDto.City.ShouldBe("Redding");
        addressDto.State.ShouldBe("CA");
        addressDto.ZipCode.ShouldBe("96002");
    }

    [Test]
    public async Task GivenValidRequestWhenBreweryDoesNotExistThrowsApiExceptionForBadRequestAsync()
    {
        // Arrange
        using IUnitOfWork unitOfWork = UnitOfWork;
        var beerCommand = new CreateBeerCommand(new CreateBeerDto
        {
            Name = "Test Beer",
            Style = "Lager",
            BreweryId = 11
        });
        var handler = new CreateBeerCommandHandler(unitOfWork, TimeProvider.System);

        // Act
        DapperyApiException result = await Should.ThrowAsync<DapperyApiException>(async () => await handler.Handle(beerCommand, CancellationTestToken));

        // Assert
        result.ShouldNotBeNull().StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }

    [Test]
    public async Task GivenValidRequestWithInvalidBeerStyleReturnsMappedAndCreatedBeerWithOtherAsStyleAsync()
    {
        // Arrange
        using IUnitOfWork unitOfWork = UnitOfWork;
        var beerCommand = new CreateBeerCommand(new CreateBeerDto
        {
            Name = "Test Beer",
            Style = "Not defined!",
            BreweryId = 1
        });
        var handler = new CreateBeerCommandHandler(unitOfWork, TimeProvider.System);

        // Act
        BeerResource result = await handler.Handle(beerCommand, CancellationTestToken);

        // Assert
        BeerDto beerDto = result
            .ShouldNotBeNull()
            .ShouldBeOfType<BeerResource>()
            .Self
            .ShouldNotBeNull();
        beerDto.Name.ShouldBe(beerCommand.Dto.Name);
        beerDto.Style.ShouldBe(nameof(BeerStyle.Other));

        BreweryDto breweryDto = beerDto.Brewery.ShouldNotBeNull();
        breweryDto.Beers.ShouldBeNull();
        breweryDto.Id.ShouldBe(1);
        breweryDto.Name.ShouldBe("Fall River Brewery");

        AddressDto addressDto = breweryDto.Address.ShouldNotBeNull();
        addressDto.StreetAddress.ShouldBe("1030 E Cypress Ave Ste D");
        addressDto.City.ShouldBe("Redding");
        addressDto.State.ShouldBe("CA");
        addressDto.ZipCode.ShouldBe("96002");
    }
}
