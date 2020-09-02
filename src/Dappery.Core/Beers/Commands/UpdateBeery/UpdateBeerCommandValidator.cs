using Dappery.Core.Extensions;
using FluentValidation;

namespace Dappery.Core.Beers.Commands.UpdateBeery
{
    public class UpdateBeerCommandValidator : AbstractValidator<UpdateBeerCommand>
    {
        public UpdateBeerCommandValidator()
        {
            this.RuleFor(b => b.Dto)
                .NotNull()
                .WithMessage("Must supply a beer to update");

            this.RuleFor(b => b.Dto!.Name)
                .NotNullOrEmpty();

            this.RuleFor(b => b.Dto!.Style)
                .NotNullOrEmpty();
        }
    }
}
