using System.Net;
using System.Threading.Tasks;
using Dappery.Core.Breweries.Commands.DeleteBrewery;
using Dappery.Core.Exceptions;
using MediatR;
using Shouldly;
using Xunit;

namespace Dappery.Core.Tests.Breweries
{
    public class DeleteBreweryCommandHandlerTest : TestFixture
    {
        [Fact]
        public async Task GivenValidDeleteRequestWhenBreweryExistsIsRemovedFromDatabaseIncludingAllBeers()
        {
            // Arrange
            using var unitOfWork = this.UnitOfWork;
            var deleteCommand = new DeleteBreweryCommand(1);
            var handler = new DeleteBreweryCommandHandler(unitOfWork);

            // Act
            var result = await handler.Handle(deleteCommand, CancellationTestToken).ConfigureAwait(false);

            // Assert
            result.ShouldBeOfType<Unit>();
        }

        [Fact]
        public async Task GivenValidDeleteRequestWhenDoesNotBreweryExistIsNotRemovedFromDatabaseAndExceptionIsThrown()
        {
            // Arrange
            using var unitOfWork = this.UnitOfWork;
            var deleteCommand = new DeleteBreweryCommand(11);
            var handler = new DeleteBreweryCommandHandler(unitOfWork);

            // Act
            var result = await Should.ThrowAsync<DapperyApiException>(async () => await handler.Handle(deleteCommand, CancellationTestToken).ConfigureAwait(false)).ConfigureAwait(false);

            // Assert
            result.ShouldNotBeNull();
            result.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        }
    }
}
