namespace LibraryManagement.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Changeborrowidcolumnname : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.BorrowRecordDetails", new[] { "BorrowID" });
            CreateIndex("dbo.BorrowRecordDetails", "BorrowId");
        }
        
        public override void Down()
        {
            DropIndex("dbo.BorrowRecordDetails", new[] { "BorrowId" });
            CreateIndex("dbo.BorrowRecordDetails", "BorrowID");
        }
    }
}
