namespace BookStore.Domain.Entities;

public class Author
{
    public int Id { get; private set; }

    public string Name { get; private set; } = string.Empty;

    private Author()
    {
    }

    public Author(string name)
    {
        Name = name;
    }

    public void UpdateName(string name)
    {
        Name = name;
    }
}