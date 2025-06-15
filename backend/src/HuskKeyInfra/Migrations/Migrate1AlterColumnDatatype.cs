using FluentMigrator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HuskKeyInfra.Migrations
{
    [Migration(1)]
    public class Migrate1AlterColumnDatatype : Migration
    {
        public override void Up()
        {
            // 1. Drop primary key
            Delete.PrimaryKey("PK_EncryptedSecretStats").FromTable("EncryptedSecretStats");

            // 2. Drop identity (PostgreSQL-specific)
            Execute.Sql(@"ALTER TABLE ""EncryptedSecretStats"" ALTER COLUMN ""Id"" DROP IDENTITY IF EXISTS;");

            // 3. Change column type
            Execute.Sql(@"ALTER TABLE ""EncryptedSecretStats"" ALTER COLUMN ""Id"" TYPE VARCHAR(64) USING ""Id""::VARCHAR;");

            // 4. Re-add primary key
            Create.PrimaryKey("PK_EncryptedSecretStats")
                .OnTable("EncryptedSecretStats")
                .Column("Id");
        }

        public override void Down()
        {
            // 1. Drop primary key
            Delete.PrimaryKey("PK_EncryptedSecretStats").FromTable("EncryptedSecretStats");

            // 2. Change type back to int
            Execute.Sql(@"ALTER TABLE ""EncryptedSecretStats"" ALTER COLUMN ""Id"" TYPE INTEGER USING ""Id""::INTEGER;");

            // 3. Re-add identity (PostgreSQL-specific)
            Execute.Sql(@"ALTER TABLE ""EncryptedSecretStats"" ALTER COLUMN ""Id"" ADD GENERATED ALWAYS AS IDENTITY;");

            // 4. Re-add primary key
            Create.PrimaryKey("PK_EncryptedSecretStats")
                .OnTable("EncryptedSecretStats")
                .Column("Id");
        }
    }

}
