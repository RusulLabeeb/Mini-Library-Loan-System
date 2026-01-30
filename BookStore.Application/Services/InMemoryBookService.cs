using BookStore.Application.DTOs;
using BookStore.Application.Common;
using BookStore.Application.Interfaces;
using BookStore.Domain.Entities;
using Mapster;

namespace BookStore.Application.Services;

public class InMemoryBookService : IBookService
{
    private List<Book> Books = new List<Book>()
    {
        new Book() { Id = 1, Title = "Abc" }
    };

    public Book CreateBook(BookRequest request)
    {
        var newId = Books.Max(b => b.Id) + 1;
        var newBook = new Book()
        {
            Id = newId,
            Title = request.Title.Trim(),
        };
        Books.Add(newBook);
        return newBook;
    }

    public Book? GetById(int id)
    {
        return Books.FirstOrDefault(b => b.Id == id);
    }

    public List<BookDto> GetBooks() => Books.AsQueryable().ProjectToType<BookDto>().ToList();

    public PagedList<BookDto> GetBooksPaged(int pageNumber, int pageSize)
    {
        var query = Books.AsQueryable().ProjectToType<BookDto>();
        return PagedList<BookDto>.Create(query, pageNumber, pageSize);
    }
    public Task<bool> UpdateBook(UpdateBookRequest book)
    {
        throw new NotImplementedException();
    }


    public async Task<bool> DeleteBook(int id)
    {
        var book = Books.FirstOrDefault(b => b.Id == id);
        if (book is null)
            return false;
        Books.Remove(book);
        return true;
    }
}