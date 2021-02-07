using FluentValidation;
using FluentValidation.Validators;
using MyWarehouse.Application.Common.Dependencies.DataAccess.Repositories;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MyWarehouse.Application.Transactions.CreateTransaction
{
    public class CreateTransactionCommandValidator : AbstractValidator<CreateTransactionCommand>
    {
        private readonly IProductRepository _productRepository;

        public CreateTransactionCommandValidator(IProductRepository productRepository)
        {
            _productRepository = productRepository;

            RuleFor(x => x.TransactionLines)
                .NotEmpty()
                .DependentRules(() => {

                    RuleForEach(x => x.TransactionLines)
                        .Must(l => l.ProductQuantity > 0)
                        .WithMessage("Product quantity at line {CollectionIndex} must be larger than 0");

                    RuleFor(x => x.TransactionLines)
                        .Must(x => x.GroupBy(x => x.ProductId).Any(g => g.Count() == 1))
                        .WithMessage("Can't have more than one transaction lines for the same product.");

                    RuleFor(x => x.TransactionLines)
                        .MustAsync(HaveSufficientStock)
                        .WithMessage("Cannot record a sales transaction of quantity {RequestedQty} for product '{ProductName}', because current stock is {ProductStock}.");
                });
        }

        private async Task<bool> HaveSufficientStock(CreateTransactionCommand c, CreateTransactionCommand.TransactionLine[] lines, PropertyValidatorContext ctx, CancellationToken _)
        {
            if (c.TransactionType == Domain.TransactionType.Procurement)
            {
                return true;
            }

            var requestedProducts = await _productRepository.GetFiltered(
                x => lines.Select(l => l.ProductId).Contains(x.Id));

            foreach (var line in lines)
            {
                var product = requestedProducts.Where(p => p.Id == line.ProductId).Single();

                if (product.NumberInStock < line.ProductQuantity)
                {
                    ctx.MessageFormatter.AppendArgument("ProductName", product.Name);
                    ctx.MessageFormatter.AppendArgument("ProductStock", product.NumberInStock);
                    ctx.MessageFormatter.AppendArgument("RequestedQty", line.ProductQuantity);
                    return false;
                }
            }

            return true;
        }
    }
}
