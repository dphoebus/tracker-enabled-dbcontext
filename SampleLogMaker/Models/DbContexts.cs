namespace SampleLogMaker.Models
{
    using System.Data.Entity;

    using TrackerEnabledDbContext.Identity;

    public class ApplicationDbContext : TrackerIdentityContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection")
        {
        }

        public DbSet<Blog> Blogs { get; set; }

        public DbSet<Comment> Comments { get; set; }
    }
}