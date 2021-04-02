using MediatR;
using MyWarehouse.Application.Common.Dependencies.DataAccess;
using MyWarehouse.Application.Common.Exceptions;
using MyWarehouse.Domain;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MyWarehouse.Application.Transactions.CreateTransaction
{
    public record CreateTransactionCommand : IRequest<int>
    {
        public int PartnerId { get; init; }
        public TransactionType TransactionType { get; init; }
        public TransactionLine[] TransactionLines { get; init; }

        public struct TransactionLine
        {
            public int ProductId { get; init; }
            public int ProductQuantity { get; init; }
        }
    }

    public class CreateTransactionCommandHandler : IRequestHandler<CreateTransactionCommand, int>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateTransactionCommandHandler(IUnitOfWork unitOfWork)
            => _unitOfWork = unitOfWork;

        public async Task<int> Handle(CreateTransactionCommand request, CancellationToken cancellationToken)
        {
            var partner = await _unitOfWork.Partners.GetByIdAsync(request.PartnerId)
                ?? throw new InputValidationException((nameof(request.PartnerId), $"Partner (id: {request.PartnerId}) was not found."));

            // Try not to confuse DB transaction with the "Transaction" domain entity. :)
            await _unitOfWork.BeginTransactionAsync();
            int createdTransactionId = 0;
            try
            {
                var orderedProductIds = request.TransactionLines.Select(x => x.ProductId).Distinct();
                var orderedProducts = await _unitOfWork.Products.GetFiltered(x => orderedProductIds.Contains(x.Id));

                var validLines = request.TransactionLines.Select(line =>
                    (
                        product: orderedProducts.FirstOrDefault(p => p.Id == line.ProductId)
                            ?? throw new InputValidationException((nameof(line.ProductId), $"Product (id: {line.ProductId}) was not found.")),
                        qty: line.ProductQuantity
                    )
                );

                var transaction = request.TransactionType switch
                {
                    TransactionType.Sales => partner.SellTo(validLines),
                    TransactionType.Procurement => partner.ProcureFrom(validLines),
                    _ => throw new InvalidEnumArgumentException($"No operation is defined for {nameof(TransactionType)} of '{request.TransactionType}'.")
                };

                await _unitOfWork.SaveChanges();
                createdTransactionId = transaction.Id;
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }

            await _unitOfWork.CommitTransactionAsync();

            return createdTransactionId;
        }
    }
}
