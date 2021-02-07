using NUnit.Framework;
using Moq;
using MyWarehouse.Application.Dependencies.Services;
using MyWarehouse.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using System.Data.Common;
using Microsoft.Data.Sqlite;
using MyWarehouse.Domain.Partners;
using System.Threading.Tasks;
using System;

namespace MyWarehouse.Infrastructure.UnitTests.Persistence.Context
{
    public class ApplicationDbContextTests
    {
        const string TestUserId = "TestUserId";
        readonly DateTime TestDateTime = DateTime.Now;
        ApplicationDbContext sut;

        [SetUp]
        public void SetUp()
        {
            var mockCurrentUser = new Mock<ICurrentUserService>();
            mockCurrentUser.Setup(x => x.UserId).Returns(TestUserId);

            var mockDateTime = new Mock<IDateTime>();
            mockDateTime.Setup(x => x.Now).Returns(TestDateTime);

            var dbContextOptionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            dbContextOptionsBuilder.UseSqlite(CreateInMemoryDatabase());

            sut = new ApplicationDbContext(dbContextOptionsBuilder.Options, mockCurrentUser.Object, mockDateTime.Object);
            sut.Database.EnsureCreated();

            static DbConnection CreateInMemoryDatabase()
            {
                var connection = new SqliteConnection("Filename=:memory:");
                connection.Open();
                return connection;
            }
        }

        [TearDown]
        public void TearDown()
        {
            sut.Dispose();
        }

        [Test]
        public async Task Add_NewEntity_PopulatesCreatedFields()
        {
            // Act
            sut.Partners.Add(new Partner(name: "John Doe", address: new Address("street", "city", "country", "zipcode")));
            sut.SaveChanges();

            Assert.AreEqual(1, await sut.Partners.CountAsync(), "Expected 1 entity in the collection.");
            
            var res = await sut.Partners.FirstAsync();
            Assert.AreEqual("John Doe", res.Name, $"Expected {nameof(Partner.Name)} to be set to the name provided during addition.");
            Assert.AreEqual(TestDateTime, res.CreatedAt, $"Expected {nameof(Partner.CreatedAt)} date to be set to the current datetime.");
            Assert.AreEqual(TestUserId, res.CreatedBy, $"Expected {nameof(Partner.CreatedBy)} to be set to the current user ID.");

            Assert.Multiple(() => {
                var message = "Expected {0} to be left empty since no relevant operation occurred.";
                Assert.Null(res.LastModifiedAt, string.Format(message, nameof(Partner.LastModifiedAt)));
                Assert.Null(res.LastModifiedBy, string.Format(message, nameof(Partner.LastModifiedBy)));
                Assert.Null(res.DeletedAt, string.Format(message, nameof(Partner.DeletedAt)));
                Assert.Null(res.DeletedBy, string.Format(message, nameof(Partner.DeletedBy)));
            });
        }

        [Test]
        public async Task Edit_ExistingEntity_PopulatesEditFields()
        {
            sut.Partners.Add(new Partner(name: "John Doe", address: new Address("street", "city", "country", "zipcode")));
            sut.SaveChanges();

            var partner = await sut.Partners.SingleAsync();
            Assert.Null(partner.LastModifiedAt, $"Arrange failed.");
            Assert.Null(partner.LastModifiedBy, $"Arrange failed.");

            // Act
            sut.Entry(partner).State = EntityState.Modified;
            sut.SaveChanges();

            partner = await sut.Partners.FirstAsync();
            Assert.AreEqual(TestDateTime, partner.LastModifiedAt, $"Expected {nameof(Partner.LastModifiedAt)} to be set to the current datetime.");
            Assert.AreEqual(TestUserId, partner.LastModifiedBy, $"Expected {nameof(Partner.LastModifiedBy)} to be set to the current user ID.");
        }

        [Test]
        public async Task Delete_ExistingEntity_ShouldNotBeRetrievable()
        {
            sut.Partners.Add(new Partner(name: "John Doe", address: new Address("street", "city", "country", "zipcode")));
            sut.SaveChanges();
            var partner = await sut.Partners.SingleAsync();

            // Act
            sut.Remove(partner);
            sut.SaveChanges();

            Assert.Null(await sut.Partners.SingleOrDefaultAsync(), "Entity must not be found after removal.");
        }

        [Test]
        public async Task Delete_ExistingEntity_DeleteShouldBeSoft()
        {
            sut.Partners.Add(new Partner(name: "John Doe", address: new Address("street", "city", "country", "zipcode")));
            sut.SaveChanges();
            var partner = await sut.Partners.SingleAsync();

            Assert.Null(partner.DeletedAt, $"Arrange failed.");
            Assert.Null(partner.DeletedBy, $"Arrange failed.");

            // Act
            sut.Remove(partner);
            sut.SaveChanges();

            partner = await sut.Partners.IgnoreQueryFilters().SingleOrDefaultAsync(); // Notice IgnoreQueryFilters()
            Assert.NotNull(partner, "Deleted entity expected to be found with ignored query filters.");
            Assert.AreEqual(TestDateTime, partner.DeletedAt, $"Expected {nameof(Partner.DeletedAt)} to be set to the current datetime.");
            Assert.AreEqual(TestUserId, partner.DeletedBy, $"Expected {nameof(Partner.DeletedBy)} to be set to the current user ID.");
        }
    }
}