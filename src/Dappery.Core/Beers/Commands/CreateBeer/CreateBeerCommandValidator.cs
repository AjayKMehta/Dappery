using Dappery.Core.Extensions;

using FluentValidation;

namespace Dappery.Core.Beers.Commands.CreateBeer
{
    public class CreateBeerCommandValidator : AbstractValidator<CreateBeerCommand>
    {
        public CreateBeerCommandValidator()
        {
            _ = this.RuleFor(b => b.Dto)
                .NotNull()
                .WithMessage("Must supply a request object to create a beer");

            this.RuleFor(b => b.Dto!.Name)
                .NotNullOrEmpty();

            this.RuleFor(b => b.Dto!.Style)
                .NotNullOrEmpty();

            _ = this.RuleFor(b => b.Dto!.BreweryId)
                .NotEmpty()
                .WithMessage("Must supply the brewery ID");
        }
    }
}
