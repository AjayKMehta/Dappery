using System.Net;
using System.Threading.Tasks;

using Dappery.Core.Breweries.Queries.RetrieveBrewery;
using Dappery.Core.Data;
using Dappery.Core.Exceptions;
using Dappery.Domain.Dtos.Brewery;
using Dappery.Domain.Media;

using Shouldly;

using Xunit;

namespace Dappery.Core.Tests.Breweries;

public class RetrieveBreweryQueryHandlerTest : TestFixture
{
    [Fact]
    public async Task RetrieveBreweryHandlerGivenExistingBreweryIdReturnsBreweryWithBeersAsync()
    {
        // Arrange
        using IUnitOfWork unitOfWork = UnitOfWork;
        var query = new RetrieveBreweryQuery(1);
        var handler = new RetrieveBreweryQueryHandler(unitOfWork);

        // Act
        BreweryResource response = await handler.Handle(query, CancellationTestToken);

        // Assert
        BreweryDto breweryDto = response
            .ShouldNotBeNull()
            .ShouldBeOfType<BreweryResource>()
            .Self
            .ShouldNotBeNull();
        _ = breweryDto.Address.ShouldNotBeNull();
        breweryDto
            .Beers
            .ShouldNotBeNull()
            .ShouldNotBeEmpty();
        breweryDto.Name.ShouldBe("Fall River Brewery");
        breweryDto.BeerCount?.ShouldBe(3);
    }

    [Fact]
    public async Task RetrieveBreweryHandlerGivenNonExistingBreweryIdReturnsApiExceptionAsync()
    {
        // Arrange
        using IUnitOfWork unitOfWork = UnitOfWork;
        var query = new RetrieveBreweryQuery(11);
        var handler = new RetrieveBreweryQueryHandler(unitOfWork);

        // Act
        DapperyApiException response = await Should.ThrowAsync<DapperyApiException>(async () => await handler.Handle(query, CancellationTestToken));

        // Assert
        response
            .ShouldNotBeNull()
            .ShouldBeOfType<DapperyApiException>()
            .StatusCode
            .ShouldBe(HttpStatusCode.NotFound);
    }
}
