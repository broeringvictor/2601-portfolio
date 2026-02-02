using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebApp.Client.Pages;
using WebApp.Components;
using WebApp.Components.Account;
using WebApp.Data;
using WebApp.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents()
    .AddAuthenticationStateSerialization();

builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<MarkdownService>();
builder.Services.AddScoped<IdentityRedirectManager>();
builder.Services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();

builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = IdentityConstants.ApplicationScheme;
        options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
    })
    .AddIdentityCookies();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddIdentityCore<ApplicationUser>(options =>
    {
        options.SignIn.RequireConfirmedAccount = false;
        options.Stores.SchemaVersion = IdentitySchemaVersions.Version3;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders();

builder.Services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(WebApp.Client._Imports).Assembly);

app.MapPost("/api/blog/upload", async (IFormFile file, IWebHostEnvironment env) =>
{
    if (Path.GetExtension(file.FileName) != ".md")
        return Results.BadRequest("Only .md files are allowed.");

    using var reader = new StreamReader(file.OpenReadStream());
    var content = await reader.ReadToEndAsync();

    var match = System.Text.RegularExpressions.Regex.Match(content, @"```json\s+meta\s*\n([\s\S]*?)```");
    if (!match.Success)
        return Results.BadRequest("The file must contain a ```json meta``` block with post metadata.");

    try
    {
        var meta = System.Text.Json.JsonSerializer.Deserialize<WebApp.Entities.BlogPost>(
            match.Groups[1].Value.Trim(),
            new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });

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

// Add additional endpoints required by the Identity /Account Razor components.
app.MapAdditionalIdentityEndpoints();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.Migrate();

    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
    if (await userManager.FindByEmailAsync("admin@localhost") is null)
    {
        var admin = new ApplicationUser
        {
            UserName = "admin@localhost",
            Email = "admin@localhost",
            EmailConfirmed = true
        };
        await userManager.CreateAsync(admin, "Admin123!");
    }
}

app.Run();
