using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebApp.Entities;

namespace WebApp.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser>(options)
    {
        public DbSet<BlogPost> BlogPosts => Set<BlogPost>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<BlogPost>(e =>
            {
                e.HasIndex(p => p.Slug).IsUnique();
                e.Ignore(p => p.Tags);
                e.Ignore(p => p.HtmlContent);
            });
        }
    }
}
