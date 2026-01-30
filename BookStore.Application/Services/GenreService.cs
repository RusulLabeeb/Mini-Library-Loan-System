using BookStore.Application.Common;
using BookStore.Application.DTOs;
using BookStore.Application.Interfaces;
using BookStore.Domain.Entities;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Application.Services;

public class GenreService(
    ICacheService cache,
    IBookStoreDbContext context) : IGenreService
{
    private const string GenreCachePrefix = "genres";

    public async Task<ServiceResult<PagedList<GenreDto>>> GetGenres(PaginatedRequest request)
    {
        var cacheKey = $"{GenreCachePrefix}_p{request.Page}_s{request.PageSize}";

        var cachedList = cache.Get<PagedList<GenreDto>>(cacheKey);
        if (cachedList != null)
        {
            return ServiceResult<PagedList<GenreDto>>.Success(cachedList);
        }

        var query = context.Genres
            .AsNoTracking()
            .OrderBy(g => g.Id)
            .ProjectToType<GenreDto>();

        var paginatedResult = await PagedList<GenreDto>.CreateAsync(
            query, request.Page, request.PageSize);

        cache.Set(cacheKey, paginatedResult, TimeSpan.FromSeconds(30));

        return ServiceResult<PagedList<GenreDto>>.Success(paginatedResult);
    }

    public async Task<ServiceResult<GenreDto>> CreateGenre(GenreDto request)
    {
        var sanitizedName = request.Name.Trim();
        var exists = await context.Genres.AnyAsync(g => g.Name == sanitizedName);
        if (exists)
        {
            return ServiceResult<GenreDto>.Failure(
                new ServiceError("Genre already exists", ServiceErrorType.Conflict));
        }

        var genre = request.Adapt<Genre>();

        context.Genres.Add(genre);
        await context.SaveChangesAsync(default);

        var resultDto = genre.Adapt<GenreDto>();

        cache.Remove($"{GenreCachePrefix}_p1_s10");

        return ServiceResult<GenreDto>.Success(resultDto);
    }
}