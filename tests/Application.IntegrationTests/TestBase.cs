using NUnit.Framework;
using System.Threading.Tasks;

namespace MyWarehouse.Application.IntegrationTests
{
    /// <summary>
    /// Derive all test classes from this base class to implement the cross-cutting concern
    /// of creating/disposing a context and resetting the database after each test run.
    /// </summary>
    public abstract class TestBase
    {
        [SetUp]
        public void BeforeTest()
        {
            // This creates a new DbContext for the duration of the test.
            // Having a per-test DbContext instead of a per-operation one
            // helps tests which touch multiple entities.
            // It allows them to retrieve entities and add them to other entities,
            // because the retrieved entities are still tracked by the change tracker.
            // Without a per-test DbContext we'd need to attach all such entities
            // when saving as part of other entities.
            TestFramework.Context.CreateScope();
        }

        [TearDown]
        public async Task AfterTest()
        {
            await TestFramework.Data.ResetDatabase();
            TestFramework.Context.DisposeScope();
        }
    }
}