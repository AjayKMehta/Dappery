using System.Net;
using System.Threading.Tasks;

using Dappery.Core.Breweries.Commands.UpdateBrewery;
using Dappery.Core.Data;
using Dappery.Core.Exceptions;
using Dappery.Domain.Dtos;
using Dappery.Domain.Dtos.Brewery;
using Dappery.Domain.Media;

using Shouldly;

using Xunit;

namespace Dappery.Core.Tests.Breweries;

public class UpdateBreweryCommandHandlerTest : TestFixture
{
    [Fact]
    public async Task GivenValidUpdateRequestWhenBreweryExistsReturnsUpdatedMappedBreweryAsync()
    {
        // Arrange
        using var unitOfWork = UnitOfWork;
        const int BreweryId = 1;
        var updateCommand = new UpdateBreweryCommand(new UpdateBreweryDto
        {
            Address = new AddressDto
            {
                City = "Updated City",
                State = "Updated State",
                StreetAddress = "Updated Street Address",
                ZipCode = "12345"
            },
            Name = "Updated Brewery Name"
        }, BreweryId);

        // Act
        var commandHandler = new UpdateBreweryCommandHandler(unitOfWork);
        var result = await commandHandler.Handle(updateCommand, CancellationTestToken).ConfigureAwait(false);

        // Assert
        var breweryDTo = result
            .ShouldNotBeNull()
            .Self
            .ShouldNotBeNull();
        breweryDTo.Id.ShouldBe(BreweryId);
        breweryDTo.Name.ShouldBe(updateCommand.Dto.Name);

        _ = result.ApiVersion.ShouldNotBeNull();

        var addressDto = breweryDTo.Address.ShouldNotBeNull();
        addressDto.City.ShouldBe(updateCommand.Dto.Address?.City);
        addressDto.State.ShouldBe(updateCommand.Dto.Address?.State);
        addressDto.StreetAddress.ShouldBe(updateCommand.Dto.Address?.StreetAddress);
        addressDto.ZipCode.ShouldBe(updateCommand.Dto.Address?.ZipCode);
    }

    [Fact]
    public async Task GivenValidUpdateRequestWhenBreweryDoesNotExistThrowsNotFoundExceptionAsync()
    {
        // Arrange
        using var unitOfWork = UnitOfWork;
        const int BreweryId = 11;
        var updateCommand = new UpdateBreweryCommand(new UpdateBreweryDto
        {
            Address = new AddressDto
            {
                City = "Doesn't Exist!",
                State = "Doesn't Exist!",
                StreetAddress = "Doesn't Exist!",
                ZipCode = "Doesn't Exist!"
            },
            Name = "Doesn't Exist!"
        }, BreweryId);

        // Act
        var commandHandler = new UpdateBreweryCommandHandler(unitOfWork);
        var result = await Should.ThrowAsync<DapperyApiException>(async () => await commandHandler.Handle(updateCommand, CancellationTestToken).ConfigureAwait(false)).ConfigureAwait(false);

        // Assert
        result.ShouldNotBeNull().StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GivenValidUpdateRequestWhenBreweryDoesExistAndAddressIsNotUpdatedReturnsMappedBreweryWithNoUpdatedAddressAsync()
    {
        // Arrange
        using IUnitOfWork? unitOfWork = UnitOfWork;
        const int BreweryId = 1;
        var updateCommand = new UpdateBreweryCommand(new UpdateBreweryDto
        {
            Name = "Cedar Crest Brewery"
        }, BreweryId);

        // Act
        var commandHandler = new UpdateBreweryCommandHandler(unitOfWork);
        BreweryResource? result = await commandHandler.Handle(updateCommand, CancellationTestToken).ConfigureAwait(false);

        // Assert
        _ = result
            .ShouldNotBeNull()
            .ApiVersion
            .ShouldNotBeNull();
        BreweryDto? breweryDto = result.Self.ShouldNotBeNull();
        breweryDto.Name.ShouldBe(updateCommand.Dto.Name);
        breweryDto.Id.ShouldBe(BreweryId);

        AddressDto? addressDto = breweryDto.Address.ShouldNotBeNull();
        addressDto.StreetAddress.ShouldBe("1030 E Cypress Ave Ste D");
        addressDto.City.ShouldBe("Redding");
        addressDto.State.ShouldBe("CA");
        addressDto.ZipCode.ShouldBe("96002");
    }
}
