using MyWarehouse.Application.Common.Mapping;
using MyWarehouse.Domain.Transactions;
using System;

namespace MyWarehouse.Application.Transactions.GetTransactionsList
{
    public record TransactionDto : IMapFrom<Transaction>
    {
        public int Id { get; init; }
        public DateTime CreatedAt { get; init; }
        public int TransactionType { get; init; }
        public string PartnerName { get; init; }

        public decimal TotalAmount { get; init; }
        public string TotalCurrencyCode { get; init; }
    }
}