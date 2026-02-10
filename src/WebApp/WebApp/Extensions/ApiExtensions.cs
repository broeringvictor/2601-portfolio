using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using WebApp.Data;
using WebApp.Entities;
using WebApp.Services;

namespace WebApp.Extensions;

public static class ApiExtensions
{
    public static WebApplication MapApi(this WebApplication app)
    {
        
        
        
        
        app.MapPost("/api/blog/upload", async (IFormFile file, ApplicationDbContext db, MarkdownService markdownService) =>
        {
            if (Path.GetExtension(file.FileName) != ".md")
                return Results.BadRequest("Only .md files are allowed.");

            using var reader = new StreamReader(file.OpenReadStream());
            var content = await reader.ReadToEndAsync();

            var hasJsonMeta = Regex.IsMatch(content, @"```json\s+meta\s*\n([\s\S]*?)```");
            var hasYamlMeta = Regex.IsMatch(content, @"^---\s*\r?\n[\s\S]*?\r?\n---");
            if (!hasJsonMeta && !hasYamlMeta)
                return Results.BadRequest("The file must contain a ```json meta``` block or YAML front matter (---) with post metadata.");

            BlogPost parsed;
            try
            {
                parsed = markdownService.ParsePost(content);

                var missing = new List<string>();
                if (string.IsNullOrWhiteSpace(parsed.Title)) missing.Add("title");
                if (string.IsNullOrWhiteSpace(parsed.Slug)) missing.Add("slug");
                if (string.IsNullOrWhiteSpace(parsed.Author)) missing.Add("author");
                if (string.IsNullOrWhiteSpace(parsed.Excerpt)) missing.Add("excerpt");
                if (parsed.PublishedAt == default) missing.Add("publishedAt");

                if (missing.Count > 0)
                    return Results.BadRequest($"Missing required fields in json meta: {string.Join(", ", missing)}.");
            }
            catch
            {
                return Results.BadRequest("The json meta block contains invalid JSON.");
            }

            var existing = await db.BlogPosts.FirstOrDefaultAsync(p => p.Slug == parsed.Slug);
            if (existing is not null)
            {
                existing.Title = parsed.Title;
                existing.Lead = parsed.Lead;
                existing.IsPublished = parsed.IsPublished;
                existing.PublishedAt = parsed.PublishedAt;
                existing.Author = parsed.Author;
                existing.Excerpt = parsed.Excerpt;
                existing.OpenGraphImage = parsed.OpenGraphImage;
                existing.TagsJson = parsed.TagsJson;
                existing.MarkdownContent = content;
            }
            else
            {
                parsed.MarkdownContent = content;
                db.BlogPosts.Add(parsed);
            }

            await db.SaveChangesAsync();

            return Results.Ok(new { file.FileName });
        })
        .RequireAuthorization()
        .DisableAntiforgery();

        app.MapPost("/api/image/upload", async (IFormFile file, ImageSevice imageSevice) =>
        {
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();

            if (!allowedExtensions.Contains(ext))
                return Results.BadRequest("Only image files (.jpg, .jpeg, .png, .gif, .webp) are allowed.");

            using var stream = file.OpenReadStream();
            await imageSevice.UploadImageAsync(stream, file.FileName);

            return Results.Ok(new { file.FileName });
        })
        .RequireAuthorization()
        .DisableAntiforgery();

        app.MapGet("/api/images/{fileName}", (string fileName, ImageSevice imageSevice) =>
        {
            var filePath = imageSevice.GetImagePath(fileName);

            if (!File.Exists(filePath))
                return Results.NotFound();

            var ext = Path.GetExtension(fileName).ToLowerInvariant();
            var contentType = ext switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                ".webp" => "image/webp",
                _ => "application/octet-stream"
            };

            return Results.File(filePath, contentType);
        });

        app.MapDelete("/api/blog/{id}", (int id, ApplicationDbContext db) =>
            {
                var post = db.BlogPosts.Find(id)
                           ?? throw new InvalidOperationException("Post não encontrado.");

                db.BlogPosts.Remove(post);
                db.SaveChanges();

                return Results.Ok();

            })
            .RequireAuthorization();

        app.MapGet("/sitemap.xml", async (HttpContext context, ApplicationDbContext db) =>
        {
            var baseUrl = $"{context.Request.Scheme}://{context.Request.Host}";

            var posts = await db.BlogPosts
                .Where(p => p.IsPublished)
                .OrderByDescending(p => p.PublishedAt)
                .Select(p => new { p.Slug, p.PublishedAt })
                .ToListAsync();

            var sb = new StringBuilder();
            sb.AppendLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            sb.AppendLine("<urlset xmlns=\"http://www.sitemaps.org/schemas/sitemap/0.9\">");

            sb.AppendLine("  <url>");
            sb.AppendLine($"    <loc>{baseUrl}/</loc>");
            sb.AppendLine("    <changefreq>monthly</changefreq>");
            sb.AppendLine("    <priority>1.0</priority>");
            sb.AppendLine("  </url>");

            sb.AppendLine("  <url>");
            sb.AppendLine($"    <loc>{baseUrl}/blog</loc>");
            sb.AppendLine("    <changefreq>weekly</changefreq>");
            sb.AppendLine("    <priority>0.8</priority>");
            sb.AppendLine("  </url>");

            foreach (var post in posts)
            {
                sb.AppendLine("  <url>");
                sb.AppendLine($"    <loc>{baseUrl}/post/{post.Slug}</loc>");
                sb.AppendLine($"    <lastmod>{post.PublishedAt:yyyy-MM-dd}</lastmod>");
                sb.AppendLine("    <changefreq>monthly</changefreq>");
                sb.AppendLine("    <priority>0.6</priority>");
                sb.AppendLine("  </url>");
            }

            sb.AppendLine("</urlset>");

            return Results.Text(sb.ToString(), "application/xml", Encoding.UTF8);
        });

        return app;
    }

    
}
