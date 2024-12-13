using System.Diagnostics.CodeAnalysis;

using FluentValidation;

namespace Dappery.Core.Breweries.Commands.UpdateBrewery;

[ExcludeFromCodeCoverage]
public class UpdateBreweryCommandValidator : AbstractValidator<UpdateBreweryCommand>
{
    public UpdateBreweryCommandValidator()
    {
        _ = RuleFor(request => request.Dto)
            .NotNull()
            .WithMessage("Must supply a request body");

        _ = RuleFor(request => request.BreweryId)
            .NotEmpty()
            .WithMessage("Must supply a valid brewery ID");
    }
}
