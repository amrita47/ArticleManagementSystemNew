
using Microsoft.EntityFrameworkCore;
using ArticleManagementSystemNew.Models;

namespace ArticleManagementSystemNew.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Article> Articles { get; set; }
        public DbSet<Content> Contents { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

          
            modelBuilder.Entity<Article>()
                .Property(a => a.Status)
                .HasConversion<string>();

            modelBuilder.Entity<Content>()
                .Property(c => c.Status)
                .HasConversion<string>();

            modelBuilder.Entity<Content>()
                .Property(c => c.Language)
                .HasConversion<string>();

           
            modelBuilder.Entity<Content>()
                .Property(c => c.ContentData)
                .HasColumnName("Content");

           
            modelBuilder.Entity<Content>()
                .HasOne(c => c.AuthorUser)
                .WithMany(u => u.Contents)
                .HasForeignKey(c => c.Author);

           
            modelBuilder.Entity<Content>()
                .HasOne(c => c.Article)
                .WithMany(a => a.Contents)
                .HasForeignKey(c => c.ArticleId);
        }
    }
}
