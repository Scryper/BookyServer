using BookyServer.Domain.Entities;

namespace BookyServer.Infrastructure.Persistence;

internal static class BookCatalogSeed
{
    public static readonly Book[] All =
    [
        Book("10000000-0000-0000-0000-000000000001", "L’Étranger", "Albert Camus", 1942, "Classique", "Un roman bref sur le regard des autres."),
        Book("10000000-0000-0000-0000-000000000002", "La Vie devant soi", "Émile Ajar", 1975, "Roman", "Une amitié inattendue et une immense tendresse."),
        Book("10000000-0000-0000-0000-000000000003", "Chanson douce", "Leïla Slimani", 2016, "Roman contemporain", "Une tension qui s’installe page après page."),
        Book("10000000-0000-0000-0000-000000000004", "L’Anomalie", "Hervé Le Tellier", 2020, "Imaginaire", "Plusieurs destins face à l’inexplicable."),
        Book("10000000-0000-0000-0000-000000000005", "Les Choses humaines", "Karine Tuil", 2019, "Roman contemporain", "Une histoire de convictions, de familles et de justice."),
        Book("10000000-0000-0000-0000-000000000006", "Rien ne s’oppose à la nuit", "Delphine de Vigan", 2011, "Récit", "Une enquête intime à travers la mémoire familiale."),
        Book("10000000-0000-0000-0000-000000000007", "Leurs enfants après eux", "Nicolas Mathieu", 2018, "Roman contemporain", "Jeunesse et désir d’ailleurs dans l’est de la France."),
        Book("10000000-0000-0000-0000-000000000008", "S’adapter", "Clara Dupont-Monod", 2021, "Roman", "Une fratrie racontée avec délicatesse."),
        Book("10000000-0000-0000-0000-000000000009", "La Tresse", "Laetitia Colombani", 2017, "Roman", "Trois parcours de femmes qui se répondent."),
        Book("10000000-0000-0000-0000-000000000010", "Le Comte de Monte-Cristo", "Alexandre Dumas", 1844, "Classique", "Une grande histoire d’évasion et de revanche."),
        Book("10000000-0000-0000-0000-000000000011", "1984", "George Orwell", 1949, "Anticipation", "Une dystopie qui reste saisissante."),
        Book("10000000-0000-0000-0000-000000000012", "La Horde du Contrevent", "Alain Damasio", 2004, "Imaginaire", "Un voyage collectif contre le vent."),
        Book("10000000-0000-0000-0000-000000000013", "Une vie", "Simone Veil", 2007, "Mémoires", "Un témoignage et un parcours de convictions."),
        Book("10000000-0000-0000-0000-000000000014", "Au revoir là-haut", "Pierre Lemaitre", 2013, "Roman historique", "Une fresque inventive après la Grande Guerre."),
        Book("10000000-0000-0000-0000-000000000015", "Petit Pays", "Gaël Faye", 2016, "Roman", "Une enfance bouleversée, racontée avec poésie.")
    ];

    private static Book Book(
        string id, string title, string author, int year, string genre, string synopsis) => new()
    {
        Id = Guid.Parse(id),
        Title = title,
        Author = author,
        PublicationYear = year,
        Genre = genre,
        Synopsis = synopsis,
        Source = "Booky initial catalog"
    };
}
