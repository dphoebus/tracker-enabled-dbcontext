namespace Tests
{
    using System;
    using System.Linq;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using SampleLogMaker.Models;

    using TrackerEnabledDbContext.Common.Models;

    [TestClass]
    public class Tracker_Tests : PersistanceTests
    {
        [TestMethod]
        public void Can_track_row_addition()
        {
            //add a blog
            var blog = this.DB.CreateBlog();

            //fetch log for it
            var log = this.DB.GetLogs<Blog>(blog.Id).SingleOrDefault(x => x.EventType == EventType.Added);

            Assert.IsNotNull(log);
        }

        [TestMethod]
        public void Can_track_deletion()
        {
            //create log
            var blog = this.DB.CreateBlog();
            var blogId = blog.Id.ToString();
            Assert.IsTrue(blog.Id > 0);

            //remove log
            this.DB.Blogs.Remove(blog);
            this.DB.SaveChanges(ModelFactory.Username);

            //fetch removal log
            var log = this.DB.GetLogs<Blog>(blogId).SingleOrDefault(x => x.RecordId == blogId && x.EventType == EventType.Deleted);
            Assert.IsNotNull(log);
        }

        [TestMethod]
        public void Can_track_local_propery_change()
        {
            //create log
            var blog = this.DB.CreateBlog();

            //change property
            var oldTitle = blog.Title;
            var newTitle = Guid.NewGuid().ToString();
            blog.Title = newTitle;
            this.DB.SaveChanges(ModelFactory.Username);

            //fetch log
            var log =
                this.DB.GetLogs<Blog>(blog.Id)
                    .ToList()
                    .SingleOrDefault(x => x.RecordId == blog.Id.ToString() && x.EventType == EventType.Modified);
            Assert.IsNotNull(log);
            Assert.IsTrue(log.LogDetails.Any(x => x.ColumnName == "Title" && x.NewValue == newTitle && x.OrginalValue == oldTitle));
        }

        [TestMethod]
        public void Can_track_navigational_property_change()
        {
            var blog1 = this.DB.CreateBlog();
            var comment = this.DB.CreateComment(blog1);

            var blog2 = this.DB.CreateBlog();
            comment.ParentBlog = blog2;

            var state = this.DB.Entry(comment);

            this.DB.SaveChanges(ModelFactory.Username);

            //fetch log
            var logPresent = this.DB
                                 .GetLogs<Comment>(comment.Id)
                                 .ToList()
                                 .Any(x => x.EventType == EventType.Modified);
            Assert.IsTrue(logPresent);
        }
    }
}