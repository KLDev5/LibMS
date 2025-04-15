namespace LibraryManagement.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ModifyMembersAddIsDeleted : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Members", "IsDeleted", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Members", "IsDeleted");
        }
    }
}
