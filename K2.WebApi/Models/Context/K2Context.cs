using System.Configuration;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Data.SqlClient;
using Configuration = K2.WebApi.Migrations.Configuration;


namespace K2.WebApi.Models.Context
{
    public class K2Context:DbContext
    {
        static K2Context()
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<K2Context, Configuration>());
        }
        public K2Context() :
            base(new SqlConnection(ConfigurationManager.ConnectionStrings["K2Context"].ConnectionString), true)
        {
            this.Configuration.LazyLoadingEnabled = false;
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)

        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

        }
        public DbSet<ConceptInfo> ConceptInfos { get; set; }
        public DbSet<DocumentInfo> DocumentInfos { get; set; }
        public DbSet<DocumentTypeInfo> DocumentTypeInfos { get; set; }
        public DbSet<StakeHolder> StakeHolders { get; set; }
        public DbSet<PauseInfo> PauseInfos { get; set; }
        public DbSet<ActionHistoryInfo> ActionHistoryInfos { get; set; }
        public DbSet<BrdInfo> BrdInfos { get; set; }
        public DbSet<RoadMap> RoadMaps { get; set; }
        public DbSet<RoadMapMaster> RoadMapMasters { get; set; }
        public DbSet<TaskInfo> TaskInfos { get; set; }
        public DbSet<SlaCategory> SlaCategories { get; set; }
        public DbSet<ImplementationInfo> ImplementationInfos { get; set; }
        public DbSet<ImplementationTimeline> ImplementationTimelines { get; set; }
        public DbSet<ActivityInfo> ActivityInfos { get; set; }

    }
}