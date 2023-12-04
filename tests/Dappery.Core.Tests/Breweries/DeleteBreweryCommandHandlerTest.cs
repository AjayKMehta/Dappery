using System.Net;
using System.Threading.Tasks;

using Dappery.Core.Breweries.Commands.DeleteBrewery;
using Dappery.Core.Data;
using Dappery.Core.Exceptions;

using MediatR;

using Shouldly;

using Xunit;

namespace Dappery.Core.Tests.Breweries;

public class DeleteBreweryCommandHandlerTest : TestFixture
{
    [Fact]
    public async Task GivenValidDeleteRequestWhenBreweryExistsIsRemovedFromDatabaseIncludingAllBeersAsync()
    {
        // Arrange
        using IUnitOfWork unitOfWork = UnitOfWork;
        var deleteCommand = new DeleteBreweryCommand(1);
        var handler = new DeleteBreweryCommandHandler(unitOfWork);

        // Act
        Unit result = await handler.Handle(deleteCommand, CancellationTestToken);

        // Assert
        _ = result.ShouldBeOfType<Unit>();
    }

    [Fact]
    public async Task GivenValidDeleteRequestWhenDoesNotBreweryExistIsNotRemovedFromDatabaseAndExceptionIsThrownAsync()
    {
        // Arrange
        using IUnitOfWork unitOfWork = UnitOfWork;
        var deleteCommand = new DeleteBreweryCommand(11);
        var handler = new DeleteBreweryCommandHandler(unitOfWork);

        // Act
        DapperyApiException result = await Should.ThrowAsync<DapperyApiException>(async () => await handler.Handle(deleteCommand, CancellationTestToken));

        // Assert
        result.ShouldNotBeNull().StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }
}
