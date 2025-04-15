namespace LibraryManagement.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ColumnBorrowIDRename : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.BorrowRecordDetails", name: "BorrowRecordId", newName: "BorrowID");
            RenameIndex(table: "dbo.BorrowRecordDetails", name: "IX_BorrowRecordId", newName: "IX_BorrowID");
        }
        
        public override void Down()
        {
            RenameIndex(table: "dbo.BorrowRecordDetails", name: "IX_BorrowID", newName: "IX_BorrowRecordId");
            RenameColumn(table: "dbo.BorrowRecordDetails", name: "BorrowID", newName: "BorrowRecordId");
        }
    }
}
