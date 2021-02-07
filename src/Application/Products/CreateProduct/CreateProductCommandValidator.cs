using FluentValidation;
using MyWarehouse.Domain.Products;

namespace MyWarehouse.Application.Products.CreateProduct
{
    public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
    {
        public CreateProductCommandValidator()
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

            RuleFor(x => x.MassUnitSymbol)
                .NotEmpty()
                .Equal(ProductInvariants.DefaultMassUnit.Symbol)
                .WithMessage(x => $"Mass unit '{x.MassUnitSymbol}' cannot be accepted. Currently the only value supported is '{ProductInvariants.DefaultMassUnit.Symbol}'.");

            RuleFor(x => x.PriceCurrencyCode)
                .NotEmpty()
                .Equal(ProductInvariants.DefaultPriceCurrency.Code)
                .WithMessage(x => $"Currency code '{x.PriceCurrencyCode}' cannot be accepted. Currently the only acceptable value is '{ProductInvariants.DefaultPriceCurrency.Code}'.");
        }
    }
}
