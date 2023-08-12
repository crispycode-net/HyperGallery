using HyperGallery.Shared.DBModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace HyperGallery.Shared.DBAccess
{
    public class MainContext : DbContext
    {
        public MainContext(DbContextOptions<MainContext> options) : base (options)
        {                
        }

        public DbSet<MediaFile>? Files { get; set; }
        public DbSet<ScanError>? ScanErrors { get; set; }

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    IConfigurationRoot configuration = new ConfigurationBuilder()
        //       .SetBasePath(Directory.GetCurrentDirectory())
        //       .AddJsonFile("appsettings.json")
        //       .Build();
        //    var connectionString = configuration.GetConnectionString("DbCoreConnectionString");
        //    optionsBuilder.UseSqlServer(connectionString);

        //    //optionsBuilder.UseSqlServer(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=MediaSchnaff;");
        //}

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

            modelBuilder.Entity<MediaFile>()
                .Property(b => b.LocalMediaPath)
                .HasMaxLength(1024);

            modelBuilder.Entity<MediaFile>()
                .Property(b => b.BestGuessMimeType)
                .HasMaxLength(50);
        }
    }
}
