using MyWarehouse.Application.Common.Dependencies.DataAccess;
using MyWarehouse.Application.Common.Dependencies.DataAccess.Repositories.Common;

namespace MyWarehouse.Application.Partners.GetPartners;

public class GetPartnersListQueryHandler : IRequestHandler<ListQueryModel<PartnerDto>, IListResponseModel<PartnerDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetPartnersListQueryHandler(IUnitOfWork unitOfWork)
        => _unitOfWork = unitOfWork;

    public Task<IListResponseModel<PartnerDto>> Handle(ListQueryModel<PartnerDto> request, CancellationToken cancellationToken)
        => _unitOfWork.Partners.GetProjectedListAsync(request, readOnly: true);
}
