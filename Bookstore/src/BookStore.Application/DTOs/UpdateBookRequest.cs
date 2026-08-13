namespace BookStore.Application.DTOs;

public class UpdateBookRequest
{
    public string Title { get; set; } = string.Empty;
    public int AuthorId { get; set; }
    public string ISBN { get; set; } = string.Empty;
    public DateOnly PublishedDate { get; set; }
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public bool IsPremium { get; set; }
    public bool IsLoaned { get; set; }
    public bool IsHistorical { get; set; }
    public bool IsArchived { get; set; }
}