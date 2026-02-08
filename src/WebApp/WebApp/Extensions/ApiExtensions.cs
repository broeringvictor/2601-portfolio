using System.Text.Json;
using System.Text.RegularExpressions;
using WebApp.Entities;
using WebApp.Services;

namespace WebApp.Extensions;

public static class ApiExtensions
{
    public static WebApplication MapApi(this WebApplication app)
    {
        app.MapPost("/api/blog/upload", async (IFormFile file, IWebHostEnvironment env) =>
        {
            if (Path.GetExtension(file.FileName) != ".md")
                return Results.BadRequest("Only .md files are allowed.");

            using var reader = new StreamReader(file.OpenReadStream());
            var content = await reader.ReadToEndAsync();

            var match = Regex.Match(content, @"```json\s+meta\s*\n([\s\S]*?)```");
            if (!match.Success)
                return Results.BadRequest("The file must contain a ```json meta``` block with post metadata.");

            try
            {
                var meta = JsonSerializer.Deserialize<BlogPost>(
                    match.Groups[1].Value.Trim(),
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                var missing = new List<string>();
                if (string.IsNullOrWhiteSpace(meta?.Title)) missing.Add("title");
                if (string.IsNullOrWhiteSpace(meta?.Slug)) missing.Add("slug");
                if (string.IsNullOrWhiteSpace(meta?.Author)) missing.Add("author");
                if (string.IsNullOrWhiteSpace(meta?.Excerpt)) missing.Add("excerpt");
                if (meta?.PublishedAt == default) missing.Add("publishedAt");

                if (missing.Count > 0)
                    return Results.BadRequest($"Missing required fields in json meta: {string.Join(", ", missing)}.");
            }
            catch
            {
                return Results.BadRequest("The json meta block contains invalid JSON.");
            }

            var savePath = Path.Combine(env.WebRootPath, "Asserts", "Markdown", file.FileName);
            await File.WriteAllTextAsync(savePath, content);

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

        return app;
    }
}
