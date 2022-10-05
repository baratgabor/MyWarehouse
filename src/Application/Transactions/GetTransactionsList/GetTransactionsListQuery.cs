using MyWarehouse.Application.Common.Dependencies.DataAccess;
using MyWarehouse.Application.Common.Dependencies.DataAccess.Repositories.Common;
using MyWarehouse.Domain;

namespace MyWarehouse.Application.Transactions.GetTransactionsList;

public class GetTransactionListQuery : ListQueryModel<TransactionDto>
{
    public TransactionType? Type { get; init; }
}

public class GetTransactionsListQueryHandler : IRequestHandler<GetTransactionListQuery, IListResponseModel<TransactionDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetTransactionsListQueryHandler(IUnitOfWork unitOfWork)
        => _unitOfWork = unitOfWork;

    public async Task<IListResponseModel<TransactionDto>> Handle(GetTransactionListQuery request, CancellationToken cancellationToken)
        => await _unitOfWork.Transactions.GetProjectedListAsync(request,
            additionalFilter: request.Type.HasValue ? x => x.TransactionType == request.Type : null,
            readOnly: true);
}
