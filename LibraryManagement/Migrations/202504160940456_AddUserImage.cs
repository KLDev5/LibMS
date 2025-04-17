namespace LibraryManagement.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUserImage : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Members", "MemberImage", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Members", "MemberImage");
        }
    }
}
