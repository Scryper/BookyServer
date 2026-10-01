namespace BookyServer.Infrastructure;

public static class Constants
{
    public static class Configuration
    {
        public const string BookyDatabaseConnectionString = "BookyDatabase";
        public const string MissingBookyDatabaseConnectionString =
            "La configuration ConnectionStrings:BookyDatabase est manquante.";
    }
}
