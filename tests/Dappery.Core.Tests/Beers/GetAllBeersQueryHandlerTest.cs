using System.Threading.Tasks;

using Dappery.Core.Beers.Queries.GetBeers;
using Dappery.Core.Data;
using Dappery.Domain.Media;

using Shouldly;

using Xunit;

namespace Dappery.Core.Tests.Beers;

public class GetAllBeersQueryHandlerTest : TestFixture
{
    [Fact]
    public async Task GivenValidRequestWhenBeersArePopulatedReturnsMappedBeerListAsync()
    {
        // Arrange
        using IUnitOfWork unitOfWork = UnitOfWork;
        var query = new GetBeersQuery();
        var handler = new GetBeersQueryHandler(unitOfWork);

        // Act
        BeerResourceList result = await handler.Handle(query, CancellationTestToken).ConfigureAwait(false);

        // Assert
        result.ShouldNotBeNull().Count.ShouldBe(5);
        _ = result.Items.ShouldNotBeNull();
    }

    [Fact]
    public async Task GivenValidRequestWhenBeersAreNotPopulatedReturnsMappedEmptyBeerListAsync()
    {
        // Arrange
        using IUnitOfWork unitOfWork = UnitOfWork;
        var query = new GetBeersQuery();
        var handler = new GetBeersQueryHandler(unitOfWork);
        await unitOfWork.BeerRepository.DeleteBeerAsync(1, CancellationTestToken).ConfigureAwait(false);
        await unitOfWork.BeerRepository.DeleteBeerAsync(2, CancellationTestToken).ConfigureAwait(false);
        await unitOfWork.BeerRepository.DeleteBeerAsync(3, CancellationTestToken).ConfigureAwait(false);
        await unitOfWork.BeerRepository.DeleteBeerAsync(4, CancellationTestToken).ConfigureAwait(false);
        await unitOfWork.BeerRepository.DeleteBeerAsync(5, CancellationTestToken).ConfigureAwait(false);

        // Act
        BeerResourceList result = await handler.Handle(query, CancellationTestToken).ConfigureAwait(false);

        // Assert
        result.ShouldNotBeNull().Count.ShouldBe(0);
        _ = result.Items.ShouldNotBeNull();
    }
}
