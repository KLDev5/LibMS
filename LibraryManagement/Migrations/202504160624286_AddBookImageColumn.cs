namespace LibraryManagement.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddBookImageColumn : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Books", "BookImage", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Books", "BookImage");
        }
    }
}
