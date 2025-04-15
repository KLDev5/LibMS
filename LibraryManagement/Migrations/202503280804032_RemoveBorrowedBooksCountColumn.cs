namespace LibraryManagement.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveBorrowedBooksCountColumn : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Members", "BorrowedBooksCount");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Members", "BorrowedBooksCount", c => c.Int());
        }
    }
}
