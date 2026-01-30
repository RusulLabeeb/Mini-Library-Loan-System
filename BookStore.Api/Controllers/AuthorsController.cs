using BookStore.Api.Common;
using BookStore.Application.Common;
using BookStore.Application.DTOs;
using BookStore.Application.Interfaces;
using BookStore.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.Api.Controllers;

public class AuthorsController(IAuthorService authorService) : BaseController
{
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public ActionResult<ApiResponse<Author>> CreateAuthor([FromBody] AuthorRequest request)
    {
        var result = authorService.CreateAuthor(request);
        return Ok(new ApiResponse<Author>(result));
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedList<AuthorDto>>>> GetAuthors([FromQuery] PaginatedRequest request)
    {
        var result = await authorService.GetAuthors(request);
        return Ok(new ApiResponse<PagedList<AuthorDto>>(result));
    }

    [HttpGet("search")]
    public async Task<ActionResult<ApiResponse<PagedList<AuthorDto>>>> SearchAuthors([FromQuery] string query, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var request = new PaginatedRequest
        {
            Page = page,
            PageSize = pageSize,
            SearchQuery = query
        };
        var result = await authorService.GetAuthors(request);
        return Ok(new ApiResponse<PagedList<AuthorDto>>(result));
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<bool>>> UpdateAuthor(int id, [FromBody] AuthorRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(new ApiResponse<bool>(new List<string> { "Invalid data" }, "Validation failed"));

        var result = await authorService.UpdateAuthor(id, request);
        if (result)
            return Ok(new ApiResponse<bool>(true, "Author updated successfully"));

        return NotFound(new ApiResponse<bool>(new List<string> { "Author not found" }, "Update failed"));
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteAuthor(int id)
    {
        var result = await authorService.DeleteAuthor(id);
        if (result)
            return Ok(new ApiResponse<bool>(true, "Author deleted successfully"));

        return NotFound(new ApiResponse<bool>(new List<string> { "Author not found" }, "Delete failed"));
    }
}