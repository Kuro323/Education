using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace ApplicationsForFinance.models
{
    public class AppFinanceDbContext : DbContext
    {
        public AppFinanceDbContext(DbContextOptions<AppFinanceDbContext> options) : base(options)
        {
        }

        public DbSet<Article> Articles { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Transaction> Transactions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Category>(entity =>
            {
                entity.Property(c => c.NameCategory).IsRequired().HasMaxLength(150);
                entity.Property(c => c.Budget).HasColumnType("decimal(18,2)");
            });

            modelBuilder.Entity<Article>().Property(a => a.NameArticle).IsRequired().HasMaxLength(150);

            modelBuilder.Entity<Transaction>(entity =>
            {
                entity.Property(t => t.Sum).HasColumnType("decimal(18,2)");
                entity.ToTable(t => t.HasCheckConstraint("CK_Transaction_Sum", "[Sum] > 0"));
                entity.Property(t => t.Comment).IsRequired(false).HasMaxLength(500);
            });
        }
    }
}
