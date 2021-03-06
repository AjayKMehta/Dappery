using System.Net;
using System.Threading.Tasks;
using Dappery.Core.Breweries.Queries.RetrieveBrewery;
using Dappery.Core.Exceptions;
using Dappery.Domain.Media;
using Shouldly;
using Xunit;

namespace Dappery.Core.Tests.Breweries
{
    public class RetrieveBreweryQueryHandlerTest : TestFixture
    {
        [Fact]
        public async Task RetrieveBreweryHandlerGivenExistingBreweryIdReturnsBreweryWithBeers()
        {
            // Arrange
            using var unitOfWork = this.UnitOfWork;
            var query = new RetrieveBreweryQuery(1);
            var handler = new RetrieveBreweryQueryHandler(unitOfWork);

            // Act
            var response = await handler.Handle(query, CancellationTestToken).ConfigureAwait(false);

            // Assert
            response.ShouldNotBeNull();
            response.ShouldBeOfType<BreweryResource>();
            response.Self.ShouldNotBeNull();
            response.Self.Address?.ShouldNotBeNull();
            response.Self.Beers.ShouldNotBeNull();
            response.Self.Beers.ShouldNotBeEmpty();
            response.Self.Name.ShouldBe("Fall River Brewery");
            response.Self.BeerCount?.ShouldBe(3);
        }

        [Fact]
        public async Task RetrieveBreweryHandlerGivenNonExistingBreweryIdReturnsApiException()
        {
            // Arrange
            using var unitOfWork = this.UnitOfWork;
            var query = new RetrieveBreweryQuery(11);
            var handler = new RetrieveBreweryQueryHandler(unitOfWork);

            // Act
            var response = await Should.ThrowAsync<DapperyApiException>(async () => await handler.Handle(query, CancellationTestToken).ConfigureAwait(false)).ConfigureAwait(false);

            // Assert
            response.ShouldNotBeNull();
            response.ShouldBeOfType<DapperyApiException>();
            response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        }
    }
}
