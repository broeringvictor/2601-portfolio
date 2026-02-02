namespace WebApp.Entities;

public class BlogPost
{
    public string Title { get; set; } = string.Empty;
    public string Lead { get; set; } = string.Empty;
    public bool IsPublished { get; set; }
    public DateTime PublishedAt { get; set; }
    public string Slug { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public string Excerpt { get; set; } = string.Empty;
    public string OpenGraphImage { get; set; } = string.Empty;
    public List<string> Tags { get; set; } = new();
    public string HtmlContent { get; set; } = string.Empty;
}
