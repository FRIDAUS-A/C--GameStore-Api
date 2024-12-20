using System;

namespace GameStore.Api.Mapping;
using GameStore.Api.Dtos;
using GameStore.Api.Entities;

public static class GenreMapping
{
	public static GenreDto ToDto(this Genre genre) {
		return new GenreDto(genre.Id, genre.Name);
	}
}
