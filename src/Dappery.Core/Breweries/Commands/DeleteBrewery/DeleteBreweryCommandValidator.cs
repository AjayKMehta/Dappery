using System.Diagnostics.CodeAnalysis;

using FluentValidation;

namespace Dappery.Core.Breweries.Commands.DeleteBrewery;

[ExcludeFromCodeCoverage]
public class DeleteBreweryCommandValidator : AbstractValidator<DeleteBreweryCommand>
{
    public DeleteBreweryCommandValidator() => RuleFor(b => b.BreweryId)
        .NotEmpty()
        .WithMessage("Must supply the brewery ID");
}
