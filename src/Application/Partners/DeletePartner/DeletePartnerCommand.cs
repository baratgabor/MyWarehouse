using MediatR;
using MyWarehouse.Application.Common.Dependencies.DataAccess;
using MyWarehouse.Application.Common.Exceptions;
using MyWarehouse.Domain.Partners;
using System.Threading;
using System.Threading.Tasks;

namespace MyWarehouse.Application.Partners.DeletePartner
{
    public record DeletePartnerCommand : IRequest
    {
        public int Id { get; init; }
    }

    public class DeletePartnerCommandHandler : IRequestHandler<DeletePartnerCommand>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeletePartnerCommandHandler(IUnitOfWork unitOfWork)
            => _unitOfWork = unitOfWork;

        public async Task<Unit> Handle(DeletePartnerCommand request, CancellationToken cancellationToken)
        {
            var partner = await _unitOfWork.Partners.GetByIdAsync(request.Id)
                ?? throw new EntityNotFoundException(nameof(Partner), request.Id);

            _unitOfWork.Partners.Remove(partner);
            await _unitOfWork.SaveChanges();

            return Unit.Value;
        }
    }
}
