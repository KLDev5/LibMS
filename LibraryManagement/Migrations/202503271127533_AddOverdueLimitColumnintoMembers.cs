namespace LibraryManagement.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddOverdueLimitColumnintoMembers : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Members", "OverdueLimit", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Members", "OverdueLimit");
        }
    }
}
