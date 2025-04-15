namespace LibraryManagement.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddMember : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Members",
                c => new
                    {
                        MemberId = c.Long(nullable: false, identity: true),
                        UserId = c.Long(nullable: false),
                        MembershipCode = c.String(nullable: false),
                        PhoneNumber = c.String(),
                        Address = c.String(),
                        DateOfBirth = c.DateTime(nullable: false),
                        JoinDate = c.DateTime(nullable: false),
                        ExpiryDate = c.DateTime(nullable: false),
                        TotalBooksBorrowed = c.Int(nullable: false),
                        OutstandingFines = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.MemberId)
                .ForeignKey("dbo.Users", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Members", "UserId", "dbo.Users");
            DropIndex("dbo.Members", new[] { "UserId" });
            DropTable("dbo.Members");
        }
    }
}
