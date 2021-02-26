using MediatR;
using MyWarehouse.Application.Common.Dependencies.DataAccess;
using MyWarehouse.Application.Common.Exceptions;
using MyWarehouse.Domain.Products;
using System.Threading;
using System.Threading.Tasks;

namespace MyWarehouse.Application.Products.DeleteProduct
{
    public record DeleteProductCommand : IRequest
    {
        public int Id { get; init; }
    }

    public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeleteProductCommandHandler(IUnitOfWork unitOfWork)
            => _unitOfWork = unitOfWork;

        public async Task<Unit> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(request.Id)
                ?? throw new EntityNotFoundException(nameof(Product), request.Id);

            _unitOfWork.Products.Remove(product);
            await _unitOfWork.SaveChanges();

            return Unit.Value;
        }
    }
}
