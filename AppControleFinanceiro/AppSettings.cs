namespace AppControleFinanceiro
{
    public class AppSettings
    {
        private static string DatabaseName = "database.db";
        private static string DatabaseDirectory = FileSystem.AppDataDirectory;
        public static readonly string DatabasePath = Path.Combine(DatabaseDirectory, DatabaseName);
    }
}
