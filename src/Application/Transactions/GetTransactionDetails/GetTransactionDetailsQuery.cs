using AutoMapper;
using MediatR;
using MyWarehouse.Application.Common.Dependencies.DataAccess;
using MyWarehouse.Application.Common.Exceptions;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;

namespace MyWarehouse.Application.Transactions.GetTransactionDetails
{
    public record GetTransactionDetailsQuery : IRequest<TransactionDetailsDto>
    {
        public int Id { get; set; }
    }

    public class GetTransactionDetailsQueryHandler : IRequestHandler<GetTransactionDetailsQuery, TransactionDetailsDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetTransactionDetailsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<TransactionDetailsDto> Handle(GetTransactionDetailsQuery request, CancellationToken cancellationToken)
            => await _unitOfWork.Transactions.GetProjectedAsync<TransactionDetailsDto>(request.Id, readOnly: true)
                ?? throw new EntityNotFoundException(nameof(Transaction), request.Id);
    }
}
