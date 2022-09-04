using FluentValidation;

namespace Dappery.Core.Breweries.Commands.DeleteBrewery;

public class DeleteBreweryCommandValidator : AbstractValidator<DeleteBreweryCommand>
{
    public DeleteBreweryCommandValidator() => RuleFor(b => b.BreweryId)
        .NotEmpty()
        .WithMessage("Must supply the brewery ID");
}
