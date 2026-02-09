using System.Text.Json;
using Markdig;
using Markdig.Extensions.Yaml;
using Markdig.Syntax;
using Microsoft.EntityFrameworkCore;
using WebApp.Data;
using WebApp.Entities;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace WebApp.Services;

public class MarkdownService(ApplicationDbContext db)
{
    private readonly MarkdownPipeline _pipeline = new MarkdownPipelineBuilder()
        .UseAdvancedExtensions()
        .Build();

    private const string MetaFence = "meta";

    public async Task<BlogPost?> GetPostBySlugAsync(string slug)
    {
        var post = await db.BlogPosts.AsNoTracking()
            .FirstOrDefaultAsync(p => p.Slug == slug);

        if (post is null)
            return null;

        FillHtmlContent(post);
        return post;
    }

    public async Task<List<BlogPost>> GetAllPostsAsync()
    {
        var posts = await db.BlogPosts.AsNoTracking()
            .Where(p => p.IsPublished)
            .OrderByDescending(p => p.PublishedAt)
            .ToListAsync();

        foreach (var post in posts)
            FillHtmlContent(post);

        return posts;
    }
    
    public async Task<List<BlogPost>> GetAllPostsTitlesAsync()
    {
        var posts = await db.BlogPosts.AsNoTracking()
            .OrderByDescending(p => p.PublishedAt)
            .ToListAsync();

        return posts;
    }

    public async Task<bool> DeletePostByIdAsync(int id)
    {
        var post = await db.BlogPosts.FindAsync(id);
        if (post is null)
            return false;

        db.BlogPosts.Remove(post);
        await db.SaveChangesAsync();
        return true;
    }



    public BlogPost ParsePost(string postText)
    {
        var markDoc = Markdown.Parse(postText, _pipeline);

        // Try to find JSON meta block
        var jsonMetadata = markDoc
            .OfType<FencedCodeBlock>()
            .FirstOrDefault(x =>
                x.Arguments is not null &&
                x.Arguments.Contains(MetaFence));

        // Try to find YAML front matter
        var yamlMetadata = markDoc
            .OfType<YamlFrontMatterBlock>()
            .FirstOrDefault();

        if (jsonMetadata == null && yamlMetadata == null)
            throw new InvalidOperationException("Metadados não encontrados no post. Use ```json meta ou YAML front matter (---).");

        BlogPost? post = null;

        if (jsonMetadata != null)
        {
            var metaContent = jsonMetadata.Lines.ToString();
            post = JsonSerializer.Deserialize<BlogPost>(metaContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            markDoc.Remove(jsonMetadata);
        }
        else if (yamlMetadata != null)
        {
            var yamlContent = yamlMetadata.Lines.ToString();
            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .IgnoreUnmatchedProperties()
                .Build();
            
            post = deserializer.Deserialize<BlogPost>(yamlContent);
            markDoc.Remove(yamlMetadata);
        }

        if (post == null)
            throw new InvalidOperationException("Erro ao deserializar metadados");

        post.MarkdownContent = postText;

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

        var jsonMetadata = markDoc
            .OfType<FencedCodeBlock>()
            .FirstOrDefault(x =>
                x.Arguments is not null &&
                x.Arguments.Contains(MetaFence));

        var yamlMetadata = markDoc
            .OfType<YamlFrontMatterBlock>()
            .FirstOrDefault();

        if (jsonMetadata != null)
            markDoc.Remove(jsonMetadata);
        
        if (yamlMetadata != null)
            markDoc.Remove(yamlMetadata);

        using var writer = new StringWriter();
        var renderer = new Markdig.Renderers.HtmlRenderer(writer);
        _pipeline.Setup(renderer);
        renderer.Render(markDoc);
        post.HtmlContent = writer.ToString().Replace("<img ", "<img loading=\"lazy\" ");
    }
}
