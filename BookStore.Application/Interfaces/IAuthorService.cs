using BookStore.Application.Common;
using BookStore.Application.DTOs;
using BookStore.Domain.Entities;

namespace BookStore.Application.Interfaces;

public interface IAuthorService
{
    Author CreateAuthor(AuthorRequest request);
    Task<PagedList<AuthorDto>> GetAuthors(PaginatedRequest request);
    Task<bool> UpdateAuthor(int id, AuthorRequest request);
    Task<bool> DeleteAuthor(int id);
}