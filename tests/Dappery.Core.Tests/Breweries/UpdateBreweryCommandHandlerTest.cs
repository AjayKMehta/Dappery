using System.Net;
using System.Threading.Tasks;
using Dappery.Core.Breweries.Commands.UpdateBrewery;
using Dappery.Core.Exceptions;
using Dappery.Domain.Dtos;
using Dappery.Domain.Dtos.Brewery;
using Shouldly;
using Xunit;

namespace Dappery.Core.Tests.Breweries
{
    public class UpdateBreweryCommandHandlerTest : TestFixture
    {
        [Fact]
        public async Task GivenValidUpdateRequestWhenBreweryExistsReturnsUpdatedMappedBrewery()
        {
            // Arrange
            using var unitOfWork = this.UnitOfWork;
            const int breweryId = 1;
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
            }, breweryId);

            // Act
            var commandHandler = new UpdateBreweryCommandHandler(unitOfWork);
            var result = await commandHandler.Handle(updateCommand, CancellationTestToken).ConfigureAwait(false);

            // Assert
            result.ShouldNotBeNull();
            result.Self.ShouldNotBeNull();
            result.ApiVersion.ShouldNotBeNull();
            result.Self.Id.ShouldBe(breweryId);
            result.Self.Address?.ShouldNotBeNull();
            result.Self.Address?.City.ShouldBe(updateCommand.Dto.Address?.City);
            result.Self.Address?.State.ShouldBe(updateCommand.Dto.Address?.State);
            result.Self.Address?.StreetAddress.ShouldBe(updateCommand.Dto.Address?.StreetAddress);
            result.Self.Address?.ZipCode.ShouldBe(updateCommand.Dto.Address?.ZipCode);
            result.Self.Name.ShouldBe(updateCommand.Dto.Name);
        }

        [Fact]
        public async Task GivenValidUpdateRequestWhenBreweryDoesNotExistThrowsNotFoundException()
        {
            // Arrange
            using var unitOfWork = this.UnitOfWork;
            const int breweryId = 11;
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
            }, breweryId);

            // Act
            var commandHandler = new UpdateBreweryCommandHandler(unitOfWork);
            var result = await Should.ThrowAsync<DapperyApiException>(async () => await commandHandler.Handle(updateCommand, CancellationTestToken).ConfigureAwait(false)).ConfigureAwait(false);

            // Assert
            result.ShouldNotBeNull().StatusCode.ShouldBe(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task GivenValidUpdateRequestWhenBreweryDoesExistAndAddressIsNotUpdatedReturnsMappedBreweryWithNoUpdatedAddress()
        {
            // Arrange
            using var unitOfWork = this.UnitOfWork;
            const int breweryId = 1;
            var updateCommand = new UpdateBreweryCommand(new UpdateBreweryDto
            {
                Name = "Cedar Crest Brewery"
            }, breweryId);

            // Act
            var commandHandler = new UpdateBreweryCommandHandler(unitOfWork);
            var result = await commandHandler.Handle(updateCommand, CancellationTestToken).ConfigureAwait(false);

            // Assert
            _ = result
                .ShouldNotBeNull()
                .ApiVersion
                .ShouldNotBeNull();
            var breweryDto = result.Self.ShouldNotBeNull();
            breweryDto.Name.ShouldBe(updateCommand.Dto.Name);
            breweryDto.Id.ShouldBe(breweryId);

            var addressDto = breweryDto.Address.ShouldNotBeNull();
            addressDto.StreetAddress.ShouldBe("1030 E Cypress Ave Ste D");
            addressDto.City.ShouldBe("Redding");
            addressDto.State.ShouldBe("CA");
            addressDto.ZipCode.ShouldBe("96002");
        }
    }
}
