using BookStore.Application.DTOs;
using BookStore.Application.Interfaces;
using BookStore.Domain.Entities;

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

    public List<BookDto> GetBooks() => throw new NotImplementedException();
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