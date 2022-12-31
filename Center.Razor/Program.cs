using MediaSchnaff.Shared.DBAccess;
using MediaSchnaff.Shared.LocalData;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System.IO;

namespace Center.Razor
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddRazorPages();
            builder.Services.AddSingleton<IDirectories, Directories>();
            builder.Services.AddDbContext<MainContext>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapRazorPages();

            app.MapGet("/api/MediaItem/Years", async (MainContext db) => {
                var years = await db.Files!.Select(f => f.BestGuessYear).Distinct().OrderBy(y => y).ToListAsync();
                return Results.Ok(years);
            });

            app.MapGet("/api/Years/{year}", (int year, MainContext db) => {
                var files = db.Files!
                    .Where(f => f.BestGuessYear == year)
                    .OrderBy(f => f.BestGuess)
                    .ToList()
                    .Select(f => new { id = f.Id, kind = f.Kind, path = $"Thumbnails/{year}/{f.BestGuess.ToString("yyyy-MM-dd")}_{f.ThumbGuid}.jpg" });
                return Results.Ok(files);
            });

            app.MapGet("/api/Video/{id}", (int id, MainContext db) =>
            {
                var file = db.Files!.Find(id);
                if (file == null || !File.Exists(file.SourcePath))
                    return Results.NotFound();

                var filestream = File.OpenRead(file.SourcePath);
                return Results.File(filestream, contentType: "video/mp4", fileDownloadName: Path.GetFileName(file.SourcePath), enableRangeProcessing: true);
            });

            app.MapGet("/api/Photo/{id}", (int id, MainContext db) =>
            {
                var file = db.Files!.Find(id);
                if (file == null || !File.Exists(file.SourcePath))
                    return Results.NotFound();

                var filestream = File.OpenRead(file.SourcePath);
                return Results.File(filestream, contentType: "image/jpg", fileDownloadName: Path.GetFileName(file.SourcePath));
            });

            app.Run();
        }
    }
}