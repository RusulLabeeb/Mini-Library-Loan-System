using System.Security.Claims;
using BookStore.Api.Common;
using BookStore.Application.DTOs;
using BookStore.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.Api.Controllers;

[Authorize]
public class LoansController(ILoanService loanService) : BaseController
{
    [HttpPost]
    public async Task<ActionResult<LoanDto>> CreateLoan([FromBody] CreateLoanRequest request)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
        var result = await loanService.CreateLoanAsync(userId, request);
        return CreatedAtAction(nameof(CreateLoan), new { id = result.Id }, result);
    }
}
