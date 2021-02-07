using FluentValidation;
using MyWarehouse.Domain.Products;

namespace MyWarehouse.Application.Partners.UpdatePartner
{
    public class UpdateProductCommandValidator : AbstractValidator<UpdateProductCommand>
    {
        public UpdateProductCommandValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .MaximumLength(ProductInvariants.NameMaxLength);

            RuleFor(x => x.Description)
                .NotEmpty()
                .MaximumLength(ProductInvariants.DescriptionMaxLength);

            RuleFor(x => x.MassValue)
                .GreaterThanOrEqualTo(ProductInvariants.MassMinimum);

            RuleFor(x => x.PriceAmount)
                .GreaterThanOrEqualTo(ProductInvariants.PriceMinimum);
        }
    }
}
