using BookStore.Api.ActionFilters;
using BookStore.Api.Common;
using BookStore.Application.Common;
using BookStore.Application.DTOs;
using BookStore.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.Api.Controllers;

public class GenresController(IGenreService genreService) : BaseController
{
    [HttpGet]
    [ServiceFilter(typeof(LogActivityAttribute))]
    public async Task<ActionResult<ApiResponse<PagedList<GenreDto>>>> Get([FromQuery] PaginatedRequest request)
    {
        var result = await genreService.GetGenres(request);
        return HandleResult(result);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<GenreDto>>> Post([FromBody] GenreDto request)
    {
        var result = await genreService.CreateGenre(request);
        return HandleResult(result);
    }
}