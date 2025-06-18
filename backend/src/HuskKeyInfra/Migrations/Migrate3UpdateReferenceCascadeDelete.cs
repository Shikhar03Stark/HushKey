using FluentMigrator;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HuskKeyInfra.Migrations
{
    [Migration(3)]
    public class Migrate3UpdateReferenceCascadeDelete : Migration
    {
        public override void Up()
        {
            // Update foreign key constraint to cascade delete
            Execute.Sql(@"ALTER TABLE ""EncryptedSecretStats"" DROP CONSTRAINT ""FK_EncryptedSecretStats_EncryptedSecrets"";");
            Create.ForeignKey("FK_EncryptedSecretStats_EncryptedSecrets")
                .FromTable("EncryptedSecretStats").ForeignColumn("EncryptedSecretId")
                .ToTable("EncryptedSecrets").PrimaryColumn("Id")
                .OnDelete(Rule.Cascade);
        }
        public override void Down()
        {
            // Revert foreign key constraint to no action on delete
            Execute.Sql(@"ALTER TABLE ""EncryptedSecretStats"" DROP CONSTRAINT ""FK_EncryptedSecretStats_EncryptedSecrets"";");
            Create.ForeignKey("FK_EncryptedSecretStats_EncryptedSecrets")
                .FromTable("EncryptedSecretStats").ForeignColumn("EncryptedSecretId")
                .ToTable("EncryptedSecrets").PrimaryColumn("Id")
                .OnDelete(Rule.None);
        }
    }
}
