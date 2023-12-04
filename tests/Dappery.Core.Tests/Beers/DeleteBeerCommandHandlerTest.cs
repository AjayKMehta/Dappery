using System.Net;
using System.Threading.Tasks;

using Dappery.Core.Beers.Commands.DeleteBeer;
using Dappery.Core.Data;
using Dappery.Core.Exceptions;

using MediatR;

using Shouldly;

using Xunit;

namespace Dappery.Core.Tests.Beers;

public class DeleteBeerCommandHandlerTest : TestFixture
{
    [Fact]
    public async Task GivenValidRequestWhenBeerExistsDeletesBeerAndReturnsUnitAsync()
    {
        // Arrange
        using IUnitOfWork unitOfWork = UnitOfWork;
        var deleteCommand = new DeleteBeerCommand(1);
        var handler = new DeleteBeerCommandHandler(unitOfWork);

        // Act
        Unit result = await handler.Handle(deleteCommand, CancellationTestToken);

        // Assert
        _ = result.ShouldBeOfType<Unit>();
    }

    [Fact]
    public async Task GivenValidRequestWhenBeerDoesNotExistThrowsApiErrorForNotFoundAsync()
    {
        // Arrange
        using IUnitOfWork unitOfWork = UnitOfWork;
        var deleteCommand = new DeleteBeerCommand(11);
        var handler = new DeleteBeerCommandHandler(unitOfWork);

        // Act
        DapperyApiException result = await Should.ThrowAsync<DapperyApiException>(async () => await handler.Handle(deleteCommand, CancellationTestToken));

        // Assert
        result.ShouldNotBeNull().StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }
}
