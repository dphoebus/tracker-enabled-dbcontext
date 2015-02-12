//namespace SampleLogMaker.MyMigrations
//{
//    using System.Data.Entity.Migrations;

//    public partial class Addedforkeyforblog : DbMigration
//    {
//        public override void Up()
//        {
//            RenameColumn("dbo.Comments", "ParentBlog_Id", "ParentBlogId");
//            RenameIndex("dbo.Comments", "IX_ParentBlog_Id", "IX_ParentBlogId");
//        }

//        public override void Down()
//        {
//            RenameIndex("dbo.Comments", "IX_ParentBlogId", "IX_ParentBlog_Id");
//            RenameColumn("dbo.Comments", "ParentBlogId", "ParentBlog_Id");
//        }
//    }
//}