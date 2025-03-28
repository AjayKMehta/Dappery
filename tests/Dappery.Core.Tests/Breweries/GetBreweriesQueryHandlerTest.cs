using System.Collections.Generic;
using System.Threading.Tasks;

using Dappery.Core.Breweries.Queries.GetBreweries;
using Dappery.Core.Data;
using Dappery.Domain.Dtos.Brewery;
using Dappery.Domain.Media;

using Shouldly;

namespace Dappery.Core.Tests.Breweries;

internal sealed class GetBreweriesQueryHandlerTest : TestFixture
{
    [Test]
    public async Task GetBreweriesQueryHandlerWhenBreweriesExistReturnsListOfBreweriesWithBeersAsync()
    {
        // Arrange
        using IUnitOfWork unitOfWork = UnitOfWork;
        var handler = new GetBreweriesQueryHandler(unitOfWork);

        // Act
        BreweryResourceList response = await handler.Handle(new GetBreweriesQuery(), CancellationTestToken);

        // Assert
        BreweryResourceList breweryList = response
            .ShouldNotBeNull()
            .ShouldBeOfType<BreweryResourceList>();
        breweryList.Count.ShouldBe(2);

        IEnumerable<BreweryDto> items11 = breweryList
            .Items
            .ShouldNotBeNull();
        items11.ShouldNotBeEmpty();

        BreweryDto? firstBreweryDto = null, secondBreweryDto = null;
        foreach (BreweryDto dto in items11)
        {
            if (firstBreweryDto is null && dto.Id == 1)
                firstBreweryDto = dto;
            if (secondBreweryDto is null && dto.Id == 2)
                secondBreweryDto = dto;
            if (firstBreweryDto is not null && secondBreweryDto is not null)
                break;
        }

        _ = firstBreweryDto?.Address.ShouldNotBeNull();
        firstBreweryDto?.Beers.ShouldNotBeEmpty();
        firstBreweryDto?.BeerCount.ShouldBe(3);

        _ = secondBreweryDto?.Address.ShouldNotBeNull();
        secondBreweryDto?.Beers.ShouldNotBeEmpty();
        _ = secondBreweryDto?.BeerCount.ShouldNotBeNull();
        secondBreweryDto?.BeerCount.ShouldBe(2);
    }

    [Test]
    public async Task GetBreweriesQueryHandlerWhenNoBreweriesExistReturnsEmptyListOfBreweriesAsync()
    {
        // Arrange, remove all breweries from the test database
        using IUnitOfWork unitOfWork = UnitOfWork;
        await UnitOfWork.BreweryRepository.DeleteBrewery(1, CancellationTestToken);
        await UnitOfWork.BreweryRepository.DeleteBrewery(2, CancellationTestToken);
        var handler = new GetBreweriesQueryHandler(unitOfWork);

        // Act
        BreweryResourceList response = await handler.Handle(new GetBreweriesQuery(), CancellationTestToken);

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
