using FluentValidation;

namespace Dappery.Core.Beers.Commands.DeleteBeer
{
    public class DeleteBeerCommandValidator : AbstractValidator<DeleteBeerCommand>
    {
        public DeleteBeerCommandValidator() => this.RuleFor(b => b.BeerId)
                .NotNull()
                .WithMessage("Must supply the beer ID");
    }
}
