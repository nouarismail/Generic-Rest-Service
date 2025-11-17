using OrdersApp.Data;
using Microsoft.EntityFrameworkCore;
using OrdersApp.Models;
using Microsoft.Extensions.FileProviders;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddDbContext<FormsDbContext>(options =>
    options.UseInMemoryDatabase("FormsDb"));

builder.Services.AddScoped<IFormEntryRepository, FormEntryRepository>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy
            .WithOrigins("http://localhost:5173")   
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var distPath = Path.Combine(builder.Environment.ContentRootPath, "dist");
Console.WriteLine("Dist path: " + distPath);
Console.WriteLine("Dist exists: " + Directory.Exists(distPath));

builder.Environment.WebRootPath = distPath;

var app = builder.Build();


using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<FormsDbContext>();
    DbInitializer.Seed(db);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(distPath),
    RequestPath = ""        // serve at root: /
});

//app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapFallback(async context =>
{
    context.Response.ContentType = "text/html";
    await context.Response.SendFileAsync(Path.Combine(distPath, "index.html"));
});

//app.UseCors("AllowFrontend");

app.Run();
