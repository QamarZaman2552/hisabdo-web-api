using HisabDo.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HisabDo.Infrastructure.Data;

public class HisabDoDbContext(DbContextOptions<HisabDoDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Transaction> Transactions => Set<Transaction>();
    public DbSet<Setting> Settings => Set<Setting>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(u => u.Id);
            entity.Property(u => u.FullName).IsRequired().HasMaxLength(100);
            entity.Property(u => u.BusinessName).HasMaxLength(100);
            entity.Property(u => u.Email).IsRequired().HasMaxLength(100);
            entity.HasIndex(u => u.Email).IsUnique();
            entity.Property(u => u.Phone).HasMaxLength(20);
            entity.Property(u => u.Role).HasMaxLength(20);
            entity.Property(u => u.CurrencyCode).HasMaxLength(10);
            entity.Property(u => u.LanguageCode).HasMaxLength(10);
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(c => c.Id);
            entity.Property(c => c.Name).IsRequired().HasMaxLength(100);
            entity.Property(c => c.Phone).HasMaxLength(20);
            entity.Property(c => c.Email).HasMaxLength(100);
            entity.Property(c => c.Notes).HasMaxLength(500);
            entity.HasIndex(c => c.UserId);
            entity.HasIndex(c => new { c.UserId, c.Name });
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(c => c.Id);
            entity.Property(c => c.Name).IsRequired().HasMaxLength(50);
            entity.HasIndex(c => c.UserId);
        });

        modelBuilder.Entity<Transaction>(entity =>
        {
            entity.HasKey(t => t.Id);
            entity.Property(t => t.Amount).HasColumnType("decimal(18,2)");
            entity.Property(t => t.Note).HasMaxLength(500);
            entity.HasIndex(t => t.UserId);
            entity.HasIndex(t => new { t.UserId, t.TransactionDate });
            entity.HasIndex(t => t.CustomerId);
            entity.HasIndex(t => t.CategoryId);
        });

        modelBuilder.Entity<Setting>(entity =>
        {
            entity.HasKey(s => s.Id);
            entity.HasIndex(s => s.UserId).IsUnique().HasFilter("[IsDeleted] = 0");
            entity.Property(s => s.CurrencyCode).HasMaxLength(10);
            entity.Property(s => s.LanguageCode).HasMaxLength(10);
        });

        modelBuilder.Entity<User>()
            .HasMany(u => u.Customers)
            .WithOne(c => c.User)
            .HasForeignKey(c => c.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<User>()
            .HasMany(u => u.Categories)
            .WithOne(c => c.User)
            .HasForeignKey(c => c.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<User>()
            .HasMany(u => u.Transactions)
            .WithOne(t => t.User)
            .HasForeignKey(t => t.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<User>()
            .HasOne(u => u.Setting)
            .WithOne(s => s.User)
            .HasForeignKey<Setting>(s => s.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Customer>()
            .HasMany(c => c.Transactions)
            .WithOne(t => t.Customer)
            .HasForeignKey(t => t.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Category>()
            .HasMany(c => c.Transactions)
            .WithOne(t => t.Category)
            .HasForeignKey(t => t.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        SeedData(modelBuilder);
    }

    private static void SeedData(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().HasData(
            new User
            {
                Id = 1,
                FullName = "Demo User",
                BusinessName = "Demo Shop",
                Email = "demo@hisabdo.com",
                Phone = "03000000000",
                PasswordHash = "$2a$11$s8ApQ4Gy1p04rFS0jgd/deaRe9QbZM/4AhyqmgFytPQW6dp5A/lD.",
                Role = "Admin",
                CurrencyCode = "PKR",
                LanguageCode = "en",
                CreatedAt = new DateTime(2026, 8, 8, 0, 0, 0, DateTimeKind.Utc)
            });

        modelBuilder.Entity<Category>().HasData(
            new Category { Id = 1, UserId = 1, Name = "Sales", IsDefault = true, CreatedAt = new DateTime(2026, 8, 8, 0, 0, 0, DateTimeKind.Utc) },
            new Category { Id = 2, UserId = 1, Name = "Purchase", IsDefault = true, CreatedAt = new DateTime(2026, 8, 8, 0, 0, 0, DateTimeKind.Utc) },
            new Category { Id = 3, UserId = 1, Name = "Rent", IsDefault = true, CreatedAt = new DateTime(2026, 8, 8, 0, 0, 0, DateTimeKind.Utc) },
            new Category { Id = 4, UserId = 1, Name = "Food", IsDefault = true, CreatedAt = new DateTime(2026, 8, 8, 0, 0, 0, DateTimeKind.Utc) },
            new Category { Id = 5, UserId = 1, Name = "Transport", IsDefault = true, CreatedAt = new DateTime(2026, 8, 8, 0, 0, 0, DateTimeKind.Utc) },
            new Category { Id = 6, UserId = 1, Name = "Salary", IsDefault = true, CreatedAt = new DateTime(2026, 8, 8, 0, 0, 0, DateTimeKind.Utc) },
            new Category { Id = 7, UserId = 1, Name = "Others", IsDefault = true, CreatedAt = new DateTime(2026, 8, 8, 0, 0, 0, DateTimeKind.Utc) });

        modelBuilder.Entity<Setting>().HasData(
            new Setting
            {
                Id = 1,
                UserId = 1,
                CurrencyCode = "PKR",
                LanguageCode = "en"
            });
    }
}