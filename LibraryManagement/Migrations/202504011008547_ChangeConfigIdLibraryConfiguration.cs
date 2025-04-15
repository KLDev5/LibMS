namespace LibraryManagement.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangeConfigIdLibraryConfiguration : DbMigration
    {
        public override void Up()
        {
            DropPrimaryKey("dbo.LibraryConfigurations");
            AlterColumn("dbo.LibraryConfigurations", "ConfigId", c => c.Long(nullable: false, identity: true));
            AddPrimaryKey("dbo.LibraryConfigurations", "ConfigId");
        }
        
        public override void Down()
        {
            DropPrimaryKey("dbo.LibraryConfigurations");
            AlterColumn("dbo.LibraryConfigurations", "ConfigId", c => c.Int(nullable: false, identity: true));
            AddPrimaryKey("dbo.LibraryConfigurations", "ConfigId");
        }
    }
}
