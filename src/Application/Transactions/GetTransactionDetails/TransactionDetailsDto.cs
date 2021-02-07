using AutoMapper;
using MyWarehouse.Application.Common.Mapping;
using MyWarehouse.Domain.Transactions;
using System;
using System.Collections.Generic;

namespace MyWarehouse.Application.Transactions.GetTransactionDetails
{
    public record TransactionDetailsDto : IMapFrom<Transaction>
    {
        public int Id { get; init; }
        public int TransactionType { get; init; }

        public DateTime CreatedAt { get; init; }
        public string CreatedBy { get; init; }

        public DateTime? ModifiedAt { get; init; }
        public string ModifiedBy { get; init; }

        public int PartnerId { get; init; }
        public string PartnerName { get; init; }
        public string PartnerAddress { get; init; }
        public bool PartnerIsDeleted { get; init; }

        public decimal TotalAmount { get; init; }
        public string TotalCurrencyCode { get; init; }

        public List<TransactionLineDto> TransactionLines { get; init; }

        public struct TransactionLineDto
        {
            public int ProductId { get; init; }
            public string ProductName { get; init; }
            public int Quantity { get; init; }
            public bool ProductIsDeleted { get; init; }

            public string UnitPrice { get; init; }
            public decimal UnitPriceAmount { get; init; }
            public string UnitPriceCurrencyCode { get; init; }
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<Transaction, TransactionDetailsDto>()
                .ForMember(dest => dest.PartnerIsDeleted, cfg => cfg
                    .MapFrom(src => src.Partner.DeletedAt != null));

            profile.CreateMap<TransactionLine, TransactionLineDto>()
                .ForMember(dest => dest.ProductIsDeleted, cfg => cfg
                    .MapFrom(src => src.Product.DeletedAt != null));
        }
    }
}