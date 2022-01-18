using System.Net;
using System.Threading.Tasks;

using Dappery.Core.Beers.Queries.RetrieveBeer;
using Dappery.Core.Exceptions;

using Shouldly;

using Xunit;

namespace Dappery.Core.Tests.Beers;

public class RetrieveBeerQueryHandlerTest : TestFixture
{
    [Fact]
    public async Task GivenValidRequestWhenBeerExistsReturnsMappedBeer()
    {
        // Arrange
        using var unitOfWork = this.UnitOfWork;
        var query = new RetrieveBeerQuery(1);
        var handler = new RetrieveBeerQueryHandler(unitOfWork);

        // Act
        var result = await handler.Handle(query, CancellationTestToken).ConfigureAwait(false);

        // Assert
        var beerDto =
            result.ShouldNotBeNull()
                .Self
                .ShouldNotBeNull();

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
    public async Task GivenValidRequestWhenBeerDoesNotExistThrowsApiExceptionForNotFound()
    {
        // Arrange
        using var unitOfWork = this.UnitOfWork;
        var query = new RetrieveBeerQuery(11);
        var handler = new RetrieveBeerQueryHandler(unitOfWork);

        // Act
        var result = await Should.ThrowAsync<DapperyApiException>(async () => await handler.Handle(query, CancellationTestToken).ConfigureAwait(false)).ConfigureAwait(false);

        // Assert
        result.ShouldNotBeNull().StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }
}
