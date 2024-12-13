using System.Diagnostics.CodeAnalysis;

using FluentValidation;

namespace Dappery.Core.Beers.Commands.DeleteBeer;

[ExcludeFromCodeCoverage]
public class DeleteBeerCommandValidator : AbstractValidator<DeleteBeerCommand>
{
    public DeleteBeerCommandValidator() => RuleFor(b => b.BeerId)
        .NotEmpty()
        .WithMessage("Must supply the beer ID");
}
