using Repository.Pattern.Ef6;
using System.Data.Entity;
using Portalia.Core.Entity;

namespace Portalia.Repository
{
    public class PortaliaContext : DataContext
    {
        static PortaliaContext()
        {
            Database.SetInitializer<PortaliaContext>(null);
        }

        public PortaliaContext() : base("PortaliaContext")
        {
        }

        public DbSet<Proposal> Proposals { get; set; }
        public DbSet<Document> Documents { get; set; }
        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<AttributeDetail> AttributeDetails { get; set; }
        public DbSet<UserProfileAttribute> UserProfileAttributes { get; set; }
        public DbSet<AttributeType> AttributeTypes { get; set; }
        public DbSet<AspNetRole> AspNetRoles { get; set; }
        public DbSet<AspNetUser> AspNetUsers { get; set; }
        public DbSet<AspNetUserClaim> AspNetUserClaims { get; set; }
        public DbSet<AspNetUserLogin> AspNetUserLogins { get; set; }
        public DbSet<Language> Languages { get; set; }
        public DbSet<CountryLanguage> CountryLanguages { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<BankInformationSystem> BankInformationSystems { get; set; }
        public DbSet<Currency> Currencies { get; set; }
        public DbSet<BirthPlace> BirthPlaces { get; set; }
        public DbSet<FolderTypeEntity> FolderTypes { get; set; }
        public DbSet<TrackingAction> TrackingActions { get; set; }
        public DbSet<WorkContract> WorkContracts { get; set; }
        public DbSet<StatusMapping> StatusMappings { get; set; }
        public DbSet<RefData> RefDatas { get; set; }
        public DbSet<FieldDetail> FieldDetails { get; set; }
        public DbSet<WorkContractStatus> WorkContractStatuses { get; set; }
        public DbSet<TrackingChange> TrackingChanges { get; set; }
        public DbSet<DataField> DataFields { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<Skill> Skills { get; set; }
        public DbSet<Amaris_Smart_City> Amaris_Smart_Cities { get; set; }

        public DbSet<Migration> Migrations { get; set; }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.AddFromAssembly(typeof(PortaliaContext).Assembly);
        }
    }
}
