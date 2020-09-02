using FluentValidation;

namespace Dappery.Core.Breweries.Queries.RetrieveBrewery
{
    public class RetrieveBreweryQueryValidator : AbstractValidator<RetrieveBreweryQuery>
    {
        public RetrieveBreweryQueryValidator() => this.RuleFor(b => b.Id)
                .GreaterThanOrEqualTo(1)
                .WithMessage("Must be a valid brewery ID");
    }
}
