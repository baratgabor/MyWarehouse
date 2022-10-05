using MyWarehouse.Application.Common.Dependencies.DataAccess;
using MyWarehouse.Application.Common.Exceptions;
using MyWarehouse.Domain.Partners;

namespace MyWarehouse.Application.Partners.UpdatePartner;

public record UpdatePartnerCommand : IRequest
{
    public int Id { get; init; }
    public string Name { get; init; } = null!;
    public AddressDto Address { get; init; } = null!;

    public record AddressDto
    {
        public string Country { get; init; } = null!;
        public string ZipCode { get; init; } = null!;
        public string Street { get; init; } = null!;
        public string City { get; init; } = null!;
    }
}

public class UpdatePartnerCommandHandler : IRequestHandler<UpdatePartnerCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdatePartnerCommandHandler(IUnitOfWork unitOfWork)
        => _unitOfWork = unitOfWork;

    public async Task<Unit> Handle(UpdatePartnerCommand request, CancellationToken cancellationToken)
    {
        var partner = await _unitOfWork.Partners.GetByIdAsync(request.Id)
            ?? throw new EntityNotFoundException(nameof(Partner), request.Id);

        partner.UpdateName(request.Name.Trim());
        partner.UpdateAddress(new Address(
            country: request.Address.Country.Trim(),
            zipcode: request.Address.ZipCode.Trim(),
            street: request.Address.Street.Trim(),
            city: request.Address.City.Trim()
        ));

        await _unitOfWork.SaveChanges();

        return Unit.Value;
    }
}
