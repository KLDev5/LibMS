namespace LibraryManagement.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class BookCatalogueBorrow : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.BorrowRecords",
                c => new
                    {
                        BorrowID = c.Long(nullable: false, identity: true),
                        MemberId = c.Long(nullable: false),
                        BookId = c.Long(nullable: false),
                        BorrowDate = c.DateTime(),
                        ScheduledReturnDate = c.DateTime(),
                        ActualReturnDate = c.DateTime(),
                        BorrowStatusId = c.Long(nullable: false),
                        OverdueStatus = c.Boolean(nullable: false),
                        OverdueAmount = c.Double(),
                    })
                .PrimaryKey(t => t.BorrowID)
                .ForeignKey("dbo.Books", t => t.BookId, cascadeDelete: true)
                .ForeignKey("dbo.BorrowStatus", t => t.BorrowStatusId, cascadeDelete: true)
                .ForeignKey("dbo.Members", t => t.MemberId, cascadeDelete: true)
                .Index(t => t.MemberId)
                .Index(t => t.BookId)
                .Index(t => t.BorrowStatusId);
            
            CreateTable(
                "dbo.BorrowStatus",
                c => new
                    {
                        BorrowStatusId = c.Long(nullable: false, identity: true),
                        BorrowStatusName = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.BorrowStatusId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.BorrowRecords", "MemberId", "dbo.Members");
            DropForeignKey("dbo.BorrowRecords", "BorrowStatusId", "dbo.BorrowStatus");
            DropForeignKey("dbo.BorrowRecords", "BookId", "dbo.Books");
            DropIndex("dbo.BorrowRecords", new[] { "BorrowStatusId" });
            DropIndex("dbo.BorrowRecords", new[] { "BookId" });
            DropIndex("dbo.BorrowRecords", new[] { "MemberId" });
            DropTable("dbo.BorrowStatus");
            DropTable("dbo.BorrowRecords");
        }
    }
}
