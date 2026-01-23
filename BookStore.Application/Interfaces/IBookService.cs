using BookStore.Application.DTOs;
using BookStore.Domain.Entities;

namespace BookStore.Application.Interfaces;

public interface IBookService
{
    Book CreateBook(BookRequest request);
    Book? GetById(int id);
    List<BookDto> GetBooks();
    Task<bool> UpdateBook(UpdateBookRequest book);
    Task<bool> DeleteBook(int id);
}