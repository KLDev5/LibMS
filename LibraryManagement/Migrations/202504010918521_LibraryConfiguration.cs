namespace LibraryManagement.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class LibraryConfiguration : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.LibraryConfigurations",
                c => new
                    {
                        ConfigId = c.Int(nullable: false, identity: true),
                        ConfigName = c.String(),
                        ConfigValue = c.String(),
                        ConfigType = c.String(),
                        EffectiveDate = c.DateTime(nullable: false),
                        ExpiryDate = c.DateTime(),
                        IsActive = c.Boolean(nullable: false),
                        CreatedAt = c.DateTime(nullable: false),
                        UpdatedAt = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ConfigId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.LibraryConfigurations");
        }
    }
}
