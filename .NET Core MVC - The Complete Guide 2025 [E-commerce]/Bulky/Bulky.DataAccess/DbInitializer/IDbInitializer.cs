namespace Bulky.DataAccess.DbInitializer
{
    public interface IDbInitializer
    {
        Task Initialize();

        void SeedData();

        void MigrateDatabase();

        void EnsureCreated();

        void EnsureDeleted();

        void ApplyMigrations();

        void CreateDatabaseIfNotExists();

        void ConfigureDatabaseSettings();

        void LogDatabaseInitializationStatus();
    }
}