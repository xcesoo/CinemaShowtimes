namespace Domain.Entities;

public class Movie
{
    public Guid Id { get; init; }
    public string Title { get; private set; }
    public string Category { get; private set; }
    public int Year { get; private set; }
    
    private Movie (){} // for ef core

    public static Movie Create(string title, string category, int year)
    {
        ArgumentNullException.ThrowIfNull(title);
        ArgumentNullException.ThrowIfNull(category);
        if (year < 1) throw new ArgumentOutOfRangeException(nameof(year));
        
        return new Movie()
        {
            Id = Guid.CreateVersion7(),
            Title = title,
            Category = category,
            Year = year
        };
    }
}