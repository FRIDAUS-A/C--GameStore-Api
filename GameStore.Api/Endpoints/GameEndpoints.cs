using System;

namespace GameStore.Api.Endpoints;

using GameStore.Api.Data;
using GameStore.Api.Dtos;
using GameStore.Api.Entities;
using GameStore.Api.Mapping;
using Microsoft.EntityFrameworkCore;

public static class GameEndpoints
{
	const string GetGameEndpointName = "GetGame";

private static readonly List<GameSummaryDto> games = [
	 new (
                1,
                "Street Fighter II",
                "Fighting",
                19.99M,
                new DateOnly(1992, 7, 15)
        ),
        new (
                2,
                "Final Fantasy XIV",
                "RolePlaying",
                59.99M,
                new DateOnly(2010, 9, 30)
        ),
        new (
                3,
                "FIFA 23",
                "Sports",
                69.99M,
                new DateOnly(2022, 9, 27)
        )
];
	public static RouteGroupBuilder MapGamesEndpoints(this WebApplication app) {
		var group = app.MapGroup("games")
						.WithParameterValidation();
group.MapGet("", async (GameStoreContext dbContext) => await dbContext.Games
.Include(game => game.Genre)
.Select((game) => game.ToGameSummaryDto())
.AsNoTracking()
.ToListAsync());
group.MapGet("/{id}", async (int id, GameStoreContext dbContext) => {
        // var game = games.Find((game) => game.Id == id);
        // return game is null ? Results.NoContent() : Results.Ok(game);
		Game? game = await dbContext.Games.FindAsync(id);
		return game is null ? Results.NoContent() : Results.Ok(game.ToGameDetailsDto());
}).WithName(GetGameEndpointName);

group.MapPost("", async (CreateGameDto  newGame,  GameStoreContext dbContext) => {
	// GameDto game = new (
	// 	games.Count + 1,
	// 	newGame.Name,
	// 	newGame.Genre,
	// 	newGame.price,
	// 	newGame.ReleaseDate
	// );
	// games.Add(game);
	// return Results.CreatedAtRoute(GetGameEndpointName, new { id = game.Id}, game);
	// using Entity
	// Game game = new()
	// {
	// 	Name = newGame.Name,
	// 	Genre = dbContext.Genres.Find(newGame.GenreId),
	// 	GenreId = newGame.GenreId,
	// 	Price = newGame.price,
	// 	ReleaseDate = newGame.ReleaseDate
	// };
	Game game = newGame.ToEntity();
	// game.Genre = dbContext.Genres.Find(newGame.GenreId); // Entity Framework would take of assigning value to this
	dbContext.Games.Add(game);
	await dbContext.SaveChangesAsync();

	

	return Results.CreatedAtRoute(
		GetGameEndpointName,
		new { id = game.Id },
		game.ToGameDetailsDto()
	);
});

group.MapPut("/{id}", async (int id, UpdateGameDto updatedGame, GameStoreContext dbContext) => {
	var existingGame = await dbContext.Games.FindAsync(id);
	// var Index = games.FindIndex((game) => game.Id == id);
	// if (Index == -1) return Results.NotFound();
	// GameSummaryDto game = new (
	// 	id,
	// 	newGame.Name,
	// 	newGame.Genre,
	// 	newGame.Price,
	// 	newGame.ReleaseDate
	// );
	// games[Index] = game;
	if (existingGame is null) {
		return Results.NotFound();
	}
	dbContext.Entry(existingGame)
	.CurrentValues
	.SetValues(updatedGame.ToEntity(id));
	await dbContext.SaveChangesAsync();
    return Results.NoContent();
});


group.MapDelete("/{id}", async (int id, GameStoreContext dbContext) => {
        // games.RemoveAll((game) => game.Id == id);
        // return Results.NoContent();
		await dbContext.Games
				.Where(game => game.Id == id)
				.ExecuteDeleteAsync();
		return Results.NoContent();
});
	return group;
	}
}
