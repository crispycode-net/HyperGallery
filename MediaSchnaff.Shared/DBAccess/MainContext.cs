using MediaSchnaff.Shared.DBModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaSchnaff.Shared.DBAccess
{
    public class MainContext : DbContext
    {
        public DbSet<MediaFile>? Files { get; set; }
        public DbSet<ScanError>? ScanErrors { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=MediaSchnaff;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MediaFile>()
                .Property(b => b.SourcePath)
                .HasMaxLength(1024)
                .IsRequired();
            modelBuilder.Entity<MediaFile>()
                .HasIndex(b => b.SourcePath)
                .IsUnique();
            modelBuilder.Entity<MediaFile>()
                .Property(b => b.Kind)
                .HasMaxLength(10)
                .IsRequired();
            modelBuilder.Entity<MediaFile>()
                .Property(b => b.BestGuess)
                .IsRequired();
            modelBuilder.Entity<MediaFile>()
                .Property(b => b.BestGuessYear)
                .IsRequired();
            modelBuilder.Entity<MediaFile>()
                .Property(b => b.BestGuessMonth)
                .IsRequired();
            modelBuilder.Entity<MediaFile>()
                .Property(b => b.ThumbGuid)
                .HasMaxLength(50)
                .IsRequired();
            modelBuilder.Entity<MediaFile>()
                .Property(b => b.BestGuessSource)
                .HasMaxLength(50)
                .IsRequired();

            modelBuilder.Entity<ScanError>()
                .Property(b => b.Error)
                .HasMaxLength(1024)
                .IsRequired();
            modelBuilder.Entity<ScanError>()
                .Property(b => b.SourcePath)
                .HasMaxLength(1024)
                .IsRequired();

            modelBuilder.Entity<MediaFile>()
                .HasIndex(i => i.BestGuessYear)
                .IsUnique(false);

            modelBuilder.Entity<MediaFile>()
                .HasIndex(i => i.BestGuess)
                .IsUnique(false);
        }
    }
}
