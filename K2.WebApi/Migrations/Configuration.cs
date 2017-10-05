using System.Data.Entity.Migrations;
using K2.WebApi.Models.Context;

namespace K2.WebApi.Migrations
{
    internal sealed class Configuration:DbMigrationsConfiguration<K2Context>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;
        }

        protected override void Seed(K2Context context)
        {
        }
    }
}