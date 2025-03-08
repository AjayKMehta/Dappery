using System.Net;
using System.Threading.Tasks;

using Dappery.Core.Beers.Queries.RetrieveBeer;
using Dappery.Core.Data;
using Dappery.Core.Exceptions;
using Dappery.Domain.Dtos;
using Dappery.Domain.Dtos.Beer;
using Dappery.Domain.Dtos.Brewery;
using Dappery.Domain.Media;

using Shouldly;

namespace Dappery.Core.Tests.Beers;

internal sealed class RetrieveBeerQueryHandlerTest : TestFixture
{
    [Test]
    public async Task GivenValidRequestWhenBeerExistsReturnsMappedBeerAsync()
    {
        // Arrange
        using IUnitOfWork unitOfWork = UnitOfWork;
        var query = new RetrieveBeerQuery(1);
        var handler = new RetrieveBeerQueryHandler(unitOfWork);

        // Act
        BeerResource result = await handler.Handle(query, CancellationTestToken);

        // Assert
        BeerDto beerDto =
            result.ShouldNotBeNull()
                .Self
                .ShouldNotBeNull();

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
    public async Task GivenValidRequestWhenBeerDoesNotExistThrowsApiExceptionForNotFoundAsync()
    {
        // Arrange
        using IUnitOfWork unitOfWork = UnitOfWork;
        var query = new RetrieveBeerQuery(11);
        var handler = new RetrieveBeerQueryHandler(unitOfWork);

        // Act
        DapperyApiException result = await Should.ThrowAsync<DapperyApiException>(async () => await handler.Handle(query, CancellationTestToken));

        // Assert
        result.ShouldNotBeNull().StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }
}
