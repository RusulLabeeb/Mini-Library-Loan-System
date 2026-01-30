using BookStore.Api.Common;
using BookStore.Application.Common;
using BookStore.Application.DTOs;
using BookStore.Application.Interfaces;
using BookStore.Application.Services;
using BookStore.Domain.Entities;
using BookStore.Infrastructure.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class BooksController(IBookService bookService, ICacheService cacheService) : BaseController
{
    [HttpGet]
    public ActionResult<ApiResponse<PagedList<BookDto>>> GetBooks([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var cacheKey = $"books_all_{pageNumber}_{pageSize}";
        var cachedBooks = cacheService.Get<PagedList<BookDto>>(cacheKey);

        if (cachedBooks != null)
        {
            return Ok(new ApiResponse<PagedList<BookDto>>(cachedBooks, "Fetched from cache"));
        }

        var result = bookService.GetBooksPaged(pageNumber, pageSize);
        cacheService.Set(cacheKey, result, TimeSpan.FromSeconds(30));

        return Ok(new ApiResponse<PagedList<BookDto>>(result));
    }

    [HttpGet("{id:int}")]
    public ActionResult<ApiResponse<Book?>> GetBookById(int id)
    {
        var result = bookService.GetById(id);
        if (result is null)
            return NotFound(new ApiResponse<Book?>(new List<string> { "Book was not found" }, "Error"));

        return Ok(new ApiResponse<Book?>(result));
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public ActionResult<ApiResponse<Book>> CreateBook([FromBody] BookRequest request)
    {
        var result = bookService.CreateBook(request);
        return CreatedAtAction(nameof(CreateBook), new { id = result.Id }, new ApiResponse<Book>(result));
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