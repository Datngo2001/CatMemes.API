using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddDbContext<MemeDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("SqlServerConnection")));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.MapGet("/api/catmemes", async (MemeDbContext db) =>
{
    var memes = await db.Memes.ToListAsync();
    return Results.Ok(memes);
});

app.MapGet("/api/catmemes/{id}", async (string id, MemeDbContext db) =>
{
    var meme = await db.Memes.FindAsync(id);
    return meme != null ? Results.Ok(meme) : Results.NotFound($"Meme with ID {id} not found.");
});

app.Run();

public class Meme
{
    public string Id { get; set; }
    public string Url { get; set; }
    public string Caption { get; set; }
}

public class MemeDbContext : DbContext
{
    public MemeDbContext(DbContextOptions<MemeDbContext> options) : base(options) { }
    public DbSet<Meme> Memes { get; set; }
}
