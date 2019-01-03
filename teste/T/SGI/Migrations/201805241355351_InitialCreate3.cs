namespace SGI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate3 : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.T_FILA_PRODUCAO", "FPR_SEQUENCIA");
        }
        
        public override void Down()
        {
            AddColumn("dbo.T_FILA_PRODUCAO", "FPR_SEQUENCIA_OPS", c => c.Int(nullable: false));
        }
    }
}
