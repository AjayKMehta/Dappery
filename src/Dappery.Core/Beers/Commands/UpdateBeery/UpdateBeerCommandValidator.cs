namespace Dappery.Core.Beers.Commands.UpdateBeery
{
    using Extensions;
    using FluentValidation;

    public class UpdateBeerCommandValidator : AbstractValidator<UpdateBeerCommand>
    {
        public UpdateBeerCommandValidator()
        {
            RuleFor(b => b.Dto)
                .NotNull()
                .WithMessage("Must supply a beer to update");

            RuleFor(b => b.Dto.Name)
                .NotNullOrEmpty();

            RuleFor(b => b.Dto.Style)
                .NotNullOrEmpty();
        }
    }
}