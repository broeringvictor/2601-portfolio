using System.Text.Json;
using Markdig;
using Markdig.Syntax;
using WebApp.Entities;

namespace WebApp.Services;

public class MarkdownService
{
    private readonly MarkdownPipeline _pipeline;
    private readonly IWebHostEnvironment _env;
    private const string MetaFence = "meta";
    private const string PostsDirectory = "Asserts/Markdown";

    public MarkdownService(IWebHostEnvironment env)
    {
        _env = env;
        _pipeline = new MarkdownPipelineBuilder()
            .UseAdvancedExtensions()
            .Build();
    }

    public async Task<BlogPost?> GetPostBySlugAsync(string slug)
    {
        var postsPath = Path.Combine(_env.WebRootPath, PostsDirectory);

        if (!Directory.Exists(postsPath))
            return null;

        var files = Directory.GetFiles(postsPath, "*.md");

        foreach (var file in files)
        {
            var text = await File.ReadAllTextAsync(file);
            var post = ParsePost(text);

            if (post.Slug == slug)
                return post;
        }

        return null;
    }

    public async Task<List<BlogPost>> GetAllPostsAsync()
    {
        var postsPath = Path.Combine(_env.WebRootPath, PostsDirectory);

        if (!Directory.Exists(postsPath))
            return new List<BlogPost>();

        var files = Directory.GetFiles(postsPath, "*.md");
        var posts = new List<BlogPost>();

        foreach (var file in files)
        {
            var text = await File.ReadAllTextAsync(file);
            var post = ParsePost(text);

            if (post.IsPublished)
                posts.Add(post);
        }

        return posts.OrderByDescending(p => p.PublishedAt).ToList();
    }

    public BlogPost ParsePost(string postText)
    {
        var markDoc = Markdown.Parse(postText, _pipeline);

        var postMetadata = markDoc
            .OfType<FencedCodeBlock>()
            .FirstOrDefault(x =>
                x.Arguments is not null &&
                x.Arguments.Contains(MetaFence));

        if (postMetadata == null)
            throw new InvalidOperationException("Metadados não encontrados no post");

        var metaContent = postMetadata.Lines.ToString();
        var post = JsonSerializer.Deserialize<BlogPost>(metaContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        if (post == null)
            throw new InvalidOperationException("Erro ao deserializar metadados");

        markDoc.Remove(postMetadata);

        using var writer = new StringWriter();
        var renderer = new Markdig.Renderers.HtmlRenderer(writer);
        _pipeline.Setup(renderer);
        renderer.Render(markDoc);
        post.HtmlContent = writer.ToString().Replace("<img ", "<img loading=\"lazy\" ");

        return post;
    }
}
