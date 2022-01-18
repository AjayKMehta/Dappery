using System.Net;
using System.Threading.Tasks;

using Dappery.Core.Beers.Commands.CreateBeer;
using Dappery.Core.Exceptions;
using Dappery.Domain.Dtos.Beer;
using Dappery.Domain.Entities;
using Dappery.Domain.Media;

using Shouldly;

using Xunit;

namespace Dappery.Core.Tests.Beers;

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
        var beerDto = result
            .ShouldNotBeNull()
            .ShouldBeOfType<BeerResource>()
            .Self
            .ShouldNotBeNull();
        beerDto.Name.ShouldBe(beerCommand.Dto.Name);
        beerDto.Style.ShouldBe(beerCommand.Dto.Style);

        var breweryDto = beerDto.Brewery.ShouldNotBeNull();
        breweryDto.Beers.ShouldBeNull();
        breweryDto.Id.ShouldBe(1);
        breweryDto.Name.ShouldBe("Fall River Brewery");

        var addressDto = breweryDto.Address.ShouldNotBeNull();
        addressDto.StreetAddress.ShouldBe("1030 E Cypress Ave Ste D");
        addressDto.City.ShouldBe("Redding");
        addressDto.State.ShouldBe("CA");
        addressDto.ZipCode.ShouldBe("96002");
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
        result.ShouldNotBeNull().StatusCode.ShouldBe(HttpStatusCode.BadRequest);
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
        var beerDto = result
            .ShouldNotBeNull()
            .ShouldBeOfType<BeerResource>()
            .Self
            .ShouldNotBeNull();
        beerDto.Name.ShouldBe(beerCommand.Dto.Name);
        beerDto.Style.ShouldBe(nameof(BeerStyle.Other));

        var breweryDto = beerDto.Brewery.ShouldNotBeNull();
        breweryDto.Beers.ShouldBeNull();
        breweryDto.Id.ShouldBe(1);
        breweryDto.Name.ShouldBe("Fall River Brewery");

        var addressDto = breweryDto.Address.ShouldNotBeNull();
        addressDto.StreetAddress.ShouldBe("1030 E Cypress Ave Ste D");
        addressDto.City.ShouldBe("Redding");
        addressDto.State.ShouldBe("CA");
        addressDto.ZipCode.ShouldBe("96002");
    }
}
