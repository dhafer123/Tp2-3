using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace CinemaManager_Dhafer.Models.Cinema;

public partial class CinemaDbContext : DbContext
{
    public CinemaDbContext()
    {
    }

    public CinemaDbContext(DbContextOptions<CinemaDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Producer> Producers { get; set; }
    public virtual DbSet<Movie> Movies { get; set; } // Added Movies DbSet

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Name=CinemaCS");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Producer>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__producer__3214EC079DB6E2C4");

            entity.ToTable("producer");

            entity.Property(e => e.Email).HasMaxLength(30);
            entity.Property(e => e.Name)
                .HasMaxLength(30)
                .HasColumnName("name");
            entity.Property(e => e.Nationality).HasMaxLength(30);

            // Added relationship configuration
            entity.HasMany(p => p.Movies)
                  .WithOne(m => m.Producer)
                  .HasForeignKey(m => m.ProducerId)
                  .HasConstraintName("FK_Movie_Producer");
        });

        // Added Movie entity configuration
        modelBuilder.Entity<Movie>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__movies__3214EC07AEA1A41D");

            entity.ToTable("movies");

            entity.Property(e => e.Title)
                .HasMaxLength(30)
                .IsRequired();

            entity.Property(e => e.Genre)
                .HasMaxLength(20)
                .IsRequired();

            entity.Property(e => e.ProducerId)
                .IsRequired();

            // Relationship is already configured in Producer entity
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}