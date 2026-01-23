using BookStore.Api.Common;
using BookStore.Application.DTOs;
using BookStore.Application.Interfaces;
using BookStore.Domain.Entities;
using BookStore.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.Api.Controllers;


public class AuthorsController(IAuthorService authorService) : BaseController
{
    [HttpPost]
    public ActionResult<Author> CreateAuthor(AuthorRequest request)
    {
        return Ok(authorService.CreateAuthor(request));
    }
    
    [HttpGet]
    [Authorize(Roles = "Admin")]
    public ActionResult<List<Author>> GetAuthors()
    {
        return Ok(authorService.GetAuthors());
    }
}