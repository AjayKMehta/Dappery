using System.Diagnostics.CodeAnalysis;

using FluentValidation;

namespace Dappery.Core.Beers.Queries.RetrieveBeer;

[ExcludeFromCodeCoverage]
public class RetrieveBeerQueryValidator : AbstractValidator<RetrieveBeerQuery>
{
    public RetrieveBeerQueryValidator() => RuleFor(b => b.Id)
        .NotEmpty()
        .WithMessage("Must supply an ID to retrieve a beer")
        .GreaterThanOrEqualTo(1)
        .WithMessage("Must be a valid beer ID");
}
