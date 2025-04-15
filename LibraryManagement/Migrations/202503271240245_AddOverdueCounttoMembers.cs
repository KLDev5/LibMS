namespace LibraryManagement.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddOverdueCounttoMembers : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Members", "OverdueCount", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Members", "OverdueCount");
        }
    }
}
