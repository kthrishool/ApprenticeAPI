namespace ADMS.Apprentice.Core
{
    public class OurDatabaseSettings
    {
        public string DatabaseConnectionString { get; set; }
        public bool MigrateDatabaseOnStartup { get; set; }
        public bool SeedSampleData { get; set; }
    }
}