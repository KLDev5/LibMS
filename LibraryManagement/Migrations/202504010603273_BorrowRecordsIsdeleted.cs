namespace LibraryManagement.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class BorrowRecordsIsdeleted : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.BorrowRecordDetails", "isDeleted", c => c.Boolean(nullable: false));
            AddColumn("dbo.BorrowRecords", "isDeleted", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.BorrowRecords", "isDeleted");
            DropColumn("dbo.BorrowRecordDetails", "isDeleted");
        }
    }
}
