namespace PKRY.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.user",
                c => new
                    {
                        id_user = c.Int(nullable: false, identity: true),
                        user_name = c.String(unicode: false),
                        hash = c.String(unicode: false),
                        salt = c.String(unicode: false),
                    })
                .PrimaryKey(t => t.id_user);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.user");
        }
    }
}
