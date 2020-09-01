namespace Dappery.Core.Breweries.Commands.UpdateBrewery
{
    using FluentValidation;

    public class UpdateBreweryCommandValidator : AbstractValidator<UpdateBreweryCommand>
    {
        public UpdateBreweryCommandValidator()
        {
            this.RuleFor(request => request.Dto)
                .NotNull()
                .WithMessage("Must supply a request body");

            this.RuleFor(request => request.BreweryId)
                .NotNull()
                .WithMessage("Must supply a valid brewery ID");
        }
    }
}
