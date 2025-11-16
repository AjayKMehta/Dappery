using System.Diagnostics.CodeAnalysis;

using Dappery.Core.Extensions;

using FluentValidation;

namespace Dappery.Core.Beers.Commands.UpdateBeer;

[ExcludeFromCodeCoverage]
public class UpdateBeerCommandValidator : AbstractValidator<UpdateBeerCommand>
{
    public UpdateBeerCommandValidator()
    {
        _ = RuleFor(b => b.Dto)
            .NotNull()
            .WithMessage("Must supply a beer to update");

        RuleFor(static b => b.Dto.Name)
            .NotNullOrEmpty();

        RuleFor(static b => b.Dto.Style)
            .NotNullOrEmpty();
    }
}
