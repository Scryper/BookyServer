namespace BookyServer.Infrastructure;

public static class Constants
{
    public static class Configuration
    {
        public const string BookyDatabaseConnectionString = "BookyDatabase";
        public const string MissingBookyDatabaseConnectionString =
            "La configuration ConnectionStrings:BookyDatabase est manquante.";
    }

    public static class Database
    {
        public const string BooksTable = "Books";
        public const string ConversationMessagesTable = "ConversationMessages";
        public const string GroupMembershipsTable = "GroupMemberships";
        public const string MapMarkersTable = "MapMarkers";
        public const string ProfileBooksTable = "ProfileBooks";
        public const string ProfileInterestsTable = "ProfileInterests";
        public const string ProfilePhotosTable = "ProfilePhotos";
        public const string ProfilesTable = "Profiles";
        public const string ReaderGroupsTable = "ReaderGroups";
        public const string DateColumnType = "date";
        public const string BookIsbnFilter = "[Isbn13] IS NOT NULL";
        public const string BookSourceAndExternalIdFilter = "[Source] IS NOT NULL AND [ExternalId] IS NOT NULL";
        public const string MapMarkerLatitudeConstraint = "CK_MapMarkers_Latitude";
        public const string MapMarkerLatitudePredicate = "[Latitude] >= -90 AND [Latitude] <= 90";
        public const string MapMarkerLongitudeConstraint = "CK_MapMarkers_Longitude";
        public const string MapMarkerLongitudePredicate = "[Longitude] >= -180 AND [Longitude] <= 180";
        public const string ReaderGroupMaximumMembersConstraint = "CK_ReaderGroups_MaxMembers";
        public const string ReaderGroupMaximumMembersPredicate = "[MaxMembers] >= 1 AND [MaxMembers] <= 8";
    }

    public static class SeedData
    {
        public const string BookCatalogSource = "Booky initial catalog";
    }
}
