namespace LibraryManagement.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ModifyMemberProperties : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Members", "DateOfBirth", c => c.DateTime());
            AlterColumn("dbo.Members", "JoinDate", c => c.DateTime());
            AlterColumn("dbo.Members", "ExpiryDate", c => c.DateTime());
            AlterColumn("dbo.Members", "TotalBooksBorrowed", c => c.Int());
            AlterColumn("dbo.Members", "OutstandingFines", c => c.Double());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Members", "OutstandingFines", c => c.Double(nullable: false));
            AlterColumn("dbo.Members", "TotalBooksBorrowed", c => c.Int(nullable: false));
            AlterColumn("dbo.Members", "ExpiryDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Members", "JoinDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Members", "DateOfBirth", c => c.DateTime(nullable: false));
        }
    }
}
