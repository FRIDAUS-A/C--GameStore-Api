using GameStore.Api.Data;
using GameStore.Api.Dtos;
using GameStore.Api.Endpoints;

var builder = WebApplication.CreateBuilder(args);

// var connString = "Data Source=GameStore.db";
var connString = builder.Configuration.GetConnectionString("Gamestore");

builder.Services.AddSqlite<GameStoreContext>(connString);
builder.Services.AddScoped<GameStoreContext>();

var app = builder.Build();

app.MapGamesEndpoints();
app.MapGenresEndpoints();

await app.MigrateDbAsync();

app.Run();