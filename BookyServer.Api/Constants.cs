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

    public static class Headers
    {
        public const string P3P = "P3P";
        public const string P3PValue = "CP=\"IDC DSP COR ADM DEVi TAIi PSA PSD IVAi IVDi CONi HIS OUR IND CNT\"";
        public const string PoweredBy = "X-Powered-By";
        public const string Server = "Server";
        public const string ContentSecurityPolicy = "Content-Security-Policy";
        public const string ContentSecurityPolicyValue = "default-src 'self'";
        public const string FrameOptions = "X-Frame-Options";
        public const string FrameOptionsValue = "DENY";
        public const string XssProtection = "X-XSS-Protection";
        public const string XssProtectionValue = "1; mode=block";
        public const string ContentTypeOptions = "X-Content-Type-Options";
        public const string ContentTypeOptionsValue = "nosniff";
        public const string StrictTransportSecurity = "Strict-Transport-Security";
        public const string StrictTransportSecurityValue = "max-age=31536000; includeSubDomains";
    }

    public static class OpenApi
    {
        public const string Endpoint = "/openapi/v1.json";
        public const string Title = "BookyServer API v1";
    }

    public static class Logging
    {
        public const string UnhandledException = "{Message}\n{StackTrace}";
    }

    public static class ResponseContentTypes
    {
        public const string Json = "application/json";
        public const string Utf8Json = "application/json; charset=utf-8";
    }

    public static class Requests
    {
        public const string FormDataContentDisposition = "Content-Disposition: form-data";
    }

    public static class Routes
    {
        public const string Authentication = "/api/v1/auth";
        public const string Books = "api/v1/books";
        public const string Conversations = "api/v1/groups/{groupId:guid}/messages";
        public const string MapMarkers = "api/v1/map-markers";
        public const string MapMarkersPath = "/api/v1/map-markers";
        public const string Profiles = "api/v1/profiles";
        public const string ReaderGroups = "api/v1/groups";
        public const string CurrentProfile = "me";
        public const string ProfileBook = "me/books/{bookId:guid}";
        public const string ProfileById = "{id:guid}";
        public const string ReaderGroupMembers = "{groupId:guid}/members";
        public const string ReaderGroupJoin = "{groupId:guid}/join";
        public const string HealthLive = "/health/live";
        public const string HealthReady = "/health/ready";
    }

    public static class Statuses
    {
        public const string Live = "live";
        public const string Ready = "ready";
    }
}
