namespace LibraryManagement.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveBookDetails : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.BorrowRecordDetails", "ApproverMemberId", "dbo.Members");
            DropForeignKey("dbo.BorrowRecordDetails", "BorrowerMemberId", "dbo.Members");
            DropForeignKey("dbo.BorrowRecordDetails", "BorrowID", "dbo.BorrowRecords");
            DropForeignKey("dbo.BorrowRecordDetails", "BorrowStatusId", "dbo.BorrowStatus");
            DropIndex("dbo.BorrowRecordDetails", new[] { "BorrowID" });
            DropIndex("dbo.BorrowRecordDetails", new[] { "BorrowStatusId" });
            DropIndex("dbo.BorrowRecordDetails", new[] { "BorrowerMemberId" });
            DropIndex("dbo.BorrowRecordDetails", new[] { "ApproverMemberId" });
            DropTable("dbo.BorrowRecordDetails");
        }
        
        public override void Down()
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
                .PrimaryKey(t => t.BorrowRecordDetailsId);
            
            CreateIndex("dbo.BorrowRecordDetails", "ApproverMemberId");
            CreateIndex("dbo.BorrowRecordDetails", "BorrowerMemberId");
            CreateIndex("dbo.BorrowRecordDetails", "BorrowStatusId");
            CreateIndex("dbo.BorrowRecordDetails", "BorrowID");
            AddForeignKey("dbo.BorrowRecordDetails", "BorrowStatusId", "dbo.BorrowStatus", "BorrowStatusId");
            AddForeignKey("dbo.BorrowRecordDetails", "BorrowID", "dbo.BorrowRecords", "BorrowID", cascadeDelete: true);
            AddForeignKey("dbo.BorrowRecordDetails", "BorrowerMemberId", "dbo.Members", "MemberId");
            AddForeignKey("dbo.BorrowRecordDetails", "ApproverMemberId", "dbo.Members", "MemberId");
        }
    }
}
