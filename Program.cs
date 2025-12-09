using ClassHub.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddOpenApi();

// MySQL DATABASE CONNECTION
builder.Services.AddDbContext<ExternalDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("ExternalDb"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("ExternalDb"))
    ));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();

app.Run();
