namespace Tests
{
    using System.Data.Entity;

    using SampleLogMaker.Models;

    using TrackerEnabledDbContext;

    public class TestTrackerContext : TrackerContext
    {
        public TestTrackerContext()
            : base("DefaultConnection")
        {
        }

        public DbSet<Blog> Blogs { get; set; }

        public DbSet<Comment> Comments { get; set; }
    }
}