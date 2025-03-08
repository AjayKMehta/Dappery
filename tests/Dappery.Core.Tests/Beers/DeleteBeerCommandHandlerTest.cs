using System.Net;
using System.Threading.Tasks;

using Dappery.Core.Beers.Commands.DeleteBeer;
using Dappery.Core.Data;
using Dappery.Core.Exceptions;

using MediatR;

using Shouldly;

namespace Dappery.Core.Tests.Beers;

internal sealed class DeleteBeerCommandHandlerTest : TestFixture
{
    [Test]
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

    [Test]
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
