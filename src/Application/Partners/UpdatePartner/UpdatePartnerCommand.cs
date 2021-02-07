using MediatR;
using MyWarehouse.Application.Common.Dependencies.DataAccess;
using MyWarehouse.Application.Common.Exceptions;
using MyWarehouse.Domain.Partners;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;

namespace MyWarehouse.Application.Partners.UpdatePartner
{
    public record UpdatePartnerCommand : IRequest
    {
        public int Id { get; init; }
        public string Name { get; init; }
        public AddressDto Address { get; init; }

        public record AddressDto
        {
            [Required, MinLength(1), MaxLength(100)]
            public string Country { get; init; }
            [Required, MinLength(1), MaxLength(100)]
            public string ZipCode { get; init; }
            [Required, MinLength(1), MaxLength(100)]
            public string Street { get; init; }
            [Required, MinLength(1), MaxLength(100)]
            public string City { get; init; }
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
}
