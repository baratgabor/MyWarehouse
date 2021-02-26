using MediatR;
using MyWarehouse.Application.Common.Dependencies.DataAccess;
using MyWarehouse.Application.Common.Exceptions;
using MyWarehouse.Domain.Products;
using System.Threading;
using System.Threading.Tasks;

namespace MyWarehouse.Application.Products.UpdateProduct
{
    public record UpdateProductCommand : IRequest
    {
        public int Id { get; init; }

        public string Name { get; init; }
        public string Description { get; init; }

        public float MassValue { get; init; }
        public decimal PriceAmount { get; init; }
    }

    public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateProductCommandHandler(IUnitOfWork unitOfWork)
            => _unitOfWork = unitOfWork;

        public async Task<Unit> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(request.Id)
                ?? throw new EntityNotFoundException(nameof(Product), request.Id);

            product.UpdateName(request.Name.Trim());
            product.UpdateDescription(request.Description.Trim());
            product.UpdateMass(request.MassValue);
            product.UpdatePrice(request.PriceAmount);

            await _unitOfWork.SaveChanges();

            return Unit.Value;
        }
    }
}
