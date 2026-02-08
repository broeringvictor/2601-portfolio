using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApp.Entities;

public class BlogPost
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;
    public string Lead { get; set; } = string.Empty;
    public bool IsPublished { get; set; }
    public DateTime PublishedAt { get; set; }
    public string Slug { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public string Excerpt { get; set; } = string.Empty;
    public string OpenGraphImage { get; set; } = string.Empty;
    public string TagsJson { get; set; } = "[]";
    public string MarkdownContent { get; set; } = string.Empty;

    [NotMapped]
    public List<string> Tags
    {
        get => System.Text.Json.JsonSerializer.Deserialize<List<string>>(TagsJson ?? "[]") ?? new();
        set => TagsJson = System.Text.Json.JsonSerializer.Serialize(value ?? new List<string>());
    }

    [NotMapped]
    public string HtmlContent { get; set; } = string.Empty;
}
