namespace LibraryManagement.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addBookStatus : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Books",
                c => new
                    {
                        BookId = c.Long(nullable: false, identity: true),
                        Title = c.String(nullable: false, maxLength: 128),
                        Author = c.String(nullable: false, maxLength: 128),
                        Publisher = c.String(),
                        ISBN = c.String(),
                        PublishedDate = c.DateTime(nullable: false),
                        isDeleted = c.Boolean(nullable: false),
                        BookStatusID = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.BookId)
                .ForeignKey("dbo.BookStatus", t => t.BookStatusID, cascadeDelete: true)
                .Index(t => t.BookStatusID);
            
            CreateTable(
                "dbo.BookStatus",
                c => new
                    {
                        BookStatusID = c.Long(nullable: false, identity: true),
                        BookStatusName = c.String(),
                    })
                .PrimaryKey(t => t.BookStatusID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Books", "BookStatusID", "dbo.BookStatus");
            DropIndex("dbo.Books", new[] { "BookStatusID" });
            DropTable("dbo.BookStatus");
            DropTable("dbo.Books");
        }
    }
}
