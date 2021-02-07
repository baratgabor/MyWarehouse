using MyWarehouse.Domain.Common;
using MyWarehouse.Domain.Common.ValueObjects.Mass;
using MyWarehouse.Domain.Common.ValueObjects.Money;
using MyWarehouse.Domain.Exceptions;
using MyWarehouse.Domain.Transactions;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MyWarehouse.Domain.Products
{
    public class Product : MyEntity
    {
        [Required]
        [StringLength(ProductInvariants.NameMaxLength)]
        public string Name { get; private set; }

        [Required]
        [StringLength(ProductInvariants.DescriptionMaxLength)]
        public string Description { get; private set; }

        [Required]
        public Money Price { get; private set; }

        [Required]
        public Mass Mass { get; private set; }

        public int NumberInStock { get; private set; }

        private Product() // EF
        {}

        public Product(string name, string description, Money price, Mass mass)
        {
            UpdateName(name);
            UpdateDescription(description);

            CheckMass(mass?.Value ?? throw new ArgumentNullException(nameof(mass)));
            CheckPrice(price?.Amount ?? throw new ArgumentNullException(nameof(price)));

            Mass = mass;
            Price = price;

            NumberInStock = 0;
        }

        public void UpdateMass(float value)
        {
            CheckMass(value);
            Mass = new Mass(value, Mass?.Unit ?? ProductInvariants.DefaultMassUnit);
        }

        public void UpdatePrice(decimal amount)
        {
            CheckPrice(amount);
            Price = new Money(amount, Price?.Currency ?? ProductInvariants.DefaultPriceCurrency);
        }

        public void UpdateName(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Name cannot be empty.");

            if (value.Length > ProductInvariants.NameMaxLength)
                throw new ArgumentException($"Length of value ({value.Length}) exceeds maximum name length ({ProductInvariants.NameMaxLength}).");

            Name = value;
        }

        public void UpdateDescription(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Description cannot be empty.");

            if (value.Length > ProductInvariants.DescriptionMaxLength)
                throw new ArgumentException($"Length of value ({value.Length}) exceeds maximum description length ({ProductInvariants.NameMaxLength}).");

            Description = value;
        }

        /// <summary>
        /// Adjust product stock based on a transaction occurred.
        /// </summary>
        internal void RecordTransaction(TransactionLine transactionLine)
        {
            if (transactionLine.Quantity < 1)
                throw new ArgumentException("Product quantity in transaction must be 1 or greater.");

            switch (transactionLine.Transaction.TransactionType)
            {
                case TransactionType.Sales:
                    if (transactionLine.Quantity > NumberInStock)
                        throw new InsufficientStockException(this, transactionLine.Quantity, NumberInStock);
                    NumberInStock -= transactionLine.Quantity;
                    break;
                case TransactionType.Procurement:
                    NumberInStock += transactionLine.Quantity;
                    break;
                default:
                    throw new InvalidEnumArgumentException($"Unexpected {nameof(TransactionType)}: '{transactionLine.Transaction.TransactionType}'.");
            }
        }

        private static void CheckMass(float value)
        {
            if (value < ProductInvariants.MassMinimum)
                throw new ArgumentException($"Value '{value}' is smaller than the minimum required mass of {ProductInvariants.MassMinimum}.");
        }

        private static void CheckPrice(decimal amount)
        {
            if (amount < ProductInvariants.PriceMinimum)
                throw new ArgumentException($"Amount '{amount}' is smaller than the minimum required price of {ProductInvariants.MassMinimum}.");
        }
    }
}