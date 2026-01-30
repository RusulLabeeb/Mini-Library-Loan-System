using BookStore.Application.Common;
using BookStore.Application.DTOs;

namespace BookStore.Application.Interfaces;

public interface IGenreService
{
    Task<ServiceResult<PagedList<GenreDto>>> GetGenres(PaginatedRequest request);
    Task<ServiceResult<GenreDto>> CreateGenre(GenreDto request);
}