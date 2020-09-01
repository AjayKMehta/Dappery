using Dappery.Core.Breweries.Commands.CreateBrewery;
using Dappery.Domain.Dtos;
using Dappery.Domain.Dtos.Brewery;
using Shouldly;
using Xunit;

namespace Dappery.Core.Tests.Breweries
{
    public class CreateBreweryCommandValidatorExampleTest
    {
        [Fact]
        public void Test()
        {
            // Arrange
            var command = new CreateBreweryCommand(
                new CreateBreweryDto
                {
                    Name = "Wild Card Brewing Company",
                    Address = new AddressDto
                    {
                        City = "Redding",
                        State = "CA",
                        StreetAddress = "123 Pine St.",
                        ZipCode = "96002"
                    }
                });

            // Act
            var validationResult = new CreateBreweryCommandValidator().Validate(command);

            // Assert
            validationResult.IsValid.ShouldBeTrue();
        }
    }
}
