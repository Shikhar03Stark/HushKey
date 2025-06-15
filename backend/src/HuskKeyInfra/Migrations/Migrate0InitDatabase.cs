using FluentMigrator;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HuskKeyInfra.Migrations
{
    [Migration(0)]
    public class Migrate0InitDatabase : Migration
    {
        public override void Down()
        {
            // DROP INDEXES
            Delete.Index("IX_EncryptedSecretsStats_EncryptedSecretId").OnTable("EncryptedSecretStats");

            // Drop Foreign Key Constraints
            Delete.ForeignKey("FK_EncryptedSecretStats_EncryptedSecrets").OnTable("EncryptedSecretStats");

            // Drop Tables
            Delete.Table("EncryptedSecretStats");
            Delete.Table("EncryptedSecrets");
        }

        public override void Up()
        {
            // Create Tables
            Create.Table("EncryptedSecrets")
                .WithColumn("Id").AsString(64).PrimaryKey().NotNullable()
                .WithColumn("EncryptedSecret").AsString().NotNullable()
                .WithColumn("EncryptionType").AsInt32().NotNullable()
                .WithColumn("CreatedAt").AsDateTime().NotNullable().WithDefault(SystemMethods.CurrentDateTime)
                .WithColumn("ExpiresAt").AsDateTime().Nullable();

            Create.Table("EncryptedSecretStats")
                .WithColumn("Id").AsInt32().PrimaryKey().Identity()
                .WithColumn("EncryptedSecretId").AsString(64).NotNullable()
                .WithColumn("AccessCount").AsInt32().NotNullable().WithDefaultValue(0)
                .WithColumn("LastAccessedAt").AsDateTime().Nullable();

            // Create Foreign Key Constraints
            Create.ForeignKey("FK_EncryptedSecretStats_EncryptedSecrets")
                .FromTable("EncryptedSecretStats").ForeignColumn("EncryptedSecretId")
                .ToTable("EncryptedSecrets").PrimaryColumn("Id")
                .OnDelete(Rule.None);

            // Create Indexes
            Create.Index("IX_EncryptedSecretsStats_EncryptedSecretId")
                .OnTable("EncryptedSecretStats")
                .OnColumn("EncryptedSecretId").Ascending()
                .WithOptions().Unique();
        }
    }
}
