using System.Text.Json;
using Markdig;
using Markdig.Syntax;
using Microsoft.EntityFrameworkCore;
using WebApp.Data;
using WebApp.Entities;

namespace WebApp.Services;

public class MarkdownService
{
    private readonly MarkdownPipeline _pipeline;
    private readonly ApplicationDbContext _db;
    private const string MetaFence = "meta";

    public MarkdownService(ApplicationDbContext db)
    {
        _db = db;
        _pipeline = new MarkdownPipelineBuilder()
            .UseAdvancedExtensions()
            .Build();
    }

    public async Task<BlogPost?> GetPostBySlugAsync(string slug)
    {
        var post = await _db.BlogPosts.AsNoTracking()
            .FirstOrDefaultAsync(p => p.Slug == slug);

        if (post is null)
            return null;

        FillHtmlContent(post);
        return post;
    }

    public async Task<List<BlogPost>> GetAllPostsAsync()
    {
        var posts = await _db.BlogPosts.AsNoTracking()
            .Where(p => p.IsPublished)
            .OrderByDescending(p => p.PublishedAt)
            .ToListAsync();

        foreach (var post in posts)
            FillHtmlContent(post);

        return posts;
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

        post.MarkdownContent = postText;

        markDoc.Remove(postMetadata);

        using var writer = new StringWriter();
        var renderer = new Markdig.Renderers.HtmlRenderer(writer);
        _pipeline.Setup(renderer);
        renderer.Render(markDoc);
        post.HtmlContent = writer.ToString().Replace("<img ", "<img loading=\"lazy\" ");

        return post;
    }

    private void FillHtmlContent(BlogPost post)
    {
        if (string.IsNullOrEmpty(post.MarkdownContent))
            return;

        var markDoc = Markdown.Parse(post.MarkdownContent, _pipeline);

        var postMetadata = markDoc
            .OfType<FencedCodeBlock>()
            .FirstOrDefault(x =>
                x.Arguments is not null &&
                x.Arguments.Contains(MetaFence));

        if (postMetadata != null)
            markDoc.Remove(postMetadata);

        using var writer = new StringWriter();
        var renderer = new Markdig.Renderers.HtmlRenderer(writer);
        _pipeline.Setup(renderer);
        renderer.Render(markDoc);
        post.HtmlContent = writer.ToString().Replace("<img ", "<img loading=\"lazy\" ");
    }
}
