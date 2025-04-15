namespace LibraryManagement.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddBorrowedBooksCountColumntoMember : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Members", "BorrowedBooksCount", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Members", "BorrowedBooksCount");
        }
    }
}
