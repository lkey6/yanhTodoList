using System;
using System.Collections.Generic;
using AzurePJ.Models;
using Microsoft.EntityFrameworkCore;

namespace AzurePJ.DbContexts;

public partial class DbToDoListContext : DbContext
{
    public DbToDoListContext()
    {
    }

    public DbToDoListContext(DbContextOptions<DbToDoListContext> options)
        : base(options)
    {
    }

    public virtual DbSet<FamilyUsersLogin> FamilyUsersLogins { get; set; }

    public virtual DbSet<ToDo> ToDos { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public DbSet<Album> Albums { get; set; }
    public DbSet<Photo> Photos { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Server=tcp:yangx.database.windows.net,1433;Initial Catalog=DB-ToDoList;Persist Security Info=False;User ID=yx707835645;Password=Password01;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<FamilyUsersLogin>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__FamilyUs__1788CCACE6184245");

            entity.ToTable("FamilyUsersLogin");

            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Password).HasMaxLength(100);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.LastLogin).HasColumnType("datetime");
            entity.Property(e => e.Phone).HasMaxLength(20);
            entity.Property(e => e.Relationship).HasMaxLength(20);
            entity.Property(e => e.UserName).HasMaxLength(50);
        });

        modelBuilder.Entity<ToDo>(entity =>
        {
            entity.ToTable("ToDo");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("id");
            entity.Property(e => e.DueDate).HasColumnType("datetime");
            entity.Property(e => e.ToDo1)
                .HasMaxLength(50)
                .HasColumnName("ToDo");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__1788CC4C66F8342D");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.LastLoginIp)
                .HasMaxLength(45)
                .HasColumnName("LastLoginIP");
            entity.Property(e => e.LastLoginTime).HasColumnType("datetime");
            entity.Property(e => e.Password).HasMaxLength(255);
            entity.Property(e => e.PhoneNumber).HasMaxLength(20);
            entity.Property(e => e.Status).HasDefaultValue((byte)1);
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
        });

        modelBuilder.Entity<Album>(entity =>
        {
            entity.ToTable("Albums");
            entity.HasKey(a => a.Id);
            entity.Property(a => a.Name).IsRequired().HasMaxLength(200);
            entity.Property(a => a.Description).HasMaxLength(500);
            entity.Property(a => a.CoverUrl).HasMaxLength(500);
            entity.Property(a => a.CreatedAt).HasDefaultValueSql("SYSUTCDATETIME()");
        });

        modelBuilder.Entity<Photo>(entity =>
        {
            entity.ToTable("Photos");
            entity.HasKey(p => p.Id);
            entity.Property(p => p.ThumbnailPath).IsRequired().HasMaxLength(500);
            entity.Property(p => p.OriginalPath).IsRequired().HasMaxLength(500);
            entity.Property(p => p.UploadedAt).HasDefaultValueSql("SYSUTCDATETIME()");
            entity.Property(p => p.Description)
                            .HasColumnType("nvarchar(max)")
                            .IsRequired(false);
            entity.Property(p => p.Title)
            .HasMaxLength(255)
            .IsRequired(false);
            entity.HasOne(p => p.Album)
                  .WithMany(a => a.Photos)
                  .HasForeignKey(p => p.AlbumId)
                  .OnDelete(DeleteBehavior.SetNull);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
