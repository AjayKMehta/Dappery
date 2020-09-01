using System.Net;
using System.Threading.Tasks;
using Dappery.Core.Beers.Commands.DeleteBeer;
using Dappery.Core.Exceptions;
using MediatR;
using Shouldly;
using Xunit;

namespace Dappery.Core.Tests.Beers
{
    public class DeleteBeerCommandHandlerTest : TestFixture
    {
        [Fact]
        public async Task GivenValidRequest_WhenBeerExists_DeletesBeerAndReturnsUnit()
        {
            // Arrange
            using var unitOfWork = UnitOfWork;
            var deleteCommand = new DeleteBeerCommand(1);
            var handler = new DeleteBeerCommandHandler(unitOfWork);

            // Act
            var result = await handler.Handle(deleteCommand, CancellationTestToken);

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBeOfType<Unit>();
        }

        [Fact]
        public async Task GivenValidRequest_WhenBeerDoesNotExist_ThrowsApiErrorForNotFound()
        {
            // Arrange
            using var unitOfWork = UnitOfWork;
            var deleteCommand = new DeleteBeerCommand(11);
            var handler = new DeleteBeerCommandHandler(unitOfWork);

            // Act
            var result = await Should.ThrowAsync<DapperyApiException>(async () => await handler.Handle(deleteCommand, CancellationTestToken));

            // Assert
            result.ShouldNotBeNull();
            result.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        }
    }
}
