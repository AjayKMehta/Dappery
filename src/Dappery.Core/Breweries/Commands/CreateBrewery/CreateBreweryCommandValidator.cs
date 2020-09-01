using Dappery.Core.Extensions;
using FluentValidation;

namespace Dappery.Core.Breweries.Commands.CreateBrewery
{
    public class CreateBreweryCommandValidator : AbstractValidator<CreateBreweryCommand>
    {
        public CreateBreweryCommandValidator()
        {
            this.RuleFor(b => b.Dto)
                .NotNull()
                .WithMessage("A request must contain valid creation data");

            this.RuleFor(b => b.Dto.Name)
                .NotNullOrEmpty();

            this.RuleFor(b => b.Dto.Address)
                .NotNull()
                .WithMessage("Must supply the address of the brewery when creating");

            this.RuleFor(b => b.Dto.Address!.City)
                .NotNullOrEmpty();

            this.RuleFor(b => b.Dto.Address!.State)
                .HasValidStateAbbreviation();

            this.RuleFor(b => b.Dto.Address!.StreetAddress)
                .HasValidStreetAddress();

            this.RuleFor(b => b.Dto.Address!.ZipCode)
                .HasValidZipCode();
        }
    }
}
