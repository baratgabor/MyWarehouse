using MediatR;
using MyWarehouse.Application.Common.Dependencies.DataAccess;
using MyWarehouse.Domain.Partners;
using System.Threading;
using System.Threading.Tasks;

namespace MyWarehouse.Application.Partners.CreatePartner
{
    public record CreatePartnerCommand : IRequest<int>
    {
        public string Name { get; init; }
        public AddressDto Address { get; init; }

        public record AddressDto
        {
            public string Country { get; init; }
            public string ZipCode { get; init; }
            public string Street { get; init; }
            public string City { get; init; }
        }
    }

    public class CreatePartnerCommandHandler : IRequestHandler<CreatePartnerCommand, int>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreatePartnerCommandHandler(IUnitOfWork unitOfWork)
            => _unitOfWork = unitOfWork;

        public async Task<int> Handle(CreatePartnerCommand request, CancellationToken cancellationToken)
        {
            var partner = new Partner(
                name: request.Name.Trim(),
                address: new Address(
                    country: request.Address.Country.Trim(),
                    zipcode: request.Address.ZipCode.Trim(),
                    street: request.Address.Street.Trim(),
                    city: request.Address.City.Trim()
                ));

            _unitOfWork.Partners.Add(partner);
            await _unitOfWork.SaveChanges();

            return partner.Id;
        }
    }
}
