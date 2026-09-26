namespace BookyServer.Api;

public static class Constants
{
    public static class Authorization
    {
        public const string AdministratorPolicy = "AdminOnly";
        public const string AdministratorRole = "Admin";
    }

    public static class Configuration
    {
        public const string RequireConfirmedEmail = "Authentication:RequireConfirmedEmail";
        public const string AllowedCorsOrigins = "Cors:AllowedOrigins";
    }

    public static class Cookies
    {
        public const string AuthenticationName = "__Host-BookyServer.Auth";
    }

    public static class Errors
    {
        public const string InvalidConversationMessage = "Message invalide";
        public const string InvalidMapMarker = "Lieu invalide";
        public const string InvalidProfile = "Profil invalide";
        public const string InvalidBookRating = "Avis de lecture invalide";
        public const string InvalidCurrentUser = "L'identifiant utilisateur de la session est invalide.";
        public const string ReaderGroupAtCapacity = "Le groupe a atteint sa capacité maximale.";
        public const string AntiXss = "Error from AntiXssMiddleware";
    }

}
