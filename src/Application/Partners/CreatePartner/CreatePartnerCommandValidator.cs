using FluentValidation;
using MyWarehouse.Domain.Partners;

namespace MyWarehouse.Application.Partners.CreatePartner
{
    public class CreatePartnerCommandValidator : AbstractValidator<CreatePartnerCommand>
    {
        public CreatePartnerCommandValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .MaximumLength(PartnerInvariants.NameMaxLength);

            RuleFor(x => x.Address).NotNull().DependentRules(() =>
            {
                RuleFor(x => x.Address.Country).NotNull().MaximumLength(100);
                RuleFor(x => x.Address.ZipCode).NotNull().MaximumLength(100);
                RuleFor(x => x.Address.Street).NotNull().MaximumLength(100);
                RuleFor(x => x.Address.City).NotNull().MaximumLength(100);
            });
        }
    }
}
