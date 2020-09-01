using FluentValidation;

namespace Dappery.Core.Breweries.Commands.DeleteBrewery
{
    public class DeleteBreweryCommandValidator : AbstractValidator<DeleteBreweryCommand>
    {
        public DeleteBreweryCommandValidator() => this.RuleFor(b => b.BreweryId)
                .NotNull()
                .WithMessage("Must supply the brewery ID");
    }
}
