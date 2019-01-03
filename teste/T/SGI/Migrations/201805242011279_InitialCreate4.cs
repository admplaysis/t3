namespace SGI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate4 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.T_FILA_PRODUCAO", "FPR_ORDEM_FILA", c => c.Single(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.T_FILA_PRODUCAO", "FPR_ORDEM_FILA", c => c.String());
        }
    }
}
