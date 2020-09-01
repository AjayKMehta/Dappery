using FluentValidation;

namespace Dappery.Core.Breweries.Queries.RetrieveBrewery
{
    public class RetrieveBreweryQueryValidator : AbstractValidator<RetrieveBreweryQuery>
    {
        public RetrieveBreweryQueryValidator() => this.RuleFor(b => b.Id)
                .NotNull()
                .NotEmpty()
                .WithMessage("Must supply an ID to retrieve a brewery")
                .GreaterThanOrEqualTo(1)
                .WithMessage("Must be a valid brewery ID");
    }
}
