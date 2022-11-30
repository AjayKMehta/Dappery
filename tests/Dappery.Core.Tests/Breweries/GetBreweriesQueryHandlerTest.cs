using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Dappery.Core.Breweries.Queries.GetBreweries;
using Dappery.Core.Data;
using Dappery.Domain.Dtos.Brewery;
using Dappery.Domain.Media;

using Shouldly;

using Xunit;

namespace Dappery.Core.Tests.Breweries;

public class GetBreweriesQueryHandlerTest : TestFixture
{
    [Fact]
    public async Task GetBreweriesQueryHandlerWhenBreweriesExistReturnsListOfBreweriesWithBeersAsync()
    {
        // Arrange
        using IUnitOfWork unitOfWork = UnitOfWork;
        var handler = new GetBreweriesQueryHandler(unitOfWork);

        // Act
        BreweryResourceList response = await handler.Handle(new GetBreweriesQuery(), CancellationTestToken).ConfigureAwait(false);

        // Assert
        BreweryResourceList breweryList = response
            .ShouldNotBeNull()
            .ShouldBeOfType<BreweryResourceList>();
        breweryList.Count.ShouldBe(2);

        IEnumerable<BreweryDto> items11 = breweryList
            .Items
            .ShouldNotBeNull();
        items11.ShouldNotBeEmpty();

        BreweryDto? firstBreweryDto = items11.FirstOrDefault(b => b.Id == 1);
        _ = firstBreweryDto?.Address.ShouldNotBeNull();
        firstBreweryDto?.Beers.ShouldNotBeEmpty();
        firstBreweryDto?.BeerCount.ShouldBe(3);

        BreweryDto? secondBreweryDto = items11.FirstOrDefault(b => b.Id == 2);
        _ = secondBreweryDto?.Address.ShouldNotBeNull();
        secondBreweryDto?.Beers.ShouldNotBeEmpty();
        _ = secondBreweryDto?.BeerCount.ShouldNotBeNull();
        secondBreweryDto?.BeerCount.ShouldBe(2);
    }

    [Fact]
    public async Task GetBreweriesQueryHandlerWhenNoBreweriesExistReturnsEmptyListOfBreweriesAsync()
    {
        // Arrange, remove all breweries from the test database
        using IUnitOfWork unitOfWork = UnitOfWork;
        await UnitOfWork.BreweryRepository.DeleteBrewery(1, CancellationTestToken).ConfigureAwait(false);
        await UnitOfWork.BreweryRepository.DeleteBrewery(2, CancellationTestToken).ConfigureAwait(false);
        var handler = new GetBreweriesQueryHandler(unitOfWork);

        // Act
        BreweryResourceList response = await handler.Handle(new GetBreweriesQuery(), CancellationTestToken).ConfigureAwait(false);

        // Assert
        response
            .ShouldNotBeNull()
            .ShouldBeOfType<BreweryResourceList>()
            .Items
            .ShouldNotBeNull()
            .ShouldBeEmpty();
        response.Count.ShouldBe(0);
    }
}
