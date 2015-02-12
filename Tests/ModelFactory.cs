namespace Tests
{
    using System;

    using SampleLogMaker.Models;

    public static class ModelFactory
    {
        public const string Username = "unit test";

        public static Blog CreateBlog(this TestTrackerContext context)
        {
            var b = new Blog
                        {
                            Description = RandomString(),
                            Title = RandomString()
                        };

            context.Blogs.Add(b);
            context.SaveChanges(Username);
            return b;
        }

        public static Comment CreateComment(this TestTrackerContext context, Blog blog)
        {
            var c = new Comment
                        {
                            Text = RandomString(),
                            ParentBlog = blog
                        };

            context.Comments.Add(c);
            context.SaveChanges(Username);
            return c;
        }

        static string RandomString()
        {
            return Guid.NewGuid().ToString();
        }
    }
}