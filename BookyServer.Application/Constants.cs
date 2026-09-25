namespace BookyServer.Application;

public static class Constants
{
    public static class Errors
    {
        public const string InvalidConversationMessage = "Un message doit contenir de 1 à 3000 caractères.";
        public const string InvalidMapMarkerName = "Le nom du lieu est obligatoire et doit faire au plus 160 caractères.";
        public const string InvalidMapMarkerCategory = "La catégorie du lieu est obligatoire.";
        public const string InvalidMapMarkerCoordinates = "Les coordonnées géographiques sont invalides.";
        public const string InvalidProfileFirstName = "Le prénom est obligatoire et doit faire au plus 80 caractères.";
        public const string InvalidProfileBirthDate = "La date de naissance est invalide.";
        public const string TooManyProfileInterests = "Un profil peut contenir au maximum 20 centres d'intérêt.";
        public const string InvalidProfileInterest = "Chaque centre d'intérêt doit faire au plus 60 caractères.";
        public const string InvalidBookRating = "Avis attendu : deteste, pas_aime, aime ou coup_de_coeur.";
        public const string UnknownBookRating = "Avis de lecture inconnu.";
    }

    public static class Ratings
    {
        public const string Detested = "deteste";
        public const string Disliked = "pas_aime";
        public const string Liked = "aime";
        public const string Favorite = "coup_de_coeur";
    }
}
