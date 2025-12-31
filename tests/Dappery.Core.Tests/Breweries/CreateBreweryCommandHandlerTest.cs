using System;
using System.Threading;
using System.Threading.Tasks;

using Dappery.Core.Breweries.Commands.CreateBrewery;
using Dappery.Core.Data;
using Dappery.Domain.Dtos;
using Dappery.Domain.Dtos.Brewery;
using Dappery.Domain.Media;

using Shouldly;

namespace Dappery.Core.Tests.Breweries;

internal sealed class CreateBreweryCommandHandlerTest : TestFixture
{
    [Test]
    public async Task CreateBreweryCommandHandlerGivenAValidRequestCreatesBreweryAsync()
    {
        // Arrange
        var createdAddress = new AddressDto
        {
            City = "San Diego",
            State = "CA",
            StreetAddress = "123 San Diego St.",
            ZipCode = "92109"
        };
        using IUnitOfWork unitOfWork = UnitOfWork;
        var createBreweryDto = new CreateBreweryDto
        {
            Name = "Pizza Port Brewing Company",
            Address = createdAddress
        };

        // Act
        var handler = new CreateBreweryCommandHandler(unitOfWork, TimeProvider.System);
        BreweryResource createdBrewery = await handler.Handle(new CreateBreweryCommand(createBreweryDto), CancellationToken.None);

        // Assert
        BreweryDto breweryDto = createdBrewery
            .ShouldNotBeNull()
            .ShouldBeOfType<BreweryResource>()
            .Self
            .ShouldNotBeNull()
            .ShouldBeOfType<BreweryDto>();
        breweryDto.Name.ShouldNotBeNull().ShouldBe(createBreweryDto.Name);
        breweryDto.Beers.ShouldBeEmpty();
        breweryDto.BeerCount.ShouldBe(0);

        AddressDto addressDto = breweryDto.Address.ShouldNotBeNull();
        addressDto.City.ShouldBe(createdAddress.City);
        addressDto.State.ShouldBe(createdAddress.State);
        addressDto.StreetAddress.ShouldBe(createdAddress.StreetAddress);
        addressDto.ZipCode.ShouldBe(createdAddress.ZipCode);
    }
}
