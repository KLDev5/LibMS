namespace LibraryManagement.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ReaddingBorrowDetails : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.BorrowRecordDetails",
                c => new
                    {
                        BorrowRecordDetailsId = c.Long(nullable: false, identity: true),
                        BorrowID = c.Long(nullable: false),
                        BorrowStatusId = c.Long(),
                        BorrowerMemberId = c.Long(),
                        ApproverMemberId = c.Long(),
                        BorrowRecordDetailsDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.BorrowRecordDetailsId)
                .ForeignKey("dbo.Members", t => t.ApproverMemberId)
                .ForeignKey("dbo.Members", t => t.BorrowerMemberId)
                .ForeignKey("dbo.BorrowRecords", t => t.BorrowID, cascadeDelete: true)
                .ForeignKey("dbo.BorrowStatus", t => t.BorrowStatusId)
                .Index(t => t.BorrowID)
                .Index(t => t.BorrowStatusId)
                .Index(t => t.BorrowerMemberId)
                .Index(t => t.ApproverMemberId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.BorrowRecordDetails", "BorrowStatusId", "dbo.BorrowStatus");
            DropForeignKey("dbo.BorrowRecordDetails", "BorrowID", "dbo.BorrowRecords");
            DropForeignKey("dbo.BorrowRecordDetails", "BorrowerMemberId", "dbo.Members");
            DropForeignKey("dbo.BorrowRecordDetails", "ApproverMemberId", "dbo.Members");
            DropIndex("dbo.BorrowRecordDetails", new[] { "ApproverMemberId" });
            DropIndex("dbo.BorrowRecordDetails", new[] { "BorrowerMemberId" });
            DropIndex("dbo.BorrowRecordDetails", new[] { "BorrowStatusId" });
            DropIndex("dbo.BorrowRecordDetails", new[] { "BorrowID" });
            DropTable("dbo.BorrowRecordDetails");
        }
    }
}
