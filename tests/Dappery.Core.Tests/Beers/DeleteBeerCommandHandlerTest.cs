using System.Net;
using System.Threading.Tasks;

using Dappery.Core.Beers.Commands.DeleteBeer;
using Dappery.Core.Exceptions;

using MediatR;

using Shouldly;

using Xunit;

namespace Dappery.Core.Tests.Beers;

public class DeleteBeerCommandHandlerTest : TestFixture
{
    [Fact]
    public async Task GivenValidRequestWhenBeerExistsDeletesBeerAndReturnsUnit()
    {
        // Arrange
        using var unitOfWork = this.UnitOfWork;
        var deleteCommand = new DeleteBeerCommand(1);
        var handler = new DeleteBeerCommandHandler(unitOfWork);

        // Act
        var result = await handler.Handle(deleteCommand, CancellationTestToken).ConfigureAwait(false);

        // Assert
        _ = result.ShouldBeOfType<Unit>();
    }

    [Fact]
    public async Task GivenValidRequestWhenBeerDoesNotExistThrowsApiErrorForNotFound()
    {
        // Arrange
        using var unitOfWork = this.UnitOfWork;
        var deleteCommand = new DeleteBeerCommand(11);
        var handler = new DeleteBeerCommandHandler(unitOfWork);

        // Act
        var result = await Should.ThrowAsync<DapperyApiException>(async () => await handler.Handle(deleteCommand, CancellationTestToken).ConfigureAwait(false)).ConfigureAwait(false);

        // Assert
        result.ShouldNotBeNull().StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }
}
