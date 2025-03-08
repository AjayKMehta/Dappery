using System.Threading.Tasks;

using Dappery.Core.Beers.Queries.GetBeers;
using Dappery.Core.Data;
using Dappery.Domain.Media;

using Shouldly;

namespace Dappery.Core.Tests.Beers;

internal sealed class GetAllBeersQueryHandlerTest : TestFixture
{
    [Test]
    public async Task GivenValidRequestWhenBeersArePopulatedReturnsMappedBeerListAsync()
    {
        // Arrange
        using IUnitOfWork unitOfWork = UnitOfWork;
        var query = new GetBeersQuery();
        var handler = new GetBeersQueryHandler(unitOfWork);

        // Act
        BeerResourceList result = await handler.Handle(query, CancellationTestToken);

        // Assert
        result.ShouldNotBeNull().Count.ShouldBe(5);
        _ = result.Items.ShouldNotBeNull();
    }

    [Test]
    public async Task GivenValidRequestWhenBeersAreNotPopulatedReturnsMappedEmptyBeerListAsync()
    {
        // Arrange
        using IUnitOfWork unitOfWork = UnitOfWork;
        var query = new GetBeersQuery();
        var handler = new GetBeersQueryHandler(unitOfWork);
        await unitOfWork.BeerRepository.DeleteBeerAsync(1, CancellationTestToken);
        await unitOfWork.BeerRepository.DeleteBeerAsync(2, CancellationTestToken);
        await unitOfWork.BeerRepository.DeleteBeerAsync(3, CancellationTestToken);
        await unitOfWork.BeerRepository.DeleteBeerAsync(4, CancellationTestToken);
        await unitOfWork.BeerRepository.DeleteBeerAsync(5, CancellationTestToken);

        // Act
        BeerResourceList result = await handler.Handle(query, CancellationTestToken);

        // Assert
        result.ShouldNotBeNull().Count.ShouldBe(0);
        _ = result.Items.ShouldNotBeNull();
    }
}
