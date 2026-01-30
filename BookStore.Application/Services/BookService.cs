using BookStore.Application.DTOs;
using BookStore.Application.Common;
using BookStore.Application.Interfaces;
using BookStore.Domain.Entities;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Application.Services;

public class BookService(IBookStoreDbContext dbContext) : IBookService
{
    public Book CreateBook(BookRequest request)
    {
        var newBook = new Book()
        {
            Title = request.Title,
            AuthorId = request.AuthorId.Value
        };
        dbContext.Books.Add(newBook);
        dbContext.SaveChanges();
        return newBook;
    }

    public List<BookDto> GetBooks()
    {
        var books = dbContext.Books
            .AsNoTracking()
            .Include(b => b.Author)
            .ProjectToType<BookDto>()
            .ToList();
        return books;
    }

    public PagedList<BookDto> GetBooksPaged(PaginatedRequest request)
    {
        var query = dbContext.Books
            .AsNoTracking()
            .Include(b => b.Author)
            .AsQueryable();

        // Search
        if (!string.IsNullOrWhiteSpace(request.SearchQuery))
        {
            var search = request.SearchQuery.ToLower();
            query = query.Where(b => b.Title.ToLower().Contains(search));
        }

        // Sorting
        bool isDesc = request.SortOrder?.ToLower() == "desc";
        query = request.SortBy?.ToLower() switch
        {
            "title" => isDesc ? query.OrderByDescending(b => b.Title) : query.OrderBy(b => b.Title),
            "id" => isDesc ? query.OrderByDescending(b => b.Id) : query.OrderBy(b => b.Id),
            "category" => isDesc ? query.OrderByDescending(b => b.Genres.FirstOrDefault().Genre.Name) : query.OrderBy(b => b.Genres.FirstOrDefault().Genre.Name),
            _ => query.OrderBy(b => b.Id)
        };

        var projectedQuery = query.ProjectToType<BookDto>();

        return PagedList<BookDto>.Create(projectedQuery, request.Page, request.PageSize);
    }

    public async Task<bool> UpdateBook(UpdateBookRequest request)
    {
        var bookToUpdate = dbContext.Books.FirstOrDefault(b => b.Id == request.Id);
        if (bookToUpdate is null)
            return false;
        if (!string.IsNullOrWhiteSpace(request.Title) && bookToUpdate.Title.Trim() != request.Title.Trim())
            bookToUpdate.Title = request.Title.Trim();
        if (request.AuthorId.HasValue && bookToUpdate.AuthorId != request.AuthorId.Value)
        {
            var author = dbContext.Authors.Any(a => a.Id == request.AuthorId.Value);
            if (!author)
                return false;
            bookToUpdate.AuthorId = request.AuthorId.Value;
        }
        await dbContext.SaveChangesAsync(CancellationToken.None);
        return true;
    }

    public Book? GetById(int id)
    {
        return dbContext.Books
            .Include(b => b.Author)
            .FirstOrDefault(b => b.Id == id);
    }


    public async Task<bool> DeleteBook(int id)
    {
        var book = dbContext.Books.FirstOrDefault(b => b.Id == id);
        if (book is null)
            return false;
        dbContext.Books.Remove(book);
        await dbContext.SaveChangesAsync(CancellationToken.None);
        return true;
    }
}