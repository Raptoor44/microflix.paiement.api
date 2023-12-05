using Microsoft.EntityFrameworkCore;
using paiementService.Context;
using Steeltoe.Discovery.Client;
using Steeltoe.Discovery.Eureka;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddServiceDiscovery(options => options.UseEureka());

builder.Services.AddDbContext<DatabaseContext>();

var app = builder.Build();

app.Services.CreateScope().ServiceProvider.GetRequiredService<DatabaseContext>().Database.Migrate();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
