using MediatR;
using MyWarehouse.Application.Common.Dependencies.DataAccess;
using MyWarehouse.Application.Common.Exceptions;
using MyWarehouse.Domain;
using MyWarehouse.Domain.Products;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;

namespace MyWarehouse.Application.Transactions.RecordSales
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

            var transactionLines = new List<(Product product, int quantity)>();
            foreach (var line in request.TransactionLines)
            {
                var product = await _unitOfWork.Products.GetByIdAsync(line.ProductId)
                    ?? throw new InputValidationException((nameof(line.ProductId), $"Product (id: {line.ProductId}) was not found."));

                transactionLines.Add((product, line.ProductQuantity));
            }

            var transaction = request.TransactionType switch
            {
                TransactionType.Sales => partner.SellTo(transactionLines),
                TransactionType.Procurement => partner.ProcureFrom(transactionLines),
                _ => throw new InvalidEnumArgumentException($"No operation is defined for {nameof(TransactionType)} of '{request.TransactionType}'.")
            };

            await _unitOfWork.SaveChanges();

            return transaction.Id;
        }
    }
}
