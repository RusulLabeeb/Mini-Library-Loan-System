using BookStore.Application.Common;
using BookStore.Application.DTOs;
using BookStore.Application.Interfaces;
using BookStore.Domain.Entities;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Application.Services;

public class AuthorService(IBookStoreDbContext dbContext) : IAuthorService
{
    public Author CreateAuthor(AuthorRequest request)
    {
        var newAuthor = new Author()
        {
            Name = request.Name,
        };
        dbContext.Authors.Add(newAuthor);

        dbContext.SaveChanges();
        return newAuthor;
    }

    public async Task<PagedList<AuthorDto>> GetAuthors(PaginatedRequest request)
    {
        var query = dbContext.Authors
            .AsNoTracking()
            .AsQueryable();

        // Search
        if (!string.IsNullOrWhiteSpace(request.SearchQuery))
        {
            var search = request.SearchQuery.ToLower();
            query = query.Where(a => a.Name.ToLower().Contains(search));
        }

        // Sorting
        bool isDesc = request.SortOrder?.ToLower() == "desc";
        query = request.SortBy?.ToLower() switch
        {
            "name" => isDesc ? query.OrderByDescending(a => a.Name) : query.OrderBy(a => a.Name),
            "id" => isDesc ? query.OrderByDescending(a => a.Id) : query.OrderBy(a => a.Id),
            _ => query.OrderBy(a => a.Id)
        };

        var projectedQuery = query.ProjectToType<AuthorDto>();

        return await PagedList<AuthorDto>.CreateAsync(projectedQuery, request.Page, request.PageSize);
    }

    public async Task<bool> UpdateAuthor(int id, AuthorRequest request)
    {
        var author = await dbContext.Authors.FirstOrDefaultAsync(a => a.Id == id);
        if (author == null) return false;

        author.Name = request.Name;
        await dbContext.SaveChangesAsync(default);
        return true;
    }

    public async Task<bool> DeleteAuthor(int id)
    {
        var author = await dbContext.Authors.FirstOrDefaultAsync(a => a.Id == id);
        if (author == null) return false;

        dbContext.Authors.Remove(author);
        await dbContext.SaveChangesAsync(default);
        return true;
    }
}