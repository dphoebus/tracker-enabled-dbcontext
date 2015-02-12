namespace Tests
{
    using System.Data.Entity;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class PersistanceTests
    {
        public TestTrackerContext DB = new TestTrackerContext();

        public DbContextTransaction Transaction = null;

        [TestInitialize]
        public virtual void Initialize()
        {
            this.Transaction = this.DB.Database.BeginTransaction();
        }

        [TestCleanup]
        public virtual void CleanUp()
        {
            this.Transaction.Rollback();
        }
    }
}