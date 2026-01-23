using BookStore.Api.Common;
using BookStore.Application.DTOs;
using BookStore.Application.Interfaces;
using BookStore.Application.Services;
using BookStore.Domain.Entities;
using BookStore.Infrastructure.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.Api.Controllers;

public class BooksController(IBookService bookService) : BaseController
{
    [HttpGet]
    public ActionResult<List<Book>> GetBooks()
    {
        var result = bookService.GetBooks();
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public ActionResult<Book?> GetBookById(int id)
    {
        var result = bookService.GetById(id);
        if (result is null)
            return NotFound("Book was not found");
        return Ok(result);
    }

    [HttpPost]
    public ActionResult<Book> CreateBook([FromBody] BookRequest request)
    {
        var result = bookService.CreateBook(request);
        return Ok(result);
    }

    [HttpPut]
    public async Task<ActionResult<Book?>> UpdateBook([FromBody] UpdateBookRequest request)
    {
        var result = await bookService.UpdateBook(request);
        if (result)
            return NoContent();
        return BadRequest("Book was not found or data provided is invalid");
    }
    
    [HttpDelete("{id:int}")]
    [Authorize(AuthenticationSchemes = AuthConstants.BasicScheme)]
    public async Task<ActionResult<Book>> DeleteBook(int id)
    {
        var result = await bookService.DeleteBook(id);
        if (!result)
            return NotFound();
        return NoContent();
    }
    
}