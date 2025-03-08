using Dappery.Core.Breweries.Commands.CreateBrewery;
using Dappery.Domain.Dtos;
using Dappery.Domain.Dtos.Brewery;

using FluentValidation.TestHelper;

namespace Dappery.Core.Tests.Breweries;

internal sealed class CreateBreweryCommandValidatorExampleTest
{
    [Test]
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
        TestValidationResult<CreateBreweryCommand> validationResult = new CreateBreweryCommandValidator().TestValidate(command);

        // Assert
        validationResult.ShouldNotHaveValidationErrorFor(b => b.Dto);
        validationResult.ShouldNotHaveValidationErrorFor(b => b.Dto.Address);
    }
}
