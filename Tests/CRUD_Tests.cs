namespace Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using SampleLogMaker.Models;

    [TestClass]
    public class CRUD_Tests : PersistanceTests
    {
        Blog b;

        Comment c;

        [TestInitialize]
        public void LocalInitialise()
        {
            b = this.DB.CreateBlog();
            c = this.DB.CreateComment(b);
        }

        [TestMethod]
        public void CanSaveBlog()
        {
            Assert.IsTrue(b.Id > 0);
        }

        [TestMethod]
        public void Can_SaveComment()
        {
            Assert.IsTrue(c.Id > 0);
        }

        [TestMethod]
        public void Can_Fetch_Comment_and_its_blog()
        {
            var comment = this.DB.Comments.Find(c.Id);

            Assert.IsNotNull(comment);
            Assert.IsNotNull(comment.ParentBlog);
        }

        [TestMethod]
        public void Can_save_recursively()
        {
            var blog = new Blog { Description = "abc", Title = "qwe" };

            var comment = new Comment { ParentBlog = blog, Text = "tyu" };
            this.DB.Comments.Add(comment);

            this.DB.SaveChanges("unit test");

            Assert.IsTrue(blog.Id > 0);
            Assert.IsTrue(comment.Id > 0);
        }
    }
}