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

    public PagedList<BookDto> GetBooksPaged(int pageNumber, int pageSize)
    {
        var query = dbContext.Books
            .AsNoTracking()
            .Include(b => b.Author)
            .ProjectToType<BookDto>();

        return PagedList<BookDto>.Create(query, pageNumber, pageSize);
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
        throw new NotImplementedException();
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