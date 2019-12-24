namespace A3DBhavCopy.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class A3DBhavCopyData : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.BhavCopyHead",
                c => new
                {
                    iFileID = c.Int(nullable: false, identity: true),
                    cFileName = c.String(),
                    dFileDate = c.DateTime(nullable: false),
                    dFileUploadDate = c.DateTime(nullable: false),
                })
                .PrimaryKey(t => t.iFileID);

            CreateTable(
                "dbo.BhavCopyDetails",
                c => new
                {
                    iID = c.Int(nullable: false, identity: true),
                    cSYMBOL = c.String(),
                    cSERIES = c.String(),
                    cOPEN = c.String(),
                    cHIGH = c.String(),
                    cLOW = c.String(),
                    cCLOSE = c.String(),
                    cLAST = c.String(),
                    cPREVCLOSE = c.String(),
                    cTOTTRDQTY = c.String(),
                    cTOTTRDVAL = c.String(),
                    cTIMESTAMP = c.String(),
                    cTOTALTRADES = c.String(),
                    cISIN = c.String(),
                    dTIMESTAMP = c.DateTime(nullable: false),
                    iFileID = c.Int(nullable: false),
                })
                .PrimaryKey(t => t.iID);

        }

        public override void Down()
        {
            DropTable("dbo.BhavCopyDetails");
            DropTable("dbo.BhavCopyHead");
        }
    }
}
