using FluentMigrator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HuskKeyInfra.Migrations
{
    [Migration(2)]
    public class Migrate2AddColumnBurnAfterRead : Migration
    {
        public override void Up()
        {
            // Add the BurnAfterRead column to the EncryptedSecrets table
            Alter.Table("EncryptedSecrets")
                .AddColumn("BurnAfterRead").AsBoolean().NotNullable().WithDefaultValue(false);
        }
        public override void Down()
        {
            // Remove the BurnAfterRead column from the EncryptedSecrets table
            Delete.Column("BurnAfterRead").FromTable("EncryptedSecrets");
        }
    }
}
